using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShortcutManagement;


public class LevelEditorWindow : EditorWindow
{
    #region Attributes
    static LevelEditorWindow window;

    VisualElement levelEditorWindowContent;
    List<Button> menuButtons = new List<Button>();

    Box container;

    //AssetManager assetManager = new AssetManager();
    VisualElement assetManagerContent;
    VisualElement playerEditorContent;
    VisualElement enemyEditorContent;
    VisualElement levelEditorContent;

    #endregion


    [Shortcut("Refresh Level Editor", KeyCode.F9)]
    [MenuItem("Level Editor Tools/LevelEditor")]
    public static void ShowLevelEditor()
    {
        if (window)
        {
            window.Close();
        }
        window = GetWindow<LevelEditorWindow>();
        window.titleContent = new GUIContent("LevelEditor");
        window.position = new Rect(0, 0, 1200, 900);
        window.minSize = new Vector2(1200, 900);
        window.maxSize = new Vector2(1200, 900);
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;
        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditorWindow.uxml");
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        root.styleSheets.Add(styleSheet);
       
        //Get the visual editor tree
        levelEditorWindowContent = visualTree.CloneTree();
        root.Add(levelEditorWindowContent);

        //Get the container for the window content
        container = levelEditorWindowContent.Q<Box>("mainContentContainer");

        //Get the menu buttons                
        //Register Click events for the menu buttons
        string[] menuButtonNames = { "menu-button-asset", "menu-button-player", "menu-button-enemy", "menu-button-level"};
        foreach (string mbname in menuButtonNames)
        {
            Button mb = levelEditorWindowContent.Q<Button>(mbname);
            menuButtons.Add(mb);
            mb.RegisterCallback<MouseUpEvent, Button>(OnMenuButtonClick, mb);
        }

        //Add AssetManager by default
        assetManagerContent = AssetManager.GetAssetManager();
        //AssetManagerContent.visible = false;
        //container.Add(AssetManagerContent);

        playerEditorContent = PlayerEditor.GetPlayerEditor();
        //playerEditorContent.visible = false;
        //container.Add(playerEditorContent);

        enemyEditorContent = EnemyEditor.GetEnemyEditor();

        levelEditorContent = LevelEditor.GetLevelEditor();

    }

    //When a menu button is clicked
    void OnMenuButtonClick(MouseEventBase<MouseUpEvent> evt, Button b)
    {        
        foreach (Button mb in menuButtons)
        {
            //Selected Button
            if (mb == b)
            {
                mb.RemoveFromClassList("menu-button-off");
                mb.AddToClassList("menu-button-on");

                //Add to OR Edit these conditions in case the buttons or their name change -- Had kept text of the button, but since we already have the button name information, going with name

                if (container.Contains(assetManagerContent))
                {
                    container.Remove(assetManagerContent);
                }
                if (container.Contains(playerEditorContent))
                {
                    container.Remove(playerEditorContent);
                }
                if (container.Contains(enemyEditorContent))
                {
                    container.Remove(enemyEditorContent);
                }
                if (container.Contains(levelEditorContent))
                {
                    container.Remove(levelEditorContent);
                }

                switch (mb.name)
                {
                    //Do Something here
                    case "menu-button-asset":
                        //AssetManagerContent.visible = true;
                        //PlayerEditorContent.visible = false;
                        container.Add(assetManagerContent);
                       
                        break;
                    case "menu-button-player":
                        container.Add(playerEditorContent);
                        
                        break;
                    case "menu-button-enemy":
                        container.Add(enemyEditorContent);  
                       
                        break;
                    case "menu-button-level":
                        container.Add(levelEditorContent);
                       
                        break;
                    default:
                        Debug.LogError("Change the name of the button OR change the switch case here.");
                        break;
                }
            }
            else
            {
                mb.RemoveFromClassList("menu-button-on");
                mb.AddToClassList("menu-button-off");
            }
        }
    }
}