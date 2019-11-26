using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;


public class AnimationData
{
    string name;
    Rect[] spritepositions;

    public string getName() { return name; }
}

public class PlayerEditor : VisualElement
{
    static PlayerEditor instance;

    public static PlayerEditor Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerEditor();
            }
            return instance;
        }
    }

    static VisualElement playerEditor;
    static Box playerPropertiesContainer;
    static Box playerAnimationContainer;
    static List<AnimationData> animationNames = new List<AnimationData>();
    
    public static VisualElement GetPlayerEditor()
    {
        Instance.CreateVisualTree();
        return playerEditor;
    }

    void CreateVisualTree()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/PlayerEditor.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        playerEditor = visualTree.CloneTree();

        playerPropertiesContainer = playerEditor.Q<Box>("playerPropertiesContainer");
        playerAnimationContainer = playerEditor.Q<Box>("playerAnimationContainer");

        var playerSpriteSheet = new Label("Sprite Sheet");
        playerSpriteSheet.name = "playerSpriteSheet";
        var playerSpriteSelection = new ObjectField { objectType = typeof(UnityEngine.Texture) };
        playerSpriteSelection.name = "playerSpriteSelection";

        var playerSpeed = new Label("Speed");
        playerSpeed.name = "playerSpeed";
        var playerSpeedSelection = new FloatField();
        playerSpeedSelection.name = "playerSpeedSelection";

        var playerHealth = new Label("Health");
        playerHealth.name = "playerHealth";
        var playerHealthSelection = new FloatField();
        playerHealthSelection.name = "playerHealthSelection";

        playerPropertiesContainer.Add(playerSpriteSheet);
        playerPropertiesContainer.Add(playerSpriteSelection);
        playerPropertiesContainer.Add(playerSpeed);
        playerPropertiesContainer.Add(playerSpeedSelection);
        playerPropertiesContainer.Add(playerHealth);
        playerPropertiesContainer.Add(playerHealthSelection);

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
        playerAnimationContainer.Add(animationListContainer);

    }


    IMGUIContainer animationListContainer;
    ReorderableList animationList;
    void DrawAnimationList()
    {
         if (animationList != null)
        {
            animationList.DoLayoutList();
        }
    }


}