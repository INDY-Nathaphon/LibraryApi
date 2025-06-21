namespace LibraryApi.Attribute
{
    [System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false)]
    public class ErrorMetaAttribute : System.Attribute
    {
        public string Message { get; }

        public ErrorMetaAttribute(string message)
        {
            Message = message;
        }
    }
}
