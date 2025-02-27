//using CoreTeamGamesSDK.SettingsService;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEditor;
//using UnityEditorInternal;
//using UnityEngine;

//[CustomEditor(typeof(SettingsValues))]
//public class SettingsValuesEditor : Editor
//{
//    private SerializedProperty _property;
//    private ReorderableList _list;

//    private void OnEnable()
//    {
//        _property = serializedObject.FindProperty("_values");

//        _list = new ReorderableList(serializedObject, _property, true, true, true, true)
//        {
//            drawHeaderCallback = DrawHeaderCallback,
//            drawElementCallback = DrawElementCallback,
//            elementHeight = EditorGUIUtility.singleLineHeight * 3 + 4,
//        };
//    }

//    private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
//    {
//        rect.y += 2;

//        SerializedProperty elementAtIndex = _property.GetArrayElementAtIndex(index);
//        if (elementAtIndex == null)
//            return;

//        // Определяем прямоугольники для полей
//        Rect nameRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
//        Rect valueTypeRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight);
//        Rect valueRect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width, EditorGUIUtility.singleLineHeight);

//        // Рисуем поле для имени
//        SerializedProperty nameProperty = elementAtIndex.FindPropertyRelative("_name");
//        if (nameProperty != null)
//        {
//            nameProperty.stringValue = EditorGUI.TextField(nameRect, "Name", nameProperty.stringValue);
//        }

//        // Получаем сериализуемое значение
//        SerializedProperty valueProperty = elementAtIndex.FindPropertyRelative("_value");
//        if (valueProperty == null)
//            return;

//        // Получаем текущее значение через целевой объект (так как managedReferenceValue в вашей версии Unity доступен только как setter)
//        object currentValue = ((SettingsValues)target).Values[index].Value;
//        Type currentType = currentValue != null ? currentValue.GetType() : null;

//        // Определяем индекс выбранного типа (если значение уже установлено)
//        int selectedIndex = currentType != null ? GetArrayIndex(currentType.Name) : 0;

//        // Отрисовываем выпадающий список для выбора типа
//        selectedIndex = EditorGUI.Popup(valueTypeRect, selectedIndex, HandledTypes.HandledTypesNamesArray);
//        Type selectedType = HandledTypes.HandledTypesArray[selectedIndex];

//        // Если тип изменился или значение равно null, создаём новое значение
//        if (currentValue == null || currentType != selectedType)
//        {
//            try
//            {
//                if (selectedType == typeof(string))
//                {
//                    currentValue = ""; // для string – пустая строка
//                }
//                else if (typeof(UnityEngine.Object).IsAssignableFrom(selectedType))
//                {
//                    currentValue = null; // для Unity объектов оставляем null
//                }
//                else
//                {
//                    currentValue = Activator.CreateInstance(selectedType);
//                }
//                valueProperty.managedReferenceValue = currentValue;
//                // Также обновляем значение в целевом объекте, чтобы при повторном входе в DrawElementCallback оно было видно
//                ((SettingsValues)target).Values[index].SetValueWithoutNotify(currentValue);
//            }
//            catch (Exception e)
//            {
//                Debug.LogError("Could not create instance of type " + selectedType + ": " + e.Message);
//                return;
//            }
//        }

//        // Отрисовка поля редактирования с использованием switch
//        switch (selectedType)
//        {
//            case Type t when t == typeof(string):
//                {
//                    string str = currentValue as string ?? "";
//                    string newStr = EditorGUI.TextField(valueRect, str);
//                    if (newStr != str)
//                    {
//                        // Обновляем значение через SerializedProperty (хотя в вашей версии доступен только setter)
//                        valueProperty.managedReferenceValue = newStr;

//                        // Обновляем закрытое поле _value через рефлексию
//                        var settingsValues = (SettingsValues)target;
//                        var settingsValueWithName = settingsValues.Values[index];
//                        var fieldInfo = settingsValueWithName.GetType().GetField("_value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
//                        if (fieldInfo != null)
//                        {
//                            fieldInfo.SetValue(settingsValueWithName, newStr);
//                        }
//                        EditorUtility.SetDirty(target);
//                    }
//                    break;
//                }
//            case Type t when t == typeof(bool):
//                {
//                    bool boolVal = currentValue != null ? (bool)currentValue : false;
//                    bool newBool = EditorGUI.Toggle(valueRect, boolVal);
//                    valueProperty.managedReferenceValue = newBool;
//                    break;
//                }
//            case Type t when t == typeof(int):
//                {
//                    int intVal = currentValue != null ? (int)currentValue : 0;
//                    int newInt = EditorGUI.IntField(valueRect, intVal);
//                    valueProperty.managedReferenceValue = newInt;
//                    break;
//                }
//            case Type t when t == typeof(float):
//                {
//                    float floatVal = currentValue != null ? (float)currentValue : 0f;
//                    float newFloat = EditorGUI.FloatField(valueRect, floatVal);
//                    valueProperty.managedReferenceValue = newFloat;
//                    break;
//                }
//            case Type t when t == typeof(Vector2):
//                {
//                    Vector2 vec = currentValue != null ? (Vector2)currentValue : Vector2.zero;
//                    Vector2 newVec = EditorGUI.Vector2Field(valueRect, "", vec);
//                    valueProperty.managedReferenceValue = newVec;
//                    break;
//                }
//            case Type t when t == typeof(Vector2Int):
//                {
//                    Vector2Int vec = currentValue != null ? (Vector2Int)currentValue : Vector2Int.zero;
//                    Vector2Int newVec = EditorGUI.Vector2IntField(valueRect, "", vec);
//                    valueProperty.managedReferenceValue = newVec;
//                    break;
//                }
//            case Type t when t == typeof(Vector3):
//                {
//                    Vector3 vec = currentValue != null ? (Vector3)currentValue : Vector3.zero;
//                    Vector3 newVec = EditorGUI.Vector3Field(valueRect, "", vec);
//                    valueProperty.managedReferenceValue = newVec;
//                    break;
//                }
//            case Type t when t == typeof(Vector3Int):
//                {
//                    Vector3Int vec = currentValue != null ? (Vector3Int)currentValue : Vector3Int.zero;
//                    Vector3Int newVec = EditorGUI.Vector3IntField(valueRect, "", vec);
//                    valueProperty.managedReferenceValue = newVec;
//                    break;
//                }
//            case Type t when t == typeof(Resolution):
//                {
//                    Resolution res = currentValue != null ? (Resolution)currentValue : new Resolution();
//                    EditorGUI.BeginChangeCheck();
//                    int width = EditorGUI.IntField(new Rect(valueRect.x, valueRect.y, valueRect.width / 2, valueRect.height), "Width", res.width);
//                    int height = EditorGUI.IntField(new Rect(valueRect.x + valueRect.width / 2, valueRect.y, valueRect.width / 2, valueRect.height), "Height", res.height);
//                    if (EditorGUI.EndChangeCheck())
//                    {
//                        res.width = width;
//                        res.height = height;
//                        valueProperty.managedReferenceValue = res;
//                    }
//                    break;
//                }
//            default:
//                {
//                    EditorGUI.LabelField(valueRect, $"Type {selectedType.Name} not supported.");
//                    break;
//                }
//        }
//    }

//    private void DrawHeaderCallback(Rect rect)
//    {
//        EditorGUI.LabelField(rect, "SettingsValues");
//    }

//    private int GetArrayIndex(string typeName)
//    {
//        for (int i = 0; i < HandledTypes.HandledTypesNamesArray.Length; i++)
//        {
//            if (HandledTypes.HandledTypesNamesArray[i] == typeName)
//                return i;
//        }
//        return 0;
//    }

//    public override void OnInspectorGUI()
//    {
//        serializedObject.Update();
//        _list.DoLayoutList();
//        serializedObject.ApplyModifiedProperties();
//    }
//}