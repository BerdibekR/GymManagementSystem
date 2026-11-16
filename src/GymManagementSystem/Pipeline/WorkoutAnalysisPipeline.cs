using GymManagementSystem.Functional;
using GymManagementSystem.Models;
using GymManagementSystem.Native;

namespace GymManagementSystem.Pipeline;

public sealed record WorkoutLoadResult(
    int SessionId,
    int DurationMinutes,
    WorkoutIntensity Intensity,
    double Workload);

public static class WorkoutAnalysisPipeline
{
    private static Result<IReadOnlyList<WorkoutSession>> Validate(
        IReadOnlyList<WorkoutSession> sessions)
    {
        if (sessions is null)
            return Result<IReadOnlyList<WorkoutSession>>.Fail(
                "INVALID_INPUT",
                "Sessions cannot be null.");

        return Result<IReadOnlyList<WorkoutSession>>.Ok(sessions);
    }
    private static IReadOnlyList<WorkoutSession> FilterCompleted(
        IReadOnlyList<WorkoutSession> sessions)
    {
        return sessions
            .Where(s => s.Status == WorkoutStatus.Completed)
            .ToArray();
    }
    private static Result<IReadOnlyList<WorkoutLoadResult>> CalculateLoads(
        IReadOnlyList<WorkoutSession> sessions)
    {
        try
        {
            var results = sessions.Select(session =>
            {
                int intensity = session.Intensity switch
                {
                    WorkoutIntensity.Low => 1,
                    WorkoutIntensity.Medium => 2,
                    WorkoutIntensity.High => 3,
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(session.Intensity))
                };

                double workload = NativeWorkoutCalculator.CalculateLoad(
                    session.DurationMinutes,
                    intensity);

                return new WorkoutLoadResult(
                    session.Id,
                    session.DurationMinutes,
                    session.Intensity,
                    workload);
            }).ToArray();

            return Result<IReadOnlyList<WorkoutLoadResult>>.Ok(results);
        }
        catch (Exception ex) when (
            ex is DllNotFoundException ||
            ex is EntryPointNotFoundException ||
            ex is BadImageFormatException ||
            ex is ArgumentException)
        {
            return Result<IReadOnlyList<WorkoutLoadResult>>.Fail(
                "NATIVE_CALCULATION_ERROR",
                ex.Message);
        }
    }

    public static Result<IReadOnlyList<WorkoutLoadResult>> Run(
        IReadOnlyList<WorkoutSession> sessions)
    {
        return Validate(sessions)
            .Map(FilterCompleted)
            .Bind(CalculateLoads);
    }
}