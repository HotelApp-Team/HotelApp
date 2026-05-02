using ApiHotelsController = HotelApp.API.Controllers.HotelsController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

public class Sprint4AdminApiContractTests
{
    private static readonly Assembly ApiAssembly = typeof(ApiHotelsController).Assembly;

    [Fact]
    public void S401_AdminController_ShouldUseApiAdminRouteAndAdminAuthorization()
    {
        if (!Sprint4CardGuards.S401_AdminApiImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");

        Assert.True(
            HasRoute(controllerType, "api/admin"),
            "S4-01 kartina gore AdminController base route'u [Route(\"api/admin\")] olmali.");

        var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorize);
        Assert.Contains("Admin", authorize!.Roles ?? string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S401_AdminController_ShouldExposeE36ToE39PlanRoutes()
    {
        if (!Sprint4CardGuards.S401_AdminApiImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");

        if (Sprint4CardGuards.S401_E36DashboardImplemented())
            Assert.NotNull(FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "dashboard"));

        if (Sprint4CardGuards.S401_E37UsersImplemented())
            Assert.NotNull(FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "users"));

        if (Sprint4CardGuards.S401_E38StatusImplemented())
            Assert.NotNull(FindActionWithHttpAttribute<HttpPutAttribute>(controllerType, "users/{id}/status"));

        if (Sprint4CardGuards.S401_E39RoleImplemented())
            Assert.NotNull(FindActionWithHttpAttribute<HttpPutAttribute>(controllerType, "users/{id}/role"));
    }

    [Fact]
    public void S401_E36Dashboard_ShouldReturnPlanStatisticsWithoutCrashingOnEmptyData()
    {
        if (!Sprint4CardGuards.S401_E36DashboardImplemented())
            return;

        var source = ReadApiFile("Controllers", "AdminController.cs");
        var dashboardAction = FindActionWithHttpAttribute<HttpGetAttribute>(
            GetRequiredType("HotelApp.API.Controllers.AdminController"),
            "dashboard");
        var dashboardBody = ExtractMethodBody(source, dashboardAction?.Name ?? "Dashboard");
        var implementationEvidence = dashboardBody
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Domain", "Services", "AdminService.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Infrastructure", "Repositories", "AdminRepository.cs");

        AssertContainsAll(
            implementationEvidence,
            "TotalHotels",
            "TotalUsers",
            "TotalReservations",
            "TotalRevenue",
            "AverageRating");
        AssertContainsAny(implementationEvidence, "ActiveReservations", "Confirmed");
        AssertContainsAny(implementationEvidence, "RecentReservations", "Take(10", "Take( 10");
        AssertContainsAny(implementationEvidence, "CountAsync", ".Count(", "SumAsync", ".Sum(", "AverageAsync", ".Average(");
        AssertContainsAny(dashboardBody, "Ok(", "return Ok");
    }

    [Fact]
    public void S401_E37Users_ShouldSupportSearchStatusAndPagingWithoutSensitiveData()
    {
        if (!Sprint4CardGuards.S401_E37UsersImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");
        var usersAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "users");

        Assert.NotNull(usersAction);
        AssertParameter(usersAction!, "search", typeof(string), allowNullable: true);
        AssertParameter(usersAction!, "isActive", typeof(bool?), allowNullable: true);
        AssertParameter(usersAction!, "page", typeof(int));
        AssertParameter(usersAction!, "pageSize", typeof(int));

        var source = ReadApiFile("Controllers", "AdminController.cs");
        var usersBody = ExtractMethodBody(source, usersAction!.Name);

        AssertContainsAny(usersBody, "Skip(", "Page", "page");
        AssertContainsAny(usersBody, "Take(", "PageSize", "pageSize");
        AssertContainsAny(usersBody, "TotalCount", "CountAsync", ".Count(", "PagedResult");
        AssertContainsAll(usersBody, "Id", "Email", "IsActive");
        AssertContainsAny(usersBody, "FirstName", "LastName", "UserName", "Username", "Name");
        AssertContainsAny(usersBody, "Roles", "RoleName", "Role");
        Assert.DoesNotContain("PasswordHash", usersBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Token", usersBody, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void S401_E38Status_ShouldValidateUserAndUpdateIsActive()
    {
        if (!Sprint4CardGuards.S401_E38StatusImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");
        var statusAction = FindActionWithHttpAttribute<HttpPutAttribute>(controllerType, "users/{id}/status");

        Assert.NotNull(statusAction);
        AssertParameter(statusAction!, "id", typeof(int));
        Assert.True(
            statusAction!.GetParameters().Any(parameter =>
                HasProperty(parameter.ParameterType, "IsActive", typeof(bool))
                || string.Equals(parameter.Name, "isActive", StringComparison.OrdinalIgnoreCase)),
            "E38 body veya parametre olarak IsActive bool degerini almali.");

        var source = ReadApiFile("Controllers", "AdminController.cs");
        var statusBody = ExtractMethodBody(source, statusAction.Name);

        Assert.Contains("IsActive", statusBody, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(statusBody, "NotFound", "StatusCode(404", "Results.NotFound");
        AssertContainsAny(statusBody, "BadRequest", "StatusCode(400", "Results.BadRequest");
        AssertContainsAny(statusBody, "SaveChangesAsync", "UpdateAsync", ".Update(");
    }

    [Fact]
    public void S401_E39Role_ShouldValidateUserAndRoleBeforeChangingRole()
    {
        if (!Sprint4CardGuards.S401_E39RoleImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");
        var roleAction = FindActionWithHttpAttribute<HttpPutAttribute>(controllerType, "users/{id}/role");

        Assert.NotNull(roleAction);
        AssertParameter(roleAction!, "id", typeof(int));
        Assert.True(
            roleAction!.GetParameters().Any(parameter =>
                HasProperty(parameter.ParameterType, "RoleName", typeof(string))
                || string.Equals(parameter.Name, "roleName", StringComparison.OrdinalIgnoreCase)),
            "E39 body veya parametre olarak RoleName string degerini almali.");

        var source = ReadApiFile("Controllers", "AdminController.cs");
        var roleBody = ExtractMethodBody(source, roleAction.Name);

        AssertContainsAny(roleBody, "RoleName", "roleName");
        AssertContainsAny(roleBody, "NotFound", "StatusCode(404", "Results.NotFound");
        AssertContainsAny(roleBody, "BadRequest", "StatusCode(400", "Results.BadRequest");
        AssertContainsAny(roleBody, "UserRoles", ".Roles", "Roles");
        AssertContainsAny(roleBody, "Clear(", "Remove", "Delete", "UserRoles.Remove", "Roles.Remove");
        AssertContainsAny(roleBody, "Add", "AddAsync", "UserRoles.Add", "Roles.Add");
        AssertContainsAny(roleBody, "SaveChangesAsync", "UpdateAsync", ".Update(");
    }

    private static Type GetRequiredType(string fullName)
    {
        return ApiAssembly.GetType(fullName)
            ?? throw new InvalidOperationException($"{fullName} tipi bulunamadi.");
    }

    private static bool HasRoute(Type controllerType, string expectedTemplate)
    {
        return controllerType
            .GetCustomAttributes<RouteAttribute>()
            .Any(attribute => RouteEquals(attribute.Template, expectedTemplate));
    }

    private static MethodInfo? FindActionWithHttpAttribute<TAttribute>(Type controllerType, string expectedTemplate)
        where TAttribute : HttpMethodAttribute
    {
        return controllerType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .FirstOrDefault(method => method
                .GetCustomAttributes<TAttribute>()
                .Any(attribute => RouteEquals(attribute.Template, expectedTemplate)));
    }

    private static bool RouteEquals(string? actualTemplate, string expectedTemplate)
    {
        return string.Equals(
            NormalizeRoute(actualTemplate),
            NormalizeRoute(expectedTemplate),
            StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRoute(string? template)
    {
        return (template ?? string.Empty)
            .Trim()
            .Trim('/')
            .Replace("//", "/", StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);
    }

    private static void AssertParameter(MethodInfo method, string parameterName, Type expectedType, bool allowNullable = false)
    {
        var parameter = method.GetParameters()
            .FirstOrDefault(candidate => string.Equals(candidate.Name, parameterName, StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(parameter);

        var actualType = parameter!.ParameterType;
        if (allowNullable && Nullable.GetUnderlyingType(actualType) is { } nullableType)
            actualType = nullableType;

        Assert.Equal(expectedType, actualType);
    }

    private static bool HasProperty(Type type, string propertyName, Type expectedType)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
        if (property is null)
            return false;

        var actualType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        return actualType == expectedType;
    }

    private static string ReadApiFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot("HotelApp.API") }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    private static string ReadOptionalProjectFile(string projectName, params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot(projectName) }.Concat(relativePathParts).ToArray());

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    private static string ExtractMethodBody(string source, string methodName)
    {
        var match = Regex.Match(
            source,
            $@"\b(?:Task\s*<[^>]+>|ActionResult\s*<[^>]+>|IActionResult|Task<IActionResult>|Task|async\s+Task\s*<[^>]+>|async\s+Task<IActionResult>|async\s+Task|public|private|internal|protected|static|\s)+\s+{Regex.Escape(methodName)}\s*\(",
            RegexOptions.Multiline);

        var methodNameIndex = match.Success
            ? match.Index
            : source.IndexOf(methodName, StringComparison.Ordinal);

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

    private static void AssertContainsAll(string text, params string[] expectedParts)
    {
        foreach (var expected in expectedParts)
        {
            Assert.Contains(expected, text, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            expectedParts.Any(expected => text.Contains(expected, StringComparison.OrdinalIgnoreCase)),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }

    private static string FindProjectRoot(string projectName)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory != null)
        {
            var candidate = Path.Combine(directory.FullName, projectName);
            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException($"{projectName} klasoru bulunamadi.");
    }
}
