using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;
using System;

public class LevelEditor : VisualElement
{
    static LevelEditor instance;

    IMGUIContainer levelListContainer;
    ReorderableList levelList;
    List<Level> allLevels;

    public static LevelEditor Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new LevelEditor();
            }
            return instance;
        }
    }

    public static void Destroy()
    {
        instance = null;
    }

    static VisualElement levelEditor;
    static Box levelPropertiesContainer;
    static Box addLevelContainer;

    List<string> layers = new List<string> { "Environment", "StaticObjects", "Enemies", "Players"/*, "Effects", "UI", "UIEffects" */};
    ObjectField textureObjectField;
    PopupField<string> layersPopupField;
    Drawable selectedDrawable;
    Level level;

    public static VisualElement GetLevelEditor()
    {
        Instance.CreateVisualTree();
        return levelEditor;
    }

    void CreateVisualTree()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/EditorComponents/LevelEditor.uxml");
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/LevelEditorWindow.uss");
        levelEditor = visualTree.CloneTree();

        addLevelContainer = levelEditor.Q<Box>("addLevelContainer");
        levelListContainer = new IMGUIContainer(DrawAnimationList);
        levelListContainer.onGUIHandler = DrawAnimationList;
        levelList = new ReorderableList(allLevels, typeof(Level));
        levelList.drawElementCallback = DrawList;
        levelList.drawHeaderCallback = WriteHeader;
        levelList.onReorderCallback = ChangeListOrder;
        levelList.onSelectCallback = SelectListItem;
        levelList.onAddCallback = AddToList;
        levelList.onRemoveCallback = RemoveFromList;
        addLevelContainer.Add(levelListContainer);

        MapLayer map = new MapLayer();
        map.name = "Environment";
        map.drawRects = new Dictionary<Vector2Int, Drawable>();
        MapLayer objects = new MapLayer();
        objects.name = "StaticObjects";
        objects.drawRects = new Dictionary<Vector2Int, Drawable>();
        MapLayer enemies = new MapLayer();
        enemies.name = "Enemies";
        enemies.drawRects = new Dictionary<Vector2Int, Drawable>();
        MapLayer player = new MapLayer();
        player.name = "Players";
        player.drawRects = new Dictionary<Vector2Int, Drawable>();

        layersToDraw.Add(map);
        layersToDraw.Add(objects);
        layersToDraw.Add(enemies);
        layersToDraw.Add(player);

        VisualElement stampToolLayout = levelEditor.Q<Box>("levelManagerContainer");

        levelEditor.Add(stampToolLayout);

        VisualElement levelPropertyContainer = levelEditor.Q<Box>("PropertiesBarRow1");
        // Create a new field and assign it its value.
        layersPopupField = new PopupField<string>("Select Layer to Paint", layers, 0);
        layersPopupField.value = "Environment";
        layersPopupField.AddToClassList("height-width-slider");
        levelPropertyContainer.Add(layersPopupField);

        //Label selectTileMapLabel = new Label("Choose TileMap(64x64)");
        //levelPropertyContainer.Add(selectTileMapLabel);

        //textureObjectField = new ObjectField { objectType = typeof(UnityEngine.Texture) };
        ////textureObjectField.StretchToParentSize();
        //textureObjectField.AddToClassList("height-width-slider");
        //levelPropertyContainer.Add(textureObjectField);
        //// Mirror value of uxml field into the C# field.
        //layersPopupField.RegisterCallback<ChangeEvent<string>>((evt) =>
        //{
        //    styledField.value = evt.newValue;
        //});

        VisualElement levelMapContainer = levelEditor.Q<Box>("LevelMapContainer");

        mapElement = new IMGUIContainer(mapOnGUI);

        mapElement.AddToClassList("level-map-sub-container");
        mapElement.RegisterCallback<MouseMoveEvent>(OnMapMouseMove);
        mapElement.RegisterCallback<MouseDownEvent>(OnMapMouseDown);
        mapElement.RegisterCallback<MouseUpEvent>(OnMapMouseUp);
        mapElement.RegisterCallback<MouseOutEvent>(OnMapMouseExit);

        levelMapContainer.Add(mapElement);

        VisualElement levelTileContainer = levelEditor.Q<Box>("TileMapContainer");

        tileElement = new IMGUIContainer(tileOnGUI);

        tileElement.AddToClassList("tile-map-container");
        tileElement.RegisterCallback<MouseUpEvent>(OnTileMouseup);

        levelTileContainer.Add(tileElement);

        InitializeLists();
    }

    private void InitializeLists()
    {
        #region SearchForLevelObjectOrCreate //TODO: fetch from list of objects in the project object
        string[] guids = AssetDatabase.FindAssets("t:Level");
        foreach (string guid in guids)
        {
            Debug.Log("ScriptObj: " + AssetDatabase.GUIDToAssetPath(guid));
        }
        Debug.Log(guids.Length);

        if (guids != null && guids.Length > 0)
        {
            //level = ScriptableObject.CreateInstance<Level>();
            level = (Level)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(Level));
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
            level = ScriptableObject.CreateInstance<Level>();
            AssetDatabase.CreateAsset(level, "Assets/Export/Data/Level-00.asset");
        }
        #endregion
        #region EnvironmentObjects
        string[] Environment_guids = AssetDatabase.FindAssets("t:EnvironmentObject");
        if (Environment_guids != null || Environment_guids.Length > 0)
        {
            foreach (string s in Environment_guids)
            {
                EnvironmentObject eo = (EnvironmentObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(EnvironmentObject));
                bool exists = false;
                foreach (EnvironmentObject envObj in environmentTiles)
                {
                    if (envObj == eo)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    EnvironmentObject envObj = eo;
                    environmentTiles.Add(envObj);
                }
            }
        }
        #endregion
        #region StaticObjects
        string[] Static_guids = AssetDatabase.FindAssets("t:StaticObject");
        if (Static_guids != null || Static_guids.Length > 0)
        {
            foreach (string s in Static_guids)
            {
                StaticObject so = (StaticObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(StaticObject));
                bool exists = false;
                foreach (StaticObject statObj in staticObjectTiles)
                {
                    if (statObj == so)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    StaticObject statObj = so;
                    staticObjectTiles.Add(statObj);
                }
            }
        }
        #endregion
        #region EnemySpawnersObjects
        string[] Spawner_guids = AssetDatabase.FindAssets("t:EnemySpawnerObject");
        if (Spawner_guids != null || Spawner_guids.Length > 0)
        {
            foreach (string s in Spawner_guids)
            {
                EnemySpawnerObject eso = (EnemySpawnerObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(EnemySpawnerObject));
                bool exists = false;
                foreach (EnemySpawnerObject spawnObj in enemySpawnerTiles)
                {
                    if (spawnObj == eso)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    EnemySpawnerObject spawnObj = eso;
                    enemySpawnerTiles.Add(spawnObj);
                }
            }
        }
        #endregion
        #region PlayerObjects
        string[] Player_guids = AssetDatabase.FindAssets("t:PlayerObject");
        if (Player_guids != null || Player_guids.Length > 0)
        {
            foreach (string s in Player_guids)
            {
                PlayerObject po = (PlayerObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(s), typeof(PlayerObject));
                bool exists = false;
                foreach (PlayerObject plObj in playerTiles)
                {
                    if (plObj == po)
                    {
                        exists = true;
                    }
                }
                if (!exists)
                {
                    PlayerObject plObj = po;
                    playerTiles.Add(plObj);
                }
            }
        }
        #endregion
    }

    #region ReorderableListAndCallbacks

    void DrawAnimationList()
    {
        //if (levelList != null)
        //{
        //    levelList.DoLayoutList();
        //}


    }

    void DrawList(Rect rect, int index, bool isActive, bool isFocused)
    {
        var element = levelList.serializedProperty.GetArrayElementAtIndex(index);
        var name = element.FindPropertyRelative("name");
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), name.stringValue);
    }
    void WriteHeader(Rect rect)
    {
        EditorGUI.LabelField(new Rect(rect.x, rect.y, 100, EditorGUIUtility.singleLineHeight), "Levels");
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

    struct Drawable
    {
        public string assetPath;
        public Rect texCoords;
    }

    struct MapLayer
    {
        public string name;
        public Dictionary<Vector2Int, Drawable> drawRects;
    }

    List<MapLayer> layersToDraw = new List<MapLayer> { };


    List<EnvironmentObject> environmentTiles = new List<EnvironmentObject>();
    List<StaticObject> staticObjectTiles = new List<StaticObject>();
    List<EnemySpawnerObject> enemySpawnerTiles = new List<EnemySpawnerObject>();
    List<PlayerObject> playerTiles = new List<PlayerObject>();      //Multiple players kept intentionally so that the designer can check with different variations in player types/values

    IMGUIContainer mapElement;
    IMGUIContainer tileElement;
    public Vector2 mapScrollPosition = Vector2.zero;
    public Vector2 tileScrollPosition = Vector2.zero;
    int rows = 32;
    int cols = 32;
    int cell = 32;
    int tileCell = 64;

    ScriptableObject selectedScriptableObject = null;


    private void mapOnGUI()
    {
        mapScrollPosition = EditorGUILayout.BeginScrollView(mapScrollPosition, true, true, GUILayout.Width(mapElement.contentRect.width), GUILayout.Width(mapElement.contentRect.height));
        //if (textureObjectField.value != null)
        //{
        //    Texture imageToDraw = (Texture)textureObjectField.value;
        //    Rect drawRect = new Rect(0, 0, imageToDraw.width, imageToDraw.height);
        //    Rect destRect = 
        //    GUI.DrawTextureWithTexCoords(drawRect, imageToDraw, drawRect,true);
        //}

        //Rect sizeRect = new Rect(0, 0, rows * cell, cols * cell);
        EditorGUILayout.LabelField("", GUILayout.Width(cols * cell), GUILayout.Height(rows * cell));

        for (int i = 0; i <= rows; i++)
        {
            EditorGUI.DrawRect(new Rect(0, i * cell, cols * cell, 2), Color.white);
        }
        for (int j = 0; j <= cols; j++)
        {
            EditorGUI.DrawRect(new Rect(j * cell, 0, 2, rows * cell), Color.white);
        }
        for (int layerindex = 0; layerindex < 4; layerindex++)
        {
            for (int i =0; i< level.layers[layerindex].position.Count ;i++)
            {
                Rect drawRect = new Rect(cols * tileCell, rows * tileCell, tileCell, tileCell);
                Texture t;
                Rect spriteRect;
                if (layerindex == 0)
                {
                    Sprite s = ((EnvironmentObject)(level.layers[layerindex].objectOnPosition[i])).sprite.sprite;
                    t = s.texture;
                    spriteRect = s.rect;
                }
                else if (layerindex == 1)
                {
                    Sprite s = ((StaticObject)(level.layers[layerindex].objectOnPosition[i])).sprite.sprite;
                    t = s.texture;
                    spriteRect = s.rect;
                }
                else if (layerindex == 2)
                {
                    Sprite s = ((EnemyObject)(level.layers[layerindex].objectOnPosition[i])).sprite.sprite;
                    t = s.texture;
                    spriteRect = s.rect;
                }
                else if (layerindex == 3)
                {
                    Sprite s = ((PlayerObject)(level.layers[layerindex].objectOnPosition[i])).sprite.sprite;
                    t = s.texture;
                    spriteRect = s.rect;
                }
                else
                {
                    return;
                }
                spriteRect.x /= t.width;
                spriteRect.y /= t.height;
                spriteRect.y = 1 - spriteRect.y;
                spriteRect.width /= t.width;
                spriteRect.height /= t.height;
                GUI.DrawTextureWithTexCoords(drawRect, t, spriteRect);
                //GUI.DrawTextureWithTexCoords(drawRect, imageToDraw, drawRect, true);
            }
        }


            MarkDirtyRepaint();
        LevelEditorWindow.RepaintWindow();
        GUI.EndScrollView();
    }

    private void tileOnGUI()
    {
        tileScrollPosition = EditorGUILayout.BeginScrollView(tileScrollPosition, true, true, GUILayout.Width(tileElement.contentRect.width), GUILayout.Width(tileElement.contentRect.height));

        int rows = 0;
        int cols = 0;
        switch (layersPopupField.value)
        {
            case "Environment":

                for (int i = 0; i < environmentTiles.Count; i++)
                {
                    if (i != 0 && i % 6 == 0)
                    {
                        rows++;
                        cols = 0;
                    }
                    Rect drawRect = new Rect(cols * tileCell, rows * tileCell, tileCell, tileCell);
                    Texture t = environmentTiles[i].sprite.sprite.texture;
                    Rect spriteRect = environmentTiles[i].sprite.sprite.rect;

                    spriteRect.x /= t.width;
                    spriteRect.y /= t.height;
                    //spriteRect.y = 1 - spriteRect.y;
                    spriteRect.width /= t.width;
                    spriteRect.height /= t.height;
                    GUI.DrawTextureWithTexCoords(drawRect, t, spriteRect);
                    cols++;
                }
                break;

            case "StaticObjects":

                for (int i = 0; i < staticObjectTiles.Count; i++)
                {
                    if (i != 0 && i % 6 == 0)
                    {
                        rows++;
                        cols = 0;
                    }
                    Rect drawRect = new Rect(cols * tileCell, rows * tileCell, tileCell, tileCell);
                    Texture t = staticObjectTiles[i].sprite.sprite.texture;
                    Rect spriteRect = staticObjectTiles[i].sprite.sprite.rect;

                    spriteRect.x /= t.width;
                    spriteRect.y /= t.height;
                    //spriteRect.y = 1 - spriteRect.y;
                    spriteRect.width /= t.width;
                    spriteRect.height /= t.height;
                    GUI.DrawTextureWithTexCoords(drawRect, t, spriteRect);
                    cols++;
                }
                break;

            case "Enemies":

                for (int i = 0; i < enemySpawnerTiles.Count; i++)
                {
                    if (i != 0 && i % 6 == 0)
                    {
                        rows++;
                        cols = 0;
                    }
                    Rect drawRect = new Rect(cols * tileCell, rows * tileCell, tileCell, tileCell);
                    Texture t = enemySpawnerTiles[i].sprite.sprite.texture;
                    Rect spriteRect = enemySpawnerTiles[i].sprite.sprite.rect;

                    spriteRect.x /= t.width;
                    spriteRect.y /= t.height;
                    //spriteRect.y = 1 - spriteRect.y;
                    spriteRect.width /= t.width;
                    spriteRect.height /= t.height;
                    GUI.DrawTextureWithTexCoords(drawRect, t, spriteRect);
                    cols++;
                }
                break;

            case "Players":

                for (int i = 0; i < playerTiles.Count; i++)
                {
                    if (i != 0 && i % 6 == 0)
                    {
                        rows++;
                        cols = 0;
                    }
                    Rect drawRect = new Rect(cols * tileCell, rows * tileCell, tileCell, tileCell);
                    Texture t = playerTiles[i].sprite.sprite.texture;
                    Rect spriteRect = playerTiles[i].sprite.sprite.rect;

                    spriteRect.x /= t.width;
                    spriteRect.y /= t.height;
                    //spriteRect.y = 1 - spriteRect.y;
                    spriteRect.width /= t.width;
                    spriteRect.height /= t.height;
                    GUI.DrawTextureWithTexCoords(drawRect, t, spriteRect);
                    cols++;
                }
                break;
        }
        EditorGUILayout.LabelField("", GUILayout.Width(6 * tileCell), GUILayout.Height(rows * tileCell));

        MarkDirtyRepaint();
        LevelEditorWindow.RepaintWindow();
        GUI.EndScrollView();
    }

    #region MapMouseEvents
    bool paint = false;
    void OnMapMouseDown(MouseDownEvent evt)
    {
        paint = true;
    }
    void OnMapMouseUp(MouseUpEvent evt)
    {
        paint = false;
    }
    void OnMapMouseExit(MouseOutEvent evt)
    {
        paint = false;
    }

    void OnMapMouseMove(MouseMoveEvent evt)
    {
        if (!paint)
        {
            return;
        }
        if (selectedScriptableObject == null)
        {
            return;
        }
        if ((evt.localMousePosition.x < mapElement.contentRect.width - 15) && (evt.localMousePosition.y < mapElement.contentRect.height - 15))
        {
            int mapCellStartX = Mathf.FloorToInt(evt.localMousePosition.x + mapScrollPosition.x) / cell;
            int mapCellStartY = Mathf.FloorToInt(evt.localMousePosition.y + mapScrollPosition.y) / cell;
            //Debug.Log(evt.localMousePosition + mapScrollPosition);
            //Debug.Log((evt.localMousePosition + mapScrollPosition) / cell);

            Vector2Int drawPos = new Vector2Int(mapCellStartX * cell, mapCellStartY * cell);

            int selectedLayer = 0;
            switch (layersPopupField.value)
            {
                case "Environment":
                    selectedLayer = 0;
                    break;
                case "StaticObjects":
                    selectedLayer = 1;
                    break;
                case "Enemies":
                    selectedLayer = 2;
                    break;
                case "Players":
                    selectedLayer = 3;
                    break;
            }
            //for (int i=0;i<level.layers.Length;i++)
            //{
            Event e = Event.current;
            if (level.layers[selectedLayer].position == null || level.layers[selectedLayer].position.Count <=0)
            {
                level.layers[selectedLayer].position.Add(drawPos);
                EditorUtility.SetDirty(level);
                level.layers[selectedLayer].objectOnPosition.Add(selectedScriptableObject);
                EditorUtility.SetDirty(level);
            }
            foreach (Vector2Int vi in level.layers[selectedLayer].position)
            {
                if (vi == drawPos)
                {
                    int index = level.layers[selectedLayer].position.IndexOf(vi);
                    if (e.modifiers == EventModifiers.Shift)
                    {
                        level.layers[selectedLayer].position.RemoveAt(index);
                        EditorUtility.SetDirty(level);
                        level.layers[selectedLayer].objectOnPosition.RemoveAt(index);
                        EditorUtility.SetDirty(level);
                        break;
                    }
                    else
                    {
                        level.layers[selectedLayer].position[index] = drawPos;
                        EditorUtility.SetDirty(level);
                        level.layers[selectedLayer].objectOnPosition[index] = (selectedScriptableObject);
                        EditorUtility.SetDirty(level);
                        break;
                    }
                }
                else
                {
                    level.layers[selectedLayer].position.Add(drawPos);
                    EditorUtility.SetDirty(level);
                    level.layers[selectedLayer].objectOnPosition.Add(selectedScriptableObject);
                    EditorUtility.SetDirty(level);
                    break;
                }
            }
            //if (level.layers[i] == layersPopupField.value)
            //{
            //    Drawable temp;
            //    if (ml.drawRects.TryGetValue(drawPos, out temp))
            //    {
            //        Event e = Event.current;
            //        if (e.modifiers == EventModifiers.Shift)
            //        {
            //            ml.drawRects.Remove(drawPos);
            //        }
            //        else
            //        {
            //            if (!string.IsNullOrEmpty(selectedDrawable.assetPath))
            //            {
            //                temp.assetPath = selectedDrawable.assetPath;
            //                temp.texCoords = selectedDrawable.texCoords;
            //                if (ml.drawRects[drawPos].Equals(temp))
            //                {
            //                    ml.drawRects.Remove(drawPos);
            //                }
            //                else
            //                {
            //                    ml.drawRects[drawPos] = temp;
            //                }
            //            }
            //        }
            //    }
            //    else
            //    {
            //        Event e = Event.current;
            //        if (e.modifiers == EventModifiers.Shift)
            //        {
            //        }
            //        else
            //        {
            //            if (!string.IsNullOrEmpty(selectedDrawable.assetPath))
            //            {
            //                ml.drawRects.Add(drawPos, selectedDrawable);
            //            }
            //        }
            //    }
            //}
            //}

        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        MarkDirtyRepaint();
        LevelEditorWindow.RepaintWindow();
    }

    #endregion

    void OnTileMouseup(MouseUpEvent evt)
    {
        if ((evt.localMousePosition.x < tileElement.contentRect.width - 15) && (evt.localMousePosition.y < tileElement.contentRect.height - 15))
        {
            int itemNumber = -1;
            int TileStartX = (Mathf.FloorToInt((evt.localMousePosition.x + tileScrollPosition.x) / tileCell) * tileCell);
            int TileStartY = (Mathf.FloorToInt((evt.localMousePosition.y + tileScrollPosition.y) / tileCell) * tileCell);

            if (TileStartX < 6 * tileCell)  //Only span till the 6 columns
            {
                int numberOfRows = 0;
                switch (layersPopupField.value)
                {
                    case "Environment":
                        numberOfRows = environmentTiles.Count / 6;
                        if (environmentTiles.Count % 6 > 0)
                        {
                            numberOfRows++;
                        }
                        if (TileStartY < numberOfRows * tileCell)
                        {
                            itemNumber = TileStartX / 64 + (6 * TileStartY / 64);
                        }
                        if (itemNumber < environmentTiles.Count && itemNumber >= 0)
                        {
                            selectedScriptableObject = environmentTiles[itemNumber];
                            Debug.Log(selectedScriptableObject.name);
                        }
                        else
                        {
                            selectedScriptableObject = null;
                        }
                        break;

                    case "StaticObjects":
                        numberOfRows = staticObjectTiles.Count / 6;
                        if (staticObjectTiles.Count % 6 > 0)
                        {
                            numberOfRows++;
                        }
                        if (TileStartY < numberOfRows * tileCell)
                        {
                            itemNumber = TileStartX / 64 + (6 * TileStartY / 64);
                        }
                        if (itemNumber < staticObjectTiles.Count && itemNumber >= 0)
                        {
                            selectedScriptableObject = staticObjectTiles[itemNumber];
                            Debug.Log(selectedScriptableObject.name);
                        }
                        else
                        {
                            selectedScriptableObject = null;
                        }
                        break;

                    case "Enemies":
                        numberOfRows = enemySpawnerTiles.Count / 6;
                        if (enemySpawnerTiles.Count % 6 > 0)
                        {
                            numberOfRows++;
                        }
                        if (TileStartY < numberOfRows * tileCell)
                        {
                            itemNumber = TileStartX / 64 + (6 * TileStartY / 64);
                        }
                        if (itemNumber < enemySpawnerTiles.Count && itemNumber >= 0)
                        {
                            selectedScriptableObject = enemySpawnerTiles[itemNumber];
                            Debug.Log(selectedScriptableObject.name);
                        }
                        else
                        {
                            selectedScriptableObject = null;
                        }
                        break;

                    case "Players":
                        numberOfRows = playerTiles.Count / 6;
                        if (playerTiles.Count % 6 > 0)
                        {
                            numberOfRows++;
                        }
                        if (TileStartY < numberOfRows * tileCell)
                        {
                            itemNumber = TileStartX / 64 + (6 * TileStartY / 64);
                        }
                        if (itemNumber < playerTiles.Count && itemNumber >= 0)
                        {
                            selectedScriptableObject = playerTiles[itemNumber];
                            Debug.Log(selectedScriptableObject.name);
                        }
                        else
                        {
                            selectedScriptableObject = null;
                        }
                        break;
                }
            }
        }
    }
}