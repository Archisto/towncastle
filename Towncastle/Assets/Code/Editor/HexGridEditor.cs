using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

namespace Towncastle.Editor
{
    [UnityEditor.CustomEditor(typeof(HexGrid))]
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

            PopulateHexBasesButton();
            DestroyHexBasesButton();
        }

        private void PopulateHexBasesButton()
        {
            if (GUILayout.Button("Populate Cells With Hex Bases"))
            {
                targetHexGrid.PopulateHexBases();
            }
        }

        private void DestroyHexBasesButton()
        {
            if (GUILayout.Button("Destroy Hex Bases"))
            {
                targetHexGrid.DestroyHexBases();
            }
        }
    }
}
