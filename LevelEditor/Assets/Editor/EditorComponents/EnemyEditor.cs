using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;

public class EnemyEditor : VisualElement
{
    static EnemyEditor instance;

    IMGUIContainer animationListContainer;
    ReorderableList animationList;

    public static EnemyEditor Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EnemyEditor();
            }
            return instance;
        }
    }

    static VisualElement enemyEditor;
    static Box enemyPropertiesContainer;
    static Box enemyAnimationContainer;
    static List<AnimationData> animationNames = new List<AnimationData>();

    public static VisualElement GetEnemyEditor()
    {
        Instance.CreateVisualTree();
        return enemyEditor;
    }

    void CreateVisualTree()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/EnemyEditor.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        enemyEditor = visualTree.CloneTree();

        enemyAnimationContainer = enemyEditor.Q<Box>("enemyAnimationContainer");
        animationListContainer = new IMGUIContainer(DrawAnimationList);
        animationListContainer.onGUIHandler = DrawAnimationList;
        animationList = new ReorderableList(animationNames, typeof(AnimationData));
        animationList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = animationList.serializedProperty.GetArrayElementAtIndex(index);
                var name = element.FindPropertyRelative("name");
                EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), name.stringValue);
            };
        enemyAnimationContainer.Add(animationListContainer);
    }
    void DrawAnimationList()
    {
        if (animationList != null)
        {
            animationList.DoLayoutList();
        }
    }
}