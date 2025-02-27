using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace CoreTeamGamesSDK.SettingsService.Editor
{
    [CustomEditor(typeof(SettingsValue), true)]
    public class SettingsValueEditor : UnityEditor.Editor
    {
        private SerializedProperty _name;
        private SerializedProperty _valueType;
        private SerializedProperty _value;
        private SerializedProperty _defaultValue;
        private SerializedProperty _canChangeValue;
        private SerializedProperty _excludedRuntimePlatformsForValue;
        private ReorderableList _excludedRuntimePlatformsForValueReorderableList;

        private void OnEnable()
        {
            _name = serializedObject.FindProperty("_name");
            _valueType = serializedObject.FindProperty("_valueType");
            _value = serializedObject.FindProperty("value");
            _defaultValue = serializedObject.FindProperty("defaultValue");
            _canChangeValue = serializedObject.FindProperty("canChangeValue");
            _excludedRuntimePlatformsForValue = serializedObject.FindProperty("_excludedRuntimePlatformsForValue");

            if (_excludedRuntimePlatformsForValue == null)
            {
                Debug.LogError("Failed to find _excludedRuntimePlatformsForValue property.");
                return;
            }

            _excludedRuntimePlatformsForValueReorderableList = new ReorderableList(serializedObject, _excludedRuntimePlatformsForValue, true, true, true, true)
            {
                drawHeaderCallback = OnDrawHeader,
                drawElementCallback = OnDrawElement,
                elementHeight = EditorGUIUtility.singleLineHeight
            };
        }

        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, "Excluded Runtime Platforms");
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (index < 0 || index >= _excludedRuntimePlatformsForValue.arraySize)
                return;

            SerializedProperty propertyAtIndex = _excludedRuntimePlatformsForValue.GetArrayElementAtIndex(index);
            if (propertyAtIndex == null)
                return;

            EditorGUI.PropertyField(rect, propertyAtIndex, GUIContent.none);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _name.stringValue = EditorGUILayout.TextField(new GUIContent("Value Name", "The name of SettingsValue"), _name.stringValue);

            if (_excludedRuntimePlatformsForValueReorderableList != null)
            {
                _excludedRuntimePlatformsForValueReorderableList.DoLayoutList();
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}