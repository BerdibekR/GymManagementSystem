using GymManagementSystem.Functional;

namespace GymManagementSystem.Models;

public sealed record Member
{
    public int Id { get; }
    public string Name { get; }
    public int Age { get; }

    private Member(int id, string name, int age)
    {
        Id = id;
        Name = name;
        Age = age;
    }

    public static Result<Member> Create(
        int id,
        string name,
        int age)
    {
        if (id <= 0)
            return Result<Member>.Fail(
                "INVALID_ID",
                "Member ID must be positive.");

        if (string.IsNullOrWhiteSpace(name))
            return Result<Member>.Fail(
                "INVALID_NAME",
                "Member name cannot be empty.");

        if (age < 16 || age > 100)
            return Result<Member>.Fail(
                "INVALID_AGE",
                "Member age must be between 16 and 100.");

        var member = new Member(
            id,
            name.Trim(),
            age);

        return Result<Member>.Ok(member);
    }
}