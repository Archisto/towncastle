using UnityEngine;

namespace Towncastle.Editor
{
    [UnityEditor.CustomEditor(typeof(HexObject))]
    public class HexObjectEditor : UnityEditor.Editor
    {
        private HexObject targetHexObject;

        protected void OnEnable()
        {
            targetHexObject = target as HexObject;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DisplayHexMeshInfo();
        }

        private void DisplayHexMeshInfo()
        {
            if (targetHexObject.HexMesh != null)
                GUILayout.Label(string.Format("Hex Mesh: {0} ({1})", targetHexObject.HexMesh.name, targetHexObject.Type));
            else
                GUILayout.Label("Hex Mesh: " + HexObject.StructureType.Undefined);
        }
    }
}
