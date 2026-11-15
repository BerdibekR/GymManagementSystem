using GymManagementSystem.Functional;

namespace GymManagementSystem.Models;

public enum MembershipType
{
    Basic,
    Standard,
    Premium
}

public sealed record Membership
{
    public int Id { get; }
    public int MemberId { get; }
    public MembershipType Type { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    public decimal Price { get; }

    private Membership(
        int id,
        int memberId,
        MembershipType type,
        DateOnly startDate,
        DateOnly endDate,
        decimal price)
    {
        Id = id;
        MemberId = memberId;
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        Price = price;
    }

    public static Result<Membership> Create(
        int id,
        int memberId,
        MembershipType type,
        DateOnly startDate,
        DateOnly endDate,
        decimal price)
    {
        if (id <= 0 || memberId <= 0)
            return Result<Membership>.Fail(
                "INVALID_ID",
                "IDs must be positive.");

        if (!Enum.IsDefined(type))
            return Result<Membership>.Fail(
                "INVALID_TYPE",
                "Membership type is invalid.");

        if (endDate <= startDate)
            return Result<Membership>.Fail(
                "INVALID_DATES",
                "End date must be after start date.");

        if (price <= 0)
            return Result<Membership>.Fail(
                "INVALID_PRICE",
                "Price must be positive.");

        return Result<Membership>.Ok(
            new Membership(
                id, memberId, type,
                startDate, endDate, price));
    }

    public bool IsActive(DateOnly date) =>
        date >= StartDate && date <= EndDate;
}