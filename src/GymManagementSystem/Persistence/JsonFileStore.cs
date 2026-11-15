using System.Text.Json;
using GymManagementSystem.Functional;

namespace GymManagementSystem.Persistence;

public static class JsonFileStore<T>
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static Result<bool> Save(string filePath, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(data);

        try
        {
            var directory = Path.GetDirectoryName(
                Path.GetFullPath(filePath));

            if (directory is not null)
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(data, Options);
            File.WriteAllText(filePath, json);

            return Result<bool>.Ok(true);
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is UnauthorizedAccessException ||
            ex is JsonException)
        {
            return Result<bool>.Fail(
                "SAVE_ERROR",
                $"Could not save data: {ex.Message}");
        }
    }

    public static Result<T> Load(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        if (!File.Exists(filePath))
            return Result<T>.Fail(
                "FILE_NOT_FOUND",
                "The requested data file does not exist.");

        try
        {
            var json = File.ReadAllText(filePath);

            var data = JsonSerializer.Deserialize<T>(
                json, Options);

            return data is null
                ? Result<T>.Fail(
                    "EMPTY_DATA",
                    "The JSON file contains no valid data.")
                : Result<T>.Ok(data);
        }
        catch (Exception ex) when (
            ex is IOException ||
            ex is UnauthorizedAccessException ||
            ex is JsonException)
        {
            return Result<T>.Fail(
                "LOAD_ERROR",
                $"Could not load data: {ex.Message}");
        }
    }
}