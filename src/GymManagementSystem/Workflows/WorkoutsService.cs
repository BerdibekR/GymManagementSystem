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

        var trainer = trainers.FirstOrDefault(t => t.Id == trainerId);

        if (trainer is null)
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "TRAINER_NOT_FOUND",
                "Trainer was not found.");

        var endTime = startTime.AddMinutes(durationMinutes);

        var overlappingSessions = sessions
            .Where(s => s.Status == WorkoutStatus.Scheduled)
            .Where(s =>
                s.StartTime < endTime &&
                startTime < s.StartTime.AddMinutes(s.DurationMinutes))
            .ToArray();

        if (overlappingSessions.Any(s => s.MemberId == memberId))
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "MEMBER_SCHEDULE_CONFLICT",
                "Member already has a workout at this time.");

        var trainerSessions = overlappingSessions
            .Where(s => s.TrainerId == trainerId)
            .ToArray();

        if (trainerSessions.Length >= trainer.MaxClients)
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "TRAINER_CAPACITY_REACHED",
                "Trainer has reached the maximum number of clients.");

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