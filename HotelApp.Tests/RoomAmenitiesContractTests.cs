namespace HotelApp.Tests;

public class RoomAmenitiesContractTests
{
    [Fact]
    public void RoomCard_ShouldRenderDynamicRoomAmenities()
    {
        var source = ReadProjectFile("HotelApp.Web", "Views", "Hotels", "_RoomCard.cshtml");

        Assert.Contains("Model.RoomAmenities", source, StringComparison.Ordinal);
        Assert.DoesNotContain("bi bi-wifi\"></i> Wi-Fi", source, StringComparison.Ordinal);
        Assert.DoesNotContain("bi bi-snow\"></i> Klima", source, StringComparison.Ordinal);
        Assert.DoesNotContain("bi bi-tv\"></i> Smart TV", source, StringComparison.Ordinal);
    }

    [Fact]
    public void AvailableRoomViewModel_ShouldExposeRoomAmenities()
    {
        var source = ReadProjectFile("HotelApp.Web", "Models", "AvailableRoomViewModel.cs");

        Assert.Contains("RoomAmenities", source, StringComparison.Ordinal);
        Assert.Contains("List<RoomAmenityViewModel>", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Api_ShouldExposeRoomAmenityEndpoints()
    {
        var roomAmenityController = ReadProjectFile("HotelApp.API", "Controllers", "RoomAmenitiesController.cs");
        var assignmentController = ReadProjectFile("HotelApp.API", "Controllers", "HotelRoomAmenitiesController.cs");

        Assert.Contains("api/room-amenities", roomAmenityController, StringComparison.Ordinal);
        Assert.Contains("HttpGet", roomAmenityController, StringComparison.Ordinal);
        Assert.Contains("HttpPost", roomAmenityController, StringComparison.Ordinal);
        Assert.Contains("HttpPut", roomAmenityController, StringComparison.Ordinal);
        Assert.Contains("HttpDelete", roomAmenityController, StringComparison.Ordinal);

        Assert.Contains("api/hotels/{hotelId}/rooms/{roomId}/amenities", assignmentController, StringComparison.Ordinal);
        Assert.Contains("SetHotelRoomAmenitiesAsync", assignmentController, StringComparison.Ordinal);
        Assert.Contains("RemoveFromHotelRoomAsync", assignmentController, StringComparison.Ordinal);
    }

    [Fact]
    public void AdminHotelEdit_ShouldExposeRoomAmenityAssignment()
    {
        var view = ReadProjectFile("HotelApp.Web", "Views", "Admin", "HotelEdit.cshtml");
        var controller = ReadProjectFile("HotelApp.Web", "Controllers", "HotelMgmtController.cs");

        Assert.Contains("AssignRoomAmenities", view, StringComparison.Ordinal);
        Assert.Contains("roomAmenityIds", view, StringComparison.Ordinal);
        Assert.Contains("Model.AllRoomAmenities", view, StringComparison.Ordinal);
        Assert.Contains("AssignRoomAmenities", controller, StringComparison.Ordinal);
    }

    private static string ReadProjectFile(string projectName, params string[] relativePathParts)
    {
        var path = Path.Combine(new[] { FindProjectRoot(projectName) }.Concat(relativePathParts).ToArray());

        Assert.True(File.Exists(path), $"Dosya bulunamadi: {path}");
        return File.ReadAllText(path);
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
