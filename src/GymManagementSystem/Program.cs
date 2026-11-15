using GymManagementSystem.Native;

Console.WriteLine("Native C Library Test");

double low = NativeWorkoutCalculator.CalculateLoad(60, 1);
double medium = NativeWorkoutCalculator.CalculateLoad(60, 2);
double high = NativeWorkoutCalculator.CalculateLoad(60, 3);

Console.WriteLine($"Low intensity: {low}");
Console.WriteLine($"Medium intensity: {medium}");
Console.WriteLine($"High intensity: {high}");