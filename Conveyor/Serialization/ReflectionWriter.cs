using System;
using System.Collections.Generic;
using System.Reflection;

namespace Conveyor.Serialization
{
    partial class ExpressionSerializer
    {
        private static readonly Dictionary<Type, Tuple<string, string, Type[]>>
                                _cachedTypes = new Dictionary<Type, Tuple<string, string, Type[]>>();

        /// <summary>
        /// Invokes <see cref="Conveyor.Serialization.ExpressionSerializer.WriteType(Type)"/>.
        /// </summary>
        /// <param name="type">Type to be serialized</param>
        /// <returns>An <see cref="System.Action"/> instance</returns>
        private Action InvokeTypeWriter(Type type) => () => WriteType(type);

        /// <summary>
        /// Writes/serializes given type information.
        /// </summary>
        /// <param name="type">Type to be serialized</param>
        private void WriteType(Type type)
        {
            if (type == null)
            {
                this._jsonWriter.WriteNull();
            }
            else
            {
                Tuple<string, string, Type[]> tuple;
                if (!_cachedTypes.TryGetValue(type, out tuple))
                {
                    string assemblyName = type.Assembly.FullName;
                    if (type.IsGenericType)
                    {
                        Type genericTypeDefinition = type.GetGenericTypeDefinition();
                        tuple = new Tuple<string, string, Type[]>(
                                            genericTypeDefinition.Assembly.FullName, 
                                            genericTypeDefinition.FullName,
                                            type.GetGenericArguments()
                                        );
                    }
                    else
                    {
                        tuple = new Tuple<string, string, Type[]>(assemblyName, type.FullName, null);
                    }
                    _cachedTypes[type] = tuple;
                }

                this._jsonWriter.WriteStartObject();
                this.WriteKeyValuePair("assemblyName", tuple.Item1);
                this.WriteKeyValuePair("typeName", tuple.Item2);
                this.WriteKeyValuePair("genericArguments", this.Apply(tuple.Item3, this.InvokeTypeWriter));
                this._jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Invokes <see cref="Conveyor.Serialization.ExpressionSerializer.WriteConstructor(ConstructorInfo)"/>.
        /// </summary>
        /// <param name="constructor">Constructor instance to be serialized</param>
        /// <returns>An <see cref="System.Action"/> instance</returns>
        private Action InvokeConstructorWriter(ConstructorInfo constructor) => () => WriteConstructor(constructor);

        /// <summary>
        /// Writes given constructor's information.
        /// </summary>
        /// <param name="constructor">Constructor instance to be serialized</param>
        private void WriteConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
            {
                this._jsonWriter.WriteNull();
            }
            else
            {
                this._jsonWriter.WriteStartObject();
                this.WriteKeyValuePair("type", this.InvokeTypeWriter(constructor.DeclaringType));
                this.WriteKeyValuePair("name", constructor.Name);
                this.WriteKeyValuePair("signature", constructor.ToString());
                this._jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Invokes <see cref="Conveyor.Serialization.ExpressionSerializer.WriteMethod(MethodInfo)"/>.
        /// </summary>
        /// <param name="method">Method instance to be serialized</param>
        /// <returns>An <see cref="System.Action"/> instance</returns>
        private Action InvokeMethodWriter(MethodInfo method) => () => this.WriteMethod(method);

        /// <summary>
        /// Writes given method's information.
        /// </summary>
        /// <param name="method">Method instance to be serialized</param>
        private void WriteMethod(MethodInfo method)
        {
            if (method == null)
            {
                this._jsonWriter.WriteNull();
            }
            else
            {
                this._jsonWriter.WriteStartObject();
                if (method.IsGenericMethod)
                {
                    MethodInfo genericMethodDefinition = method.GetGenericMethodDefinition();
                    Type[] arguments = method.GetGenericArguments();

                    this.WriteKeyValuePair("type", this.InvokeTypeWriter(genericMethodDefinition.DeclaringType));
                    this.WriteKeyValuePair("name", genericMethodDefinition.Name);
                    this.WriteKeyValuePair("signature", genericMethodDefinition.ToString());
                    this.WriteKeyValuePair("generic", this.Apply(arguments, this.InvokeTypeWriter));
                }
                else
                {
                    this.WriteKeyValuePair("type", this.InvokeTypeWriter(method.DeclaringType));
                    this.WriteKeyValuePair("name", method.Name);
                    this.WriteKeyValuePair("signature", method.ToString());
                }
                this._jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Invokes <see cref="Conveyor.Serialization.ExpressionSerializer.WriteProperty(PropertyInfo)"/>.
        /// </summary>
        /// <param name="property">Property instance to be serialized</param>
        /// <returns>An <see cref="System.Action"/> instance</returns>
        private Action InvokePropertyWriter(PropertyInfo property) => () => this.WriteProperty(property);

        /// <summary>
        /// Writes given property's information
        /// </summary>
        /// <param name="property">Property instance to be serialized</param>
        private void WriteProperty(PropertyInfo property)
        {
            if (property == null)
            {
                this._jsonWriter.WriteNull();
            }
            else
            {
                this._jsonWriter.WriteStartObject();
                this.WriteKeyValuePair("type", this.InvokeTypeWriter(property.DeclaringType));
                this.WriteKeyValuePair("name", property.Name);
                this.WriteKeyValuePair("signature", property.ToString());
                this._jsonWriter.WriteEndObject();
            }
        }

        /// <summary>
        /// Invokes <see cref="Conveyor.Serialization.ExpressionSerializer.WriteMember(MemberInfo)"/>.
        /// </summary>
        /// <param name="member">Member instance to be serialized</param>
        /// <returns>An <see cref="System.Action"/> instance</returns>
        private Action InvokeMemberWriter(MemberInfo member) => () => this.WriteMember(member);

        /// <summary>
        /// Writes given member's information
        /// </summary>
        /// <param name="member">Member instance to be serialized</param>
        private void WriteMember(MemberInfo member)
        {
            if (member == null)
            {
                this._jsonWriter.WriteNull();
            }
            else
            {
                this._jsonWriter.WriteStartObject();
                this.WriteKeyValuePair("type", this.InvokeTypeWriter(member.DeclaringType));
                this.WriteKeyValuePair("memberType", (int)member.MemberType);
                this.WriteKeyValuePair("name", member.Name);
                this.WriteKeyValuePair("signature", member.ToString());
                this._jsonWriter.WriteEndObject();
            }
        }
    }
}
