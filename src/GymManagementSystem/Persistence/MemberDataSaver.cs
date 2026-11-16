using GymManagementSystem.Functional;
using GymManagementSystem.Models;

namespace GymManagementSystem.Persistence;

public static class MemberDataSaver
{
    public static Result<bool> SaveMembers(
        string filePath,
        IReadOnlyList<Member> members)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        ArgumentNullException.ThrowIfNull(members);

        var data = members
            .Select(member => new MemberData(
                member.Id,
                member.Name,
                member.Age))
            .ToList();

        return JsonFileStore<List<MemberData>>.Save(
            filePath,
            data);
    }
}