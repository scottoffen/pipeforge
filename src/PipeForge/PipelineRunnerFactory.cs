#if NET5_0_OR_GREATER
using System.Reflection;
using System.Reflection.Emit;

namespace PipeForge;

internal static class PipelineRunnerFactory
{
    private static readonly AssemblyBuilder _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("PipeForge.DynamicPipelines"), AssemblyBuilderAccess.Run);

    private static readonly ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule("MainModule");

    private static readonly Type[] _parameterTypes = [typeof(IServiceProvider)];

    public static Type CreatePipelineRunner<TContext, TStepInterface, TRunnerInterface>()
        where TContext : class
        where TStepInterface : IPipelineStep<TContext>
        where TRunnerInterface : IPipelineRunner<TContext, TStepInterface>
    {
        if (!typeof(TRunnerInterface).IsInterface)
            throw new InvalidOperationException("TRunnerInterface must be an interface.");

        var typeBuilder = _moduleBuilder.DefineType(
            $"PipeForge.DynamicPipelines.{typeof(TRunnerInterface).Name}_Proxy",
            TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class,
            typeof(PipelineRunner<TContext, TStepInterface>));

        typeBuilder.AddInterfaceImplementation(typeof(TRunnerInterface));

        // Implement constructor(s) from base class
        ImplementConstructor(typeBuilder, typeof(PipelineRunner<TContext, TStepInterface>));

        return typeBuilder.CreateTypeInfo()!;
    }

    private static void ImplementConstructor(TypeBuilder typeBuilder, Type baseType)
    {
        var baseCtor = baseType.GetConstructor(_parameterTypes)
            ?? throw new InvalidOperationException("Expected base constructor with IServiceProvider parameter.");

        var ctorBuilder = typeBuilder.DefineConstructor(
            MethodAttributes.Public,
            CallingConventions.Standard,
            _parameterTypes);

        var il = ctorBuilder.GetILGenerator();

        // Call base constructor
        il.Emit(OpCodes.Ldarg_0); // this
        il.Emit(OpCodes.Ldarg_1); // IServiceProvider
        il.Emit(OpCodes.Call, baseCtor);
        il.Emit(OpCodes.Ret);
    }
}
#endif
