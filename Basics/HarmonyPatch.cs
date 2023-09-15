using Harmony.Utils;
using MonoMod.Cil;
using MonoMod.RuntimeDetour.HookGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace Harmony
{
    public class HarmonyPatch
    {
        private const BindingFlags CommonFlag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
        static List<(MethodInfo, Delegate)> Ons = new();
        static List<(MethodInfo, ILContext.Manipulator)> ILs = new();
        static HarmonyPatch()
        {
            Harmony.OnUnload += delegate
            {
                Ons.Clear();
                ILs.Clear();
            };
        }
        private static void CheckParamters(MethodInfo target, MethodInfo method)
        {
            var tps = target.GetParameters();
            var mps = method.GetParameters();
            if(CommonUtils.CompareArray(tps, mps, (p1, p2) =>
            {
                return p1.ParameterType == p2.ParameterType;
            }))
            {
                return;
            }
            string info = $"[HarmonyPatch]Fail to Patch because the parameters do not match.Patch [{target.Name}] requires matching the following parameter list:\n\t";
            info += string.Join(",", target.GetParameters().Convert(p => $"{p.ParameterType.Name} {p.Name}"));
            Debugger.Error(new ArgumentException(info));
        }
        public static void PatchMethod_On(MethodInfo target, Delegate patch)
        {
            CheckParamters(target, patch.Method);
            MonoModHooks.Add(target, patch);
            Ons.Add((target, patch));
        }
        public static void PatchMethod_On(Delegate target, Delegate patch)
        {
            PatchMethod_On(target.Method, patch);
        }
        public static void PatchMethod_On(Type type, string methodName, Delegate patch, BindingFlags flag = CommonFlag)
        {
            MethodInfo target = type.GetMethod(methodName, flag) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_On(target, patch);
        }
        public static void PatchMethod_On(Type type, string methodName, Type[] paramtersLimit, Delegate patch, BindingFlags flag = CommonFlag)
        {
            MethodInfo target = type.GetMethod(methodName, flag, paramtersLimit) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_On(target, patch);
        }
        public static void PatchMethod_On(Mod mod, string typeName, string methodName, Delegate patch, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            MethodInfo target = type.GetMethod(methodName, flag) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_On(target, patch);
        }
        public static void PatchMethod_On(Mod mod, string typeName, string methodName, Type[] paramtersLimit, Delegate patch, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            MethodInfo target = type.GetMethod(methodName, flag, paramtersLimit) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_On(target, patch);
        }
        public static void PatchProperty_On(PropertyInfo target, Delegate patchGet = null, Delegate patchSet = null)
        {
            if (patchGet is not null)
            {
                MethodInfo getMethod = target.GetGetMethod(true) ?? throw new InvalidOperationException("This property don't have getter");
                PatchMethod_On(getMethod, patchGet);
            }
            if (patchSet is not null)
            {
                MethodInfo setMethod = target.GetSetMethod(true) ?? throw new InvalidOperationException("This property don't have setter");
                PatchMethod_On(setMethod, patchSet);
            }
        }
        public static void PatchProperty_On(Type type, string propertyName, Delegate patchGet = null, Delegate patchSet = null, BindingFlags flag = CommonFlag)
        {
            PropertyInfo target = type.GetProperty(propertyName, flag) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_On(target, patchGet, patchSet);
        }
        public static void PatchProperty_On(Mod mod, string typeName, string propertyName, Delegate patchGet, Delegate patchSet, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            PropertyInfo target = type.GetProperty(propertyName, flag) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_On(target, patchGet, patchSet);
        }
        public static void PatchIndexer_On(PropertyInfo target, Delegate patchGet = null, Delegate patchSet = null)
        {
            PatchProperty_On(target, patchGet, patchSet);
        }
        public static void PatchIndexer_On(Type type, Delegate patchGet = null, Delegate patchSet = null, BindingFlags flag = CommonFlag)
        {
            PatchProperty_On(type, "Item", patchGet, patchSet, flag);
        }
        public static void PatchIndexer_On(Type type, Type[] parametersLimit = null, Delegate patchGet = null, Delegate patchSet = null, BindingFlags flag = CommonFlag)
        {
            PropertyInfo target = type.GetProperties(flag).FirstOrDefault(p =>
            {
                if (p.Name != "Item")
                {
                    return false;
                }
                var ps = p.GetIndexParameters();
                if (ps.Length != parametersLimit.Length)
                {
                    return false;
                }
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].ParameterType != parametersLimit[i])
                    {
                        return false;
                    }
                }
                return true;
            }, null) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_On(target, patchGet, patchSet);
        }
        public static void PatchIndexer_On(Mod mod, string typeName, Delegate patchGet = null, Delegate patchSet = null, BindingFlags flag = CommonFlag)
        {
            PatchProperty_On(mod, typeName, "Item", patchGet, patchSet, flag);
        }
        public static void PatchIndexer_On(Mod mod, string typeName, Type[] parametersLimit, Delegate patchGet = null, Delegate patchSet = null, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            PropertyInfo target = type.GetProperties(flag).FirstOrDefault(p =>
            {
                if (p.Name != "Item")
                {
                    return false;
                }
                var ps = p.GetIndexParameters();
                if (ps.Length != parametersLimit.Length)
                {
                    return false;
                }
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].ParameterType != parametersLimit[i])
                    {
                        return false;
                    }
                }
                return true;
            }, null) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_On(target, patchGet, patchSet);
        }
        public static void PatchMethod_IL(MethodInfo target, ILContext.Manipulator patch)
        {
            MonoModHooks.Modify(target, patch);
            ILs.Add((target, patch));
        }
        public static void PatchMethod_IL(Delegate target, ILContext.Manipulator patch)
        {
            PatchMethod_IL(target.Method, patch);
        }
        public static void PatchMethod_IL(Type type, string methodName, ILContext.Manipulator patch, BindingFlags flag = CommonFlag)
        {
            MethodInfo target = type.GetMethod(methodName, flag) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_IL(target, patch);
        }
        public static void PatchMethod_IL(Type type, string methodName, Type[] paramterLimit, ILContext.Manipulator patch, BindingFlags flag = CommonFlag)
        {
            MethodInfo target = type.GetMethod(methodName, flag, paramterLimit) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_IL(target, patch);
        }
        public static void PatchMethod_IL(Mod mod, string typeName, string methodName, ILContext.Manipulator patch, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            MethodInfo target = type.GetMethod(methodName, flag) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_IL(target, patch);
        }
        public static void PatchMethod_IL(Mod mod, string typeName, string methodName, Type[] paramterLimit, ILContext.Manipulator patch, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            MethodInfo target = type.GetMethod(methodName, flag, paramterLimit) ?? throw new ArgumentException("Can't find target method");
            PatchMethod_IL(target, patch);
        }
        public static void PatchProperty_IL(PropertyInfo target, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null)
        {
            if (patchGet is not null)
            {
                MethodInfo getMethod = target.GetGetMethod(true) ?? throw new InvalidOperationException("This property don't have getter");
                PatchMethod_IL(getMethod, patchGet);
            }
            if (patchSet is not null)
            {
                MethodInfo setMethod = target.GetSetMethod(true) ?? throw new InvalidOperationException("This property don't have setter");
                PatchMethod_IL(setMethod, patchSet);
            }
        }
        public static void PatchProperty_IL(Type type, string propertyName, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null, BindingFlags flag = CommonFlag)
        {
            PropertyInfo target = type.GetProperty(propertyName, flag) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_IL(target, patchGet, patchSet);
        }
        public static void PatchProperty_IL(Mod mod, string typeName, string propertyName, ILContext.Manipulator patchGet, ILContext.Manipulator patchSet, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            PropertyInfo target = type.GetProperty(propertyName, flag) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_IL(target, patchGet, patchSet);
        }
        public static void PatchIndexer_IL(PropertyInfo target, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null)
        {
            PatchProperty_IL(target, patchGet, patchSet);
        }
        public static void PatchIndexer_IL(Type type, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null, BindingFlags flag = CommonFlag)
        {
            PatchProperty_IL(type, "Item", patchGet, patchSet, flag);
        }
        public static void PatchIndexer_IL(Type type, Type[] parametersLimit = null, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null, BindingFlags flag = CommonFlag)
        {
            PropertyInfo target = type.GetProperties(flag).FirstOrDefault(p =>
            {
                if (p.Name != "Item")
                {
                    return false;
                }
                var ps = p.GetIndexParameters();
                if (ps.Length != parametersLimit.Length)
                {
                    return false;
                }
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].ParameterType != parametersLimit[i])
                    {
                        return false;
                    }
                }
                return true;
            }, null) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_IL(target, patchGet, patchSet);
        }
        public static void PatchIndexer_IL(Mod mod, string typeName, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null, BindingFlags flag = CommonFlag)
        {
            PatchProperty_IL(mod, typeName, "Item", patchGet, patchSet, flag);
        }
        public static void PatchIndexer_IL(Mod mod, string typeName, Type[] parametersLimit, ILContext.Manipulator patchGet = null, ILContext.Manipulator patchSet = null, BindingFlags flag = CommonFlag)
        {
            Assembly assembly = mod?.Code ?? typeof(Main).Assembly;
            Type type = assembly.GetType(typeName, true, true);
            PropertyInfo target = type.GetProperties(flag).FirstOrDefault(p =>
            {
                if (p.Name != "Item")
                {
                    return false;
                }
                var ps = p.GetIndexParameters();
                if (ps.Length != parametersLimit.Length)
                {
                    return false;
                }
                for (int i = 0; i < ps.Length; i++)
                {
                    if (ps[i].ParameterType != parametersLimit[i])
                    {
                        return false;
                    }
                }
                return true;
            }, null) ?? throw new ArgumentException("Can't find target property");
            PatchProperty_IL(target, patchGet, patchSet);
        }
    }
}