using GymManagementSystem.Models;

namespace GymManagementSystem.Reports;

public sealed record MembershipTypeRevenue(
    MembershipType Type,
    int MembershipCount,
    decimal TotalRevenue,
    decimal AveragePrice);

public sealed record MembershipRevenueSummary(
    int TotalMemberships,
    decimal TotalRevenue,
    IReadOnlyList<MembershipTypeRevenue> RevenueByType);

public static class MembershipRevenueReport
{
    public static MembershipRevenueSummary Generate(
        IEnumerable<Membership> memberships)
    {
        ArgumentNullException.ThrowIfNull(memberships);

        var data = memberships.ToArray();

        var revenueByType = data
            .GroupBy(m => m.Type)
            .Select(group => new MembershipTypeRevenue(
                group.Key,
                group.Count(),
                group.Sum(m => m.Price),
                group.Average(m => m.Price)))
            .OrderByDescending(item => item.TotalRevenue)
            .ThenBy(item => item.Type)
            .ToArray();

        return new MembershipRevenueSummary(
            data.Length,
            data.Sum(m => m.Price),
            revenueByType);
    }
}