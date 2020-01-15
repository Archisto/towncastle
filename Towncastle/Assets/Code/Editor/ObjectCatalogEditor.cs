using UnityEngine;
using UnityEditor;

namespace Towncastle.Editor
{
    [CustomEditor(typeof(ObjectCatalog))]
    public class ObjectCatalogEditor : UnityEditor.Editor
    {
        private ObjectCatalog targetObjectCatalog;

        protected void OnEnable()
        {
            targetObjectCatalog = target as ObjectCatalog;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            //SortButton();
        }

        private void SortButton()
        {
            if (GUILayout.Button("Sort"))
            {
                targetObjectCatalog.Sort();
            }
        }
    }
}
