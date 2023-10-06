namespace AJE.UnitTests;
public class Utility
{
    /// <summary>
    /// Convert temp html file names from old style to just yle unique id
    /// TODO: Remove this method when not needed anymore
    /// </summary>
    [Fact]
    public void ConvertTempHtmlFileNames()
    {
        var folder = "/var/aje/yle";
        var files = Directory.GetFiles(folder, "http*.html");
        foreach (var file in files)
        {
            var newFile = file
                .Replace("httpsylefia74", "74-")
                .Replace("originrss", string.Empty);

            File.Move(file, newFile);
        }
    }
}
