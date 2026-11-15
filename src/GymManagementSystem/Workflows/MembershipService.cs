using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Workflows;

public static class MembershipService
{
    public static Result<IReadOnlyList<Membership>> CreateMembership(
        IReadOnlyList<Member> members,
        IReadOnlyList<Membership> memberships,
        int membershipId,
        int memberId,
        MembershipType type,
        DateOnly startDate,
        DateOnly endDate,
        Func<MembershipType, decimal> pricePolicy)
    {
        ArgumentNullException.ThrowIfNull(members);
        ArgumentNullException.ThrowIfNull(memberships);
        ArgumentNullException.ThrowIfNull(pricePolicy);

        var memberLookup = MemberService.FindById(
            members, memberId);

        return memberLookup switch
        {
            Maybe<Member>.Some(var member) =>
                CreateForMember(
                    memberships,
                    membershipId,
                    member,
                    type,
                    startDate,
                    endDate,
                    pricePolicy),

            Maybe<Member>.None =>
                Result<IReadOnlyList<Membership>>.Fail(
                    "MEMBER_NOT_FOUND",
                    "Cannot create membership: member was not found."),

            _ => throw new InvalidOperationException(
                "Unknown Maybe state.")
        };
    }

    private static Result<IReadOnlyList<Membership>> CreateForMember(
        IReadOnlyList<Membership> memberships,
        int membershipId,
        Member member,
        MembershipType type,
        DateOnly startDate,
        DateOnly endDate,
        Func<MembershipType, decimal> pricePolicy)
    {
        if (memberships.Any(m => m.Id == membershipId))
        {
            return Result<IReadOnlyList<Membership>>.Fail(
                "DUPLICATE_MEMBERSHIP_ID",
                "Membership ID already exists.");
        }

        bool hasOverlap = memberships.Any(m =>
            m.MemberId == member.Id &&
            m.StartDate <= endDate &&
            startDate <= m.EndDate);

        if (hasOverlap)
        {
            return Result<IReadOnlyList<Membership>>.Fail(
                "MEMBERSHIP_OVERLAP",
                "Member already has a membership during this period.");
        }

        decimal price = pricePolicy(type);

        return Membership.Create(
                membershipId,
                member.Id,
                type,
                startDate,
                endDate,
                price)
            .Map(membership =>
                (IReadOnlyList<Membership>)
                memberships.Append(membership).ToArray());
    }
}