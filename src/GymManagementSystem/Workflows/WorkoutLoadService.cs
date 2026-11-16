using GymManagementSystem.Functional;
using GymManagementSystem.Models;
using GymManagementSystem.Native;

namespace GymManagementSystem.Workflows;

public static class WorkoutLoadService
{
    public static Result<double> CalculateLoad(WorkoutSession session)
    {
        ArgumentNullException.ThrowIfNull(session);

        try
        {
            int intensity = session.Intensity switch
            {
                WorkoutIntensity.Low => 1,
                WorkoutIntensity.Medium => 2,
                WorkoutIntensity.High => 3,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(session.Intensity))
            };

            double load = NativeWorkoutCalculator.CalculateLoad(
                session.DurationMinutes,
                intensity);

            return Result<double>.Ok(load);
        }
        catch (DllNotFoundException)
        {
            return Result<double>.Fail(
                "NATIVE_LIBRARY_NOT_FOUND",
                "Workout calculator DLL was not found.");
        }
        catch (EntryPointNotFoundException)
        {
            return Result<double>.Fail(
                "NATIVE_FUNCTION_NOT_FOUND",
                "Native calculation function was not found.");
        }
        catch (BadImageFormatException)
        {
            return Result<double>.Fail(
                "NATIVE_LIBRARY_INVALID",
                "Native library has an incompatible format.");
        }
        catch (ArgumentException ex)
        {
            return Result<double>.Fail(
                "INVALID_WORKOUT_PARAMETERS",
                ex.Message);
        }
    }
}