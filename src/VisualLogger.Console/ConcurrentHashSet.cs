using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Console
{
    public class ConcurrentHashSet
    {
        public struct Property
        {
            public Property(string name, Type type)
            {
                Name = name;
                Type = type;
            }

            public string Name { get; set; }
            public Type Type { get; set; }
        }
        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.typebuilder?view=net-6.0
        /// </summary>
        /// <param name="namespace"></param>
        /// <param name="className"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static object? CreateObject(string @namespace, string className, params Property[] properties)
        {
            AssemblyName assemblyName = new AssemblyName(@namespace);
            if (assemblyName.Name == null)
            {
                return null;
            }
            AssemblyBuilder assemblyBuilder =
            AssemblyBuilder.DefineDynamicAssembly(
                assemblyName,
                AssemblyBuilderAccess.Run);

            ModuleBuilder moduleBuilder =
               assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                className,
                TypeAttributes.Public);

            foreach (var property in properties)
            {
                if (string.IsNullOrEmpty(property.Name))
                {
                    continue;
                }
                FieldBuilder fieldBuilder = typeBuilder.DefineField(
                    $"m_{property.Name}",
                    property.Type,
                    FieldAttributes.Private);

                Type[] parameterTypes = { property.Type };
                ConstructorBuilder ctor1 = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    parameterTypes);

                ILGenerator ctor1IL = ctor1.GetILGenerator();

                // For a constructor, argument zero is a reference to the new
                // instance. Push it on the stack before calling the base
                // class constructor. Specify the default constructor of the
                // base class (System.Object) by passing an empty array of
                // types (Type.EmptyTypes) to GetConstructor.
                ctor1IL.Emit(OpCodes.Ldarg_0);
                ctor1IL.Emit(OpCodes.Call,
                    typeof(object).GetConstructor(Type.EmptyTypes));
                // Push the instance on the stack before pushing the argument
                // that is to be assigned to the private field m_number.
                ctor1IL.Emit(OpCodes.Ldarg_0);
                ctor1IL.Emit(OpCodes.Ldarg_1);
                ctor1IL.Emit(OpCodes.Stfld, fieldBuilder);
                ctor1IL.Emit(OpCodes.Ret);

                // Define a default constructor that supplies a default value
                // for the private field. For parameter types, pass the empty
                // array of types or pass null.
                ConstructorBuilder ctor0 = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    Type.EmptyTypes);

                ILGenerator ctor0IL = ctor0.GetILGenerator();
                // For a constructor, argument zero is a reference to the new
                // instance. Push it on the stack before pushing the default
                // value on the stack, then call constructor ctor1.
                ctor0IL.Emit(OpCodes.Ldarg_0);
                ctor0IL.Emit(OpCodes.Ldc_I4_S, 42);
                ctor0IL.Emit(OpCodes.Call, ctor1);
                ctor0IL.Emit(OpCodes.Ret);

                // Define a property named Number that gets and sets the private
                // field.
                //
                // The last argument of DefineProperty is null, because the
                // property has no parameters. (If you don't specify null, you must
                // specify an array of Type objects. For a parameterless property,
                // use the built-in array with no elements: Type.EmptyTypes)
                PropertyBuilder pbNumber = typeBuilder.DefineProperty(
                    property.Name,
                    PropertyAttributes.HasDefault,
                    property.Type,
                    null);

                // The property "set" and property "get" methods require a special
                // set of attributes.
                MethodAttributes getSetAttr = MethodAttributes.Public |
                    MethodAttributes.SpecialName | MethodAttributes.HideBySig;

                // Define the "get" accessor method for Number. The method returns
                // an integer and has no arguments. (Note that null could be
                // used instead of Types.EmptyTypes)
                MethodBuilder mbNumberGetAccessor = typeBuilder.DefineMethod(
                    $"get_{property.Name}",
                    getSetAttr,
                    property.Type,
                    Type.EmptyTypes);

                ILGenerator numberGetIL = mbNumberGetAccessor.GetILGenerator();
                // For an instance property, argument zero is the instance. Load the
                // instance, then load the private field and return, leaving the
                // field value on the stack.
                numberGetIL.Emit(OpCodes.Ldarg_0);
                numberGetIL.Emit(OpCodes.Ldfld, fieldBuilder);
                numberGetIL.Emit(OpCodes.Ret);

                // Define the "set" accessor method for Number, which has no return
                // type and takes one argument of type int (Int32).
                MethodBuilder mbNumberSetAccessor = typeBuilder.DefineMethod(
                    $"set_{property.Name}",
                    getSetAttr,
                    null,
                    parameterTypes);

                ILGenerator numberSetIL = mbNumberSetAccessor.GetILGenerator();
                // Load the instance and then the numeric argument, then store the
                // argument in the field.
                numberSetIL.Emit(OpCodes.Ldarg_0);
                numberSetIL.Emit(OpCodes.Ldarg_1);
                numberSetIL.Emit(OpCodes.Stfld, fieldBuilder);
                numberSetIL.Emit(OpCodes.Ret);

                // Last, map the "get" and "set" accessor methods to the
                // PropertyBuilder. The property is now complete.
                pbNumber.SetGetMethod(mbNumberGetAccessor);
                pbNumber.SetSetMethod(mbNumberSetAccessor);
            }

            // Finish the type.
            Type? type = typeBuilder.CreateType();
            if (type == null)
            {
                return null;
            }
            var instance = Activator.CreateInstance(type);

            return instance;
        }
        /// <summary>
        /// Create a class from scratch.
        /// </summary>
        public static void CreateClass()
        {

            object o = CreateObject("VisualLogger.Console", "TestClass",
                new Property("Age", typeof(int)),
                new Property("Name", typeof(string)),
                new Property("Sex", typeof(bool)));

            var ps = o.GetType().GetProperties();
        }

    }
}