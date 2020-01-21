using UnityEngine;
using UnityEditor;

namespace Towncastle.Editor
{
    [CustomEditor(typeof(HexBase)), CanEditMultipleObjects]
    public class HexBaseEditor : UnityEditor.Editor
    {
        private HexBase targetHexBase;

        protected void OnEnable()
        {
            targetHexBase = target as HexBase;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DisplayHexBaseInfo();
            ShapeTerrainButton();
        }

        private void DisplayHexBaseInfo()
        {
            GUILayout.Label(string.Format("Position Y: {0}\nHeight level: {1}",
                targetHexBase.transform.position.y, targetHexBase.HeightLevel));
        }

        private void ShapeTerrainButton()
        {
            EditorGUI.BeginDisabledGroup(targetHexBase.frozen);

            if (GUILayout.Button("Shape Terrain"))
            {
                targetHexBase.ShapeTerrain();
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
