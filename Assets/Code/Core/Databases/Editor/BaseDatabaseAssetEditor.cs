using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class BaseDatabaseAssetEditor<TDatabaseAsset, TAssetItem> : Editor where TDatabaseAsset : BaseDatabaseAsset<TAssetItem> where TAssetItem : BaseDatabaseItem
{
    private TDatabaseAsset _asset;
    private Dictionary<TAssetItem, Editor> _data;
    private Dictionary<TAssetItem, bool> _foldout;
    private int _removeIndex = -1;
    private int _moveUpIndex = -1;
    private int _moveDownIndex = -1;

    private void OnEnable()
    {
        _asset = target as TDatabaseAsset;
        _data = new Dictionary<TAssetItem, Editor>();
        _foldout = new Dictionary<TAssetItem, bool>();

        foreach (var item in _asset.Items)
        {
            _foldout[item] = false;
            AddEditor(item);
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        for (int i = 0; i < _asset.Items.Count; i++)
        {
            var item = _asset.Items[i];
            var e = _data[item];

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(_foldout[item] ? "-" : "+", GUILayout.Width(20)))
                _foldout[item] = !_foldout[item];

            EditorGUILayout.LabelField(item.GetDatabaseName());

            if (i != 0 && GUILayout.Button("↑"))
            {
                _moveUpIndex = i;
            }

            if (i != _asset.Items.Count -1 && GUILayout.Button("↓"))
            {
                _moveDownIndex = i;
            }

            if (GUILayout.Button("Remove"))
                _removeIndex = i;

            EditorGUILayout.EndHorizontal();

            if (_foldout[item])
            {
                EditorGUILayout.BeginVertical();
                e.serializedObject.Update();
                e.OnInspectorGUI();
                e.serializedObject.ApplyModifiedProperties();

                if (GUI.changed && EditorUtility.IsPersistent(_asset.Items[i]))
                    EditorUtility.SetDirty(_asset.Items[i]);

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
        }

        if (_removeIndex >= 0)
        {
            if (EditorUtility.IsPersistent(_asset))
                AssetDatabase.RemoveObjectFromAsset(_asset.Items[_removeIndex]);

            _data.Remove(_asset.Items[_removeIndex]);
            _asset.Items.RemoveAt(_removeIndex);
            _removeIndex = -1;

            serializedObject.ApplyModifiedProperties();

            if (EditorUtility.IsPersistent(_asset))
            {
                EditorUtility.SetDirty(_asset);
                AssetDatabase.SaveAssets();
            }
        }

        if (_moveUpIndex >= 0)
        {
            var other = _asset.Items[_moveUpIndex - 1];
            _asset.Items[_moveUpIndex - 1] = _asset.Items[_moveUpIndex];
            _asset.Items[_moveUpIndex] = other;

            _moveUpIndex = -1;

            serializedObject.ApplyModifiedProperties();

            if (EditorUtility.IsPersistent(_asset))
            {
                EditorUtility.SetDirty(_asset);
                AssetDatabase.SaveAssets();
            }
        }

        if (_moveDownIndex >= 0)
        {
            var other = _asset.Items[_moveDownIndex + 1];
            _asset.Items[_moveDownIndex + 1] = _asset.Items[_moveDownIndex];
            _asset.Items[_moveDownIndex] = other;

            _moveDownIndex = -1;

            serializedObject.ApplyModifiedProperties();

            if (EditorUtility.IsPersistent(_asset))
            {
                EditorUtility.SetDirty(_asset);
                AssetDatabase.SaveAssets();
            }
        }

        if (GUILayout.Button("Add"))
        {
            var menu = new GenericMenu();

            foreach (var t in AssemblyUtils.GetAllTypesDerivedFrom<TAssetItem>())
                menu.AddItem(new GUIContent(t.Name), false, () => AddItem(t));

            menu.ShowAsContext();
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void AddItem(Type type)
    {
        serializedObject.Update();

        var item = _asset.AddItem(type);
        var itemId = Guid.NewGuid().ToString("N");
        item.DatabaseInit(itemId, $"{type.Name} {itemId}");

        if (EditorUtility.IsPersistent(_asset))
            AssetDatabase.AddObjectToAsset(item, _asset);

        _foldout[item] = true;
        AddEditor(item);

        serializedObject.ApplyModifiedProperties();

        if (EditorUtility.IsPersistent(_asset))
        {
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssets();
        }
    }

    private void AddEditor(TAssetItem item)
    {
        var e = CreateEditor(item);
        e.name = item.name;

        _data.Add(item, e);
    }
}
