using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class TrainerDataLoader
{
    public static Result<IReadOnlyList<Trainer>> LoadTrainers(
        string filePath)
    {
        var loadResult =
            JsonFileStore<List<TrainerData>>.Load(filePath);

        if (loadResult is Result<List<TrainerData>>.Failure failure)
        {
            return Result<IReadOnlyList<Trainer>>.Fail(
                failure.ErrorCode,
                failure.Message);
        }

        var data =
            ((Result<List<TrainerData>>.Success)loadResult).Value;

        var trainers = new List<Trainer>();

        foreach (var item in data)
        {
            var trainerResult = Trainer.Create(
                item.Id,
                item.Name,
                item.Specialization,
                item.MaxClients);

            if (trainerResult is Result<Trainer>.Failure trainerFailure)
            {
                return Result<IReadOnlyList<Trainer>>.Fail(
                    trainerFailure.ErrorCode,
                    trainerFailure.Message);
            }

            var trainer =
                ((Result<Trainer>.Success)trainerResult).Value;

            if (trainers.Any(t => t.Id == trainer.Id))
            {
                return Result<IReadOnlyList<Trainer>>.Fail(
                    "DUPLICATE_TRAINER_ID",
                    $"Duplicate trainer ID: {trainer.Id}");
            }

            trainers.Add(trainer);
        }

        return Result<IReadOnlyList<Trainer>>.Ok(
            trainers.ToArray());
    }
}