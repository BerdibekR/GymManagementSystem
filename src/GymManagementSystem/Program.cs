using GymManagementSystem.Functional;
using GymManagementSystem.Models;
using GymManagementSystem.Persistence;

var membersResult =
    MemberDataLoader.LoadMembers("data/members.json");

if (membersResult is Result<IReadOnlyList<Member>>.Failure memberError)
{
    Console.WriteLine($"Member loading error: {memberError.Message}");
    return;
}

var members =
    ((Result<IReadOnlyList<Member>>.Success)membersResult).Value;

var membershipsResult =
    MembershipDataLoader.LoadMemberships(
        "data/memberships.json",
        members);

switch (membershipsResult)
{
    case Result<IReadOnlyList<Membership>>.Success(var memberships):
        Console.WriteLine("Memberships loaded successfully!");
        Console.WriteLine($"Total memberships: {memberships.Count}");

        foreach (var membership in memberships)
        {
            Console.WriteLine(
                $"ID: {membership.Id} | " +
                $"Member ID: {membership.MemberId} | " +
                $"Type: {membership.Type} | " +
                $"Price: {membership.Price}");
        }
        break;

    case Result<IReadOnlyList<Membership>>.Failure(var code, var message):
        Console.WriteLine($"Error: {code}");
        Console.WriteLine(message);
        break;
}