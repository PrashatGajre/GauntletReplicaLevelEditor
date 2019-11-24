using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class AssetManager : VisualElement
{
    static VisualElement assetManager;
    [MenuItem("Window/UIElements/AssetManager")]
    public static VisualElement GetAssetManager()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/AssetManager.uxml");
        assetManager = visualTree.CloneTree();
        return assetManager;
    }

    public void OnEnable()
    {
        
    }
}