using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(Skill), true)]
public class SampleEditor : Editor
{
    private ReorderableList list;

    private struct StepCreationParams
    {
        public eSkillStepType type;
    }

    private void OnEnable()
    {
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("performSteps"), true, true, true, true);
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;

            EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("type"), GUIContent.none);
            //EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("testPrefab"), GUIContent.none);
            //EditorGUI.PropertyField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("duration"), GUIContent.none);
            
            var type = element.FindPropertyRelative("type").enumValueIndex; // 이 방법으로 enum의 int 변환
            switch((eSkillStepType)type)
            {
                case eSkillStepType.Idle:
                    EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("duration"), GUIContent.none);
                    break;
                case eSkillStepType.SpawnProjectile:
                    {
                        var inst = element.FindPropertyRelative("projectile");
                        EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("code"), GUIContent.none);
                    }
                    break;
                case eSkillStepType.TrackTarget:
                    {
                        var inst = element.FindPropertyRelative("trackTarget");
                        EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, 30, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("speed"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 60 + 60, rect.y, 30, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("offset").FindPropertyRelative("x"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 60 + 60 + 30, rect.y, 30, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("offset").FindPropertyRelative("y"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 60 + 60 + 60, rect.y, 30, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("offset").FindPropertyRelative("z"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 60 + 60 + 60 + 30, rect.y, 30, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("stopDistance"), GUIContent.none);
                        EditorGUI.PropertyField(new Rect(rect.x + 60 + 60 + 60 + 60, rect.y, rect.width - -240, EditorGUIUtility.singleLineHeight), inst.FindPropertyRelative("canPenetrateEnviroment"), GUIContent.none);
                    }
                    break;
            }
        };

        list.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Perform Steps");
        };

        list.onSelectCallback = (ReorderableList l) =>
        {
            /*var prefab = l.serializedProperty.GetArrayElementAtIndex(l.index).FindPropertyRelative("testPrefab").objectReferenceValue as GameObject;
            if (prefab)
                EditorGUIUtility.PingObject(prefab.gameObject);
            */
        };

        list.onCanRemoveCallback = (ReorderableList l) =>
        {
            return l.count > 1;
        };

        list.onAddCallback = (ReorderableList l) => 
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var element = l.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("Type").enumValueIndex = 0;
            //element.FindPropertyRelative("testPrefab").objectReferenceValue = 
            //element.FindPropertyRelative("duration").floatValue = 1f;
        };

        list.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            for (int i = 0; i <= (int)eSkillStepType.TrackTarget; i++) // 갱신할 필요가 있다!
            {
                menu.AddItem(new GUIContent(((eSkillStepType)i).ToString()), false, clickHandler, new StepCreationParams { type = (eSkillStepType)i});
            }

            menu.ShowAsContext();
        };
    }

    public override void OnInspectorGUI()
    {
        base.DrawDefaultInspector();
        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }

    private void clickHandler(object target)
    {
        var data = (StepCreationParams)target;
        var index = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
        list.index = index;
        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        switch (data.type)
        {
            case eSkillStepType.SpawnProjectile:
                {
                    var inst = element.FindPropertyRelative("SpawnProjectile");
                }
                break;
        }

        element.FindPropertyRelative("type").enumValueIndex = (int)data.type;
        serializedObject.ApplyModifiedProperties();
    }
}

#endif
