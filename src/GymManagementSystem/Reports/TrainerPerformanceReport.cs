using GymManagementSystem.Models;

namespace GymManagementSystem.Reports;

public sealed record TrainerPerformance(
    int TrainerId,
    string TrainerName,
    int CompletedSessions,
    int TotalMinutes,
    int HighIntensitySessions);

public static class TrainerPerformanceReport
{
    public static IReadOnlyList<TrainerPerformance> Generate(
        IEnumerable<Trainer> trainers,
        IEnumerable<WorkoutSession> sessions)
    {
        ArgumentNullException.ThrowIfNull(trainers);
        ArgumentNullException.ThrowIfNull(sessions);

        var completedSessions = sessions
            .Where(s => s.Status == WorkoutStatus.Completed)
            .ToArray();

        var report = trainers
            .GroupJoin(
                completedSessions,
                trainer => trainer.Id,
                session => session.TrainerId,
                (trainer, trainerSessions) => new TrainerPerformance(
                    trainer.Id,
                    trainer.Name,
                    trainerSessions.Count(),
                    trainerSessions.Sum(s => s.DurationMinutes),
                    trainerSessions.Count(s =>
                        s.Intensity == WorkoutIntensity.High)))
            .OrderByDescending(r => r.CompletedSessions)
            .ThenBy(r => r.TrainerName)
            .ToArray();

        return report;
    }
}