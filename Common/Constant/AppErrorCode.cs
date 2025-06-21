using LibraryApi.Attribute;

namespace LibraryApi.Common.Constant
{
    public enum AppErrorCode
    {
        [ErrorMeta("An unexpected error occurred.")]
        InternalError = 1500,

        [ErrorMeta("Resource not found.")]
        NotFound = 1404,

        [ErrorMeta("Invalid input.")]
        ValidationError = 1400,

        [ErrorMeta("Unauthorized access.")]
        Unauthorized = 1401,

        [ErrorMeta("Data conflict.")]
        Conflict = 1409
    }
}
