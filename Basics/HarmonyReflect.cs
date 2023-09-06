using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Harmony
{
    public class HarmonyReflect
    {
        private const BindingFlags CommonFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        static HarmonyReflect()
        {
            Harmony.OnUnload += delegate
            {
            };
        }
        static MethodInfo FindMethod(Type type, string methodName, Type[] paramtersTypes)
        {
            var cache = Harmony.Instance.Cache<string, List<MethodInfo>>();
            if (cache.Count == 0)
            {
                var methods = type.GetMethods(CommonFlag);
                foreach (var method in methods)
                {
                    if (cache.TryGet(method.Name, out var ms))
                    {
                        ms.Add(method);
                    }
                    else
                    {
                        cache.Add(method.Name, new() { method });
                    }
                }
            }
            if (cache.TryGet(methodName, out var list))
            {
                return list.FirstOrDefault(m =>
                {
                    return Utils.CompareArray(m.GetParameters().Convert(p => p.ParameterType), paramtersTypes, (p1, p2) => p2.IsClassOrSubClass(p1));
                }, null);
            }
            return null;
        }
        public static void Reflect_Static(Type type, string methodName, object[] input)
        {
            string key = $"{type.FullName}.{methodName}.{string.Join("_", input.Convert(i => i.GetType().Name))}";
            if (!Harmony.Instance.Cache<string, MethodInfo>().TryGet(key, out var method))
            {
                method = FindMethod(type, methodName, input.Convert(i => i.GetType()));
                if (method is not null)
                {
                    Harmony.Instance.Cache<string, MethodInfo>().Add(key, method);
                }
            }
            if (method is null)
            {
                string info = $"[HarmonyReflect]Reflection failed. No method matching the input parameters was found";
                Debugger.Warn(info);
                return;
            }
            method.Invoke(null, input);
        }
        public static void Reflect_Instance(Type type, string methodName, object instance, object[] input)
        {
            string key = $"{type.FullName}.{methodName}.{string.Join("_", input.Convert(i => i.GetType().Name))}";
            if (!Harmony.Instance.Cache<string, MethodInfo>().TryGet(key, out var method))
            {
                Type[] types = new Type[input.Length + 1];
                types[0] = instance.GetType();
                for (int i = 0; i < types.Length; i++)
                {
                    types[i] = input[i - 1].GetType();
                }
                method = FindMethod(type, methodName, types);
                if (method is not null)
                {
                    Harmony.Instance.Cache<string, MethodInfo>().Add(key, method);
                }
            }
            if (method is null)
            {
                string info = $"[HarmonyReflect]Reflection failed. No method matching the input parameters was found";
                Debugger.Warn(info);
                return;
            }
            method.Invoke(instance, input);
        }
    }
}