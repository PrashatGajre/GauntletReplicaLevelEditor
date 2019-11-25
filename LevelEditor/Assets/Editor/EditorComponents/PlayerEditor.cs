using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
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

    }

}