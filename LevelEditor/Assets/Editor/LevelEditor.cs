using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.ShortcutManagement;


public class LevelEditor : EditorWindow
{
    #region Attributes
    static LevelEditor window;

    VisualElement levelEditorContent;
    List<Button> menuButtons = new List<Button>();

    VisualElement AssetManagerContent;
    VisualElement PlayerEditorContent;
    VisualElement EnemyEditorContent;
    VisualElement LevelEditorContent;

    #endregion


    [Shortcut("Refresh Level Editor", KeyCode.F9)]
    [MenuItem("Level Editor Tools/LevelEditor")]
    public static void ShowLevelEditor()
    {
        if (window)
        {
            window.Close();
        }
        window = GetWindow<LevelEditor>();
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
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/LevelEditor.uxml");
        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditor.uss");
        root.styleSheets.Add(styleSheet);
       
        //Get the visual editor tree
        levelEditorContent = visualTree.CloneTree();
        root.Add(levelEditorContent);
        root.Add(AssetManager.GetAssetManager());
        string[] menuButtonNames = { "menu-button-asset", "menu-button-player", "menu-button-enemy", "menu-button-level"};
        
        //Get the menu buttons        
        //Register Click events for the menu buttons
        foreach (string mbname in menuButtonNames)
        {
            Button mb = levelEditorContent.Q<Button>(mbname);
            menuButtons.Add(mb);
            mb.RegisterCallback<MouseUpEvent, Button>(OnMenuButtonClick, mb);
        }
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
                switch (mb.name)
                {
                    //Do Something here
                    case "menu-button-asset":
                        break;
                    case "menu-button-player":
                        break;
                    case "menu-button-enemy":
                        break;
                    case "menu-button-level":
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