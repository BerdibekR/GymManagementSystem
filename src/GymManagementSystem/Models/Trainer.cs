using GymManagementSystem.Functional;

namespace GymManagementSystem.Models;

public sealed record Trainer
{
    public int Id { get; }
    public string Name { get; }
    public string Specialization { get; }
    public int MaxClients { get; }

    private Trainer(
        int id,
        string name,
        string specialization,
        int maxClients)
    {
        Id = id;
        Name = name;
        Specialization = specialization;
        MaxClients = maxClients;
    }

    public static Result<Trainer> Create(
        int id,
        string name,
        string specialization,
        int maxClients)
    {
        if (id <= 0)
            return Result<Trainer>.Fail(
                "INVALID_ID",
                "Trainer ID must be positive.");

        if (string.IsNullOrWhiteSpace(name))
            return Result<Trainer>.Fail(
                "INVALID_NAME",
                "Trainer name cannot be empty.");

        if (string.IsNullOrWhiteSpace(specialization))
            return Result<Trainer>.Fail(
                "INVALID_SPECIALIZATION",
                "Specialization cannot be empty.");

        if (maxClients <= 0)
            return Result<Trainer>.Fail(
                "INVALID_CAPACITY",
                "Maximum clients must be positive.");

        return Result<Trainer>.Ok(
            new Trainer(
                id,
                name.Trim(),
                specialization.Trim(),
                maxClients));
    }
}