using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Editor
{
    [CustomPropertyDrawer(typeof(Spline))]
    public class SplinePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Draw the default property field
            EditorGUI.PropertyField(position, property, label, true);

            // Check for right-click on the property
            Event current = Event.current;
            if (current.type == EventType.ContextClick && position.Contains(current.mousePosition))
            {
                ShowContextMenu(property);
                current.Use();
            }
        }

        private void ShowContextMenu(SerializedProperty property)
        {
            GenericMenu menu = new GenericMenu();

            // Option 1: Paste from clipboard
            if (SplineClipboard.HasSpline)
            {
                menu.AddItem(
                    new GUIContent("Paste Spline from Clipboard"),
                    false,
                    () => PasteSplineFromClipboard(property)
                );
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste Spline from Clipboard (Empty)"));
            }

            menu.AddSeparator("");

            // Option 2: Copy from selected GameObject's SplineContainer
            GameObject selected = Selection.activeGameObject;
            SplineContainer sourceContainer = selected != null ? selected.GetComponent<SplineContainer>() : null;

            if (sourceContainer != null && sourceContainer.Spline != null)
            {
                menu.AddItem(
                    new GUIContent("Copy from Selected SplineContainer"),
                    false,
                    () => CopySplineDirectly(property, sourceContainer.Spline)
                );
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Copy from Selected SplineContainer"));
            }

            menu.ShowAsContext();
        }

        private void PasteSplineFromClipboard(SerializedProperty property)
        {
            Spline copiedSpline = SplineClipboard.GetCopiedSpline();
            if (copiedSpline != null)
            {
                CopySplineDirectly(property, copiedSpline);
            }
        }

        private void CopySplineDirectly(SerializedProperty property, Spline sourceSpline)
        {
            // Get the target object that contains this spline field
            var targetObject = property.serializedObject.targetObject;

            // Record undo operation
            Undo.RecordObject(targetObject, "Copy Spline");

            // Get the parent object and field
            var parent = GetParentObject(property.propertyPath, targetObject);

            if (parent != null && fieldInfo != null)
            {
                // Get or create the target spline
                Spline targetSpline = fieldInfo.GetValue(parent) as Spline;

                if (targetSpline == null)
                {
                    targetSpline = new Spline();
                    fieldInfo.SetValue(parent, targetSpline);
                }

                // Clear existing knots
                targetSpline.Clear();

                // Copy all knots from source spline
                foreach (var knot in sourceSpline)
                {
                    targetSpline.Add(knot);
                }

                // Copy spline properties
                targetSpline.Closed = sourceSpline.Closed;

                // Mark the object as dirty to ensure changes are saved
                EditorUtility.SetDirty(targetObject);
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();

                Debug.Log($"Copied spline with {sourceSpline.Count} knots to {property.displayName}");
            }
            else
            {
                Debug.LogError("Failed to copy spline: Could not access parent object or field info");
            }
        }

        /// <summary>
        /// Gets the parent object of a serialized property using reflection
        /// </summary>
        private object GetParentObject(string path, object obj)
        {
            string[] pathParts = path.Split('.');

            // Navigate to the parent (exclude the last part which is the field name)
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                string fieldName = pathParts[i];

                // Handle array elements (e.g., "Array.data[0]")
                if (fieldName == "Array")
                {
                    i++; // Skip "data"
                    if (i < pathParts.Length)
                    {
                        string indexStr = pathParts[i];
                        int startIndex = indexStr.IndexOf('[') + 1;
                        int endIndex = indexStr.IndexOf(']');
                        int index = int.Parse(indexStr.Substring(startIndex, endIndex - startIndex));

                        if (obj is System.Collections.IList list)
                        {
                            obj = list[index];
                        }
                    }
                }
                else
                {
                    var fieldInfo = obj.GetType().GetField(fieldName,
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        obj = fieldInfo.GetValue(obj);
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            return obj;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
