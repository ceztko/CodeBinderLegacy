// SPDX-FileCopyrightText: (C) 2020 Francesco Pretto <ceztko@gmail.com>
// SPDX-License-Identifier: MIT

namespace CodeBinder.Attributes;

public class CodeBinderAttribute : Attribute
{
    public const string ConditionString = "CODE_BINDER";

    protected CodeBinderAttribute() { }
}

/// <summary>
/// Describe a module useful for native code generation and others (eg. JNI)
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ModuleAttribute : CodeBinderAttribute
{
    public ModuleAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }
}

/// <summary>
/// Describe an additional import for the target language
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class ImportAttribute : CodeBinderAttribute
{
    public string Name { get; private set; }

    public string? Condition { get; set; }

    /// <summary>
    /// To be used only in private implementation
    /// </summary>
    public bool Private { get; set; }

    public ImportAttribute(string name)
    {
        Name = name;
    }
}

[Conditional(ConditionString)]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class ConditionAttribute : CodeBinderAttribute
{
    public string Condition { get; private set; }

    public ConditionAttribute(string condition)
    {
        Condition = condition;
    }
}

[Conditional(ConditionString)]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class OrderAttribute : CodeBinderAttribute
{
    public int Order { get; private set; }

    public OrderAttribute([CallerLineNumber]int order = 0)
    {
        Order = order;
    }
}

[Conditional(ConditionString)]
[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method | AttributeTargets.Constructor
    | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct
    | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class IgnoreAttribute : CodeBinderAttribute
{

    public const Conversions test = Conversions.Native;


    public Conversions Conversion { get; private set; }

    public IgnoreAttribute(Conversions conversion = Conversions.All)
    {
        Conversion = conversion;
    }
}

[Conditional(ConditionString)]
[AttributeUsage(AttributeTargets.Delegate | AttributeTargets.Method | AttributeTargets.Constructor
    | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct
    | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class RequiresAttribute : CodeBinderAttribute
{
    public string[] Policies { get; private set; }

    public RequiresAttribute(params string[] policies)
    {
        Policies = policies;
    }
}

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor, Inherited = false)]
public sealed class OverloadBindingAttribute : CodeBinderAttribute
{
    /// <summary>
    /// Suffix for methods, static method name for constructors
    /// </summary>
    public string Stem { get; private set; }

    /// <summary>
    /// Enable this overload binding if the language conversion is missing this feature
    /// </summary>
    public OverloadFeature EnableIfMissing { get; private set; }

    public OverloadBindingAttribute(string stem, OverloadFeature enableIfMissing)
    {
        Stem = stem;
        EnableIfMissing = enableIfMissing;
    }
}

[Flags]
public enum OverloadFeature
{
    None = 0,
    TypeMatch = 1,
    ParameterArity = 2,
    FullSupport = TypeMatch | ParameterArity
}

/// <summary>
/// This attribute tells that the method, parameter or return value that should map to a native counterpart
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public sealed class NativeAttribute : CodeBinderAttribute
{
    public NativeAttribute()
    { }
}

/// <summary>
/// Inerit this class to create a binder of a parameter/return value type to a differently named type
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
public class NativeTypeBinder : CodeBinderAttribute
{
    protected NativeTypeBinder() { }
}

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Enum
    | AttributeTargets.Field | AttributeTargets.Delegate)]
public sealed class NativeBindingAttribute : CodeBinderAttribute
{
    public NativeBindingAttribute(string name)
    {
        Name = name;
    }

    /// <summary>Binded name</summary>
    public string Name { get; private set; }
}

/// <summary>
/// Substitution pattern for enum members, example NativeSubstitution(@"R(\d+)", "$1")
/// </summary>
[AttributeUsage(AttributeTargets.Enum )]
public sealed class NativeSubstitutionAttribute : CodeBinderAttribute
{
    public NativeSubstitutionAttribute(string pattern, string replacement)
    {
        Pattern = pattern;
        Replacement = replacement;
    }

    public string Pattern { get; private set; }
    public string Replacement { get; private set; }
}

/// <summary>
/// Set the return value const
/// </summary>
[AttributeUsage(AttributeTargets.ReturnValue| AttributeTargets.Parameter)]
public sealed class ConstAttribute : CodeBinderAttribute
{
}

/// <summary>
/// This attribute rapresents a stem that is used during the generation.
///
/// Currently used for enums as a prefix for items or max value infix
/// </summary>
public sealed class NativeStemAttribute : CodeBinderAttribute
{
    public NativeStemAttribute(string prefix)
    {
        Prefix = prefix;
    }

    /// <summary>Binded enum prefix</summary>
    public string Prefix { get; private set; }
}

/// <summary>
/// Attribute to specify the library name. Used for example in the export defines in CLang
/// </summary>
public sealed class NativeLibraryAttribute : CodeBinderAttribute
{
    public NativeLibraryAttribute(string name)
    {
        Name = name;
    }

    /// <summary>Binded enum prefix</summary>
    public string Name { get; private set; }
}

/// <summary>
/// Attribute to specify a native library dependency
/// </summary>
public sealed class NativeDependencyAttribute : CodeBinderAttribute
{
    public NativeDependencyAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; private set; }

    /// <summary>
    /// The dependency is not required to be present
    /// </summary>
    public bool Optional { get; set; }
}

/// <summary>
/// Verbatim conversion writing
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method, AllowMultiple = true)]
public sealed class VerbatimConversionAttribute : CodeBinderAttribute
{
    public ConversionType ConversionType { get; private set; }

    public string Verbatim { get; private set; }

    public VerbatimConversionAttribute(string verbatim)
       : this(ConversionType.Implementation, verbatim) { }

    public VerbatimConversionAttribute(ConversionType type, string verbatim)
    {
        ConversionType = type;
        Verbatim = verbatim;
    }
}

public static class Features
{
    public const string Delegates = "{0CA8BB17-A589-41A9-A97C-0E1870837AB4}";
    public const string OperatorOverloading = "{B19FE437-C7FE-41A5-841F-D9170CCEEBA1}";
    public const string ReifiedGenerics = "{E3D821B4-562B-4C9F-8753-1E9C6F4D93A1}";
    public const string Iterators = "{6D4647AE-CA4C-49F0-8BE6-138512992E21}";
    public const string Generators = "{6D4647AE-CA4C-49F0-8BE6-138512992E21}";
    public const string InstanceFinalizers = "{E3645336-1AE0-4D1A-9CCB-D371B8759D1A}";
    public const string GarbageCollection = "{A7FB6B87-B687-40B4-BD70-F7289401D4A4}";
    /// <summary>
    /// Represent functionalities tipically present in all .NET base libraries
    /// </summary>
    public const string DotNet = "{162BC1C4-BEEA-4E30-A525-E084E528232C}";
    /// <summary>
    /// Represent functionalities tipically present only with .NET Framework
    /// </summary>
    public const string NETFramework = "{5DD7ECEA-0F59-4781-BCA2-1DC916EB3CFC}";
    public const string ValueTypes = "{5DD7ECEA-0F59-4781-BCA2-1DC916EB3CFC}";
    public const string PassByRef = "{5B8D618C-93E8-46F2-85FE-DE28FEE86196}";
}

/// <summary>
/// Conversion types
/// </summary>
[Flags]
public enum Conversions
{
    None = 0,
    Native = 1,
    Regular = 2,
    All = Native | Regular,
}
