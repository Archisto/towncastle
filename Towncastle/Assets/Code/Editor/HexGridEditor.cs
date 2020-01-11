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
            ShapeTerrainButton();
            PopulateHexBasesButton();
            DestroyHexBasesButton();
            EditorGUI.EndDisabledGroup();
        }

        private void ShapeTerrainButton()
        {
            EditorGUI.BeginDisabledGroup(
                targetHexGrid.HexBaseCount == 0 ||
                targetHexGrid.terrainShapingHexBases == null ||
                targetHexGrid.terrainShapingHexBases.Count == 0);

            if (GUILayout.Button("Shape Terrain With Selected HexBases"))
            {
                targetHexGrid.ShapeTerrainWithSelectedHexBases();
            }

            EditorGUI.EndDisabledGroup();
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

        private void DestroyHexBasesButton()
        {
            EditorGUI.BeginDisabledGroup(targetHexGrid.HexBaseCount == 0);

            if (GUILayout.Button("Destroy Hex Bases"))
            {
                targetHexGrid.DestroyHexBases();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
