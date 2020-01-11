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

            ShapeTerrainButton();
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
