using GymManagementSystem.Native;

namespace GymManagementSystem.Native;

public static class NativeCalculatorTest
{
    public static void Run()
    {
        Console.WriteLine("Native C Library Test");

        Console.WriteLine(
            $"Low intensity: {NativeWorkoutCalculator.CalculateLoad(60, 1)}");

        Console.WriteLine(
            $"Medium intensity: {NativeWorkoutCalculator.CalculateLoad(60, 2)}");

        Console.WriteLine(
            $"High intensity: {NativeWorkoutCalculator.CalculateLoad(60, 3)}");
    }
}