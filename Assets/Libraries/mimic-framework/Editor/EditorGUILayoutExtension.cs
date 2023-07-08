using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class EditorGUILayoutExtension {
    public static void ListField<T>(string label, List<T> list, bool saveChanges = true) {
        EditorGUILayout.LabelField(label);
        EditorGUI.indentLevel++;
        int newCount = Mathf.Max(0, EditorGUILayout.IntField("Size", list.Count));
        while (newCount < list.Count)
            list.RemoveAt(list.Count - 1);
        while (newCount > list.Count) {
            list.Add(default(T));
        }

        for (int i = 0; i < list.Count; i++) {
            T newValue = GenericField($"Element {i}", list[i]);
            if (saveChanges) {
                list[i] = newValue;
            }
        }
        EditorGUI.indentLevel--;
    }

    public static T GenericField<T>(string label, T value, params GUILayoutOption[] options) {
        switch (value) {
            case int intValue:
                return (T)Convert.ChangeType(EditorGUILayout.IntField(label, intValue, options), typeof(T));
            case float floatValue:
                return (T)Convert.ChangeType(EditorGUILayout.FloatField(label, floatValue, options), typeof(T));
            case string stringValue:
                return (T)Convert.ChangeType(EditorGUILayout.TextField(label, stringValue, options), typeof(T));
            case Object objectValue:
                return (T)(object)EditorGUILayout.ObjectField(label, objectValue, objectValue.GetType(), false, options);
            default:
                if (value == null) {
                    return (T)(object)EditorGUILayout.ObjectField(label, null, typeof(T), false, options);
                } else {
                    Debug.LogError($"Object type {typeof(T)} not handled");
                    return value;
                }
        }
    }

    public static void DictionaryField<T1, T2>(string label, Dictionary<T1, T2> dictionary, ref T1 lastNewKey, bool saveChanges = true){
        EditorGUILayout.LabelField(label);
        EditorGUI.indentLevel++;

        EditorGUILayout.BeginHorizontal();
        lastNewKey = GenericField("New key", lastNewKey);
        if (GUILayout.Button("Add Key")) {
            if (dictionary.ContainsKey(lastNewKey)) {
                EditorUtility.DisplayDialog("Duplicated key", $"Can't add {lastNewKey} since it would become a duplicated key", "ok");
            } else {
                if (typeof(T2) == typeof(string)) {
                    dictionary.Add(lastNewKey, (T2)Convert.ChangeType("",typeof(T2)));
                } else {
                    dictionary.Add(lastNewKey, default);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        T1 keyToRemove = default, keyToChange = default;
        T2 newTempValue, newValue = default;
        bool keyToRemoveSet = false, keyToChangeSet = false;
        foreach (KeyValuePair<T1, T2> element in dictionary) {

            EditorGUILayout.BeginHorizontal();
            if (typeof(T1).Equals(typeof(Object)) || typeof(T1).IsSubclassOf(typeof(Object))) {
                GenericField("Key", element.Key);
                newTempValue = GenericField("", element.Value);
            } else {
                newTempValue = GenericField($"Key: {element.Key}", element.Value);
            }
            if (GUILayout.Button("Remove Element")) {
                keyToRemove = element.Key; //key to remove could be a list, but I don't think it's necessary
                keyToRemoveSet = true;
            }
            EditorGUILayout.EndHorizontal();
            if (saveChanges && !keyToChangeSet && newTempValue!=null && !newTempValue.Equals(element.Value)) {
                keyToChangeSet = true;
                keyToChange = element.Key;
                newValue = newTempValue;
            }
        }
        if (keyToRemoveSet) {
            dictionary.Remove(keyToRemove);
        }
        if (saveChanges && keyToChangeSet) {
            dictionary[keyToChange] = newValue;
        }

        EditorGUI.indentLevel--;
    }

    public static void CenteredGUILayout(Action guiLayoutCall) {
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        guiLayoutCall();
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }
}