using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditorInternal;

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

    static VisualElement levelEditor;
    static Box levelPropertiesContainer;
    static Box addLevelContainer;

    List<string> layers = new List<string> { "Environment", "StaticObjects", "Enemies", "Players"/*, "Effects", "UI", "UIEffects" */};
    ObjectField textureObjectField;
    PopupField<string> layersPopupField;
    Drawable selectedDrawable;

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

        Label selectTileMapLabel = new Label("Choose TileMap(64x64)");
        levelPropertyContainer.Add(selectTileMapLabel);

        textureObjectField = new ObjectField { objectType = typeof(UnityEngine.Texture) };
        //textureObjectField.StretchToParentSize();
        textureObjectField.AddToClassList("height-width-slider");
        levelPropertyContainer.Add(textureObjectField);
        //// Mirror value of uxml field into the C# field.
        //layersPopupField.RegisterCallback<ChangeEvent<string>>((evt) =>
        //{
        //    styledField.value = evt.newValue;
        //});

        VisualElement levelMapContainer = levelEditor.Q<Box>("LevelMapContainer");

        mapElement = new IMGUIContainer(mapOnGUI);

        mapElement.AddToClassList("level-map-sub-container");
        mapElement.RegisterCallback<MouseUpEvent>(OnMapMouseup);

        levelMapContainer.Add(mapElement);

        VisualElement levelTileContainer = levelEditor.Q<Box>("TileMapContainer");

        tileElement = new IMGUIContainer(tileOnGUI);

        tileElement.AddToClassList("tile-map-container");
        tileElement.RegisterCallback<MouseUpEvent>(OnTileMouseup);

        levelTileContainer.Add(tileElement);
    }
    void DrawAnimationList()
    {
        //if (levelList != null)
        //{
        //    levelList.DoLayoutList();
        //}

        
    }

    #region ReorderableListCallbacks

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

    IMGUIContainer mapElement;
    IMGUIContainer tileElement;
    public Vector2 mapScrollPosition = Vector2.zero;
    public Vector2 tileScrollPosition = Vector2.zero;
    int rows = 32;
    int cols = 32;
    int cell = 32;
    int tileCell = 64;
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

        foreach (MapLayer m in layersToDraw)
        {
            foreach (KeyValuePair<Vector2Int, Drawable> d in m.drawRects)
            {
                Texture TextureToDraw = (Texture)AssetDatabase.LoadAssetAtPath(d.Value.assetPath, typeof(Texture));
                Rect drawRect = new Rect(d.Key, new Vector2((cell/2), (cell/2)));
                Rect destRect = d.Value.texCoords;

                destRect.x /= TextureToDraw.width;
                destRect.y /= TextureToDraw.height;
                destRect.y = 1 - destRect.y;
                destRect.width /= TextureToDraw.width;
                destRect.height /= TextureToDraw.height;

                //Debug.Log("Dest: " + destRect.x + " : " + destRect.y);
                GUI.DrawTextureWithTexCoords(drawRect, TextureToDraw, destRect);
            }
        }
        MarkDirtyRepaint();
        LevelEditorWindow.RepaintWindow();
        GUI.EndScrollView();
    }

    private void tileOnGUI()
    {
        tileScrollPosition = EditorGUILayout.BeginScrollView(tileScrollPosition, true, true, GUILayout.Width(tileElement.contentRect.width), GUILayout.Width(tileElement.contentRect.height));

        if (textureObjectField.value != null)
        {
            Texture imageToDraw = (Texture)textureObjectField.value;
            Rect drawRect = new Rect(0, 0, imageToDraw.width, imageToDraw.height);
            GUI.DrawTexture(drawRect, imageToDraw);
            EditorGUILayout.LabelField("", GUILayout.Width(imageToDraw.width), GUILayout.Height(imageToDraw.height));
        }
        ////Rect sizeRect = new Rect(0, 0, rows * cell, cols * cell);
        //EditorGUILayout.LabelField("", GUILayout.Width(cols * cell), GUILayout.Height(rows * cell));

        //for (int i = 0; i <= rows; i++)
        //{
        //    EditorGUI.DrawRect(new Rect(0, i * cell, cols * cell, 2), Color.white);
        //}
        //for (int j = 0; j <= cols; j++)
        //{
        //    EditorGUI.DrawRect(new Rect(j * cell, 0, 2, rows * cell), Color.white);
        //}
        GUI.EndScrollView();
        MarkDirtyRepaint();
        LevelEditorWindow.RepaintWindow();
    }

    void OnMapMouseup(MouseUpEvent evt)
    {
        if ((evt.localMousePosition.x < mapElement.contentRect.width - 15) && (evt.localMousePosition.y < mapElement.contentRect.height - 15))
        {
            int mapCellStartX = Mathf.FloorToInt(evt.localMousePosition.x + mapScrollPosition.x) / (cell/2);
            int mapCellStartY = Mathf.FloorToInt(evt.localMousePosition.y + mapScrollPosition.y) / (cell/2);
            //Debug.Log(evt.localMousePosition + mapScrollPosition);
            //Debug.Log((evt.localMousePosition + mapScrollPosition) / (cell/2));

            Vector2Int drawPos = new Vector2Int(mapCellStartX * (cell/2), mapCellStartY * (cell/2));
            foreach (MapLayer ml in layersToDraw)
            {
                if (ml.name == layersPopupField.value)
                {
                    Drawable temp;
                    if (ml.drawRects.TryGetValue(drawPos, out temp))
                    {
                        Event e = Event.current;
                        if (e.modifiers == EventModifiers.Shift)
                        {
                            ml.drawRects.Remove(drawPos);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(selectedDrawable.assetPath))
                            {
                                temp.assetPath = selectedDrawable.assetPath;
                                temp.texCoords = selectedDrawable.texCoords;
                                if (ml.drawRects[drawPos].Equals(temp))
                                {
                                    ml.drawRects.Remove(drawPos);
                                }
                                else
                                {
                                    ml.drawRects[drawPos] = temp;
                                }
                            }
                        }
                    }
                    else
                    {
                        Event e = Event.current;
                        if (e.modifiers == EventModifiers.Shift)
                        {
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(selectedDrawable.assetPath))
                            {
                                ml.drawRects.Add(drawPos, selectedDrawable);
                            }
                        }
                    }
                }
            }

        }
        MarkDirtyRepaint();
        LevelEditorWindow.RepaintWindow();
    }

    void OnTileMouseup(MouseUpEvent evt)
    {
        if (textureObjectField.value != null)
        {
            if ((evt.localMousePosition.x < tileElement.contentRect.width - 15) && (evt.localMousePosition.y < tileElement.contentRect.height - 15))
            {
                int TileStartX = (Mathf.FloorToInt((evt.localMousePosition.x + tileScrollPosition.x) / tileCell) * tileCell);
                int TileStartY = (Mathf.FloorToInt((evt.localMousePosition.y + tileScrollPosition.y) / tileCell) * tileCell);

                //Debug.Log(TileStartX +":"+TileStartY);
                if ((TileStartX < ((Texture)textureObjectField.value).width) && (TileStartY < ((Texture)textureObjectField.value).height))
                {
                    //Debug.Log("within Bounds : " + TileStartX + ":" + TileStartY + " / " + ((Texture)textureObjectField.value).width + " : " + ((Texture)textureObjectField.value).height);
                    selectedDrawable.assetPath = AssetDatabase.GetAssetPath(textureObjectField.value);
                    //Debug.Log(selectedDrawable.assetPath);
                    selectedDrawable.texCoords = new Rect(TileStartX, TileStartY + tileCell, tileCell, tileCell);
                }
                else
                {
                    //Debug.Log("Out of Bounds: " + TileStartX + ":" + TileStartY + " / " + ((Texture)textureObjectField.value).width + " : " + ((Texture)textureObjectField.value).height);
                }
            }
        }
    }
    #endregion
}