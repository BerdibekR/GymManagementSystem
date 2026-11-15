namespace GymManagementSystem.Persistence;

public sealed record TrainerData(
    int Id,
    string Name,
    string Specialization,
    int MaxClients);