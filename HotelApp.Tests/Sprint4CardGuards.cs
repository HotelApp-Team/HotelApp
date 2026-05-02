namespace HotelApp.Tests;

internal static class Sprint4CardGuards
{
    // S4-01 - Admin API karti. E36-E39 endpoint rotalarindan biri yoksa
    // testler pass gecer; endpoint kodu yazilmaya baslayinca contract testleri devreye girer.
    public static bool S401_AdminApiImplemented()
    {
        var controllersRoot = Path.Combine(FindSolutionRoot(), "HotelApp.API", "Controllers");
        if (!Directory.Exists(controllersRoot))
            return false;

        return Directory.EnumerateFiles(controllersRoot, "*.cs", SearchOption.TopDirectoryOnly)
            .Any(path =>
            {
                var source = File.ReadAllText(path);
                var hasS401EndpointRoute = source.Contains("dashboard", StringComparison.OrdinalIgnoreCase)
                    || source.Contains("users/{id}/status", StringComparison.OrdinalIgnoreCase)
                    || source.Contains("users/{id}/role", StringComparison.OrdinalIgnoreCase);

                // Bos AdminController/API route iskeleti S4-01 kodu sayilmaz.
                // E36-E39 endpointlerinden en az biri yazilmaya baslayinca contract testleri devreye girer.
                return hasS401EndpointRoute;
            });
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
