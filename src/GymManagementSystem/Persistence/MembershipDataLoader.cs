using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class MembershipDataLoader
{
    public static Result<IReadOnlyList<Membership>> LoadMemberships(
        string filePath,
        IReadOnlyList<Member> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        var loadResult =
            JsonFileStore<List<MembershipData>>.Load(filePath);

        if (loadResult is Result<List<MembershipData>>.Failure failure)
            return Result<IReadOnlyList<Membership>>.Fail(
                failure.ErrorCode, failure.Message);

        var data =
            ((Result<List<MembershipData>>.Success)loadResult).Value;

        var memberships = new List<Membership>();

        foreach (var item in data)
        {
            if (item is null)
                return Result<IReadOnlyList<Membership>>.Fail(
                    "INVALID_DATA", "Null membership record.");

            if (!members.Any(m => m.Id == item.MemberId))
                return Result<IReadOnlyList<Membership>>.Fail(
                    "MEMBER_NOT_FOUND",
                    $"Member {item.MemberId} does not exist.");

            if (memberships.Any(m => m.Id == item.Id))
                return Result<IReadOnlyList<Membership>>.Fail(
                    "DUPLICATE_MEMBERSHIP",
                    $"Duplicate membership ID: {item.Id}");

            var result = Membership.Create(
                item.Id,
                item.MemberId,
                item.Type,
                item.StartDate,
                item.EndDate,
                item.Price);

            if (result is Result<Membership>.Failure error)
                return Result<IReadOnlyList<Membership>>.Fail(
                    error.ErrorCode, error.Message);

            memberships.Add(
                ((Result<Membership>.Success)result).Value);
        }

        return Result<IReadOnlyList<Membership>>.Ok(
            memberships.ToArray());
    }
}