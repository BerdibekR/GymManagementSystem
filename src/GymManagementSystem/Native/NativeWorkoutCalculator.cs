using System.Runtime.InteropServices;

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

    public static double CalculateLoad(
        int durationMinutes,
        int intensity)
    {
        var result = CalculateWorkoutLoadNative(
            durationMinutes,
            intensity);

        if (result < 0)
            throw new ArgumentException(
                "Invalid workout parameters.");

        return result;
    }
}