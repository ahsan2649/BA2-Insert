using System.IO;
using UnityEditor;
using UnityEngine;

namespace Ahsan.ScriptableObjects.Editor
{
    public class Chart_Editor
    {
        [MenuItem("Assets/Create/Scriptable Objects/Create Chart from JSON", false, 1000)]
        public static void CreateChartFromJson()
        {
            TextAsset chartFile = Selection.activeObject as TextAsset;
            if (chartFile == null)
            {
                Debug.LogWarning("No JSON file selected.");
                return;
            }

            ChartObject deserializedChart = JsonUtility.FromJson<ChartObject>(chartFile.text);
            if (deserializedChart == null)
            {
                Debug.LogWarning("Invalid chart file");
                return;
            }

            Chart targetChart = ScriptableObject.CreateInstance<Chart>();
            targetChart.notes = deserializedChart.notes;

            string folderPath = "Assets/ScriptableObjects";
            string assetPath = AssetDatabase.GetAssetPath(chartFile);
            if (!string.IsNullOrEmpty(assetPath))
            {
                folderPath = Directory.Exists(assetPath) ? assetPath : Path.GetDirectoryName(assetPath);
            }

            string defaultAssetName = chartFile.name + "_Chart";
            string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{folderPath}/{defaultAssetName}.asset");

            AssetDatabase.CreateAsset(targetChart, uniquePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = targetChart;
            EditorApplication.delayCall += () =>
            {
                AssetDatabase.StartAssetEditing();
                EditorGUIUtility.PingObject(targetChart);
                AssetDatabase.StopAssetEditing();
                EditorApplication.ExecuteMenuItem("Assets/Rename");
            };

            Debug.Log($"Chart '{targetChart.name}' created successfully at: {uniquePath}");
        }

        [MenuItem("Assets/Create/Scriptable Objects/Create Chart from JSON", true)]
        public static bool ValidateJson()
        {
            var selected = Selection.activeObject;
            if (selected == null)
                return false;

            string path = AssetDatabase.GetAssetPath(selected);
            return path.EndsWith(".json") && selected is TextAsset;
        }
    }
}