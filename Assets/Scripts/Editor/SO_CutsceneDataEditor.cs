using Scripts.Cutscenes;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Editor
{
    [CustomEditor(typeof(SO_CutsceneData))]
    public class SO_CutsceneDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Add helper section at the top
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Spline Paste Helper", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (SplineClipboard.HasSpline)
            {
                EditorGUILayout.LabelField("Clipboard has a spline ready to paste", EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField("Copy a spline from SplineContainer first", EditorStyles.miniLabel);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "1. Select a GameObject with SplineContainer\n" +
                "2. Click 'Copy Spline to Clipboard' button\n" +
                "3. Expand the cutscene step below\n" +
                "4. Click 'Paste Into Spline' button next to the spline field",
                MessageType.Info);

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(10);

            // Draw cutscene ID
            SerializedProperty cutsceneIDProp = serializedObject.FindProperty("cutsceneID");
            EditorGUILayout.PropertyField(cutsceneIDProp);

            EditorGUILayout.Space(5);

            // Draw steps list with custom handling
            SerializedProperty stepsProp = serializedObject.FindProperty("cutscenesSteps");

            if (stepsProp.isExpanded = EditorGUILayout.Foldout(stepsProp.isExpanded, "Cutscene Steps", true))
            {
                EditorGUI.indentLevel++;

                // Array size
                int newSize = EditorGUILayout.IntField("Size", stepsProp.arraySize);
                if (newSize != stepsProp.arraySize)
                {
                    stepsProp.arraySize = newSize;
                }

                // Draw each step
                for (int i = 0; i < stepsProp.arraySize; i++)
                {
                    EditorGUILayout.Space(5);
                    SerializedProperty stepDefiner = stepsProp.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    // Draw the step type dropdown
                    SerializedProperty stepTypeProp = stepDefiner.FindPropertyRelative("cutsceneStepType");
                    EditorGUILayout.PropertyField(stepTypeProp, new GUIContent($"Step {i}"));

                    // Draw the actual step data
                    SerializedProperty stepProp = stepDefiner.FindPropertyRelative("step");
                    if (stepProp != null && stepProp.managedReferenceValue != null)
                    {
                        EditorGUI.indentLevel++;

                        // Check if this is a CameraMove step with splines
                        DrawStepWithSplineHelpers(stepProp);

                        EditorGUI.indentLevel--;
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawStepWithSplineHelpers(SerializedProperty stepProp)
        {
            // Find all child properties
            SerializedProperty iterator = stepProp.Copy();
            SerializedProperty endProperty = stepProp.GetEndProperty();

            iterator.NextVisible(true); // Enter the first child

            while (!SerializedProperty.EqualContents(iterator, endProperty))
            {
                // Check if this property is a Spline
                if (iterator.propertyType == SerializedPropertyType.Generic &&
                    iterator.type == "Spline")
                {
                    // Draw paste button next to spline fields
                    EditorGUILayout.BeginHorizontal();

                    // Draw the property (Unity's spline drawer will handle it)
                    Rect propertyRect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(iterator, true));
                    float buttonWidth = 80;
                    Rect fieldRect = new Rect(propertyRect.x, propertyRect.y, propertyRect.width - buttonWidth - 5, propertyRect.height);
                    Rect buttonRect = new Rect(propertyRect.x + propertyRect.width - buttonWidth, propertyRect.y, buttonWidth, EditorGUIUtility.singleLineHeight);

                    EditorGUI.PropertyField(fieldRect, iterator, true);

                    // Draw paste button
                    GUI.enabled = SplineClipboard.HasSpline;
                    if (GUI.Button(buttonRect, "Paste Spline"))
                    {
                        PasteIntoSplineProperty(iterator);
                    }
                    GUI.enabled = true;

                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    // Draw normal property
                    EditorGUILayout.PropertyField(iterator, true);
                }

                if (!iterator.NextVisible(false))
                    break;
            }
        }

        private void PasteIntoSplineProperty(SerializedProperty splineProperty)
        {
            Spline copiedSpline = SplineClipboard.GetCopiedSpline();
            if (copiedSpline == null)
            {
                Debug.LogWarning("No spline in clipboard!");
                return;
            }

            // Get the target object and field through reflection
            var targetObject = splineProperty.serializedObject.targetObject;
            Undo.RecordObject(targetObject, "Paste Spline");

            // Navigate to the actual spline field
            object parent = GetParentObject(splineProperty.propertyPath, targetObject);
            string fieldName = GetFieldName(splineProperty.propertyPath);

            var fieldInfo = parent?.GetType().GetField(fieldName,
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

                Debug.Log($"✓ Pasted spline with {copiedSpline.Count} knots!");
            }
            else
            {
                Debug.LogError($"Failed to paste: couldn't find field {fieldName}");
            }
        }

        private string GetFieldName(string path)
        {
            string[] parts = path.Split('.');
            return parts[parts.Length - 1];
        }

        private object GetParentObject(string path, object obj)
        {
            string[] pathParts = path.Split('.');

            for (int i = 0; i < pathParts.Length - 1; i++)
            {
                string fieldName = pathParts[i];

                if (fieldName == "Array")
                {
                    i++;
                    if (i < pathParts.Length)
                    {
                        string indexStr = pathParts[i];
                        int startIndex = indexStr.IndexOf('[') + 1;
                        int endIndex = indexStr.IndexOf(']');
                        int index = int.Parse(indexStr.Substring(startIndex, endIndex - startIndex));

                        if (obj is System.Collections.IList list && index < list.Count)
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
    }
}
