using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

[CustomEditor(typeof(DungeonLayoutDefinition))]
public class DungeonLayoutDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var roomsProperty = serializedObject.FindProperty("rooms");

        EditorGUILayout.PropertyField(roomsProperty, new GUIContent("Rooms"), false);

        if (roomsProperty.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size", GUILayout.Width(EditorGUIUtility.labelWidth - 4));
            EditorGUILayout.LabelField(roomsProperty.arraySize.ToString(), GUILayout.Width(30));

            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                roomsProperty.arraySize++;
            }
            if (GUILayout.Button("-", GUILayout.Width(25)))
            {
                if (roomsProperty.arraySize > 0)
                    roomsProperty.arraySize--;
            }
            EditorGUILayout.EndHorizontal();

            // Draw each room element
            for (int i = 0; i < roomsProperty.arraySize; i++)
            {
                var element = roomsProperty.GetArrayElementAtIndex(i);
                DrawRoomElement(element, i);
            }

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawRoomElement(SerializedProperty element, int index)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        // Get current type
        var currentObj = element.managedReferenceValue;
        string typeName = currentObj != null ? currentObj.GetType().Name : "None";

        EditorGUILayout.BeginHorizontal();

        // Show type name and dropdown button
        EditorGUILayout.LabelField($"Room {index}: {typeName}", EditorStyles.boldLabel);

        // Type selection dropdown
        if (GUILayout.Button("Change Type", GUILayout.Width(100)))
        {
            ShowTypeMenu(element);
        }

        // Delete button
        if (GUILayout.Button("X", GUILayout.Width(25)))
        {
            element.DeleteCommand();
        }

        EditorGUILayout.EndHorizontal();

        // Draw fields
        if (currentObj != null)
        {
            EditorGUI.indentLevel++;
            DrawFieldsForObject(element);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private void DrawFieldsForObject(SerializedProperty element)
    {
        var iterator = element.Copy();
        var endProperty = iterator.GetEndProperty();

        iterator.NextVisible(true); // Skip generic field

        while (iterator.NextVisible(false) && !SerializedProperty.EqualContents(iterator, endProperty))
        {
            EditorGUILayout.PropertyField(iterator, true);
        }
    }

    private void ShowTypeMenu(SerializedProperty element)
    {
        var menu = new GenericMenu();

        // Get all types that inherit from DungeonRoomDefinition
        var baseType = typeof(DungeonRoomDefinition);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(baseType));

        menu.AddItem(new GUIContent("None"), false, () =>
        {
            element.managedReferenceValue = null;
            element.serializedObject.ApplyModifiedProperties();
        });

        foreach (var type in types)
        {
            menu.AddItem(new GUIContent(type.Name), false, () =>
            {
                element.managedReferenceValue = Activator.CreateInstance(type);
                element.serializedObject.ApplyModifiedProperties();
            });
        }

        menu.ShowAsContext();
    }
}
