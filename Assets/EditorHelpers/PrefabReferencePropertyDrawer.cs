using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace RoR2
{
    [CustomPropertyDrawer(typeof(PrefabReferenceAttribute))]
    public class PrefabReferencePropertyDrawer : PropertyDrawer
    {
        private GameObject memorizedSceneObject;
        private GameObject memorizedPrefabObject;

        private static GameObject ConvertToPrefab(GameObject sceneObject)
        {
            var prefabStage = PrefabStageUtility.GetPrefabStage(sceneObject);
            if (prefabStage == null)
            {
                return null;
            }

            var assetPath = prefabStage.prefabAssetPath;
            if (assetPath == null)
            {
                return null;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            var relativePath = Util.BuildPrefabTransformPath(sceneObject.transform.root, sceneObject.transform, false);

            return prefab?.transform.Find(relativePath)?.gameObject;
        }

        private GameObject GetDraggedPrefabObject()
        {
            var objectReferences = DragAndDrop.objectReferences;
            if (objectReferences.Length != 1 || !(objectReferences[0] is GameObject gameObject))
            {
                return null;
            }

            if (!ReferenceEquals(memorizedSceneObject, gameObject))
            {
                memorizedSceneObject = gameObject;
                memorizedPrefabObject = ConvertToPrefab(memorizedSceneObject);
            }
            return memorizedPrefabObject;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label, true);
            if (!position.Contains(Event.current.mousePosition))
            {
                return;
            }

            switch (Event.current.type)
            {
                case EventType.DragUpdated:
                    var draggedObject = GetDraggedPrefabObject();
                    if (draggedObject)
                    {
                        DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    }
                    break;
                case EventType.DragPerform:
                    draggedObject = GetDraggedPrefabObject();
                    if (draggedObject)
                    {
                        DragAndDrop.AcceptDrag();
                        property.objectReferenceValue = draggedObject;
                    }
                    break;
            }
        }
    }
}
