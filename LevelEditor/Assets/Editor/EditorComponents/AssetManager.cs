using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.IO;
//using UnityEngine.

public enum AssetType { Texture, Sound, Font};

[Serializable]
struct AssetInfo
{
    public string Class;
    public string guid;
    public string path;
}

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

    GameAssets allGameAssets;
    
    EnumField assetTypeSelection;
    ObjectField assetSelectionField;
    Button addAssetButton;
    Button saveAssetsButton;
    Box assetListBox;
    ScrollView assetListBoxContainer;
    
    
    public static VisualElement GetAssetManager()
    {
        Instance.CreateVisualTree();
        return assetManager;
    }

    void CreateVisualTree()
    {
        string[]  guids = AssetDatabase.FindAssets("t:GameAssets");
        //foreach (string guid in guids)
        //{
        //    //Debug.Log("ScriptObj: " + AssetDatabase.GUIDToAssetPath(guid));
        //}

        if (guids != null || guids.Length > 0)
        {
            //allGameAssets = ScriptableObject.CreateInstance<GameAssets>();
            allGameAssets = (GameAssets)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(GameAssets));
        }
        else
        {
            if (AssetDatabase.IsValidFolder("Assets/Export"))
            {
                //Debug.Log("Exists");
            }
            else
            {
                string ret;

                ret = AssetDatabase.CreateFolder("Assets", "Export");
                if (AssetDatabase.GUIDToAssetPath(ret) != "")
                    Debug.Log("Folder asset created");
                else
                    Debug.Log("Couldn't find the GUID for the path");
            }
            AssetDatabase.Refresh();
            if (AssetDatabase.IsValidFolder("Assets/Export/Assets"))
            {
                //Debug.Log("Exists");
            }
            else
            {
                string ret;

                ret = AssetDatabase.CreateFolder("Assets/Export", "Assets");
                if (AssetDatabase.GUIDToAssetPath(ret) != "")
                    Debug.Log("Folder asset created");
                else
                    Debug.Log("Couldn't find the GUID for the path");
            }
            if (AssetDatabase.IsValidFolder("Assets/Export/Assets/Data"))
            {
                //Debug.Log("Exists");
            }
            else
            {
                string ret;

                ret = AssetDatabase.CreateFolder("Assets/Export/Assets", "Data");
                if (AssetDatabase.GUIDToAssetPath(ret) != "")
                    Debug.Log("Folder asset created");
                else
                    Debug.Log("Couldn't find the GUID for the path");
            }
            AssetDatabase.Refresh();
            allGameAssets = ScriptableObject.CreateInstance<GameAssets>();
            AssetDatabase.CreateAsset(allGameAssets, "Assets/Export/Data/allGameAssets.asset");
        }


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

        saveAssetsButton = new Button();
        saveAssetsButton.text = "Export All Assets";
        saveAssetsButton.RegisterCallback<MouseUpEvent>(SaveAssets);

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
        addAssetContainer.Add(saveAssetsButton);

        #region LoadAssetsFromAssetData
        foreach (Texture t in allGameAssets.gameTextures)
        {
            assetListBoxContainer.Add(CreateAsset(t.name));
        }
        foreach (AudioClip ac in allGameAssets.gameAudioClips)
        {
            assetListBoxContainer.Add(CreateAsset(ac.name));
        }
        foreach (Font f in allGameAssets.gameFonts)
        {
            assetListBoxContainer.Add(CreateAsset(f.name));
        }
        #endregion
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
            //case AssetType.CustomTileMap:
            //    assetSelectionField.objectType = typeof(CustomTileMap);
            //    break;
        }
    }

    public VisualElement CreateAsset(string name, bool editable = false)
    {
        VisualElement asset = new VisualElement
        {
            focusable = true,
            name = name
        };
        asset.AddToClassList("row-elements");

        asset.RegisterCallback<KeyDownEvent, VisualElement>(DeleteAsset, asset);

        var assetName = new Label() { text = name, name = "label" };
        asset.Add(assetName);
        if (editable)
        {
            var assetEdit = new Button() { name = "edit", text = "Edit" };
            assetEdit.RegisterCallback<MouseUpEvent, VisualElement>(EditAsset, assetEdit);
            asset.Add(assetEdit);
        }
        var assetDelete = new Button() { name = "delete", text = "Delete" };
        assetDelete.RegisterCallback<MouseUpEvent, VisualElement>(DeleteAsset, asset);
        asset.Add(assetDelete);

        return asset;
    }

    void SaveAssets(MouseEventBase<MouseUpEvent> evt)
    {
        
        //Create Destination folder

        if (AssetDatabase.IsValidFolder("Assets/Export"))
        {
            //Debug.Log("Exists");
        }
        else
        {
            string ret;
            
            ret = AssetDatabase.CreateFolder("Assets", "Export");
            if (AssetDatabase.GUIDToAssetPath(ret) != "")
                Debug.Log("Folder asset created");
            else
                Debug.Log("Couldn't find the GUID for the path");
        }
        AssetDatabase.Refresh();
        if (AssetDatabase.IsValidFolder("Assets/Export/Assets"))
        {
            //Debug.Log("Exists");
        }
        else
        {
            string ret;
            
            ret = AssetDatabase.CreateFolder("Assets/Export","Assets");
            if (AssetDatabase.GUIDToAssetPath(ret) != "")
                Debug.Log("Folder asset created");
            else
                Debug.Log("Couldn't find the GUID for the path");
        }
        AssetDatabase.Refresh();

        //Create Texture Assets

        if (AssetDatabase.IsValidFolder("Assets/Export/Assets/Images"))
        {
            //Debug.Log("Exists");
        }
        else
        {
            string ret;

            ret = AssetDatabase.CreateFolder("Assets/Export/Assets", "Images");
            if (AssetDatabase.GUIDToAssetPath(ret) != "")
                Debug.Log("Folder asset created");
            else
                Debug.Log("Couldn't find the GUID for the path");
        }
        foreach (Texture t in allGameAssets.gameTextures)
        {
            if (AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(t), "Assets/Export/Assets/Images/" + t.name + ".png"))
            {
                //Debug.Log("Material asset copied as Assets/Export/Images/" + t.name + ".png");
                AssetInfo a = new AssetInfo();
                a.Class = "TextureAsset";
                a.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(t));
                a.path = "..Assets/Export/Images/" + t.name + ".png";



                string str = JsonUtility.ToJson(a);
                using (FileStream fs = new FileStream("Assets/Export/Assets/Images/" + t.name + ".png.json", FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(str);
                        writer.Close();
                        writer.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
            else
            {
                //Debug.Log("Couldn't copy the image");
            }
            // Manually refresh the Database to inform of a change
            AssetDatabase.Refresh();
        }

        //Create Audio Clip Assets

        if (AssetDatabase.IsValidFolder("Assets/Export/Assets/Audio"))
        {
            //Debug.Log("Exists");
        }
        else
        {
            string ret;

            ret = AssetDatabase.CreateFolder("Assets/Export/Assets", "Audio");
            if (AssetDatabase.GUIDToAssetPath(ret) != "")
                Debug.Log("Folder asset created");
            else
                Debug.Log("Couldn't find the GUID for the path");
        }
        foreach (AudioClip ac in allGameAssets.gameAudioClips)
        {
            if (AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(ac), "Assets/Export/Assets/Audio/" + ac.name + ".mp3"))
            {
                //Debug.Log("Material asset copied as Assets/Export/Audio/" + ac.name + ".mp3");
                AssetInfo a = new AssetInfo();
                a.Class = "AudioAsset";
                a.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(ac));
                a.path = "..Assets/Export/Audio/" + ac.name + ".mp3";



                string str = JsonUtility.ToJson(a);
                using (FileStream fs = new FileStream("Assets/Export/Assets/Audio/" + ac.name + ".mp3.json", FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(str);
                        writer.Close();
                        writer.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
            else
            {
                //Debug.Log("Couldn't copy the audio clip");
            }
            // Manually refresh the Database to inform of a change
            AssetDatabase.Refresh();
        }

        //Create Font Assets

        if (AssetDatabase.IsValidFolder("Assets/Export/Assets/Fonts"))
        {
            //Debug.Log("Exists");
        }
        else
        {
            string ret;

            ret = AssetDatabase.CreateFolder("Assets/Export/Assets", "Fonts");
            if (AssetDatabase.GUIDToAssetPath(ret) != "")
                Debug.Log("Folder asset created");
            else
                Debug.Log("Couldn't find the GUID for the path");
        }
        foreach (Font f in allGameAssets.gameFonts)
        {
            if (AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(f), "Assets/Export/Assets/Fonts/" + f.name + ".ttf"))
            {
                //Debug.Log("Material asset copied as Assets/Export/Fonts/" + f.name + ".ttf");
                AssetInfo a = new AssetInfo();
                a.Class = "FontAsset";
                a.guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(f));
                a.path = "..Assets/Export/Fonts/" + f.name + ".ttf";



                string str = JsonUtility.ToJson(a);
                using (FileStream fs = new FileStream("Assets/Export/Assets/Fonts/" + f.name + ".ttf.json", FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.Write(str);
                        writer.Close();
                        writer.Dispose();
                    }
                    fs.Close();
                    fs.Dispose();
                }
            }
            else
            {
                //Debug.Log("Couldn't copy the font");
            }
            // Manually refresh the Database to inform of a change
            AssetDatabase.Refresh();
        }
    }

    void AddAsset(MouseEventBase<MouseUpEvent> evt)
    {
        if (assetSelectionField.value != null)
        {
            AssetType assetType = (AssetType)assetTypeSelection.value;

            foreach (VisualElement v in assetListBoxContainer.Children())
            {
                foreach (VisualElement ve in v.Children())
                {
                    if (ve.GetFirstOfType<Label>() != null)
                    {
                        if (ve.GetFirstOfType<Label>().text == assetSelectionField.value.name)
                        {
                            //Debug.LogError("An asset of similar name exists.");
                            v.Focus();
                            return;
                        }
                    }
                }
            }

            switch (assetType)
            {
                case AssetType.Texture:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    allGameAssets.gameTextures.Add((Texture)assetSelectionField.value);
                    break;
                case AssetType.Sound:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    allGameAssets.gameAudioClips.Add((AudioClip)assetSelectionField.value);
                    break;
                case AssetType.Font:
                    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name));
                    allGameAssets.gameFonts.Add((Font)assetSelectionField.value);
                    break;
                    //case AssetType.CustomTileMap:
                    //    assetListBoxContainer.Add(CreateAsset(assetSelectionField.value.name, true));
                    //    CustomTileMap ctm = (CustomTileMap)assetSelectionField.value;
                    //    //ctm.CreateTileMap();
                    //    //allGameAssets.gameTileMaps.Add(ctm);
                    //    EditorUtility.SetDirty(allGameAssets);
                    //    ctm.Print();
                    //    break;
            }

            EditorUtility.SetDirty(allGameAssets);

            //assetSelectionField.value = null;
        }
    }

    public void DeleteAsset(KeyDownEvent evt, VisualElement asset)
    {
        if (evt.keyCode == KeyCode.Delete)
        {
            if (asset != null)
            {
                foreach (VisualElement ve in asset.Children())
                {
                    if (ve.GetFirstOfType<Label>() != null)
                    {
                        for (int i = allGameAssets.gameTextures.Count-1; i>=0; i--)
                        {
                            if (ve.GetFirstOfType<Label>().text == allGameAssets.gameTextures[i].name)
                            {
                                //Debug.LogError("REMOVING");
                                allGameAssets.gameTextures.RemoveAt(i);
                                asset.parent.Remove(asset);
                                EditorUtility.SetDirty(allGameAssets);
                                break;
                            }
                        }
                        for (int i = allGameAssets.gameAudioClips.Count-1; i >= 0; i--)
                        {
                            if (ve.GetFirstOfType<Label>().text == allGameAssets.gameAudioClips[i].name)
                            {
                                //Debug.LogError("REMOVING");
                                allGameAssets.gameAudioClips.RemoveAt(i);
                                asset.parent.Remove(asset);
                                EditorUtility.SetDirty(allGameAssets);
                                break;
                            }
                        }
                        for (int i = allGameAssets.gameFonts.Count-1; i >= 0; i--)
                        {
                            if (ve.GetFirstOfType<Label>().text == allGameAssets.gameFonts[i].name)
                            {
                                //Debug.LogError("REMOVING");
                                allGameAssets.gameFonts.RemoveAt(i);
                                asset.parent.Remove(asset);
                                EditorUtility.SetDirty(allGameAssets);
                                break;
                            }   
                        }
                    }
                }
                EditorUtility.SetDirty(allGameAssets);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
    }

    public void DeleteAsset(MouseUpEvent evt, VisualElement asset)
    {

        if (asset != null)
        {
            foreach (VisualElement ve in asset.Children())
            {
                if (ve.GetFirstOfType<Label>() != null)
                {
                    for (int i = allGameAssets.gameTextures.Count-1; i >= 0; i--)
                    {
                        if (ve.GetFirstOfType<Label>().text == allGameAssets.gameTextures[i].name)
                        {
                            //Debug.LogError("REMOVING");
                            allGameAssets.gameTextures.RemoveAt(i);
                            asset.parent.Remove(asset);
                            EditorUtility.SetDirty(allGameAssets);
                            break;
                        }
                    }
                    for (int i = allGameAssets.gameAudioClips.Count-1; i >= 0; i--)
                    {
                        if (ve.GetFirstOfType<Label>().text == allGameAssets.gameAudioClips[i].name)
                        {
                            //Debug.LogError("REMOVING");
                            allGameAssets.gameAudioClips.RemoveAt(i);
                            asset.parent.Remove(asset);
                            EditorUtility.SetDirty(allGameAssets);
                            break;
                        }
                    }
                    for (int i = allGameAssets.gameFonts.Count-1; i >= 0; i--)
                    {
                        if (ve.GetFirstOfType<Label>().text == allGameAssets.gameFonts[i].name)
                        {
                            //Debug.LogError("REMOVING");
                            allGameAssets.gameFonts.RemoveAt(i);
                            asset.parent.Remove(asset);
                            EditorUtility.SetDirty(allGameAssets);
                            break;
                        }
                    }
                }
            }
            EditorUtility.SetDirty(allGameAssets);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public void EditAsset(MouseUpEvent evt, VisualElement asset)
    {
        //Debug.Log("EDIT!");
        if (asset != null)
        {
            foreach (VisualElement v in assetListBoxContainer.Children())
            {
                foreach (VisualElement ve in v.Children())
                {
                    if (ve.GetFirstOfType<Label>() != null)
                    {
                        //foreach (CustomTileMap c in allGameAssets.gameTileMaps)
                        //{
                        //    if (ve.GetFirstOfType<Label>().text == assetSelectionField.value.name)
                        //    {
                        //        c.EditTileColliders();
                        //        c.Print();
                        //        return;
                        //    }
                        //}
                    }
                }
            }
        }
    }

    void OnSearchTextChanged(ChangeEvent<string> evt)
    {
        foreach (var asset in assetListBoxContainer.Children())
        {
            if (!string.IsNullOrEmpty(evt.newValue) && asset.name.ToLower().Contains(evt.newValue.ToLower()))
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