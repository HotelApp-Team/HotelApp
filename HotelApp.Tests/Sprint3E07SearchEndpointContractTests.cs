using HotelApp.API.Controllers;
using HotelApp.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

public class Sprint3E07SearchEndpointContractTests
{
    private static readonly Assembly ApiAssembly = typeof(HotelsController).Assembly;
    private static readonly Assembly DomainAssembly = typeof(Hotel).Assembly;

    [Fact]
    public void E07_SearchEndpoint_ShouldStayAnonymousAndUsePlanRoute()
    {
        if (!E07AvailabilityImplementationPresent())
            return;

        var controllerType = GetRequiredType(ApiAssembly, "HotelApp.API.Controllers.HotelsController");
        var searchAction = FindE07SearchAction(controllerType);

        Assert.NotNull(searchAction);
        Assert.True(
            AllowsAnonymous(searchAction) || !RequiresAuthorization(controllerType, searchAction),
            "E07 /api/hotels/search Anonymous olmali; login veya Admin yetkisi istememeli.");
    }

    [Fact]
    public void E07_SearchRequestDto_ShouldMakeAvailabilityParametersRequired()
    {
        if (!E07AvailabilityImplementationPresent())
            return;

        var dtoType = GetRequiredType(DomainAssembly, "HotelApp.Domain.DTOs.HotelSearchRequestDto");

        AssertRequiredProperty(dtoType, "City", typeof(string));
        AssertRequiredProperty(dtoType, "CheckIn", typeof(DateTime));
        AssertRequiredProperty(dtoType, "CheckOut", typeof(DateTime));

        var guestProperty = dtoType.GetProperty("GuestCount") ?? dtoType.GetProperty("Guests");
        Assert.NotNull(guestProperty);
        Assert.Equal(typeof(int), guestProperty!.PropertyType);
        Assert.NotNull(guestProperty.GetCustomAttribute<RequiredAttribute>());

        var guestRange = guestProperty.GetCustomAttribute<RangeAttribute>();
        Assert.NotNull(guestRange);
        Assert.Equal(1, Convert.ToInt32(guestRange!.Minimum));
        Assert.True(
            Convert.ToInt32(guestRange.Maximum) <= 20,
            "GuestCount/Guests icin ust limit plan review'unda onerilen 20 degerinden buyuk olmamali.");

        AssertNullableNumericProperty(dtoType, new[] { "StarRating", "MinStarRating" }, typeof(byte));
        AssertNullableNumericProperty(dtoType, new[] { "MinRating" }, typeof(decimal));
    }

    [Fact]
    public void E07_HotelService_ShouldDelegateAvailabilitySearchToRepositoryOnly()
    {
        if (!E07AvailabilityImplementationPresent())
            return;

        var hotelServiceSource = ReadProjectFile("HotelApp.Domain", "Services", "HotelService.cs");
        var searchBody = ExtractMethodBody(hotelServiceSource, "SearchAsync");

        Assert.Contains("SearchAvailableHotelsPagedAsync", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("hasAvailabilityFilter", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("SearchPagedAsync", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GetAllAsync", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("HotelRepository.Query", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(".Query()", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(".Where(", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(".Select(", searchBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("MapToListDto", searchBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void E07_HotelRepository_ShouldExposeAvailabilitySearchContract()
    {
        if (!E07AvailabilityImplementationPresent())
            return;

        var repositoryType = GetRequiredType(DomainAssembly, "HotelApp.Domain.Interfaces.IHotelRepository");
        var method = repositoryType.GetMethod("SearchAvailableHotelsPagedAsync");

        Assert.NotNull(method);
        AssertParameter(method!, 0, "city", typeof(string));
        AssertParameter(method!, 1, "checkIn", typeof(DateTime));
        AssertParameter(method!, 2, "checkOut", typeof(DateTime));
        AssertParameter(method!, 3, "guestCount", typeof(int));
        AssertParameter(method!, 4, "starRating", typeof(byte?));
        AssertParameter(method!, 5, "minRating", typeof(decimal?));
        AssertParameter(method!, 6, "page", typeof(int));
        AssertParameter(method!, 7, "pageSize", typeof(int));

        Assert.True(method!.ReturnType.IsGenericType, "Repository metodu Task<(List<HotelListDto> Data, int TotalCount)> dondurmeli.");
        Assert.Equal(typeof(Task<>), method.ReturnType.GetGenericTypeDefinition());

        var taskResultType = method.ReturnType.GetGenericArguments()[0];
        Assert.True(taskResultType.FullName?.StartsWith("System.ValueTuple", StringComparison.Ordinal) == true);

        var tupleArguments = taskResultType.GetGenericArguments();
        Assert.Equal(typeof(List<>), tupleArguments[0].GetGenericTypeDefinition());
        Assert.Equal("HotelApp.Domain.DTOs.HotelListDto", tupleArguments[0].GetGenericArguments()[0].FullName);
        Assert.Equal(typeof(int), tupleArguments[1]);
    }

    [Fact]
    public void E07_HotelRepositoryImplementation_ShouldUseSpOnceAndDatabaseLevelPaging()
    {
        if (!E07AvailabilityImplementationPresent())
            return;

        var repositorySource = ReadProjectFile("HotelApp.Infrastructure", "Repositories", "HotelRepository.cs");
        var methodBody = ExtractMethodBody(repositorySource, "SearchAvailableHotelsPagedAsync");

        Assert.Contains("sp_SearchAvailableRooms", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.True(
            ContainsAny(methodBody, "@HotelId = NULL", "HotelId = NULL", "DBNull.Value", "hotelId: null", "HotelId = null"),
            "sp_SearchAvailableRooms tek seferde tum oteller icin @HotelId = NULL/DBNull.Value ile cagrilmali.");
        Assert.Contains("Distinct", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("CountAsync", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Skip(", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("Take(", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("ToListAsync", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("foreach", methodBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("GetAvailableRoomsAsync", methodBody, StringComparison.OrdinalIgnoreCase);
    }

    private static bool E07AvailabilityImplementationPresent()
    {
        var controllerType = ApiAssembly.GetType("HotelApp.API.Controllers.HotelsController");
        if (FindE07SearchAction(controllerType) is null)
            return false;

        try
        {
            var hotelServiceSource = ReadProjectFile("HotelApp.Domain", "Services", "HotelService.cs");
            var searchBody = RemoveCSharpComments(ExtractMethodBody(hotelServiceSource, "SearchAsync"));
            var hotelRepositorySource = ReadProjectFile("HotelApp.Infrastructure", "Repositories", "HotelRepository.cs");

            return ContainsAny(
                    searchBody,
                    "dto.CheckIn",
                    "dto.CheckOut",
                    "dto.GuestCount",
                    "dto.Guests",
                    "hasAvailabilityFilter",
                    "SearchAvailableHotelsPagedAsync",
                    "GetAvailableRoomsAsync",
                    "sp_SearchAvailableRooms")
                || hotelRepositorySource.Contains("SearchAvailableHotelsPagedAsync", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static MethodInfo? FindE07SearchAction(Type? controllerType)
    {
        if (controllerType is null)
            return null;

        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .FirstOrDefault(method =>
                method.Name == "Search" &&
                method.GetCustomAttributes<HttpGetAttribute>()
                    .Any(attribute => string.Equals(attribute.Template, "search", StringComparison.OrdinalIgnoreCase)));
    }

    private static Type GetRequiredType(Assembly assembly, string fullName)
    {
        return assembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static bool RequiresAuthorization(Type controllerType, MethodInfo? action)
    {
        return controllerType.GetCustomAttribute<AuthorizeAttribute>() is not null
            || action?.GetCustomAttribute<AuthorizeAttribute>() is not null;
    }

    private static bool AllowsAnonymous(MethodInfo? action)
    {
        return action?.GetCustomAttribute<AllowAnonymousAttribute>() is not null;
    }

    private static void AssertRequiredProperty(Type dtoType, string propertyName, Type expectedType)
    {
        var property = dtoType.GetProperty(propertyName);

        Assert.NotNull(property);
        Assert.Equal(expectedType, property!.PropertyType);
        Assert.NotNull(property.GetCustomAttribute<RequiredAttribute>());
    }

    private static void AssertNullableNumericProperty(Type dtoType, string[] propertyNames, Type expectedUnderlyingType)
    {
        var property = propertyNames
            .Select(dtoType.GetProperty)
            .FirstOrDefault(candidate => candidate is not null);

        Assert.NotNull(property);
        Assert.Equal(expectedUnderlyingType, Nullable.GetUnderlyingType(property!.PropertyType));
    }

    private static void AssertParameter(MethodInfo method, int index, string expectedName, Type expectedType)
    {
        var parameters = method.GetParameters();

        Assert.True(parameters.Length > index, $"{method.Name} metodu {expectedName} parametresini icermeli.");
        Assert.Equal(expectedName, parameters[index].Name, ignoreCase: true);
        Assert.Equal(expectedType, parameters[index].ParameterType);
    }

    private static string ReadProjectFile(string projectName, params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindSolutionRoot(), projectName }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    private static string ExtractMethodBody(string source, string methodName)
    {
        var methodNameIndex = source.IndexOf(methodName, StringComparison.Ordinal);
        Assert.True(methodNameIndex >= 0, $"{methodName} metodu bulunamadi.");

        var openBraceIndex = source.IndexOf('{', methodNameIndex);
        Assert.True(openBraceIndex >= 0, $"{methodName} metodu icin acilis blogu bulunamadi.");

        var depth = 0;
        for (var i = openBraceIndex; i < source.Length; i++)
        {
            if (source[i] == '{')
                depth++;
            else if (source[i] == '}')
                depth--;

            if (depth == 0)
                return source[openBraceIndex..(i + 1)];
        }

        throw new InvalidOperationException($"{methodName} metodu govdesi okunamadi.");
    }

    private static bool ContainsAny(string text, params string[] expectedParts)
    {
        return expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase));
    }

    private static string RemoveCSharpComments(string source)
    {
        return Regex.Replace(source, @"//.*?$|/\*.*?\*/", string.Empty, RegexOptions.Multiline | RegexOptions.Singleline);
    }

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "HotelApp.sln")))
                return directory.FullName;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("HotelApp.sln bulunamadi.");
    }
}
