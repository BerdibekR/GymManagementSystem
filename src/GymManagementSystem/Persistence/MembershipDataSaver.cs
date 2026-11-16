using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class MembershipDataSaver
{
    public static Result<bool> SaveMemberships(
        string filePath,
        IReadOnlyList<Membership> memberships)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(memberships);

        var data = memberships
            .Select(membership => new MembershipData(
                membership.Id,
                membership.MemberId,
                membership.Type,
                membership.StartDate,
                membership.EndDate,
                membership.Price))
            .ToList();

        return JsonFileStore<List<MembershipData>>.Save(
            filePath,
            data);
    }
}