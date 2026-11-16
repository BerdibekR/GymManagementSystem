using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class WorkoutSessionDataSaver
{
    public static Result<bool> SaveSessions(
        string filePath,
        IReadOnlyList<WorkoutSession> sessions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(sessions);

        var data = sessions
            .Select(session => new WorkoutSessionData(
                session.Id,
                session.MemberId,
                session.TrainerId,
                session.StartTime,
                session.DurationMinutes,
                session.Intensity,
                session.Status))
            .ToList();

        return JsonFileStore<List<WorkoutSessionData>>.Save(
            filePath,
            data);
    }
}