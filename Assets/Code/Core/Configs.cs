using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Config : ScriptableObject
{
}

public static class Configs
{
    private static readonly Dictionary<Type, Config> _data = new Dictionary<Type, Config>();

    public static T Get<T>() where T : Config
    {
        Type type = typeof(T);
        Config config;

        if (!_data.TryGetValue(type, out config))
        {
            config = Resources.Load<Config>(type.Name);

            _data.Add(type, config);
        }

        return config as T;
    }
}