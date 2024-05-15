using CapitalPlacement.API.Shared;

namespace CapitalPlacement.API.Exceptions
{
    public static class ResultExtensions
    {
        public static T Match<T, R>(
        this Result<R> result,
        Func<R, T> onSuccess,
        Func<Error, T> onFailure)
        {
            return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
        }

        public static T Match<T>(
        this Result result,
        Func<T> onSuccess,
        Func<Error, T> onFailure)
        {
            return result.IsSuccess ? onSuccess() : onFailure(result.Error);
        }
    }
}
