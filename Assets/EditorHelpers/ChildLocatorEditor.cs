#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EditorHelpers
{
    [CustomEditor(typeof(ChildLocator), true)]
    public class ChildLocatorEditor : Editor
    {
        private static readonly TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        private SerializedProperty transformPairs;
        private Transform rootTransform;
        private string blacklist;

        public override void OnInspectorGUI()
        {
            transformPairs = serializedObject.FindProperty("transformPairs");
            rootTransform = EditorGUILayout.ObjectField("Root transform", rootTransform, typeof(Transform), true) as Transform;

            blacklist = EditorGUILayout.TextField("BlackList", blacklist);

            if (GUILayout.Button("Apply root transform"))
            {
                ApplyRoot();
            }
            base.OnInspectorGUI();
        }

        private void ApplyRoot()
        {
            if (!rootTransform)
            {
                return;
            }

            transformPairs.ClearArray();
            var index = 0;
            AddTransform(rootTransform, blacklist.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).Select(el => el.Trim()), ref index);
            serializedObject.ApplyModifiedProperties();
        }

        private void AddTransform(Transform transform, IEnumerable<string> blacklist, ref int index)
        {
            var name = transform.name;
            if (name.EndsWith("end") || (blacklist.Count() != 0 && blacklist.Any(el => name.Contains(el))))
            {
                return;
            }
            transformPairs.arraySize++;

            var element = transformPairs.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("name").stringValue = textInfo.ToTitleCase(Regex.Replace(Regex.Replace(transform.name, "[-_.]", " "), "([A-Z])", " $1")).Replace(" ", "");
            element.FindPropertyRelative("transform").objectReferenceValue = transform;
            
            index++;
            for (var i = 0; i < transform.childCount; i++)
            {
                AddTransform(transform.GetChild(i), blacklist, ref index);
            }
        }
    }
}
#endif