namespace GymManagementSystem.Functional;

public abstract record Result<T>
{
    private Result() { }

    public sealed record Success(T Value) : Result<T>;

    public sealed record Failure(
        string ErrorCode,
        string Message
    ) : Result<T>;

    public static Result<T> Ok(T value) =>
        new Success(value);

    public static Result<T> Fail(
        string errorCode,
        string message
    ) =>
        new Failure(errorCode, message);
}