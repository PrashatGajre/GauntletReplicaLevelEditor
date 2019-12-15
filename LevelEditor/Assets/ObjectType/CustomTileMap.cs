using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomTile
{
    [SerializeField]
    CustomTileMap map;
    [SerializeField]
    RectInt tileRect;
    [SerializeField]
    public List<Vector2Int> colliderPoints = new List<Vector2Int>();

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
                EditorUtility.SetDirty(this);
            }
        }
    }
    public void CreateTileMap()
    {
        for (int i = 0; i <= texture.width; i += cellSize)
        {
            for (int j = 0; j <= texture.height; j += cellSize)
            {
                Vector2Int vec = new Vector2Int(i, j);
                if (!tiles.ContainsKey(vec))
                {
                    tiles.Add(vec, new CustomTile(new RectInt(i, j, cellSize, cellSize)));
                    EditorUtility.SetDirty(this);
                }
            }
        }
    }

    public void EditTileColliders()
    {
        for (int i = 0; i < texture.width; i += cellSize)
        {
            for (int j = 0; j < texture.height; j += cellSize)
            {
                List<Vector2Int> vecs = new List<Vector2Int>();
                vecs.Add(new Vector2Int(Random.Range(0, cellSize), Random.Range(0, cellSize)));
                vecs.Add(new Vector2Int(Random.Range(0, cellSize), Random.Range(0, cellSize)));
                vecs.Add(new Vector2Int(Random.Range(0, cellSize), Random.Range(0, cellSize)));
                vecs.Add(new Vector2Int(Random.Range(0, cellSize), Random.Range(0, cellSize)));
                Vector2Int vec = new Vector2Int(i, j);

                if (tiles.ContainsKey(vec))
                {
                    tiles[vec].setNewColliderPoints(vecs);
                EditorUtility.SetDirty(this);
                    AssetDatabase.SaveAssets();
                }
            }
        }
    }

    public void Print()
    {
        string data = "";

        for (int i = 0; i < texture.width; i += cellSize)
        {
            for (int j = 0; j < texture.height; j += cellSize)
            {
                Vector2Int vec = new Vector2Int(i, j);
                foreach (Vector2Int v in tiles[vec].colliderPoints)
                {
                    data += v.ToString() + ":";
                }
                data += "\n";
            }
        }

                Debug.Log(data);
    }
}
