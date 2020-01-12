using UnityEngine;
using UnityEditor;

namespace Towncastle.Editor
{
    [CustomEditor(typeof(HexGrid))]
    public class HexGridEditor : UnityEditor.Editor
    {
        private HexGrid targetHexGrid;

        protected void OnEnable()
        {
            targetHexGrid = target as HexGrid;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(targetHexGrid.hexBasePrefab == null);

            EditorGUI.BeginDisabledGroup(
                targetHexGrid.HexBaseCount == 0 ||
                targetHexGrid.terrainShapingHexBases == null ||
                targetHexGrid.terrainShapingHexBases.Count == 0);

            ShapeTerrainButton();
            DestroySelectedHexBasesButton();

            EditorGUI.EndDisabledGroup();

            PopulateHexBasesButton();
            ReacquireHexBasesButton();
            DestroyAllHexBasesButton();

            EditorGUI.EndDisabledGroup();
        }

        private void ShapeTerrainButton()
        {
            if (GUILayout.Button("Shape Terrain With Selected HexBases"))
            {
                targetHexGrid.ShapeTerrainWithSelectedHexBases();
            }
        }

        private void DestroySelectedHexBasesButton()
        {
            if (GUILayout.Button("Destroy Selected HexBases"))
            {
                targetHexGrid.DestroySelectedHexBases();
            }
        }

        private void PopulateHexBasesButton()
        {
            EditorGUI.BeginDisabledGroup(targetHexGrid.HexBaseCount > 0);

            if (GUILayout.Button("Populate Cells With Hex Bases"))
            {
                targetHexGrid.PopulateHexBases();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void ReacquireHexBasesButton()
        {
            EditorGUI.BeginDisabledGroup(targetHexGrid.HexBaseCount > 0);

            if (GUILayout.Button("Reacquire Lost Hex Bases"))
            {
                targetHexGrid.InitHexBases();
            }

            EditorGUI.EndDisabledGroup();
        }

        private void DestroyAllHexBasesButton()
        {
            EditorGUI.BeginDisabledGroup(targetHexGrid.HexBaseCount == 0);

            if (GUILayout.Button("Destroy All Hex Bases"))
            {
                targetHexGrid.DestroyAllHexBases();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
