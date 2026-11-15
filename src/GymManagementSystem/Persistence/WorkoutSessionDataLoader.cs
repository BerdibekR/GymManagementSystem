using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class WorkoutSessionDataLoader
{
    public static Result<IReadOnlyList<WorkoutSession>> LoadSessions(
        string filePath,
        IReadOnlyList<Member> members,
        IReadOnlyList<Trainer> trainers)
    {
        ArgumentNullException.ThrowIfNull(members);
        ArgumentNullException.ThrowIfNull(trainers);

        var loadResult =
            JsonFileStore<List<WorkoutSessionData>>.Load(filePath);

        if (loadResult is Result<List<WorkoutSessionData>>.Failure failure)
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                failure.ErrorCode, failure.Message);

        var data =
            ((Result<List<WorkoutSessionData>>.Success)loadResult).Value;

        var sessions = new List<WorkoutSession>();

        foreach (var item in data)
        {
            if (item is null)
                return Result<IReadOnlyList<WorkoutSession>>.Fail(
                    "INVALID_DATA", "Null workout session record.");

            if (sessions.Any(s => s.Id == item.Id))
                return Result<IReadOnlyList<WorkoutSession>>.Fail(
                    "DUPLICATE_SESSION_ID",
                    $"Duplicate session ID: {item.Id}");

            if (!members.Any(m => m.Id == item.MemberId))
                return Result<IReadOnlyList<WorkoutSession>>.Fail(
                    "MEMBER_NOT_FOUND",
                    $"Member {item.MemberId} does not exist.");

            if (!trainers.Any(t => t.Id == item.TrainerId))
                return Result<IReadOnlyList<WorkoutSession>>.Fail(
                    "TRAINER_NOT_FOUND",
                    $"Trainer {item.TrainerId} does not exist.");

            if (!Enum.IsDefined(item.Status))
                return Result<IReadOnlyList<WorkoutSession>>.Fail(
                    "INVALID_STATUS", "Unknown workout status.");

            var result = WorkoutSession.Create(
                item.Id,
                item.MemberId,
                item.TrainerId,
                item.StartTime,
                item.DurationMinutes,
                item.Intensity);

            if (result is Result<WorkoutSession>.Failure error)
                return Result<IReadOnlyList<WorkoutSession>>.Fail(
                    error.ErrorCode, error.Message);

            var session =
                ((Result<WorkoutSession>.Success)result).Value;

            session = item.Status switch
            {
                WorkoutStatus.Scheduled => session,
                WorkoutStatus.Completed => session.Complete(),
                WorkoutStatus.Cancelled => session.Cancel(),
                _ => session
            };

            sessions.Add(session);
        }

        return Result<IReadOnlyList<WorkoutSession>>.Ok(
            sessions.ToArray());
    }
}