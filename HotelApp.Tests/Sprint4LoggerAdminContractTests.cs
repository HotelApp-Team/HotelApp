using ApiAdminController = HotelApp.API.Controllers.AdminController;
using HotelApp.Domain.Services;
using HotelApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HotelApp.Tests;

// Sprint4LoggerAdminContractTests sınıfının amacı, S4-08 kartındaki logger entegrasyonu ve admin log endpoint sözleşmesini korumaktır.
// Kod yokken CI passed geçer; logger/admin log kodu başladığında Trello ve plan kapsamındaki route, filtre ve loglama kurallarını doğrular.
// SOLID: SRP kullanır; sadece S4-08 logger ve admin log contract kontrollerinden sorumludur.
public class Sprint4LoggerAdminContractTests
{
    private static readonly Assembly ApiAssembly = typeof(ApiAdminController).Assembly;

    // S4-08 için temel logger altyapısının Singleton ve servis sözleşmesine uygun olduğunu kontrol eder.
    [Fact]
    public void S408_LoggerInfrastructure_ShouldExposeSingletonServiceContract()
    {
        // Logger altyapısı yoksa CI passed geçer; altyapı varsa contract aktif olur.
        if (!Sprint4CardGuards.S408_LoggerInfrastructureImplemented())
            return;

        var loggerInterface = typeof(ILoggerService);
        var loggerService = typeof(LoggerService);

        Assert.NotNull(loggerInterface.GetMethod("LogInfoAsync"));
        Assert.NotNull(loggerInterface.GetMethod("LogWarningAsync"));
        Assert.NotNull(loggerInterface.GetMethod("LogErrorAsync"));
        Assert.NotNull(loggerInterface.GetMethod("GetLogsAsync"));
        Assert.NotNull(loggerInterface.GetMethod("GetLogCountAsync"));

        Assert.NotNull(loggerService.GetMethod("Initialize", BindingFlags.Public | BindingFlags.Static));
        Assert.NotNull(loggerService.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static));
        Assert.True(
            loggerService.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Any(ctor => ctor.IsPrivate),
            "LoggerService Singleton pattern icin private constructor kullanmali.");

        var serviceSource = ReadProjectFile("HotelApp.Infrastructure", "Services", "LoggerService.cs");
        AssertContainsAny(serviceSource, "Lazy<LoggerService>", "static readonly Lazy", "_instance");
        AssertContainsAny(serviceSource, "DbContextOptionsBuilder", "new HotelAppDbContext");
        AssertContainsAll(serviceSource, "LogInfoAsync", "LogWarningAsync", "LogErrorAsync", "GetLogsAsync", "GetLogCountAsync");
    }

    // S4-08 için Program.cs tarafındaki logger ve global exception handler kayıtlarını kontrol eder.
    [Fact]
    public void S408_Program_ShouldRegisterLoggerSingletonAndExceptionHandler()
    {
        // Logger altyapısı yoksa CI passed geçer; altyapı varsa contract aktif olur.
        if (!Sprint4CardGuards.S408_LoggerInfrastructureImplemented())
            return;

        var programSource = ReadProjectFile("HotelApp.API", "Program.cs");

        Assert.Contains("LoggerService.Initialize", programSource, StringComparison.OrdinalIgnoreCase);
        AssertContainsAll(programSource, "AddSingleton", "ILoggerService");
        AssertContainsAny(programSource, "LoggerService.Instance", "LoggerService . Instance");
        AssertContainsAny(programSource, "AddExceptionHandler<GlobalExceptionHandler>", "AddExceptionHandler");
        Assert.Contains("UseExceptionHandler", programSource, StringComparison.OrdinalIgnoreCase);
    }

    // S4-08 için GlobalExceptionHandler'ın hata loglarını merkezi olarak yazdığını kontrol eder.
    [Fact]
    public void S408_GlobalExceptionHandler_ShouldUseLoggerForUnhandledErrors()
    {
        // Global exception loglama entegrasyonu yoksa CI passed geçer; kod varsa contract aktif olur.
        if (!Sprint4CardGuards.S408_GlobalExceptionLoggingImplemented())
            return;

        var handlerSource = ReadProjectFile("HotelApp.API", "Middleware", "GlobalExceptionHandler.cs");

        AssertContainsAll(handlerSource, "IExceptionHandler", "ILoggerService", "LogErrorAsync");
        AssertContainsAny(handlerSource, "StatusCodes.Status500InternalServerError", "500");
        AssertContainsAny(handlerSource, "InvalidOperationException", "Exception");
        AssertContainsAny(handlerSource, "exceptionDetail", "ex.ToString()", "ExceptionDetail");
        AssertContainsAny(handlerSource, "ClaimTypes.NameIdentifier", "UserId", "userId");
        AssertContainsAny(handlerSource, "ClaimTypes.Email", "UserEmail", "userEmail");
    }

    // S4-08 için AdminController'ın E44 ve E45 log route'larını admin yetkisiyle açtığını kontrol eder.
    [Fact]
    public void S408_AdminController_ShouldExposeE44AndE45LogRoutesForAdmins()
    {
        // Admin log endpoint kodu yoksa CI passed geçer; E44/E45 yazılmaya başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S408_AdminLogEndpointsImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");

        Assert.True(HasRoute(controllerType, "api/admin"),
            "S4-08 kartina gore log endpointleri AdminController altinda /api/admin route'u ile acilmali.");

        var authorize = controllerType.GetCustomAttribute<AuthorizeAttribute>();
        Assert.NotNull(authorize);
        Assert.Contains("Admin", authorize!.Roles ?? string.Empty, StringComparison.OrdinalIgnoreCase);

        var logsAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "logs");
        var logDetailAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "logs/{id}");

        Assert.NotNull(logsAction);
        Assert.NotNull(logDetailAction);
        AssertLogListParameters(logsAction!);
        AssertParameter(logDetailAction!, "id", typeof(int));
    }

    // S4-08 için E44 log listesi endpointinin filtre, sayfalama ve LoggerService kullanımını kontrol eder.
    [Fact]
    public void S408_E44LogList_ShouldUseLoggerFiltersAndPagedResponse()
    {
        // Admin log endpoint kodu yoksa CI passed geçer; E44 yazılmaya başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S408_AdminLogEndpointsImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");
        var logsAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "logs")
            ?? throw new InvalidOperationException("E44 GET /api/admin/logs action'i bulunamadi.");
        var source = ReadProjectFile("HotelApp.API", "Controllers", "AdminController.cs");
        var body = ExtractMethodBodyOrExpression(source, logsAction.Name);
        var implementationEvidence = body
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Domain", "Services", "IAdminService.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Domain", "Services", "AdminService.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Domain", "Services", "ILoggerService.cs");

        AssertContainsAny(implementationEvidence, "ILoggerService", "_logger", "GetLogsAsync");
        AssertContainsAll(body, "level", "dateFrom", "dateTo", "search");
        AssertContainsAny(implementationEvidence, "PagedResponseDto", "PagedResult", "TotalCount", "GetLogCountAsync");
        AssertContainsAny(body, "page", "pageSize", "Skip", "Take");
        AssertContainsAny(body, "Ok(", "return Ok");
        Assert.DoesNotContain("ExceptionDetail", body, StringComparison.OrdinalIgnoreCase);
    }

    // S4-08 için E45 log detay endpointinin id, 404 ve ExceptionDetail davranışını kontrol eder.
    [Fact]
    public void S408_E45LogDetail_ShouldReturnExceptionDetailAndNotFoundWhenMissing()
    {
        // Admin log endpoint kodu yoksa CI passed geçer; E45 yazılmaya başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S408_AdminLogEndpointsImplemented())
            return;

        var controllerType = GetRequiredType("HotelApp.API.Controllers.AdminController");
        var detailAction = FindActionWithHttpAttribute<HttpGetAttribute>(controllerType, "logs/{id}")
            ?? throw new InvalidOperationException("E45 GET /api/admin/logs/{id} action'i bulunamadi.");
        var source = ReadProjectFile("HotelApp.API", "Controllers", "AdminController.cs");
        var body = ExtractMethodBodyOrExpression(source, detailAction.Name);
        var implementationEvidence = body
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Domain", "DTOs", "LogDto.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Domain", "Services", "ILoggerService.cs")
            + Environment.NewLine + ReadOptionalProjectFile("HotelApp.Infrastructure", "Services", "LoggerService.cs");

        AssertParameter(detailAction, "id", typeof(int));
        AssertContainsAny(implementationEvidence, "GetLogById", "FirstOrDefault", "SingleOrDefault", "FindAsync", "GetLogsAsync");
        AssertContainsAny(body, "NotFound", "StatusCode(404", "404");
        Assert.Contains("ExceptionDetail", implementationEvidence, StringComparison.OrdinalIgnoreCase);
        AssertContainsAny(body, "Ok(", "return Ok");
    }

    // S4-08 için Auth dışındaki kritik controller loglama entegrasyonu başladığında kapsamın yeterli olduğunu kontrol eder.
    [Fact]
    public void S408_ControllerLoggingIntegration_ShouldCoverCriticalControllersWhenStarted()
    {
        // Auth dışındaki controller loglaması başlamadıysa CI passed geçer; entegrasyon başlayınca contract aktif olur.
        if (!Sprint4CardGuards.S408_ControllerLoggingIntegrationStarted())
            return;

        var controllersRoot = Path.Combine(FindSolutionRoot(), "HotelApp.API", "Controllers");
        var controllerSources = Directory.GetFiles(controllersRoot, "*.cs", SearchOption.TopDirectoryOnly)
            .Where(path => !string.Equals(Path.GetFileName(path), "AuthController.cs", StringComparison.OrdinalIgnoreCase))
            .Select(path => new
            {
                FileName = Path.GetFileName(path),
                Source = File.ReadAllText(path)
            })
            .ToList();

        var loggedControllers = controllerSources
            .Where(item => ContainsAny(item.Source, "LogInfoAsync", "LogWarningAsync", "LogErrorAsync"))
            .ToList();
        var totalLogCalls = loggedControllers.Sum(item => Regex.Matches(item.Source, @"Log(?:Info|Warning|Error)Async", RegexOptions.IgnoreCase).Count);
        var criticalControllerCount = loggedControllers.Count(item =>
            string.Equals(item.FileName, "AdminController.cs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.FileName, "HotelsController.cs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.FileName, "ReservationController.cs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.FileName, "ReservationsController.cs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.FileName, "ReviewController.cs", StringComparison.OrdinalIgnoreCase)
            || string.Equals(item.FileName, "ReviewsController.cs", StringComparison.OrdinalIgnoreCase));

        Assert.True(criticalControllerCount >= 3,
            "S4-08 kapsaminda Auth disinda en az Admin, Hotels, Reservations/Reviews gibi 3 kritik controller loglanmali.");
        Assert.True(totalLogCalls >= 10,
            "S4-08 planinda tum controller'lara yaklasik 15 log noktasi istenir; en az 10 Log*Async cagrisi beklenir.");

        foreach (var item in loggedControllers)
        {
            Assert.Contains("ILoggerService", item.Source, StringComparison.OrdinalIgnoreCase);
            AssertContainsAny(item.Source, "controller:", "Controller", "nameof(");
            AssertContainsAny(item.Source, "action:", "Action", "Request.Path", "nameof(");
        }
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
            .Any(attribute => RouteEquals(ExpandControllerToken(attribute.Template, controllerType), expectedTemplate));
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
        var normalized = (template ?? string.Empty)
            .Trim()
            .Trim('/')
            .Replace("//", "/", StringComparison.Ordinal)
            .Replace(" ", string.Empty, StringComparison.Ordinal);

        normalized = Regex.Replace(normalized, @"\{([^}:]+):[^}]+\}", "{$1}");
        return normalized;
    }

    private static string ExpandControllerToken(string? template, Type controllerType)
    {
        var controllerName = controllerType.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            ? controllerType.Name[..^"Controller".Length]
            : controllerType.Name;

        return (template ?? string.Empty).Replace("[controller]", controllerName, StringComparison.OrdinalIgnoreCase);
    }

    private static void AssertLogListParameters(MethodInfo method)
    {
        AssertParameter(method, "level", typeof(string), allowNullable: true);
        AssertParameter(method, "dateFrom", typeof(DateTime), allowNullable: true);
        AssertParameter(method, "dateTo", typeof(DateTime), allowNullable: true);
        AssertParameter(method, "search", typeof(string), allowNullable: true);
        AssertParameter(method, "page", typeof(int));
        AssertParameter(method, "pageSize", typeof(int));
    }

    private static void AssertParameter(MethodInfo method, string parameterName, Type expectedType, bool allowNullable = false)
    {
        var parameter = method.GetParameters()
            .FirstOrDefault(candidate => string.Equals(candidate.Name, parameterName, StringComparison.OrdinalIgnoreCase));

        Assert.NotNull(parameter);

        var actualType = Nullable.GetUnderlyingType(parameter!.ParameterType) ?? parameter.ParameterType;
        Assert.True(actualType == expectedType,
            $"{method.Name} action'indaki {parameterName} parametresi {expectedType.Name} tipinde olmali.");

        if (!allowNullable)
            return;

        Assert.True(
            !parameter.ParameterType.IsValueType || Nullable.GetUnderlyingType(parameter.ParameterType) is not null,
            $"{parameterName} parametresi opsiyonel query parametresi olarak nullable olmali.");
    }

    private static string ExtractMethodBodyOrExpression(string source, string methodName)
    {
        var match = Regex.Match(
            source,
            $@"\b(?:public|private|internal|protected)\s+(?:async\s+)?[^\r\n=;]+?\s+{Regex.Escape(methodName)}\s*\([^)]*\)\s*(?<start>\{{|=>)",
            RegexOptions.Multiline);

        Assert.True(match.Success, $"{methodName} metodu bulunamadi.");

        if (match.Groups["start"].Value == "=>")
        {
            var semicolonIndex = source.IndexOf(';', match.Index);
            Assert.True(semicolonIndex >= 0, $"{methodName} expression-bodied metodu icin ; bulunamadi.");
            return source[match.Index..(semicolonIndex + 1)];
        }

        var openBraceIndex = source.IndexOf('{', match.Index);
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

    private static void AssertContainsAny(string text, params string[] expectedParts)
    {
        Assert.True(
            ContainsAny(text, expectedParts),
            $"Beklenen ifadelerden en az biri bulunamadi: {string.Join(", ", expectedParts)}");
    }

    private static void AssertContainsAll(string text, params string[] expectedParts)
    {
        foreach (var expected in expectedParts)
        {
            Assert.Contains(expected, text, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static string ReadProjectFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindSolutionRoot() }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
    }

    private static string ReadOptionalProjectFile(params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindSolutionRoot() }.Concat(relativePathParts).ToArray());

        return File.Exists(path)
            ? File.ReadAllText(path)
            : string.Empty;
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
