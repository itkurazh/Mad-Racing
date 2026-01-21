using UnityEngine;
using UnityEditor;

public abstract class BaseStringIdAttributeDrawer : PropertyDrawer
{
    private string[] _names;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_names == null)
            _names = GetNames();

        var id = property.stringValue;
        var itemIndex = FindById(id);

        if (itemIndex == -1)
            itemIndex = Count() - 1;

        itemIndex = EditorGUI.Popup(position, property.displayName, itemIndex, _names);
        property.stringValue = GetItem(itemIndex).ItemId;
    }

    public abstract IStringIdItem GetItem(int index);
    public abstract int Count();

    private string[] GetNames()
    {
        var names = new string[Count()];

        for (int i = 0; i < Count(); i++)
            names[i] = GetItem(i).ItemName;

        return names;
    }

    private int FindById(string id)
    {
        for (int i = 0; i < Count(); i++)
            if (GetItem(i).ItemId.Equals(id))
                return i;

        return -1;
    }
}