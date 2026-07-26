namespace E_Commerce.Application.Common
{
    public sealed record Error(string Code, string Description, ErrorType ErrorType = ErrorType.Failure)
    {
        public static Error Failure(string code = "General.Failure", string description = "General failure has occurred!")
            => new(code, description, ErrorType.Failure);
        public static Error Validation(string code = "General.Validation", string description = "General validation has occurred!")
            => new(code, description, ErrorType.Validation);
        public static Error NotFound(string code = "General.NotFound", string description = "Resource not found!")
            => new(code, description, ErrorType.NotFound);
        public static Error Conflict(string code = "General.Conflict", string description = "General conflict has occurred!")
            => new(code, description, ErrorType.Conflict);
        public static Error Unauthorized(string code = "General.Unauthorized", string description = "Access denied! Bad authorization.")
            => new(code, description, ErrorType.Unauthorized);
        public static Error Forbidden(string code = "General.Forbidden", string description = "Operation forbidden!")
            => new(code, description, ErrorType.Forbidden);
        public static Error InvalidCredentials(string code = "General.InvalidCredentials", string description = "Provided credentials are invalid!")
            => new(code, description, ErrorType.InvalidCredentials);
    }

    public enum ErrorType
    {
        Failure = 0,
        Validation= 1,
        NotFound= 2,
        Conflict = 3,
        Unauthorized = 4,
        Forbidden = 5,
        InvalidCredentials= 6
    }
}