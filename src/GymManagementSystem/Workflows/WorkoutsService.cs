using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Workflows;

public static class WorkoutsService
{
    public static Result<IReadOnlyList<WorkoutSession>> BookWorkout(
        IReadOnlyList<Member> members,
        IReadOnlyList<Trainer> trainers,
        IReadOnlyList<WorkoutSession> sessions,
        int sessionId,
        int memberId,
        int trainerId,
        DateTime startTime,
        int durationMinutes,
        WorkoutIntensity intensity)
    {
        ArgumentNullException.ThrowIfNull(members);
        ArgumentNullException.ThrowIfNull(trainers);
        ArgumentNullException.ThrowIfNull(sessions);

        if (sessions.Any(s => s.Id == sessionId))
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "DUPLICATE_SESSION",
                "A session with this ID already exists.");

        if (!members.Any(m => m.Id == memberId))
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "MEMBER_NOT_FOUND",
                "Member was not found.");

        if (!trainers.Any(t => t.Id == trainerId))
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "TRAINER_NOT_FOUND",
                "Trainer was not found.");

        var sessionResult = WorkoutSession.Create(
            sessionId,
            memberId,
            trainerId,
            startTime,
            durationMinutes,
            intensity);

        return sessionResult.Map(session =>
            (IReadOnlyList<WorkoutSession>)sessions
                .Append(session)
                .ToArray());
    }
}