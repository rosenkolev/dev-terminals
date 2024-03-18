namespace System.Diagnostics.CodeAnalysis;

#pragma warning disable MEN008 // A file's name should match or include the name of the main type it contains.
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1019 // Define accessors for attribute arguments

/// <summary>
/// Specifies that the method or property will ensure that the listed field and property members
/// have not-null values.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullAttribute : Attribute
{
    public MemberNotNullAttribute(string member)
    {
        Members = new[] { member };
    }

    public MemberNotNullAttribute(params string[] members)
    {
        Members = members;
    }

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }
}

/// <summary>
/// Specifies that the method or property will ensure that the listed field and property members
/// have not-null values when returning with the specified return value condition.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
internal sealed class MemberNotNullWhenAttribute : Attribute
{
    public MemberNotNullWhenAttribute(bool returnValue, string member)
    {
        ReturnValue = returnValue;
        Members = new[] { member };
    }

    public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
    {
        ReturnValue = returnValue;
        Members = members;
    }

    public bool ReturnValue { get; }

    /// <summary>Gets field or property member names.</summary>
    public string[] Members { get; }
}

#pragma warning restore MEN008 // A file's name should match or include the name of the main type it contains.
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore CA1019 // Define accessors for attribute arguments
