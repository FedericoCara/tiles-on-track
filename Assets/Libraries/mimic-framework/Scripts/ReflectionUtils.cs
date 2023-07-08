using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Mimic
{
    public class ReflectionUtils
    {

        public const string LIST_TYPE = "System.Collections.Generic.List";
        public const string DICTIONARY_TYPE = "System.Collections.Generic.Dictionary";

        public static FieldInfo GetFieldViaPath(Type type, string path) {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type parent = type;
            FieldInfo field = parent.GetField(path, flags);
            string[] paths = path.Split('.');

            for (int i = 0; i < paths.Length; i++) {
                field = parent.GetField(paths[i], flags);
                if (field != null) {
                    // there are only two container field type that can be serialized:
                    // Array and List<T>
                    if (field.FieldType.IsArray) {
                        parent = field.FieldType.GetElementType();
                        i += 2;
                        continue;
                    }

                    if (field.FieldType.IsGenericType) {
                        parent = field.FieldType.GetGenericArguments()[0];
                        i += 2;
                        continue;
                    }
                    parent = field.FieldType;
                } else {
                    break;
                }
            }
            if (field == null) {
                if (type.BaseType != null) {
                    return GetFieldViaPath(type.BaseType, path);
                } else {
                    return null;
                }
            }
            return field;
        }

        public static object GetFieldValueViaPath(object obj, string path) {
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            Type type = obj.GetType();
            FieldInfo field = null;

            string[] parts = path.Split('.');
            object value = obj;

            for (int i = 0; i < parts.Length; i++) {
                field = type.GetField(parts[i], flags);
                var x = field.GetValue(value);

                if (x is IList) {
                    IList list = ((x as IList));
                    if (list.Count > 0) {
                        if (parts.Length > i + 1 && parts[i + 1] == "Array" && parts[i + 2].StartsWith("data[")) {
                            i += 2;
                            int index = int.Parse(parts[i].Substring(5, parts[i].Length - 6));
                            value = list[index];
                            type = value.GetType();
                            continue;
                        } else {
                            throw new ArgumentException($"Incorrect path for array/list values: {path}");
                        }
                    } else {
                        return list;
                    }
                } else {
                    type = x.GetType();
                }

                value = field.GetValue(value);
            }
            return value;
        }

        public static List<FieldInfo> GetConstants(Type type) {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);
            List<FieldInfo> fieldInfo = new List<FieldInfo>(fieldInfos);
            return fieldInfo.FindAll(fi => fi.IsLiteral && !fi.IsInitOnly);
        }

        public static List<FieldInfo> GetStatics(Type type) {
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public |
                 BindingFlags.Static | BindingFlags.FlattenHierarchy);
            List<FieldInfo> fieldInfo = new List<FieldInfo>(fieldInfos);
            return fieldInfo.FindAll(fi => fi.IsStatic);
        }

        public static List<Type> GetSubclasses(Type type) {
            List<Type> subclasses = new List<Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type assemblyType in assembly.GetTypes()) {
                    if (assemblyType.IsSubclassOf(type)) {
                        subclasses.Add(assemblyType);
                    }
                }
            }
            return subclasses;
        }

        public static Type GetTypeFromAllAssemblies(string fullName) {
            if(fullName == LIST_TYPE) {
                return typeof(IList);
            } else if(fullName.StartsWith(LIST_TYPE)) {
                string generic = fullName.Substring(LIST_TYPE.Length + 3, fullName.Length - LIST_TYPE.Length - 4);
                Type listType = typeof(List<>);
                return listType.MakeGenericType(GetTypeFromAllAssemblies(generic));
            } else if(fullName == DICTIONARY_TYPE) {
                return typeof(IDictionary);
            } else if (fullName.StartsWith(DICTIONARY_TYPE)) {
                string[] genericsStr = fullName.Substring(DICTIONARY_TYPE.Length + 3, fullName.Length - DICTIONARY_TYPE.Length - 4).Split(',');
                Type dictionaryType = typeof(Dictionary<,>);
                Type[] genericTypes = new Type[genericsStr.Length];
                for(int i = 0; i < genericsStr.Length; i++) {
                    genericTypes[i] = GetTypeFromAllAssemblies(genericsStr[i]);
                }
                return dictionaryType.MakeGenericType(genericTypes);
            }

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    if (type.FullName == fullName) {
                        return type;
                    }
                }
            }
            return null;
        }

        public static MethodInfo GetGenericMethod(Type classType, string methodName, Type genericType) {
            MethodInfo method = classType.GetMethod(methodName);
            return method.MakeGenericMethod(genericType);
        }
    }
}