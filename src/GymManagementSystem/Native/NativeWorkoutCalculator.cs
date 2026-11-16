using System.Runtime.InteropServices;
using GymManagementSystem.Functional;

namespace GymManagementSystem.Native;

public static class NativeWorkoutCalculator
{
    [DllImport(
        "workout_calculator.dll",
        EntryPoint = "calculate_workout_load",
        CallingConvention = CallingConvention.Cdecl)]
    private static extern double CalculateWorkoutLoadNative(
        int durationMinutes,
        int intensity);

    public static Result<double> TryCalculateLoad(
        int durationMinutes,
        int intensity)
    {
        try
        {
            var result = CalculateWorkoutLoadNative(
                durationMinutes,
                intensity);

            if (result < 0)
            {
                return Result<double>.Fail(
                    "INVALID_WORKOUT_PARAMETERS",
                    "Native calculator received invalid workout parameters.");
            }

            return Result<double>.Ok(result);
        }
        catch (DllNotFoundException)
        {
            return Result<double>.Fail(
                "NATIVE_LIBRARY_NOT_FOUND",
                "The native workout calculator DLL was not found.");
        }
        catch (EntryPointNotFoundException)
        {
            return Result<double>.Fail(
                "NATIVE_FUNCTION_NOT_FOUND",
                "The required function was not found in the native library.");
        }
        catch (BadImageFormatException)
        {
            return Result<double>.Fail(
                "INVALID_NATIVE_LIBRARY",
                "The native library has an incompatible format.");
        }
    }

    public static double CalculateLoad(
        int durationMinutes,
        int intensity)
    {
        var result = TryCalculateLoad(durationMinutes, intensity);

        return result switch
        {
            Result<double>.Success success => success.Value,

            Result<double>.Failure failure =>
                throw new InvalidOperationException(failure.Message),

            _ => throw new InvalidOperationException(
                "Unknown native calculation result.")
        };
    }
}