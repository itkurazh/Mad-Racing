using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine.AI;

public static class ExtensionMethods
{
    #region VECTORS

    public static Vector3 WithX(this Vector3 v, float x = 0f)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y = 0f)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }
    
    public static Vector3 WithZ(this Vector2 v, float z = 0f)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector3 NormalizeD(this Vector3 v)
    {
        return new Vector3((int)v.x, (int)v.y, (int)v.x); 
    }
    
    #endregion
}
