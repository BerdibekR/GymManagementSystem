using GymManagementSystem.Models;

namespace GymManagementSystem.Functional;

public static class WorkoutSequence
{
    public static IEnumerable<WorkoutSession> GetScheduledSessions(
        IEnumerable<WorkoutSession> sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);

        foreach (var session in sessions)
        {
            if (session.Status == WorkoutStatus.Scheduled)
            {
                Console.WriteLine(
                    $"Processing session {session.Id}");

                yield return session;
            }
        }
    }

    public static IEnumerable<WorkoutSession> GetHighIntensitySessions(
        IEnumerable<WorkoutSession> sessions)
    {
        ArgumentNullException.ThrowIfNull(sessions);

        foreach (var session in sessions)
        {
            if (session.Intensity == WorkoutIntensity.High)
                yield return session;
        }
    }
}