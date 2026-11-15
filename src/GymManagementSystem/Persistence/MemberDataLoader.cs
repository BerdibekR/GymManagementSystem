using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class MemberDataLoader
{
    public static Result<IReadOnlyList<Member>> LoadMembers(
        string filePath)
    {
        var loadResult =
            JsonFileStore<List<MemberData>>.Load(filePath);

        if (loadResult is Result<List<MemberData>>.Failure failure)
        {
            return Result<IReadOnlyList<Member>>.Fail(
                failure.ErrorCode,
                failure.Message);
        }

        var data =
            ((Result<List<MemberData>>.Success)loadResult).Value;

        var members = new List<Member>();

        foreach (var item in data)
        {
            var memberResult = Member.Create(
                item.Id,
                item.Name,
                item.Age);

            if (memberResult is Result<Member>.Failure memberFailure)
            {
                return Result<IReadOnlyList<Member>>.Fail(
                    memberFailure.ErrorCode,
                    memberFailure.Message);
            }

            var member =
                ((Result<Member>.Success)memberResult).Value;

            if (members.Any(m => m.Id == member.Id))
            {
                return Result<IReadOnlyList<Member>>.Fail(
                    "DUPLICATE_ID",
                    $"Duplicate member ID: {member.Id}");
            }

            members.Add(member);
        }

        return Result<IReadOnlyList<Member>>.Ok(
            members.ToArray());
    }
}