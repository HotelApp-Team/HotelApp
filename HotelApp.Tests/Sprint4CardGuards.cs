namespace HotelApp.Tests;

internal static class Sprint4CardGuards
{
    // S4-01 - Admin API karti. E36-E39 endpoint rotalarindan biri yoksa
    // testler pass gecer; endpoint kodu yazilmaya baslayinca contract testleri devreye girer.
    public static bool S401_AdminApiImplemented()
    {
        return S401_E36DashboardImplemented()
            || S401_E37UsersImplemented()
            || S401_E38StatusImplemented()
            || S401_E39RoleImplemented();
    }

    public static bool S401_E36DashboardImplemented()
    {
        return AdminControllerSourceContains("dashboard");
    }

    public static bool S401_E37UsersImplemented()
    {
        var source = ReadAdminControllerSource();

        return source.Contains("HttpGet(\"users", StringComparison.OrdinalIgnoreCase)
            || source.Contains("HttpGet(\"/users", StringComparison.OrdinalIgnoreCase)
            || source.Contains("GetUsers", StringComparison.OrdinalIgnoreCase);
    }

    public static bool S401_E38StatusImplemented()
    {
        return AdminControllerSourceContains("users/{id}/status");
    }

    public static bool S401_E39RoleImplemented()
    {
        var source = ReadAdminControllerSource();

        return source.Contains("users/{id}/roles", StringComparison.OrdinalIgnoreCase)
            || source.Contains("users/{id}/role", StringComparison.OrdinalIgnoreCase);
    }

    private static bool AdminControllerSourceContains(string expected)
    {
        return ReadAdminControllerSource().Contains(expected, StringComparison.OrdinalIgnoreCase);
    }

    private static string ReadAdminControllerSource()
    {
        var controllersRoot = Path.Combine(FindSolutionRoot(), "HotelApp.API", "Controllers");
        if (!Directory.Exists(controllersRoot))
            return string.Empty;

        var adminControllerPath = Path.Combine(controllersRoot, "AdminController.cs");

        return File.Exists(adminControllerPath)
            ? File.ReadAllText(adminControllerPath)
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
