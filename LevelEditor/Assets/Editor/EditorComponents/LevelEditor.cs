using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;

public class LevelEditor : VisualElement
{
    static LevelEditor instance;

    IMGUIContainer animationListContainer;
    ReorderableList animationList;

    public static LevelEditor Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelEditor();
            }
            return instance;
        }
    }

    static VisualElement levelEditor;
    static Box levelPropertiesContainer;
    static Box addLevelContainer;
    static List<AnimationData> animationNames = new List<AnimationData>();

    public static VisualElement GetLevelEditor()
    {
        Instance.CreateVisualTree();
        return levelEditor;
    }

    void CreateVisualTree()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/LevelEditor.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        levelEditor = visualTree.CloneTree();

        addLevelContainer = levelEditor.Q<Box>("addLevelContainer");
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
        addLevelContainer.Add(animationListContainer);
    }
    void DrawAnimationList()
    {
        if (animationList != null)
        {
            animationList.DoLayoutList();
        }
    }
}