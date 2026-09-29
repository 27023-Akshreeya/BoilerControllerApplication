namespace BoilerContollerApplication.Domain;

public class SystemCrashException : Exception
{
    public SystemCrashException(string message) : base(message) { }
}
