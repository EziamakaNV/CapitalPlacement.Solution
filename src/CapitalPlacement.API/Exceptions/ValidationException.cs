using CapitalPlacement.API.Shared;
using System.Runtime.Serialization;

namespace CapitalPlacement.API.Exceptions
{
    public class ValidationException : Exception
    {
        public List<ValidationError> Errors { get; }

        public ValidationException(List<ValidationError> errors)
            : base("One or more validation failures have occured.")
        {
            Errors = errors;
        }

        // A constructor to allow just a message - in case we need to throw a validation exception without specific property errors
        public ValidationException(string message)
            : base(message)
        {
            Errors = new List<ValidationError>();
        }

        // A constructor to allow serialization of the exception
        protected ValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (List<ValidationError>)info.GetValue(nameof(Errors), typeof(List<ValidationError>))!;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Errors), Errors, typeof(List<ValidationError>));
        }
    }
}
