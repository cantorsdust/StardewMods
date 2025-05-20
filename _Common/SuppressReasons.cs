namespace cantorsdust.Common;

/// <summary>The common reason messages when suppressing static code analysis warnings.</summary>
public static class SuppressReasons
{
    /// <summary>A null check seems to be redundant, but it's not because this is the code that validates the nullability contract.</summary>
    public const string ValidatesNullability = "This code ensures the nullability contract is correct.";

    /// <summary>Code appears to be unused, but it's not because it's accessed via reflection.</summary>
    public const string UsedViaReflection = "This code is accessed via reflection.";
}
