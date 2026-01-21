using UnityEngine;

public abstract class BaseDatabaseItem : ScriptableObject
{
    public abstract void DatabaseInit(string id, string name);
    public abstract string GetDatabaseName();
    public abstract string GetDatabaseId();
}