using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDatabaseAsset<T> : ScriptableObject where T : BaseDatabaseItem
{
    public List<T> Items = new List<T>();

    public T GetById(string id)
    {
        foreach (var item in Items)
            if (item.GetDatabaseId().Equals(id))
                return item;

        return null;
    }

    public int GetIndexById(string id)
    {
        for (int i = 0; i < Items.Count; i++)
            if (Items[i].GetDatabaseId().Equals(id))
                return i;

        return -1;
    }

#if UNITY_EDITOR
    public T AddItem(Type type)
    {
        var item = (T)CreateInstance(type);

        item.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
        item.name = type.Name;
        Items.Add(item);

        return item;
    }
#endif
}