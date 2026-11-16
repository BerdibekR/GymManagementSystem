using GymManagementSystem.Functional;
using GymManagementSystem.Models;
using GymManagementSystem.Persistence;
using GymManagementSystem.Workflows;
using GymManagementSystem.Reports;
using GymManagementSystem.Extensions;

namespace GymManagementSystem.Tests;

public class UnitTest1
{
    [Fact]
    public void Member_Create_ValidData_ReturnsSuccess()
    {
        var result = Member.Create(1, "Michael Jordan", 22);

        Assert.IsType<Result<Member>.Success>(result);
    }

    [Fact]
    public void Member_Create_InvalidId_ReturnsFailure()
    {
        var result = Member.Create(0, "Michael Jordan", 22);

        Assert.IsType<Result<Member>.Failure>(result);
    }

    [Fact]
    public void Member_Create_InvalidAge_ReturnsFailure()
    {
        var result = Member.Create(1, "Michael Jordan", 15);

        Assert.IsType<Result<Member>.Failure>(result);
    }

    [Fact]
    public void Member_Create_EmptyName_ReturnsFailure()
    {
        var result = Member.Create(1, "", 22);

        Assert.IsType<Result<Member>.Failure>(result);
    }

    [Fact]
    public void Trainer_Create_ValidData_ReturnsSuccess()
    {
        var result = Trainer.Create(
            1,
            "Cristiano Ronaldo",
            "Strength Training",
            3);

        Assert.IsType<Result<Trainer>.Success>(result);
    }

    [Fact]
    public void Trainer_Create_InvalidCapacity_ReturnsFailure()
    {
        var result = Trainer.Create(
            1,
            "Cristiano Ronaldo",
            "Strength Training",
            0);

        Assert.IsType<Result<Trainer>.Failure>(result);
    }

    [Fact]
    public void Membership_Create_ValidData_ReturnsSuccess()
    {
        var result = Membership.Create(
            1,
            1,
            MembershipType.Premium,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            30000m);

        Assert.IsType<Result<Membership>.Success>(result);
    }

    [Fact]
    public void Membership_Create_InvalidDates_ReturnsFailure()
    {
        var result = Membership.Create(
            1,
            1,
            MembershipType.Basic,
            new DateOnly(2026, 12, 31),
            new DateOnly(2026, 1, 1),
            10000m);

        Assert.IsType<Result<Membership>.Failure>(result);
    }

    [Fact]
    public void WorkoutSession_Create_ValidData_ReturnsSuccess()
    {
        var result = WorkoutSession.Create(
            1,
            1,
            1,
            new DateTime(2026, 11, 17, 10, 0, 0),
            60,
            WorkoutIntensity.High);

        Assert.IsType<Result<WorkoutSession>.Success>(result);
    }

    [Fact]
    public void WorkoutSession_Create_InvalidDuration_ReturnsFailure()
    {
        var result = WorkoutSession.Create(
            1,
            1,
            1,
            new DateTime(2026, 11, 17, 10, 0, 0),
            5,
            WorkoutIntensity.Low);

        Assert.IsType<Result<WorkoutSession>.Failure>(result);
    }

    [Fact]
    public void Result_Map_TransformsSuccessValue()
    {
        Result<int> result = Result<int>.Ok(10);

        var mapped = result.Map(value => value * 2);

        var success = Assert.IsType<Result<int>.Success>(mapped);

        Assert.Equal(20, success.Value);
    }

    [Fact]
    public void Result_Bind_PropagatesFailure()
    {
        Result<int> result =
            Result<int>.Fail("TEST_ERROR", "Test failure");

        var bound = result.Bind(
            value => Result<string>.Ok(value.ToString()));

        var failure =
            Assert.IsType<Result<string>.Failure>(bound);

        Assert.Equal("TEST_ERROR", failure.ErrorCode);
    }

    [Fact]
    public void JsonFileStore_Load_MissingFile_ReturnsFailure()
    {
        var filePath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        var result = JsonFileStore<List<Member>>.Load(filePath);

        var failure =
            Assert.IsType<Result<List<Member>>.Failure>(result);

        Assert.Equal("FILE_NOT_FOUND", failure.ErrorCode);
    }

    [Fact]
    public void JsonFileStore_Load_MalformedJson_ReturnsFailure()
    {
        var filePath = Path.Combine(
            Path.GetTempPath(),
            Guid.NewGuid() + ".json");

        try
        {
            File.WriteAllText(filePath, "{ invalid json }");

            var result =
                JsonFileStore<List<Member>>.Load(filePath);

            Assert.IsType<Result<List<Member>>.Failure>(result);
        }
        finally
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }

    [Fact]
    public void RegisterMember_ValidMember_AddsMemberToList()
    {
        IReadOnlyList<Member> members = Array.Empty<Member>();

        var result = MemberService.RegisterMember(
            members,
            1,
            "Cristiano Ronaldo",
            25);

        var success =
            Assert.IsType<Result<IReadOnlyList<Member>>.Success>(result);

        Assert.Single(success.Value);
        Assert.Equal("Cristiano Ronaldo", success.Value[0].Name);
    }

    [Fact]
    public void RegisterMember_DuplicateId_ReturnsFailure()
    {
        var existingResult = Member.Create(
            1,
            "Michael Jordan",
            22);

        var existing =
            Assert.IsType<Result<Member>.Success>(existingResult).Value;

        IReadOnlyList<Member> members = new[] { existing };

        var result = MemberService.RegisterMember(
            members,
            1,
            "Stephen Curry",
            30);

        Assert.IsType<Result<IReadOnlyList<Member>>.Failure>(result);
    }

    [Fact]
    public void CreateMembership_ValidData_ReturnsSuccess()
    {
        var memberResult = Member.Create(
            1,
            "Cristiano Ronaldo",
            25);

        var member =
            Assert.IsType<Result<Member>.Success>(memberResult).Value;

        IReadOnlyList<Member> members = new[] { member };
        IReadOnlyList<Membership> memberships =
            Array.Empty<Membership>();

        var result = MembershipService.CreateMembership(
            members,
            memberships,
            1,
            1,
            MembershipType.Premium,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            PricingPolicies.StandardPricing);

        var success =
            Assert.IsType<Result<IReadOnlyList<Membership>>.Success>(result);

        Assert.Single(success.Value);
        Assert.Equal(30000m, success.Value[0].Price);
    }

    [Fact]
    public void CreateMembership_MissingMember_ReturnsFailure()
    {
        IReadOnlyList<Member> members = Array.Empty<Member>();
        IReadOnlyList<Membership> memberships =
            Array.Empty<Membership>();

        var result = MembershipService.CreateMembership(
            members,
            memberships,
            1,
            99,
            MembershipType.Basic,
            new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31),
            PricingPolicies.StandardPricing);

        Assert.IsType<Result<IReadOnlyList<Membership>>.Failure>(result);
    }

    [Fact]
    public void BookWorkout_ValidData_AddsSession()
    {
        var memberResult = Member.Create(
            1, "Cristiano Ronaldo", 25);

        var trainerResult = Trainer.Create(
            1, "John Smith", "Strength Training", 3);

        var member =
            Assert.IsType<Result<Member>.Success>(memberResult).Value;

        var trainer =
            Assert.IsType<Result<Trainer>.Success>(trainerResult).Value;

        IReadOnlyList<Member> members = new[] { member };
        IReadOnlyList<Trainer> trainers = new[] { trainer };
        IReadOnlyList<WorkoutSession> sessions =
            Array.Empty<WorkoutSession>();

        var result = WorkoutsService.BookWorkout(
            members,
            trainers,
            sessions,
            1,
            1,
            1,
            new DateTime(2026, 11, 17, 10, 0, 0),
            60,
            WorkoutIntensity.High);

        var success =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Success>(result);

        Assert.Single(success.Value);
        Assert.Equal(WorkoutStatus.Scheduled, success.Value[0].Status);
    }

    [Fact]
    public void CompleteWorkout_ScheduledSession_ChangesStatusToCompleted()
    {
        var sessionResult = WorkoutSession.Create(
            1,
            1,
            1,
            new DateTime(2026, 11, 17, 10, 0, 0),
            60,
            WorkoutIntensity.High);

        var session =
            Assert.IsType<Result<WorkoutSession>.Success>(sessionResult).Value;

        IReadOnlyList<WorkoutSession> sessions = new[] { session };

        var result = WorkoutsService.CompleteWorkout(sessions, 1);

        var success =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Success>(result);

        Assert.Equal(
            WorkoutStatus.Completed,
            success.Value[0].Status);
    }

    [Fact]
    public void BookWorkout_MissingTrainer_ReturnsFailure()
    {
        var member = Assert.IsType<Result<Member>.Success>(
            Member.Create(1, "Michael Jordan", 22)).Value;

        IReadOnlyList<Member> members = new[] { member };
        IReadOnlyList<Trainer> trainers = Array.Empty<Trainer>();
        IReadOnlyList<WorkoutSession> sessions =
            Array.Empty<WorkoutSession>();

        var result = WorkoutsService.BookWorkout(
            members, trainers, sessions,
            1, 1, 99,
            new DateTime(2026, 11, 17, 10, 0, 0),
            60, WorkoutIntensity.High);

        var failure =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Failure>(result);

        Assert.Equal("TRAINER_NOT_FOUND", failure.ErrorCode);
    }

    [Fact]
    public void BookWorkout_MemberScheduleConflict_ReturnsFailure()
    {
        var member = Assert.IsType<Result<Member>.Success>(
            Member.Create(1, "Michael Jordan", 22)).Value;

        var trainer = Assert.IsType<Result<Trainer>.Success>(
            Trainer.Create(1, "John Smith", "Fitness", 3)).Value;

        var existing = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                1, 1, 1,
                new DateTime(2026, 11, 17, 10, 0, 0),
                60, WorkoutIntensity.High)).Value;

        IReadOnlyList<Member> members = new[] { member };
        IReadOnlyList<Trainer> trainers = new[] { trainer };
        IReadOnlyList<WorkoutSession> sessions = new[] { existing };

        var result = WorkoutsService.BookWorkout(
            members, trainers, sessions,
            2, 1, 1,
            new DateTime(2026, 11, 17, 10, 30, 0),
            60, WorkoutIntensity.Medium);

        var failure =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Failure>(result);

        Assert.Equal("MEMBER_SCHEDULE_CONFLICT", failure.ErrorCode);
    }

    [Fact]
    public void BookWorkout_TrainerCapacityReached_ReturnsFailure()
    {
        var member1 = Assert.IsType<Result<Member>.Success>(
            Member.Create(1, "Michael Jordan", 22)).Value;

        var member2 = Assert.IsType<Result<Member>.Success>(
            Member.Create(2, "Stephen Curry", 30)).Value;

        var trainer = Assert.IsType<Result<Trainer>.Success>(
            Trainer.Create(1, "John Smith", "Fitness", 1)).Value;

        var existing = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                1, 1, 1,
                new DateTime(2026, 11, 17, 10, 0, 0),
                60, WorkoutIntensity.High)).Value;

        IReadOnlyList<Member> members = new[] { member1, member2 };
        IReadOnlyList<Trainer> trainers = new[] { trainer };
        IReadOnlyList<WorkoutSession> sessions = new[] { existing };

        var result = WorkoutsService.BookWorkout(
            members, trainers, sessions,
            2, 2, 1,
            new DateTime(2026, 11, 17, 10, 30, 0),
            60, WorkoutIntensity.Medium);

        var failure =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Failure>(result);

        Assert.Equal("TRAINER_CAPACITY_REACHED", failure.ErrorCode);
    }

    [Fact]
    public void MembershipRevenueReport_Generate_CalculatesCorrectTotals()
    {
        var first = Assert.IsType<Result<Membership>.Success>(
            Membership.Create(
                1, 1, MembershipType.Basic,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31),
                10000m)).Value;

        var second = Assert.IsType<Result<Membership>.Success>(
            Membership.Create(
                2, 2, MembershipType.Basic,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31),
                12000m)).Value;

        var third = Assert.IsType<Result<Membership>.Success>(
            Membership.Create(
                3, 3, MembershipType.Premium,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31),
                30000m)).Value;

        var report = MembershipRevenueReport.Generate(
            new[] { first, second, third });

        Assert.Equal(3, report.TotalMemberships);
        Assert.Equal(52000m, report.TotalRevenue);

        var basic = report.RevenueByType.Single(
            item => item.Type == MembershipType.Basic);

        Assert.Equal(2, basic.MembershipCount);
        Assert.Equal(22000m, basic.TotalRevenue);
        Assert.Equal(11000m, basic.AveragePrice);

        var premium = report.RevenueByType.Single(
            item => item.Type == MembershipType.Premium);

        Assert.Equal(1, premium.MembershipCount);
        Assert.Equal(30000m, premium.TotalRevenue);
    }


    [Fact]
    public void MembershipRevenueReport_Generate_OrdersByRevenueDescending()
    {
        var basic = Assert.IsType<Result<Membership>.Success>(
            Membership.Create(
                1, 1, MembershipType.Basic,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31),
                10000m)).Value;

        var premium = Assert.IsType<Result<Membership>.Success>(
            Membership.Create(
                2, 2, MembershipType.Premium,
                new DateOnly(2026, 1, 1),
                new DateOnly(2026, 12, 31),
                30000m)).Value;

        var report = MembershipRevenueReport.Generate(
            new[] { basic, premium });

        Assert.Equal(
            MembershipType.Premium,
            report.RevenueByType[0].Type);

        Assert.Equal(
            MembershipType.Basic,
            report.RevenueByType[1].Type);
    }

    [Fact]
    public void TrainerPerformanceReport_Generate_CalculatesCorrectStatistics()
    {
        var trainer = Assert.IsType<Result<Trainer>.Success>(
            Trainer.Create(1, "Cristiano Ronaldo", "Fitness", 3)).Value;

        var session1 = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                1, 1, 1,
                new DateTime(2026, 11, 17, 10, 0, 0),
                60, WorkoutIntensity.High)).Value.Complete();

        var session2 = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                2, 2, 1,
                new DateTime(2026, 11, 18, 10, 0, 0),
                45, WorkoutIntensity.Medium)).Value.Complete();

        var session3 = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                3, 3, 1,
                new DateTime(2026, 11, 19, 10, 0, 0),
                30, WorkoutIntensity.High)).Value;

        var report = TrainerPerformanceReport.Generate(
            new[] { trainer },
            new[] { session1, session2, session3 });

        var result = Assert.Single(report);

        Assert.Equal(2, result.CompletedSessions);
        Assert.Equal(105, result.TotalMinutes);
        Assert.Equal(1, result.HighIntensitySessions);
    }

    [Fact]
    public void TrainerPerformanceReport_Generate_IncludesTrainerWithoutSessions()
    {
        var trainer = Assert.IsType<Result<Trainer>.Success>(
            Trainer.Create(1, "John Smith", "Fitness", 3)).Value;

        var report = TrainerPerformanceReport.Generate(
            new[] { trainer },
            Array.Empty<WorkoutSession>());

        var result = Assert.Single(report);

        Assert.Equal(0, result.CompletedSessions);
        Assert.Equal(0, result.TotalMinutes);
        Assert.Equal(0, result.HighIntensitySessions);
    }

    [Fact]
    public void MemberExtensions_IsAdult_ReturnsCorrectResult()
    {
        var adult = Assert.IsType<Result<Member>.Success>(
            Member.Create(1, "Michael Jordan", 22)).Value;

        var minor = Assert.IsType<Result<Member>.Success>(
            Member.Create(2, "Young Member", 16)).Value;

        Assert.True(adult.IsAdult());
        Assert.False(minor.IsAdult());
    }

    [Fact]
    public void MemberExtensions_ToDisplayLabel_ReturnsFormattedLabel()
    {
        var member = Assert.IsType<Result<Member>.Success>(
            Member.Create(1, "Michael Jordan", 22)).Value;

        var label = member.ToDisplayLabel();

        Assert.Equal("Michael Jordan (ID: 1)", label);
    }

    [Theory]
    [InlineData(WorkoutIntensity.Low, 60, 60.0)]
    [InlineData(WorkoutIntensity.Medium, 60, 90.0)]
    [InlineData(WorkoutIntensity.High, 60, 120.0)]
    public void WorkoutLoadService_CalculateLoad_ReturnsCorrectValue(
        WorkoutIntensity intensity,
        int duration,
        double expected)
    {
        var session = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                100,
                1,
                1,
                DateTime.Now,
                duration,
                intensity)).Value;

        var result = WorkoutLoadService.CalculateLoad(session);

        var success = Assert.IsType<Result<double>.Success>(result);
        Assert.Equal(expected, success.Value);
    }

    [Fact]
    public void CancelWorkout_ScheduledSession_CancelsSuccessfully()
    {
        var member = Assert.IsType<Result<Member>.Success>(
            Member.Create(1, "Michael Jordan", 22)).Value;

        var trainer = Assert.IsType<Result<Trainer>.Success>(
            Trainer.Create(1, "John Smith", "Fitness", 3)).Value;

        var session = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                1, 1, 1, DateTime.Now.AddDays(1), 60,
                WorkoutIntensity.Medium)).Value;

        var result = WorkoutsService.CancelWorkout(
            new[] { session }, 1);

        var updatedSessions =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Success>(
                result).Value;

        Assert.Equal(WorkoutStatus.Cancelled, updatedSessions[0].Status);
    }

    [Fact]
    public void CancelWorkout_CompletedSession_ReturnsFailure()
    {
        var session = Assert.IsType<Result<WorkoutSession>.Success>(
            WorkoutSession.Create(
                1, 1, 1, DateTime.Now.AddDays(1), 60,
                WorkoutIntensity.Medium)).Value.Complete();

        var result = WorkoutsService.CancelWorkout(
            new[] { session }, 1);

        var failure =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Failure>(
                result);

        Assert.Equal("INVALID_SESSION_STATUS", failure.ErrorCode);
    }

    [Fact]
    public void CancelWorkout_MissingSession_ReturnsFailure()
    {
        var result = WorkoutsService.CancelWorkout(
            Array.Empty<WorkoutSession>(), 999);

        var failure =
            Assert.IsType<Result<IReadOnlyList<WorkoutSession>>.Failure>(
                result);

        Assert.Equal("SESSION_NOT_FOUND", failure.ErrorCode);
    }
}