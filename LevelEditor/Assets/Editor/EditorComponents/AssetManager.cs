using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public enum AssetType { Texture, Sound, Font, CustomTileMap};
public class AssetManager : VisualElement
{
    static AssetManager instance;
    
    public static AssetManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new AssetManager();
            }
            return instance;
        }
    }

    static VisualElement assetManager;
    static Box addAssetContainer;
    
    EnumField assetTypeSelection;
    ObjectField assetSelectionField;
    Button addAssetButton;
    Box assetListBox;
    ScrollView assetListBoxContainer;
    
    public static VisualElement GetAssetManager()
    {
        Instance.CreateVisualTree();
        return assetManager;
    }

    void CreateVisualTree()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/AssetManager.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        assetManager = visualTree.CloneTree();
        
        addAssetContainer = assetManager.Q<Box>("addAssetContainer");

        //search field
        var popupSearchField = new ToolbarSearchField();
        popupSearchField.AddToClassList("asset-manager-searchfield");
        popupSearchField.RegisterValueChangedCallback(OnSearchTextChanged);
        
        assetTypeSelection = new EnumField(AssetType.Texture);
        assetTypeSelection.RegisterValueChangedCallback(AssetTypeChange);

        assetSelectionField = new ObjectField { objectType = typeof(UnityEngine.Texture) };

        addAssetButton = new Button();
        addAssetButton.text = "Add Asset";
        addAssetButton.RegisterCallback<MouseUpEvent>(AddAsset);

        assetListBox = new Box();
        assetListBox.style.height = 777;

        assetListBoxContainer = new ScrollView();
        assetListBoxContainer.showHorizontal = false;
        assetListBox.Add(assetListBoxContainer);

        addAssetContainer.Add(popupSearchField);
        addAssetContainer.Add(assetTypeSelection);
        addAssetContainer.Add(assetSelectionField);
        addAssetContainer.Add(addAssetButton);
        addAssetContainer.Add(assetListBox);
    }

    void AssetTypeChange(ChangeEvent<Enum> evt)
    {
        AssetType assetType =(AssetType) assetTypeSelection.value;
                assetSelectionField.value = null;
        switch (assetType)
        {
            case AssetType.Texture:
                assetSelectionField.objectType = typeof(Texture);
                break;
            case AssetType.Sound:
                assetSelectionField.objectType = typeof(AudioClip);
                break;
            case AssetType.Font:
                assetSelectionField.objectType = typeof(Font);
                break;
            case AssetType.CustomTileMap:
                assetSelectionField.objectType = typeof(CustomTileMap);
                break;
        }
    }

    public VisualElement CreateAsset(string name)
    {
        VisualElement asset = new VisualElement();
        asset.focusable = true;
        asset.name = name;
        asset.AddToClassList("row-elements");

        asset.RegisterCallback<KeyDownEvent, VisualElement>(DeleteTask, asset);

        var taskName = new Toggle() { text = name, name = "checkbox" };
        asset.Add(taskName);

        var taskDelete = new Button(() => asset.parent.Remove(asset)) { name = "delete", text = "Delete" };
        asset.Add(taskDelete);

        return asset;
    }

    void AddAsset(MouseEventBase<MouseUpEvent> evt)
    {
        if (assetSelectionField.value != null)
        {
            AssetType assetType = (AssetType)assetTypeSelection.value;
            switch (assetType)
            {
                case AssetType.Texture:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    break;
                case AssetType.Sound:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    break;
                case AssetType.Font:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    break;
                case AssetType.CustomTileMap:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    break;
            }

            //assetSelectionField.value = null;
        }
    }

    public void DeleteTask(KeyDownEvent e, VisualElement task)
    {
        if (e.keyCode == KeyCode.Delete)
        {
            if (task != null)
            {
                task.parent.Remove(task);
            }
        }
    }

    void OnSearchTextChanged(ChangeEvent<string> evt)
    {
        foreach (var asset in assetListBoxContainer.Children())
        {
            if (!string.IsNullOrEmpty(evt.newValue) && asset.name.Contains(evt.newValue))
            {
                asset[0].AddToClassList("highlight");
            }
            else
            {
                asset[0].RemoveFromClassList("highlight");
            }
        }
    }
}