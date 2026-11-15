using GymManagementSystem.Functional;
using GymManagementSystem.Persistence;
using GymManagementSystem.Models;

var result = TrainerDataLoader.LoadTrainers("data/trainers.json");

switch (result)
{
    case Result<IReadOnlyList<Trainer>>.Success(var trainers):
        Console.WriteLine("Trainers loaded successfully!");
        Console.WriteLine($"Total trainers: {trainers.Count}");

        foreach (var trainer in trainers)
        {
            Console.WriteLine(
                $"{trainer.Id}. {trainer.Name} | " +
                $"{trainer.Specialization} | " +
                $"Max clients: {trainer.MaxClients}");
        }
        break;

    case Result<IReadOnlyList<Trainer>>.Failure(var code, var message):
        Console.WriteLine($"Error: {code}");
        Console.WriteLine(message);
        break;
}