using Scripts.Cutscenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Editor
{
    [CustomPropertyDrawer(typeof(CutsceneCameraInstructions))]
    public class CutsceneCameraInstructionsDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the foldout
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                float yOffset = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Draw cameraSpline with paste button
                SerializedProperty cameraSplineProp = property.FindPropertyRelative("cameraSpline");

                // Calculate rects for spline field and button
                Rect splineRect = new Rect(position.x, yOffset, position.width - 80, EditorGUIUtility.singleLineHeight);
                Rect buttonRect = new Rect(position.x + position.width - 75, yOffset, 75, EditorGUIUtility.singleLineHeight);

                // Draw the spline property (Unity's built-in drawer will handle it)
                EditorGUI.PropertyField(splineRect, cameraSplineProp, new GUIContent("Camera Spline"));

                // Draw paste button
                GUI.enabled = SplineClipboard.HasSpline;
                if (GUI.Button(buttonRect, "Paste"))
                {
                    PasteSplineIntoProperty(cameraSplineProp);
                }
                GUI.enabled = true;

                yOffset += EditorGUI.GetPropertyHeight(cameraSplineProp) + EditorGUIUtility.standardVerticalSpacing;

                // Draw speedKeyframes array
                SerializedProperty speedKeyframesProp = property.FindPropertyRelative("speedKeyframes");
                Rect keyframesRect = new Rect(position.x, yOffset, position.width, EditorGUI.GetPropertyHeight(speedKeyframesProp));
                EditorGUI.PropertyField(keyframesRect, speedKeyframesProp, true);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        private void PasteSplineIntoProperty(SerializedProperty splineProperty)
        {
            Spline copiedSpline = SplineClipboard.GetCopiedSpline();
            if (copiedSpline == null)
            {
                Debug.LogWarning("No spline in clipboard!");
                return;
            }

            // Get the target object
            var targetObject = splineProperty.serializedObject.targetObject;
            Undo.RecordObject(targetObject, "Paste Spline");

            // Navigate to the actual spline object
            var parent = GetParentObject(splineProperty.propertyPath, targetObject);
            var fieldInfo = parent.GetType().GetField("cameraSpline",
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            if (fieldInfo != null)
            {
                Spline targetSpline = fieldInfo.GetValue(parent) as Spline;

                if (targetSpline == null)
                {
                    targetSpline = new Spline();
                    fieldInfo.SetValue(parent, targetSpline);
                }

                // Clear and copy
                targetSpline.Clear();
                foreach (var knot in copiedSpline)
                {
                    targetSpline.Add(knot);
                }
                targetSpline.Closed = copiedSpline.Closed;

                // Mark dirty
                EditorUtility.SetDirty(targetObject);
                splineProperty.serializedObject.ApplyModifiedProperties();
                splineProperty.serializedObject.Update();

                Debug.Log($"Pasted spline with {copiedSpline.Count} knots into {splineProperty.displayName}");
            }
            else
            {
                Debug.LogError("Failed to paste spline: Could not find cameraSpline field");
            }
        }

        private object GetParentObject(string path, object obj)
        {
            string[] pathParts = path.Split('.');

            // Navigate to parent (exclude last part which is field name)
            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                string fieldName = pathParts[i];

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
            float height = EditorGUIUtility.singleLineHeight; // Foldout line

            if (property.isExpanded)
            {
                SerializedProperty cameraSplineProp = property.FindPropertyRelative("cameraSpline");
                SerializedProperty speedKeyframesProp = property.FindPropertyRelative("speedKeyframes");

                height += EditorGUIUtility.standardVerticalSpacing;
                height += EditorGUI.GetPropertyHeight(cameraSplineProp);
                height += EditorGUIUtility.standardVerticalSpacing;
                height += EditorGUI.GetPropertyHeight(speedKeyframesProp, true);
            }

            return height;
        }
    }
}
