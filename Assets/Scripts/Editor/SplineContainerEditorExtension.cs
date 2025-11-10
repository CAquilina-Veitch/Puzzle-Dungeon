using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Scripts.Editor
{
    [CustomEditor(typeof(SplineContainer))]
    public class SplineContainerEditorExtension : UnityEditor.Editor
    {
        private UnityEditor.Editor defaultEditor;

        private void OnEnable()
        {
            // Create the default Unity editor for SplineContainer
            // We need to use the editor from the Splines package
            var editorType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Splines.SplineContainerEditor");
            if (editorType != null)
            {
                defaultEditor = CreateEditor(target, editorType);
            }
        }

        private void OnDisable()
        {
            if (defaultEditor != null)
            {
                DestroyImmediate(defaultEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            SplineContainer container = (SplineContainer)target;

            // Add our custom copy button at the top
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Spline Copy Tools", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            GUI.enabled = container.Spline != null;
            if (GUILayout.Button("Copy Spline to Clipboard", GUILayout.Height(22)))
            {
                SplineClipboard.CopySpline(container.Spline);
                Debug.Log($"Copied spline with {container.Spline.Count} knots to clipboard");
            }
            GUI.enabled = true;

            // Show clipboard status
            if (SplineClipboard.HasSpline)
            {
                EditorGUILayout.LabelField("(Clipboard: Ready)", EditorStyles.miniLabel, GUILayout.Width(120));
            }

            EditorGUILayout.EndHorizontal();

            // Add right-click context menu hint
            EditorGUILayout.HelpBox(
                "Click 'Copy Spline to Clipboard' button above, then right-click on any Spline field to paste.",
                MessageType.Info);

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            // Draw the default Unity spline inspector
            if (defaultEditor != null)
            {
                defaultEditor.OnInspectorGUI();
            }
            else
            {
                // Fallback to default if we couldn't create the Unity editor
                DrawDefaultInspector();
            }
        }
    }
}
