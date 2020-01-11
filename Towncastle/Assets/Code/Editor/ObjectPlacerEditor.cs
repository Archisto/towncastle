using UnityEngine;
using UnityEditor;

namespace Towncastle.Editor
{
    [CustomEditor(typeof(ObjectPlacer))]
    public class ObjectPlacerEditor : UnityEditor.Editor
    {
        private ObjectPlacer targetObjectPlacer;

        /// <summary>
        /// Determines whether the inspector needs to be updated each frame.
        /// </summary>
        /// <returns>Does the inspector update each frame.</returns>
        public override bool RequiresConstantRepaint() { return true; }

        protected void OnEnable()
        {
            targetObjectPlacer = target as ObjectPlacer;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DisplayPlacementInfo();
        }

        private void DisplayPlacementInfo()
        {
            GUILayout.Label(targetObjectPlacer.GetPlacementInfo());
        }
    }
}
