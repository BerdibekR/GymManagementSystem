using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public sealed record MembershipData(
    int Id,
    int MemberId,
    MembershipType Type,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal Price);