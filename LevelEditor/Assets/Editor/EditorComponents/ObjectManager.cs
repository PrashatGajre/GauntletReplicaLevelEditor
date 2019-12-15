using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System;

public class ObjectManager : VisualElement
{
    static ObjectManager instance;

    IMGUIContainer animationListContainer;
    ReorderableList animationList;

    public static ObjectManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectManager();
            }
            return instance;
        }
    }

    List<string> ObjectTypes = new List<string> { "SpriteObject", "EnvironmentObject", "StaticObject", "AnimationObject", "EnemySpawner", "Player" };
    static VisualElement objectEditor;
    static Box objectPropertiesContainer;
    static Box objectTypeContainer;
    static PopupField<string> ObjectTypeLayer;
    static Box objectEditContainer;
    static Box objectEditSelectionContainer;
    private ObjectField objectSelectionField;

    private TextField ObjectNameTextField;
    private Button AddObjectButton;
    private Button SaveObjectButton;

    public static VisualElement GetObjectManager()
    {
        Instance.CreateVisualTree();
        return objectEditor;
    }


    void CreateVisualTree()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/ObjectManager.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        objectEditor = visualTree.CloneTree();

        objectPropertiesContainer = objectEditor.Q<Box>("addObjectContainer");
        objectTypeContainer = objectEditor.Q<Box>("addObjectTypeContainer");

        ObjectNameTextField = objectPropertiesContainer.Q<TextField>("objectNameTextField");
        AddObjectButton = objectPropertiesContainer.Q<Button>("addObjectNameButton");
        AddObjectButton.RegisterCallback<MouseUpEvent>(AddNewObject);
        SaveObjectButton = objectPropertiesContainer.Q<Button>("saveObjectNameButton");
        SaveObjectButton.RegisterCallback<MouseUpEvent>(SaveEditedObject);

        ObjectTypeLayer = new PopupField<string>("Select Type Of Object", ObjectTypes, 0);
        ObjectTypeLayer.RegisterValueChangedCallback(UpdateOnTypeSelection);
        ObjectTypeLayer.Focus();
        //ObjectTypeLayer.AddToClassList("height-width-slider");
        objectTypeContainer.Add(ObjectTypeLayer);

        objectEditContainer = objectEditor.Q<Box>("editObjectContainer");
        objectEditSelectionContainer = objectEditor.Q<Box>("editObjectSelectionContainer");

        objectSelectionField = new ObjectField { objectType = typeof(SpriteObject) };
        objectSelectionField.RegisterValueChangedCallback(UpdateOnObjectSelection);

        //objectPropertiesContainer.Add(ObjectTypeLayer);
        objectEditContainer.Add(objectSelectionField);
        objectEditContainer.Add(objectEditSelectionContainer);

    }

    VisualElement ve;
    private void UpdateOnTypeSelection(ChangeEvent<string> evt)
    {
        objectSelectionField.value = null;
        if (ve != null)
        {
            ve.Clear();
        }
        switch (evt.newValue)
        {
            case "SpriteObject":
                objectSelectionField.objectType = typeof(SpriteObject);

                var spriteVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnvironmentDrawer.uxml");
                ve = spriteVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField SpriteSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                SpriteSpriteSelection.objectType = typeof(Sprite);
                //ve.Bind()
                break;
            case "EnvironmentObject":

                objectSelectionField.objectType = typeof(EnvironmentObject);

                var environementVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnvironmentDrawer.uxml");
                ve = environementVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;
                
                ObjectField EnvironementSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                EnvironementSpriteSelection.objectType = typeof(SpriteObject);
               
                ObjectField ColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                ColliderSelection.objectType = typeof(ColliderObject);
                
                objectEditSelectionContainer.Add(ve);
                break;
            case "StaticObject":
                objectSelectionField.objectType = typeof(StaticObject);

                var staticObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/StaticObjectDrawer.uxml");
                ve = staticObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField StaticObjectSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                StaticObjectSpriteSelection.objectType = typeof(SpriteObject);

                ObjectField StaticObjectColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                StaticObjectColliderSelection.objectType = typeof(ColliderObject);

                break;
            case "AnimationObject":
                objectSelectionField.objectType = typeof(AnimationObject);
                break;
            case "EnemySpawner":
                objectSelectionField.objectType = typeof(EnemyObject);
                break;
            case "Player":
                objectSelectionField.objectType = typeof(PlayerObject);
                break;
        }
    }

    private void UpdateOnObjectSelection(ChangeEvent<UnityEngine.Object> evt)
    {
        objectEditSelectionContainer.Clear();
        if (ve != null)
        {
            ve.Clear();
        }
        if (objectSelectionField.value != null)
        {
            ScriptableObject selection = (ScriptableObject)objectSelectionField.value;
            var serializedObject = new SerializedObject(selection);
            if (serializedObject == null)
                return;

            string typeOfObject = selection.GetType().ToString();
            switch (typeOfObject)
            {
                case "SpriteObject":
                    var spriteVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/SpriteDrawer.uxml");
                    ve = spriteVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField SpriteSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    SpriteSpriteSelection.objectType = typeof(Sprite);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                case "EnvironmentObject":
                    var environemntVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnvironmentDrawer.uxml");
                    ve = environemntVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField EnvironementSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    EnvironementSpriteSelection.objectType = typeof(SpriteObject);

                    ObjectField ColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                    ColliderSelection.objectType = typeof(ColliderObject);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                case "StaticObject":
                    var staticObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/StaticObjectDrawer.uxml");
                    ve = staticObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField StaticObjectSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    StaticObjectSpriteSelection.objectType = typeof(SpriteObject);

                    ObjectField StaticObjectColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                    StaticObjectColliderSelection.objectType = typeof(ColliderObject);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                case "AnimationObject":
                    break;
                case "EnemySpawner":
                    break;
                case "Player":
                    break;
            }
            //SerializedProperty property = serializedObject.GetIterator();
            //property.NextVisible(true); // Expand the first child.
            //do
            //{
            //    // Create IMGUIContainer for the IMGUI PropertyField.

            //    var copiedProperty = property.Copy();
            //    var imDefaultProperty = new IMGUIContainer(() => { DoDrawDefaultIMGUIProperty(serializedObject, copiedProperty); });
            //    imDefaultProperty.name = property.propertyPath;

            //    // Create the UIElements PropertyField.
            //    var uieDefaultProperty = new PropertyField(property);

            //    //var row = NewRow(imDefaultProperty, uieDefaultProperty);
            //    //m_ScrollView.Add(row);
            //    objectEditSelectionContainer.Add(new Label(property.name + ":" +property.ToString()));
            //}
            //while (property.NextVisible(false));
        }
    }

    private void AddNewObject(MouseUpEvent evt)
    {
        objectEditSelectionContainer.Clear();
        
        if (objectSelectionField.value == null)
        {

            string typeOfObject = ObjectTypeLayer.value;
            switch (typeOfObject)
            {
                case "SpriteObject":
                    ObjectField SpriteSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    if (SpriteSpriteSelection != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<SpriteObject>();

                        SpriteObject spriteObject = (SpriteObject)so;
                        Sprite s = (Sprite)SpriteSpriteSelection.value;
                        spriteObject.sprite = (Sprite)SpriteSpriteSelection.value;
                        EditorUtility.SetDirty(spriteObject);
                        spriteObject.x = (int)s.rect.x;
                        EditorUtility.SetDirty(spriteObject);
                        spriteObject.y = (int)s.rect.y;
                        EditorUtility.SetDirty(spriteObject);
                        spriteObject.width = (int)s.rect.width;
                        EditorUtility.SetDirty(spriteObject);
                        spriteObject.height = (int)s.rect.height;
                        EditorUtility.SetDirty(spriteObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(spriteObject, "Assets/Export/Data/SpriteData.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(spriteObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                case "EnvironmentObject":
                  
                    ObjectField EnvironmentSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    ObjectField ColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                    if (EnvironmentSpriteSelection != null && ColliderSelection != null)
                    {
                        ScriptableObject eo = ScriptableObject.CreateInstance<EnvironmentObject>();

                        EnvironmentObject environmentObject = (EnvironmentObject)eo;
                        environmentObject.sprite = (SpriteObject)EnvironmentSpriteSelection.value;
                        EditorUtility.SetDirty(environmentObject);
                        environmentObject.collider = (ColliderObject)ColliderSelection.value;
                        EditorUtility.SetDirty(environmentObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(environmentObject, "Assets/Export/Data/EnvironementData.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(environmentObject, "Assets/Export/Data/"+ ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    //ve.Bind(serializedObject);
                    break;
                case "StaticObject":
                  
                    ObjectField StaticSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    ObjectField StaticColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                    if (StaticSpriteSelection != null && StaticColliderSelection != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<StaticObject>();

                        StaticObject StaticObject = (StaticObject)so;
                        StaticObject.sprite = (SpriteObject)StaticSpriteSelection.value;
                        EditorUtility.SetDirty(StaticObject);
                        StaticObject.collider = (ColliderObject)StaticColliderSelection.value;
                        EditorUtility.SetDirty(StaticObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(StaticObject, "Assets/Export/Data/StaticObjectDrawer.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(StaticObject, "Assets/Export/Data/"+ ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                case "AnimationObject":
                    break;
                case "EnemySpawner":
                    break;
                case "Player":
                    break;
            }
        }
    }

    private void SaveEditedObject(MouseUpEvent evt)
    {
    }
}