using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTile
{
    [SerializeField]
    CustomTileMap map;
    [SerializeField]
    RectInt tileRect;
    [SerializeField]
    List<Vector2Int> colliderPoints;

    public CustomTile(RectInt rect)
    {
        tileRect = rect;
        colliderPoints.Add(new Vector2Int(0, 0));
        colliderPoints.Add(new Vector2Int(0, tileRect.width));
        colliderPoints.Add(new Vector2Int(tileRect.height, 0));
        colliderPoints.Add(new Vector2Int(tileRect.height, tileRect.width));
    }
    public void setNewColliderPoints(List<Vector2Int> newPoints)
    {
        colliderPoints = newPoints;
    }
}

[CreateAssetMenu(fileName = "CustomTileMap-00", menuName = "Custom Assets/TileMap")]
public class CustomTileMap : ScriptableObject
{
    [SerializeField]
    Texture texture;

    [SerializeField]
    int cellSize = 64;

    [SerializeField]
    Dictionary<Vector2Int, CustomTile> tiles = new Dictionary<Vector2Int, CustomTile>();

    public void CreateTileMap(Texture t, int cs)
    {
        texture = t;
        cellSize = 64;
        for (int i = 0; i < texture.width; i += cellSize)
        {
            for (int j = 0; j < texture.height; i += cellSize)
            {
                tiles.Add(new Vector2Int(i, j), new CustomTile(new RectInt(i,j,cellSize,cellSize)));
            }
        }
    }
}
