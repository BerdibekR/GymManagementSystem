using GymManagementSystem.Functional;
using GymManagementSystem.Models;
using GymManagementSystem.Persistence;
using GymManagementSystem.Workflows;
using GymManagementSystem.Reports;
using GymManagementSystem.Pipeline;

var membersResult =
    MemberDataLoader.LoadMembers("data/members.json");

if (membersResult is Result<IReadOnlyList<Member>>.Failure memberError)
{
    Console.WriteLine($"Member error: {memberError.Message}");
    return;
}

var members =
    ((Result<IReadOnlyList<Member>>.Success)membersResult).Value;

var membershipsResult =
    MembershipDataLoader.LoadMemberships(
        "data/memberships.json",
        members);

if (membershipsResult is Result<IReadOnlyList<Membership>>.Failure membershipError)
{
    Console.WriteLine($"Membership error: {membershipError.Message}");
    return;
}

var memberships =
    ((Result<IReadOnlyList<Membership>>.Success)membershipsResult).Value;

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

        Console.WriteLine("\n=== Native C Workout Load Calculation ===");

        foreach (var session in sessions)
        {
            var loadResult = WorkoutLoadService.CalculateLoad(session);

            switch (loadResult)
            {
                case Result<double>.Success(var load):
                    Console.WriteLine(
                        $"Session {session.Id}: Workout load = {load}");
                    break;

                case Result<double>.Failure(var code, var message):
                    Console.WriteLine(
                        $"Session {session.Id}: {code} - {message}");
                    break;
            }
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
if (sessionsResult is not Result<IReadOnlyList<WorkoutSession>>.Success(var loadedSessions))
{
    return;
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

while (true)
{
    Console.WriteLine("\n========== GYM MANAGEMENT SYSTEM ==========");
    Console.WriteLine("1. View workout sessions");
    Console.WriteLine("2. Calculate workout load");
    Console.WriteLine("3. Register new member");
    Console.WriteLine("4. Create membership");
    Console.WriteLine("5. Book workout");
    Console.WriteLine("6. Complete workout");
    Console.WriteLine("7. Cancel workout");
    Console.WriteLine("8. Membership revenue report");
    Console.WriteLine("9. Trainer performance report");
    Console.WriteLine("10. Analyze completed workouts (Pipeline)");
    Console.WriteLine("0. Exit");
    Console.Write("Choose an option: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine("\n--- Workout Sessions ---");

            foreach (var session in loadedSessions)
            {
                Console.WriteLine(
                    $"ID: {session.Id} | " +
                    $"Member: {session.MemberId} | " +
                    $"Trainer: {session.TrainerId} | " +
                    $"Duration: {session.DurationMinutes} min | " +
                    $"Intensity: {session.Intensity} | " +
                    $"Status: {session.Status}");
            }
            break;

        case "2":
            Console.Write("Enter session ID: ");

            if (int.TryParse(Console.ReadLine(), out int sessionId))
            {
                var selectedSession = loadedSessions
                    .FirstOrDefault(s => s.Id == sessionId);

                if (selectedSession is null)
                {
                    Console.WriteLine("Session not found.");
                    break;
                }

                var result =
                    WorkoutLoadService.CalculateLoad(selectedSession);

                switch (result)
                {
                    case Result<double>.Success(var load):
                        Console.WriteLine($"Workout load: {load}");
                        break;

                    case Result<double>.Failure(var code, var message):
                        Console.WriteLine($"{code}: {message}");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid session ID.");
            }
            break;


        case "3":
            Console.Write("Enter member ID: ");
            if (!int.TryParse(Console.ReadLine(), out int newMemberId))
            {
                Console.WriteLine("Invalid member ID.");
                break;
            }

            Console.Write("Enter member name: ");
            string newMemberName = Console.ReadLine() ?? "";

            Console.Write("Enter member age: ");
            if (!int.TryParse(Console.ReadLine(), out int newMemberAge))
            {
                Console.WriteLine("Invalid age.");
                break;
            }

            var registrationResult = MemberService.RegisterMember(
                members,
                newMemberId,
                newMemberName,
                newMemberAge);

            switch (registrationResult)
            {
                case Result<IReadOnlyList<Member>>.Success(var updatedMembers):
                    var saveResult = MemberDataSaver.SaveMembers(
                        "data/members.json",
                        updatedMembers);

                    switch (saveResult)
                    {
                        case Result<bool>.Success:
                            members = updatedMembers;
                            Console.WriteLine(
                                $"Member {newMemberName} registered successfully!");
                            Console.WriteLine(
                                $"Total members: {members.Count}");
                            break;

                        case Result<bool>.Failure(var saveCode, var saveMessage):
                            Console.WriteLine($"{saveCode}: {saveMessage}");
                            break;
                    }
                    break;

                case Result<IReadOnlyList<Member>>.Failure(var errorCode, var errorMessage):
                    Console.WriteLine($"{errorCode}: {errorMessage}");
                    break;
            }
            break;

        case "4":
            Console.Write("Enter membership ID: ");
            if (!int.TryParse(Console.ReadLine(), out int membershipId))
            {
                Console.WriteLine("Invalid membership ID.");
                break;
            }

            Console.Write("Enter member ID: ");
            if (!int.TryParse(Console.ReadLine(), out int membershipMemberId))
            {
                Console.WriteLine("Invalid member ID.");
                break;
            }

            Console.WriteLine("Membership types: Basic, Standard, Premium");
            Console.Write("Enter membership type: ");

            if (!Enum.TryParse<MembershipType>(
                    Console.ReadLine(), true, out var membershipType) ||
                !Enum.IsDefined(membershipType))
            {
                Console.WriteLine("Invalid membership type.");
                break;
            }

            Console.Write("Enter start date (yyyy-MM-dd): ");
            if (!DateOnly.TryParse(
                    Console.ReadLine(), out var startDate))
            {
                Console.WriteLine("Invalid start date.");
                break;
            }

            Console.Write("Enter end date (yyyy-MM-dd): ");
            if (!DateOnly.TryParse(
                    Console.ReadLine(), out var endDate))
            {
                Console.WriteLine("Invalid end date.");
                break;
            }

            var createResult = MembershipService.CreateMembership(
                members,
                memberships,
                membershipId,
                membershipMemberId,
                membershipType,
                startDate,
                endDate,
                PricingPolicies.StandardPricing);

            switch (createResult)
            {
                case Result<IReadOnlyList<Membership>>.Success(var updatedMemberships):
                    var membershipSave = MembershipDataSaver.SaveMemberships(
                        "data/memberships.json",
                        updatedMemberships);

                    switch (membershipSave)
                    {
                        case Result<bool>.Success:
                            memberships = updatedMemberships;
                            var created = memberships.Single(m => m.Id == membershipId);
                            Console.WriteLine("Membership created successfully!");
                            Console.WriteLine($"Type: {created.Type}");
                            Console.WriteLine($"Price: {created.Price} KZT");
                            break;

                        case Result<bool>.Failure(var saveCode, var saveMessage):
                            Console.WriteLine($"{saveCode}: {saveMessage}");
                            break;
                    }
                    break;

                case Result<IReadOnlyList<Membership>>.Failure(var errorCode, var errorMessage):
                    Console.WriteLine($"{errorCode}: {errorMessage}");
                    break;
            }
            break;

        case "5":
            Console.Write("Enter session ID: ");
            if (!int.TryParse(Console.ReadLine(), out int newSessionId))
            {
                Console.WriteLine("Invalid session ID.");
                break;
            }

            Console.Write("Enter member ID: ");
            if (!int.TryParse(Console.ReadLine(), out int workoutMemberId))
            {
                Console.WriteLine("Invalid member ID.");
                break;
            }

            Console.Write("Enter trainer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int workoutTrainerId))
            {
                Console.WriteLine("Invalid trainer ID.");
                break;
            }

            Console.Write("Enter start date and time (yyyy-MM-dd HH:mm): ");
            if (!DateTime.TryParseExact(
                    Console.ReadLine(),
                    "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var workoutStart))
            {
                Console.WriteLine("Invalid date or time.");
                break;
            }

            Console.Write("Enter duration in minutes: ");
            if (!int.TryParse(Console.ReadLine(), out int workoutDuration))
            {
                Console.WriteLine("Invalid duration.");
                break;
            }

            Console.Write("Enter intensity (Low, Medium, High): ");
            if (!Enum.TryParse<WorkoutIntensity>(
                    Console.ReadLine(), true, out var workoutIntensity) ||
                !Enum.IsDefined(workoutIntensity))
            {
                Console.WriteLine("Invalid intensity.");
                break;
            }

            var bookingResult = WorkoutsService.BookWorkout(
                members,
                trainers,
                loadedSessions,
                newSessionId,
                workoutMemberId,
                workoutTrainerId,
                workoutStart,
                workoutDuration,
                workoutIntensity);

            switch (bookingResult)
            {
                case Result<IReadOnlyList<WorkoutSession>>.Success(var updatedSessions):
                    var saveSessionResult = WorkoutSessionDataSaver.SaveSessions(
                        "data/workout-sessions.json",
                        updatedSessions);

                    switch (saveSessionResult)
                    {
                        case Result<bool>.Success:
                            loadedSessions = updatedSessions;
                            Console.WriteLine("Workout booked successfully!");
                            break;

                        case Result<bool>.Failure(var saveCode, var saveMessage):
                            Console.WriteLine($"{saveCode}: {saveMessage}");
                            break;
                    }
                    break;

                case Result<IReadOnlyList<WorkoutSession>>.Failure(var errorCode, var errorMessage):
                    Console.WriteLine($"{errorCode}: {errorMessage}");
                    break;
            }
            break;

        case "6":
            Console.Write("Enter session ID to complete: ");

            if (!int.TryParse(Console.ReadLine(), out int completeSessionId))
            {
                Console.WriteLine("Invalid session ID.");
                break;
            }

            var completeResult = WorkoutsService.CompleteWorkout(
                loadedSessions,
                completeSessionId);

            switch (completeResult)
            {
                case Result<IReadOnlyList<WorkoutSession>>.Success(var completedSessions):
                    var completeSaveResult = WorkoutSessionDataSaver.SaveSessions(
                        "data/workout-sessions.json",
                        completedSessions);

                    switch (completeSaveResult)
                    {
                        case Result<bool>.Success:
                            loadedSessions = completedSessions;
                            Console.WriteLine("Workout completed successfully!");
                            break;

                        case Result<bool>.Failure(var saveCode, var saveMessage):
                            Console.WriteLine($"{saveCode}: {saveMessage}");
                            break;
                    }
                    break;

                case Result<IReadOnlyList<WorkoutSession>>.Failure(var errorCode, var errorMessage):
                    Console.WriteLine($"{errorCode}: {errorMessage}");
                    break;
            }
            break;

        case "7":
            Console.Write("Enter session ID to cancel: ");

            if (!int.TryParse(Console.ReadLine(), out int cancelSessionId))
            {
                Console.WriteLine("Invalid session ID.");
                break;
            }

            var cancelResult = WorkoutsService.CancelWorkout(
                loadedSessions,
                cancelSessionId);

            switch (cancelResult)
            {
                case Result<IReadOnlyList<WorkoutSession>>.Success(var cancelledSessions):
                    var saveResult = WorkoutSessionDataSaver.SaveSessions(
                        "data/workout-sessions.json",
                        cancelledSessions);

                    switch (saveResult)
                    {
                        case Result<bool>.Success:
                            loadedSessions = cancelledSessions;
                            Console.WriteLine("Workout cancelled successfully!");
                            break;

                        case Result<bool>.Failure(var code, var message):
                            Console.WriteLine($"{code}: {message}");
                            break;
                    }
                    break;

                case Result<IReadOnlyList<WorkoutSession>>.Failure(var errorCode, var errorMessage):
                    Console.WriteLine($"{errorCode}: {errorMessage}");
                    break;
            }
            break;

        case "8":
            Console.WriteLine("\n=== MEMBERSHIP REVENUE REPORT ===");

            var revenueReport =
                MembershipRevenueReport.Generate(memberships);

            Console.WriteLine(
                $"Total memberships: {revenueReport.TotalMemberships}");

            Console.WriteLine(
                $"Total revenue: {revenueReport.TotalRevenue} KZT");

            Console.WriteLine("\nRevenue by membership type:");

            foreach (var item in revenueReport.RevenueByType)
            {
                Console.WriteLine(
                    $"{item.Type}: Count = {item.MembershipCount}, " +
                    $"Revenue = {item.TotalRevenue} KZT, " +
                    $"Average = {item.AveragePrice} KZT");
            }
            break;

        case "9":
            Console.WriteLine("\n=== TRAINER PERFORMANCE REPORT ===");

            var trainerReport =
                TrainerPerformanceReport.Generate(trainers, loadedSessions);

            foreach (var item in trainerReport)
            {
                Console.WriteLine(
                    $"Trainer: {item.TrainerName} | " +
                    $"Completed sessions: {item.CompletedSessions} | " +
                    $"Total minutes: {item.TotalMinutes} | " +
                    $"High intensity: {item.HighIntensitySessions}");
            }
            break;

        case "10":
            Console.WriteLine("\n=== WORKOUT ANALYSIS PIPELINE ===");

            var pipelineResult =
                WorkoutAnalysisPipeline.Run(loadedSessions);

            switch (pipelineResult)
            {
                case Result<IReadOnlyList<WorkoutLoadResult>>.Success(
                    var workloadResults):

                    if (workloadResults.Count == 0)
                    {
                        Console.WriteLine(
                            "No completed workouts available for analysis.");
                        break;
                    }

                    foreach (var item in workloadResults)
                    {
                        Console.WriteLine(
                            $"Session ID: {item.SessionId} | " +
                            $"Duration: {item.DurationMinutes} min | " +
                            $"Intensity: {item.Intensity} | " +
                            $"Workload: {item.Workload}");
                    }

                    Console.WriteLine(
                        $"Total analyzed workouts: {workloadResults.Count}");
                    break;

                case Result<IReadOnlyList<WorkoutLoadResult>>.Failure(
                    var errorCode, var errorMessage):

                    Console.WriteLine(
                        $"Pipeline error: {errorCode} - {errorMessage}");
                    break;
            }
            break;

        case "0":
            Console.WriteLine("Thank you for using Gym Management System!");
            return;

        default:
            Console.WriteLine("Invalid option. Try again.");
            break;
    }
}