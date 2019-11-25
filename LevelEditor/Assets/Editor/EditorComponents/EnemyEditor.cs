using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
public class EnemyEditor : VisualElement
{
    static EnemyEditor instance;

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
    static Box playerPropertiesContainer;

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

        //playerPropertiesContainer = enemyEditor.Q<Box>("playerPropertiesContainer");

        //var playerSpriteSheet = new Label("Sprite Sheet");
        //playerSpriteSheet.name = "playerSpriteSheet";
        //var playerSpriteSelection = new ObjectField { objectType = typeof(UnityEngine.Texture) };
        //playerSpriteSelection.name = "playerSpriteSelection";

        //var playerSpeed = new Label("Speed");
        //playerSpeed.name = "playerSpeed";
        //var playerSpeedSelection = new FloatField();
        //playerSpeedSelection.name = "playerSpeedSelection";

        //var playerHealth = new Label("Health");
        //playerHealth.name = "playerHealth";
        //var playerHealthSelection = new FloatField();
        //playerHealthSelection.name = "playerHealthSelection";

        //playerPropertiesContainer.Add(playerSpriteSheet);
        //playerPropertiesContainer.Add(playerSpriteSelection);
        //playerPropertiesContainer.Add(playerSpeed);
        //playerPropertiesContainer.Add(playerSpeedSelection);
        //playerPropertiesContainer.Add(playerHealth);
        //playerPropertiesContainer.Add(playerHealthSelection);
    }

}