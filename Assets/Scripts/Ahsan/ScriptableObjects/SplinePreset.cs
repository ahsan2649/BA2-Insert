using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace Ahsan.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SplinePreset", menuName = "Scriptable Objects/Spline Preset")]
    public class SplinePreset : ScriptableObject
    {
        public BezierKnot[] knots;

        [MenuItem("GameObject/Spline/Create Spline Preset", false, 11)]
        public static void CreateSplinePresetFromSelected()
        {
            GameObject selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                Debug.LogWarning("No selected object");
                return;
            }
        
            SplineContainer spline = selectedObject.GetComponent<SplineContainer>();

            if (spline == null)
            {
                Debug.LogWarning("No selected spline");
                return;
            }
        
            SplinePreset preset = CreateInstance<SplinePreset>();
            preset.knots = spline.Spline.Knots.ToArray();
        
            string folderPath = "Assets/ScriptableObjects";
            if (Selection.activeObject != null)
            {
                string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    folderPath = System.IO.Directory.Exists(assetPath) ? assetPath : // It's a folder
                        // It's a file, get its parent directory
                        System.IO.Path.GetDirectoryName(assetPath);
                }
            }

            string defaultAssetName = selectedObject.name + "_SplinePreset";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{defaultAssetName}.asset");


            AssetDatabase.CreateAsset(preset, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow(); // Focus on the project window first
            Selection.activeObject = preset; // Select the new asset
            EditorApplication.delayCall += () => {
                // This is crucial: delayCall ensures the rename is triggered AFTER the asset is fully selected
                // and the Project window has processed the selection.
                AssetDatabase.StartAssetEditing(); // Temporarily disable asset refresh to avoid flicker during rename
                EditorGUIUtility.PingObject(preset); // Ping it to ensure it's visible if the folder is collapsed
                AssetDatabase.StopAssetEditing(); // Re-enable asset refresh
                EditorApplication.ExecuteMenuItem("Assets/Rename"); // This executes the rename command
            };
            Debug.Log($"Spline Preset '{preset.name}' created successfully at: {uniquePath}");
        }
        
        [MenuItem("GameObject/Spline/Create Spline Preset", true)]
        public static bool ValidateCreateSplinePresetFromSelectedSpline()
        {
            return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<SplineContainer>() != null;
        }
    }
}
