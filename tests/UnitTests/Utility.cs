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
            if (file.Contains("httpsylefia74"))
            {
                var newFile = file
                    .Replace("httpsylefia74", "74-")
                    .Replace("originrss", string.Empty);
                File.Move(file, newFile);
            }
            else if (file.Contains("httpsylefia3"))
            {
                var newFile = file
                    .Replace("httpsylefia3", "3-")
                    .Replace("originrss", string.Empty);
                File.Move(file, newFile);
            }
            else
            {
                Assert.True(false, $"Can't fix: {file}");
            }
        }
    }
}
