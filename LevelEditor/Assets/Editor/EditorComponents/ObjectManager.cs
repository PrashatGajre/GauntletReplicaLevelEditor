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

    List<string> ObjectTypes = new List<string> { "SpriteObject", "EnvironmentObject", "StaticObject", "AnimationObject", "EnemyObject", "EnemySpawner", "PlayerObject", "PolygonCollider", "CircleCollider", "ProjectileObject" };
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
        ObjectNameTextField.value = null;
        objectSelectionField.value = null;
        objectEditSelectionContainer.Clear();
        if (ve != null)
        {
            ve.Clear();
        }
        switch (evt.newValue)
        {
            case "SpriteObject":
                #region
                objectSelectionField.objectType = typeof(SpriteObject);

                var spriteVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/SpriteDrawer.uxml");
                ve = spriteVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField SpriteSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                SpriteSpriteSelection.objectType = typeof(Sprite);

                objectEditSelectionContainer.Add(ve);
                //ve.Bind()
                break;
            #endregion
            case "EnvironmentObject":
                #region
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
            #endregion
            case "StaticObject":
                #region
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

                objectEditSelectionContainer.Add(ve);

                break;
            #endregion
            case "AnimationObject":
                #region
                objectSelectionField.objectType = typeof(AnimationObject);

                var animationVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/AnimationObjectDrawer.uxml");
                ve = animationVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ListView AnimationSpriteObjectsList = ve.Q<ListView>("AnimationSpriteObjectSelectionField");
                //AnimationSpriteObjectsList.bindItem
                //AnimationSpriteObjectsList.objectType = typeof(Sprite);

                objectEditSelectionContainer.Add(ve);
                break;
            #endregion
            case "EnemyObject":
                #region
                objectSelectionField.objectType = typeof(EnemyObject);
                var enemyObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnemyDrawer.uxml");
                ve = enemyObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField EnemyObjectSpriteSelection = ve.Q<ObjectField>("EnemySpriteObjectSelectionField");
                EnemyObjectSpriteSelection.objectType = typeof(SpriteObject);

                FloatField EnemyHealthField = ve.Q<FloatField>("EnemyObjectHealthField");
                FloatField EnemyAttackField = ve.Q<FloatField>("EnemyObjectAttackField");

                ObjectField EnemyObjectColliderSelection = ve.Q<ObjectField>("EnemyColliderObjectSelectionField");
                EnemyObjectColliderSelection.objectType = typeof(CircleColliderObject);

                objectEditSelectionContainer.Add(ve);
                break;
            #endregion
            case "EnemySpawner":
                #region
                objectSelectionField.objectType = typeof(EnemySpawnerObject);
                var enemySpawnerObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnemySpawnerDrawer.uxml");
                ve = enemySpawnerObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField EnemySpawnerObjectSpriteSelection = ve.Q<ObjectField>("EnemySpawnerSpriteObjectSelectionField");
                EnemySpawnerObjectSpriteSelection.objectType = typeof(SpriteObject);

                FloatField EnemySpawnerTimeField = ve.Q<FloatField>("EnemySpawnTimeField");

                ObjectField EnemySpawnerenemySelection = ve.Q<ObjectField>("EnemyObjectSelectionField");
                EnemySpawnerenemySelection.objectType = typeof(EnemyObject);
                ObjectField EnemySpawnerObjectColliderSelection = ve.Q<ObjectField>("SpawnerColliderObjectSelectionField");
                EnemySpawnerObjectColliderSelection.objectType = typeof(CircleColliderObject);

                objectEditSelectionContainer.Add(ve);
                break;
            #endregion
            //case "Player":
            //    objectSelectionField.objectType = typeof(PlayerObject);
            //    break;
            case "PolygonCollider":
                #region
                objectSelectionField.objectType = typeof(PolygonColliderObject);
                var polyColliderObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/PolygonColliderDrawer.uxml");
                ve = polyColliderObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                FloatField PolyColliderHeightField = ve.Q<FloatField>("PolygonColliderHeightField");
                FloatField PolyColliderWidthField = ve.Q<FloatField>("PolygonColliderWidthField");
                Toggle PolyColliderIsTriggerToggle = ve.Q<Toggle>("PolygonColliderSetTrigger");

                objectEditSelectionContainer.Add(ve);
                break;
                #endregion
            case "CircleCollider":
                #region
                objectSelectionField.objectType = typeof(CircleColliderObject);
                var cirColliderObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/CircleColliderDrawer.uxml");
                ve = cirColliderObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                FloatField CircleColliderHeightField = ve.Q<FloatField>("CircleColliderRadiusField");
                Toggle CircleColliderIsTriggerToggle = ve.Q<Toggle>("CircleColliderSetTrigger");

                objectEditSelectionContainer.Add(ve);
                break;
                #endregion
            case "ProjectileObject":
                #region
                //ProjectileObjectDrawer
                objectSelectionField.objectType = typeof(ProjectileObject);
                var projectileObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/ProjectileObjectDrawer.uxml");
                ve = projectileObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField ProjectileObjectSpriteSelection = ve.Q<ObjectField>("ProjectileSpriteObjectSelectionField");
                ProjectileObjectSpriteSelection.objectType = typeof(SpriteObject);

                FloatField ProjectileSpeedField = ve.Q<FloatField>("ProjectileSpeedField");

                ObjectField ProjectileObjectColliderSelection = ve.Q<ObjectField>("ProjectileColliderObjectSelectionField");
                ProjectileObjectColliderSelection.objectType = typeof(CircleColliderObject);

                objectEditSelectionContainer.Add(ve);
                break;
            #endregion
            case "PlayerObject":
                #region
                objectSelectionField.objectType = typeof(PlayerObject);
                var playerObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/PlayerDrawer.uxml");
                ve = playerObjectVisualTree.CloneTree();

                ve.style.height = 500;
                ve.style.alignContent = Align.Center;
                ve.style.paddingTop = 50;

                ObjectField PlayerObjectSpriteSelection = ve.Q<ObjectField>("PlayerSpriteObjectSelectionField");
                PlayerObjectSpriteSelection.objectType = typeof(SpriteObject);

                FloatField PlayerHealthField = ve.Q<FloatField>("PlayerObjectHealthField");
                FloatField PlayerAttackField = ve.Q<FloatField>("PlayerObjectAttackField");
                FloatField PlayerSpeedField = ve.Q<FloatField>("PlayerObjectSpeedField");

                ObjectField PlayerObjectProjectileSelection = ve.Q<ObjectField>("PlayerProjectileObjectSelectionField");
                PlayerObjectProjectileSelection.objectType = typeof(ProjectileObject);
                ObjectField PlayerObjectColliderSelection = ve.Q<ObjectField>("PlayerColliderObjectSelectionField");
                PlayerObjectColliderSelection.objectType = typeof(CircleColliderObject);

                objectEditSelectionContainer.Add(ve);
                break;
                #endregion
        }
    }

    private void UpdateOnObjectSelection(ChangeEvent<UnityEngine.Object> evt)
    {
        ObjectNameTextField.value = null;
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
                    #region
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
                #endregion
                case "EnvironmentObject":
                    #region
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
                #endregion
                case "StaticObject":
                    #region
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
                #endregion
                case "AnimationObject":
                    #region
                    var animaitonVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/AnimationObjectDrawer.uxml");
                    ve = animaitonVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ListView AnimationSpriteSelection = ve.Q<ListView>("AnimationSpriteObjectSelectionField");
                    //AnimationSpriteSelection.objectType = typeof(Sprite);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                #endregion
                case "EnemyObject":
                    #region
                    var enemyObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnemyDrawer.uxml");
                    ve = enemyObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField EnemyObjectSpriteSelection = ve.Q<ObjectField>("EnemySpriteObjectSelectionField");
                    EnemyObjectSpriteSelection.objectType = typeof(SpriteObject);

                    FloatField EnemyHealthField = ve.Q<FloatField>("EnemyObjectHealthField");
                    FloatField EnemyAttackField = ve.Q<FloatField>("EnemyObjectAttackField");

                    ObjectField EnemyObjectColliderSelection = ve.Q<ObjectField>("EnemyColliderObjectSelectionField");
                    EnemyObjectColliderSelection.objectType = typeof(CircleColliderObject);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                #endregion
                case "EnemySpawnerObject":
                    #region
                    var enemySpawnerObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/EnemySpawnerDrawer.uxml");
                    ve = enemySpawnerObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField EnemySpawnerObjectSpriteSelection = ve.Q<ObjectField>("EnemySpawnerSpriteObjectSelectionField");
                    EnemySpawnerObjectSpriteSelection.objectType = typeof(SpriteObject);

                    FloatField EnemySpawnerTimeField = ve.Q<FloatField>("EnemySpawnTimeField");

                    ObjectField EnemySpawnerenemySelection = ve.Q<ObjectField>("EnemyObjectSelectionField");
                    EnemySpawnerenemySelection.objectType = typeof(EnemyObject);
                    ObjectField EnemySpawnerObjectColliderSelection = ve.Q<ObjectField>("SpawnerColliderObjectSelectionField");
                    EnemySpawnerObjectColliderSelection.objectType = typeof(CircleColliderObject);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                #endregion
                //case "Player":
                //    break;
                case "PolygonColliderObject":
                    #region
                    var polyColliderObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/PolygonColliderDrawer.uxml");
                    ve = polyColliderObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    FloatField PolyColliderHeightField = ve.Q<FloatField>("PolygonColliderHeightField");
                    FloatField PolyColliderWidthField = ve.Q<FloatField>("PolygonColliderWidthField");
                    Toggle PolyColliderIsTriggerToggle = ve.Q<Toggle>("PolygonColliderSetTrigger");

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                #endregion
                case "CircleColliderObject":
                    #region
                    var cirColliderObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/CircleColliderDrawer.uxml");
                    ve = cirColliderObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    FloatField CircleColliderHeightField = ve.Q<FloatField>("CircleColliderRadiusField");
                    Toggle CircleColliderIsTriggerToggle = ve.Q<Toggle>("CircleColliderSetTrigger");

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                #endregion
                case "ProjectileObject":
                    #region
                    var projectileObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/ProjectileObjectDrawer.uxml");
                    ve = projectileObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField ProjectileObjectSpriteSelection = ve.Q<ObjectField>("ProjectileSpriteObjectSelectionField");
                    ProjectileObjectSpriteSelection.objectType = typeof(SpriteObject);

                    FloatField ProjectileSpeedField = ve.Q<FloatField>("ProjectileSpeedField");

                    ObjectField ProjectileObjectColliderSelection = ve.Q<ObjectField>("ProjectileColliderObjectSelectionField");
                    ProjectileObjectColliderSelection.objectType = typeof(CircleColliderObject);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                #endregion
                case "PlayerObject":
                    #region
                    var playerObjectVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/ObjectTypeEditors/PlayerDrawer.uxml");
                    ve = playerObjectVisualTree.CloneTree();

                    ve.style.height = 500;
                    ve.style.alignContent = Align.Center;
                    ve.style.paddingTop = 50;

                    ObjectField PlayerObjectSpriteSelection = ve.Q<ObjectField>("PlayerSpriteObjectSelectionField");
                    PlayerObjectSpriteSelection.objectType = typeof(SpriteObject);

                    FloatField PlayerHealthField = ve.Q<FloatField>("PlayerObjectHealthField");
                    FloatField PlayerAttackField = ve.Q<FloatField>("PlayerObjectAttackField");
                    FloatField PlayerSpeedField = ve.Q<FloatField>("PlayerObjectSpeedField");

                    ObjectField PlayerObjectProjectileSelection = ve.Q<ObjectField>("PlayerProjectileObjectSelectionField");
                    PlayerObjectProjectileSelection.objectType = typeof(ProjectileObject);
                    ObjectField PlayerObjectColliderSelection = ve.Q<ObjectField>("PlayerColliderObjectSelectionField");
                    PlayerObjectColliderSelection.objectType = typeof(CircleColliderObject);

                    objectEditSelectionContainer.Add(ve);
                    ve.Bind(serializedObject);
                    break;
                    #endregion
            }
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
                    #region
                    ObjectField SpriteSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    if (SpriteSpriteSelection.value != null)
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
                #endregion
                case "EnvironmentObject":
                    #region
                    ObjectField EnvironmentSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    ObjectField ColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                    if (EnvironmentSpriteSelection.value != null)
                    {
                        ScriptableObject eo = ScriptableObject.CreateInstance<EnvironmentObject>();

                        EnvironmentObject environmentObject = (EnvironmentObject)eo;
                        environmentObject.sprite = (SpriteObject)EnvironmentSpriteSelection.value;
                        EditorUtility.SetDirty(environmentObject);
                        if (ColliderSelection.value != null)
                        {
                            environmentObject.collider = (ColliderObject)ColliderSelection.value;
                            EditorUtility.SetDirty(environmentObject);
                        }

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(environmentObject, "Assets/Export/Data/EnvironementData.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(environmentObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();

                    //ve.Bind(serializedObject);
                    break;
                #endregion
                case "StaticObject":
                    #region
                    ObjectField StaticSpriteSelection = ve.Q<ObjectField>("SpriteImageObjectSelectionField");
                    ObjectField StaticColliderSelection = ve.Q<ObjectField>("SpriteColliderObjectSelectionField");
                    if (StaticSpriteSelection.value != null && StaticColliderSelection.value != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<StaticObject>();

                        StaticObject staticObject = (StaticObject)so;
                        staticObject.sprite = (SpriteObject)StaticSpriteSelection.value;
                        EditorUtility.SetDirty(staticObject);
                        staticObject.collider = (ColliderObject)StaticColliderSelection.value;
                        EditorUtility.SetDirty(staticObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(staticObject, "Assets/Export/Data/StaticObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(staticObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                #endregion
                case "AnimationObject":
                    #region
                    ScriptableObject animso = ScriptableObject.CreateInstance<AnimationObject>();
                    AnimationObject animObject = (AnimationObject)animso;
                    if (string.IsNullOrEmpty(ObjectNameTextField.value))
                    {
                        AssetDatabase.CreateAsset(animObject, "Assets/Export/Data/AnimationObject.asset");
                    }
                    else
                    {
                        AssetDatabase.CreateAsset(animObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                #endregion
                case "EnemyObject":
                    #region
                    ObjectField EnemyObjectSpriteSelection = ve.Q<ObjectField>("EnemySpriteObjectSelectionField");
                    FloatField EnemyHealthField = ve.Q<FloatField>("EnemyObjectHealthField");
                    FloatField EnemyAttackField = ve.Q<FloatField>("EnemyObjectAttackField");
                    ObjectField EnemyObjectColliderSelection = ve.Q<ObjectField>("EnemyColliderObjectSelectionField");
                    if (EnemyObjectSpriteSelection.value != null && EnemyHealthField.value != 0 && EnemyAttackField.value != 0 && EnemyObjectColliderSelection.value!= null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<EnemyObject>();

                        EnemyObject enemyObject = (EnemyObject)so;
                        enemyObject.sprite = (SpriteObject)EnemyObjectSpriteSelection.value;
                        EditorUtility.SetDirty(enemyObject);
                        enemyObject.health = EnemyHealthField.value;
                        EditorUtility.SetDirty(enemyObject);
                        enemyObject.attack = EnemyAttackField.value;
                        EditorUtility.SetDirty(enemyObject);
                        enemyObject.enemyCollider = (CircleColliderObject)EnemyObjectColliderSelection.value;
                        EditorUtility.SetDirty(enemyObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(enemyObject, "Assets/Export/Data/EnemyObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(enemyObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                #endregion
                case "EnemySpawner":
                    #region
                    ObjectField EnemySpawnerObjectSpriteSelection = ve.Q<ObjectField>("EnemySpawnerSpriteObjectSelectionField");
                    FloatField EnemySpawnerTimeField = ve.Q<FloatField>("EnemySpawnTimeField");
                    ObjectField EnemySpawnerenemySelection = ve.Q<ObjectField>("EnemyObjectSelectionField");
                    ObjectField EnemySpawnerObjectColliderSelection = ve.Q<ObjectField>("SpawnerColliderObjectSelectionField");
                    if (EnemySpawnerObjectSpriteSelection.value != null && EnemySpawnerTimeField != null && EnemySpawnerenemySelection.value != null && EnemySpawnerObjectColliderSelection.value != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<EnemySpawnerObject>();

                        EnemySpawnerObject enemySpawnerObject = (EnemySpawnerObject)so;
                        enemySpawnerObject.sprite = (SpriteObject)EnemySpawnerObjectSpriteSelection.value;
                        EditorUtility.SetDirty(enemySpawnerObject);
                        enemySpawnerObject.spawnTime = EnemySpawnerTimeField.value;
                        EditorUtility.SetDirty(enemySpawnerObject);
                        enemySpawnerObject.enemy = (EnemyObject)EnemySpawnerenemySelection.value;
                        EditorUtility.SetDirty(enemySpawnerObject);
                        enemySpawnerObject.spawnerCollider = (CircleColliderObject)EnemySpawnerObjectColliderSelection.value;
                        EditorUtility.SetDirty(enemySpawnerObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(enemySpawnerObject, "Assets/Export/Data/EnemySpawnerObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(enemySpawnerObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                #endregion
                //case "Player":
                //    break;
                case "PolygonCollider":
                    #region
                    FloatField PolyColliderHeightField = ve.Q<FloatField>("PolygonColliderHeightField");
                    FloatField PolyColliderWidthField = ve.Q<FloatField>("PolygonColliderWidthField");
                    Toggle PolyColliderIsTriggerToggle = ve.Q<Toggle>("PolygonColliderSetTrigger");
                    if (PolyColliderHeightField != null && PolyColliderWidthField != null && PolyColliderIsTriggerToggle != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<PolygonColliderObject>();

                        PolygonColliderObject polygonColliderObject = (PolygonColliderObject)so;
                        polygonColliderObject.height = PolyColliderHeightField.value;
                        EditorUtility.SetDirty(polygonColliderObject);
                        polygonColliderObject.width = PolyColliderWidthField.value;
                        EditorUtility.SetDirty(polygonColliderObject);
                        polygonColliderObject.isTrigger = PolyColliderIsTriggerToggle.value;
                        EditorUtility.SetDirty(polygonColliderObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(polygonColliderObject, "Assets/Export/Data/PolygonColliderObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(polygonColliderObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                #endregion
                case "CircleCollider":
                    #region
                    FloatField CircleColliderRadiustField = ve.Q<FloatField>("CircleColliderRadiusField");
                    Toggle CircleColliderIsTriggerToggle = ve.Q<Toggle>("CircleColliderSetTrigger");

                    if (CircleColliderRadiustField != null && CircleColliderIsTriggerToggle != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<CircleColliderObject>();

                        CircleColliderObject circleColliderObject = (CircleColliderObject)so;

                        circleColliderObject.radius = CircleColliderRadiustField.value;
                        EditorUtility.SetDirty(circleColliderObject);
                        circleColliderObject.isTrigger = CircleColliderIsTriggerToggle.value;
                        EditorUtility.SetDirty(circleColliderObject);


                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(circleColliderObject, "Assets/Export/Data/CircleColliderObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(circleColliderObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    break;
                #endregion
                case "ProjectileObject":
                    #region
                    ObjectField ProjectileObjectSpriteSelection = ve.Q<ObjectField>("ProjectileSpriteObjectSelectionField");
                    FloatField ProjectileSpeedField = ve.Q<FloatField>("ProjectileSpeedField");
                    ObjectField ProjectileObjectColliderSelection = ve.Q<ObjectField>("ProjectileColliderObjectSelectionField");

                    if (ProjectileObjectSpriteSelection.value != null && ProjectileSpeedField != null && ProjectileObjectColliderSelection.value != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<ProjectileObject>();

                        ProjectileObject projectileObject = (ProjectileObject)so;
                        projectileObject.sprite = (SpriteObject)ProjectileObjectSpriteSelection.value;
                        EditorUtility.SetDirty(projectileObject);
                        projectileObject.speed = ProjectileSpeedField.value;
                        EditorUtility.SetDirty(projectileObject);
                        projectileObject.projectileCollider = (CircleColliderObject)ProjectileObjectColliderSelection.value;
                        EditorUtility.SetDirty(projectileObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(projectileObject, "Assets/Export/Data/ProjectileObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(projectileObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                #endregion
                case "PlayerObject":
                    #region
                    ObjectField PlayerObjectSpriteSelection = ve.Q<ObjectField>("PlayerSpriteObjectSelectionField");
                    FloatField PlayerHealthField = ve.Q<FloatField>("PlayerObjectHealthField");
                    FloatField PlayerAttackField = ve.Q<FloatField>("PlayerObjectAttackField");
                    FloatField PlayerSpeedField = ve.Q<FloatField>("PlayerObjectSpeedField");
                    ObjectField PlayerObjectProjectileSelection = ve.Q<ObjectField>("PlayerProjectileObjectSelectionField");
                    ObjectField PlayerObjectColliderSelection = ve.Q<ObjectField>("PlayerColliderObjectSelectionField");

                    if (PlayerObjectSpriteSelection.value != null && PlayerHealthField.value != 0 && PlayerAttackField.value != 0 && PlayerSpeedField.value != 0 && PlayerObjectProjectileSelection.value != null && PlayerObjectColliderSelection.value != null)
                    {
                        ScriptableObject so = ScriptableObject.CreateInstance<PlayerObject>();

                        PlayerObject playerObject = (PlayerObject)so;
                        playerObject.sprite = (SpriteObject)PlayerObjectSpriteSelection.value;
                        EditorUtility.SetDirty(playerObject);
                        playerObject.health = PlayerHealthField.value;
                        EditorUtility.SetDirty(playerObject);
                        playerObject.attack = PlayerAttackField.value;
                        EditorUtility.SetDirty(playerObject);
                        playerObject.projectile = (ProjectileObject)PlayerObjectProjectileSelection.value;
                        EditorUtility.SetDirty(playerObject);
                        playerObject.playerCollider = (CircleColliderObject)PlayerObjectColliderSelection.value;
                        EditorUtility.SetDirty(playerObject);

                        if (string.IsNullOrEmpty(ObjectNameTextField.value))
                        {
                            AssetDatabase.CreateAsset(playerObject, "Assets/Export/Data/PlayerObject.asset");
                        }
                        else
                        {
                            AssetDatabase.CreateAsset(playerObject, "Assets/Export/Data/" + ObjectNameTextField.value + ".asset");
                        }
                    }
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    break;
                    #endregion
            }
        }
    }

    private void SaveEditedObject(MouseUpEvent evt)
    {
    }
}