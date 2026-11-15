namespace GymManagementSystem.Functional;

public static class ResultExtensions
{
    public static Result<TResult> Map<T, TResult>(
        this Result<T> result,
        Func<T, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(mapper);

        return result switch
        {
            Result<T>.Success(var value) =>
                Result<TResult>.Ok(mapper(value)),

            Result<T>.Failure(var code, var message) =>
                Result<TResult>.Fail(code, message),

            _ => throw new InvalidOperationException(
                "Unknown Result state.")
        };
    }

    public static Result<TResult> Bind<T, TResult>(
        this Result<T> result,
        Func<T, Result<TResult>> binder)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentNullException.ThrowIfNull(binder);

        return result switch
        {
            Result<T>.Success(var value) =>
                binder(value),

            Result<T>.Failure(var code, var message) =>
                Result<TResult>.Fail(code, message),

            _ => throw new InvalidOperationException(
                "Unknown Result state.")
        };
    }
}