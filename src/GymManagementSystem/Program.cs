using GymManagementSystem.Functional;
using GymManagementSystem.Persistence;

var result = MemberDataLoader.LoadMembers("data/members.json");

switch (result)
{
    case Result<IReadOnlyList<GymManagementSystem.Models.Member>>
        .Success(var members):

        Console.WriteLine("Members loaded successfully!");
        Console.WriteLine($"Total members: {members.Count}");

        foreach (var member in members)
        {
            Console.WriteLine(
                $"{member.Id}. {member.Name}, Age: {member.Age}");
        }

        break;

    case Result<IReadOnlyList<GymManagementSystem.Models.Member>>
        .Failure(var code, var message):

        Console.WriteLine($"Error: {code}");
        Console.WriteLine(message);
        break;
}