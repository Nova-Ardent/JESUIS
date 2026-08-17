using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System;
using UnityEditor;
using UnityEngine;

namespace JESUIS.Shared.ScreenData.DataBindings
{
    public static class DataBindingContainer
    {
        static Dictionary<Type, Dictionary<System.Guid, DataBinding>> bindings = new Dictionary<Type, Dictionary<Guid, DataBinding>>();

        public static void AddBinding(DataBinding dataBinding)
        {
            if (!bindings.ContainsKey(dataBinding.CurrentType))
            {
                bindings[dataBinding.CurrentType] = new Dictionary<Guid, DataBinding>();
            }

            if (!bindings[dataBinding.CurrentType].ContainsKey(dataBinding.UID))
            {
                bindings[dataBinding.CurrentType][dataBinding.UID] = dataBinding;
            }
            else
            {
                Debug.Log($"found binding with duplicate UID, {dataBinding.UID} {dataBinding.Name}");
            }
        }

        public static IEnumerable<DataBinding<T>> GetDataBindingsOfType<T>()
        {
            if (!bindings.ContainsKey(typeof(T)))
                yield break;

            foreach (DataBinding binding in bindings[typeof(T)].Values)
            {
                yield return (DataBinding<T>)binding;
            }
        }

        public static void BuildDataContainer(Type type)
        {
            foreach (var member in type.GetMembers(
                        BindingFlags.Public |
                        BindingFlags.NonPublic |
                        BindingFlags.Static |
                        BindingFlags.DeclaredOnly))
            {
                object value = member switch
                {
                    FieldInfo field => field.GetValue(null),
                    PropertyInfo property when property.GetMethod != null
                        => property.GetValue(null),
                    _ => null
                };

                if (value is DataBinding dataBinding) 
                {
                    if (!string.IsNullOrEmpty(type.Namespace))
                        dataBinding.GenerateUIDAndBind(type.Assembly.GetName().Name + "." + type.Namespace + "." + type.Name + "." + member.Name);
                    else
                        dataBinding.GenerateUIDAndBind(type.Assembly.GetName().Name + "." + type.Name + "." + member.Name);
                }
            }
        }
    }

    public class DataBinding
    {
        public Type CurrentType { get; private set; }
        public System.Guid UID { get; private set; }
        public string Name { get; private set; }

        public DataBinding(Type type, string name = "") 
        {
            Name = name;
            CurrentType = type;
        }

        public void GenerateUIDAndBind(string name)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                if (string.IsNullOrEmpty(Name))
                {
                    Name = name.Replace('.', '/');
                }

                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(name));

                byte[] guidBytes = new byte[16];
                Array.Copy(hash, guidBytes, 16);

                UID = new System.Guid(guidBytes);

                DataBindingContainer.AddBinding(this);
            }
        }
    }

    public class DataBinding<T> : DataBinding
    {
        public DataBinding(string name = "") : base(typeof(T), name)
        {
        }
    }

    public static partial class Common
    {
        [InitializeOnLoadMethod]
        public static void Initialize()
        {
            DataBindingContainer.BuildDataContainer(typeof(Common)); 
        }

        public static readonly DataBinding<float> TestDataBinding = new DataBinding<float>(); 
    }
}