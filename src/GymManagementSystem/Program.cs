using GymManagementSystem.Functional;
using GymManagementSystem.Models;
using GymManagementSystem.Persistence;

var membersResult =
    MemberDataLoader.LoadMembers("data/members.json");

if (membersResult is Result<IReadOnlyList<Member>>.Failure memberError)
{
    Console.WriteLine($"Member error: {memberError.Message}");
    return;
}

var members =
    ((Result<IReadOnlyList<Member>>.Success)membersResult).Value;

var trainersResult =
    TrainerDataLoader.LoadTrainers("data/trainers.json");

if (trainersResult is Result<IReadOnlyList<Trainer>>.Failure trainerError)
{
    Console.WriteLine($"Trainer error: {trainerError.Message}");
    return;
}

var trainers =
    ((Result<IReadOnlyList<Trainer>>.Success)trainersResult).Value;

var sessionsResult =
    WorkoutSessionDataLoader.LoadSessions(
        "data/workout-sessions.json",
        members,
        trainers);

switch (sessionsResult)
{
    case Result<IReadOnlyList<WorkoutSession>>.Success(var sessions):
        Console.WriteLine("All data loaded successfully!");
        Console.WriteLine($"Members: {members.Count}");
        Console.WriteLine($"Trainers: {trainers.Count}");
        Console.WriteLine($"Workout sessions: {sessions.Count}");

        Console.WriteLine("\nWorkout Sessions:");

        foreach (var session in sessions)
        {
            Console.WriteLine(
                $"ID: {session.Id} | " +
                $"Member: {session.MemberId} | " +
                $"Trainer: {session.TrainerId} | " +
                $"Duration: {session.DurationMinutes} min | " +
                $"Intensity: {session.Intensity} | " +
                $"Status: {session.Status}");
        }

        Console.WriteLine("\n=== Deferred Execution Test ===");

        var lazySessions =
            WorkoutSequence.GetScheduledSessions(sessions);

        Console.WriteLine("Sequence created. No sessions processed yet.");

        Console.WriteLine("\nMaterializing sequence with ToList():");

        var scheduledSessions = lazySessions.ToList();

        Console.WriteLine(
            $"Scheduled sessions loaded: {scheduledSessions.Count}");

        var highIntensitySessions =
            WorkoutSequence.GetHighIntensitySessions(sessions).ToArray();

        Console.WriteLine(
            $"High intensity sessions: {highIntensitySessions.Length}");

        break;

    case Result<IReadOnlyList<WorkoutSession>>.Failure(var code, var message):
        Console.WriteLine($"Error: {code}");
        Console.WriteLine(message);
        break;
}

Console.WriteLine("\n=== Functional Pricing Test ===");

var studentPricing = FunctionalPricing.CreateStudentPricing();
var vipPricing = FunctionalPricing.CreateVIPPricing();

Console.WriteLine("\nStudent Pricing (20% discount):");

foreach (MembershipType type in Enum.GetValues<MembershipType>())
{
    Console.WriteLine($"{type}: {studentPricing(type)} KZT");
}

Console.WriteLine("\nVIP Pricing (10% discount):");

foreach (MembershipType type in Enum.GetValues<MembershipType>())
{
    Console.WriteLine($"{type}: {vipPricing(type)} KZT");
}