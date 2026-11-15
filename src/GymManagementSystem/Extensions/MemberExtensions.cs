using GymManagementSystem.Models;

namespace GymManagementSystem.Extensions;

public static class MemberExtensions
{
    public static bool IsAdult(this Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        return member.Age >= 18;
    }

    public static string ToDisplayLabel(this Member member)
    {
        ArgumentNullException.ThrowIfNull(member);
        return $"{member.Name} (ID: {member.Id})";
    }
}