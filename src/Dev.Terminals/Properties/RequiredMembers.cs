namespace System.Runtime.CompilerServices;

#pragma warning disable MEN008 // A file's name should match or include the name of the main type it contains.
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1019 // Define accessors for attribute arguments

[AttributeUsage(
    AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false)]
public sealed class RequiredMemberAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
    public CompilerFeatureRequiredAttribute(string name)
    {
    }
}

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class SetsRequiredMembersAttribute : Attribute
{
}

#pragma warning restore MEN008 // A file's name should match or include the name of the main type it contains.
#pragma warning restore SA1649 // File name should match first type name
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore CA1019 // Define accessors for attribute arguments
