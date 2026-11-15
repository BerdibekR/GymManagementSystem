using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Workflows;

public static class MemberService
{
    public static Result<IReadOnlyList<Member>> RegisterMember(
        IReadOnlyList<Member> members,
        int id,
        string name,
        int age)
    {
        ArgumentNullException.ThrowIfNull(members);

        return Member.Create(id, name, age)
            .Bind(member =>
                members.Any(existing => existing.Id == member.Id)
                    ? Result<Member>.Fail(
                        "DUPLICATE_ID",
                        "A member with this ID already exists.")
                    : Result<Member>.Ok(member))
            .Map(member =>
                (IReadOnlyList<Member>)
                members.Append(member).ToArray());
    }

    public static Maybe<Member> FindById(
        IReadOnlyList<Member> members,
        int id)
    {
        ArgumentNullException.ThrowIfNull(members);

        var member = members.FirstOrDefault(
            existing => existing.Id == id);

        return Maybe<Member>.From(member);
    }

    public static int CountMembers(
        IReadOnlyList<Member> members)
    {
        ArgumentNullException.ThrowIfNull(members);

        return members.Count;
    }
}