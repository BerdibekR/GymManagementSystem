using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public sealed record WorkoutSessionData(
    int Id,
    int MemberId,
    int TrainerId,
    DateTime StartTime,
    int DurationMinutes,
    WorkoutIntensity Intensity,
    WorkoutStatus Status);