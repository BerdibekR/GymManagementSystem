using GymManagementSystem.Functional;

namespace GymManagementSystem.Models;

public enum WorkoutIntensity
{
    Low,
    Medium,
    High
}

public enum WorkoutStatus
{
    Scheduled,
    Completed,
    Cancelled
}

public sealed record WorkoutSession
{
    public int Id { get; }
    public int MemberId { get; }
    public int TrainerId { get; }
    public DateTime StartTime { get; }
    public int DurationMinutes { get; }
    public WorkoutIntensity Intensity { get; }
    public WorkoutStatus Status { get; }

    private WorkoutSession(
        int id,
        int memberId,
        int trainerId,
        DateTime startTime,
        int durationMinutes,
        WorkoutIntensity intensity,
        WorkoutStatus status)
    {
        Id = id;
        MemberId = memberId;
        TrainerId = trainerId;
        StartTime = startTime;
        DurationMinutes = durationMinutes;
        Intensity = intensity;
        Status = status;
    }

    public static Result<WorkoutSession> Create(
        int id,
        int memberId,
        int trainerId,
        DateTime startTime,
        int durationMinutes,
        WorkoutIntensity intensity)
    {
        if (id <= 0 || memberId <= 0 || trainerId <= 0)
            return Result<WorkoutSession>.Fail(
                "INVALID_ID",
                "All IDs must be positive.");

        if (durationMinutes < 15 || durationMinutes > 180)
            return Result<WorkoutSession>.Fail(
                "INVALID_DURATION",
                "Duration must be between 15 and 180 minutes.");

        if (!Enum.IsDefined(intensity))
            return Result<WorkoutSession>.Fail(
                "INVALID_INTENSITY",
                "Workout intensity is invalid.");

        if (startTime == default)
            return Result<WorkoutSession>.Fail(
                "INVALID_START_TIME",
                "Start time is required.");

        return Result<WorkoutSession>.Ok(
            new WorkoutSession(
                id,
                memberId,
                trainerId,
                startTime,
                durationMinutes,
                intensity,
                WorkoutStatus.Scheduled));
    }

    public WorkoutSession Complete() =>
    new WorkoutSession(
        Id,
        MemberId,
        TrainerId,
        StartTime,
        DurationMinutes,
        Intensity,
        WorkoutStatus.Completed);

    public WorkoutSession Cancel() =>
        new WorkoutSession(
            Id,
            MemberId,
            TrainerId,
            StartTime,
            DurationMinutes,
            Intensity,
            WorkoutStatus.Cancelled);
}