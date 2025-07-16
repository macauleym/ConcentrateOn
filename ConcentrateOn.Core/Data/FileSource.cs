using System.Text.Json;

namespace ConcentrateOn.Core.Data;

public class FileSource<T>(string fileName) : IDataSource<T>
{
    readonly string configurationPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.UserProfile), ".concentrate_on");

    public async Task<List<T>> ReadAsync()
    {
        Directory.CreateDirectory(configurationPath);

        try
        {
            var text = await File.ReadAllTextAsync(Path.Combine(configurationPath, fileName));
            
            return JsonSerializer.Deserialize<List<T>>(text) ?? [];
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"Failed to read through the data in {fileName}.\n{ex.Message}");
            
            return [];
        }
    }

    public Task WriteAsync(List<T> content)
    {
        Directory.CreateDirectory(configurationPath);
        var toWrite = JsonSerializer.Serialize(content);
        
        return File.WriteAllTextAsync(Path.Combine(configurationPath, fileName), toWrite);
    }
}
