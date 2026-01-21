using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class AssemblyUtils
{
    public static IEnumerable<Type> GetAllAssemblyTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(t =>
            {
                // Ugly hack to handle mis-versioned dlls
                var innerTypes = new Type[0];
                try
                {
                    innerTypes = t.GetTypes();
                }
                catch { }
                return innerTypes;
            });
    }

    public static IEnumerable<Type> GetAllTypesDerivedFrom<T>()
    {
#if UNITY_EDITOR && UNITY_2019_2_OR_NEWER
        return UnityEditor.TypeCache.GetTypesDerivedFrom<T>();
#else
            return GetAllAssemblyTypes().Where(t => t.IsSubclassOf(typeof(T)));
#endif
    }
}