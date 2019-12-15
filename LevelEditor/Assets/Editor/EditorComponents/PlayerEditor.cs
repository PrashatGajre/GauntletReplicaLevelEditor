using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;


public class AnimationData
{
    public string name;
    public Rect[] spritepositions;

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
    static List<AnimationObject> animationNames = new List<AnimationObject>();
    
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
        animationList = new ReorderableList(animationNames, typeof(AnimationObject));
        animationList.drawElementCallback = DrawList;
        animationList.drawHeaderCallback = WriteHeader;
        animationList.onReorderCallback = ChangeListOrder;
        animationList.onSelectCallback = SelectListItem;
        animationList.onAddCallback = AddToList;
        animationList.onRemoveCallback = RemoveFromList;
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

    #region ReorderableListCallbacks

    void DrawList(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = animationList.serializedProperty.GetArrayElementAtIndex(index);
        var name = element.FindPropertyRelative("name");
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), name.stringValue);
    }
    void WriteHeader(Rect rect)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), "Animations");
    }
    void ChangeListOrder(ReorderableList list)
    {
        Debug.Log("CHANGE ORDER HERE");
    }
    void SelectListItem(ReorderableList list)
    {
        Debug.Log("SELECT HERE");
    }
    void AddToList(ReorderableList list)
    {
        Debug.Log("ADD HERE");
    }
    void RemoveFromList(ReorderableList list)
    {
        Debug.Log("DELETE HERE");
    }
    #endregion

}