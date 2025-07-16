namespace ConcentrateOn.Core.Data;

public class FileSource(string fileName) : IDataSource
{
    readonly string configurationPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.UserProfile), ".concentrate_on");

    public async Task<string> ReadAsync()
    {
        Directory.CreateDirectory(configurationPath);

        try
        {
            var text = await File.ReadAllTextAsync(Path.Combine(configurationPath, fileName));

            return text;
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public Task WriteAsync(string content)
    {
        Directory.CreateDirectory(configurationPath);
        
        return File.WriteAllTextAsync(Path.Combine(configurationPath, fileName), content);
    }
}
