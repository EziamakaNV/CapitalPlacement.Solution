namespace CapitalPlacement.API.Shared
{
    public class Result
    {
        private Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None ||
            !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        internal static Result Failure(object errorUpdatingReview)
        {
            throw new NotImplementedException();
        }
    }

    public class Result<T>
    {
        private readonly T _value;

        private Result(bool isSuccess, T value, Error error)
        {
            if (isSuccess && error != Error.None)
            {
                throw new ArgumentException("Successful result must not have an error", nameof(error));
            }

            if (!isSuccess && error == Error.None)
            {
                throw new ArgumentException("Failure must have an error", nameof(error));
            }

            IsSuccess = isSuccess;
            _value = value;
            Error = error;
        }

        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        public T Value
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException("There is no value for failure.");
                }

                return _value;
            }
        }

        public static Result<T> Success(T value) => new(true, value, Error.None);

        public static Result<T> Failure(Error error) => new(false, default!, error);
    }
}
