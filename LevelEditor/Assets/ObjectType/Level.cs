using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layers { Environment, StaticObjects, Enemies, Players, Effects, UI, UIEffects };

[Serializable]
public struct LevelTile
{
    public Vector2Int position;
    public ScriptableObject[] objectsOnPosition;
}

[Serializable]
public class Level : ScriptableObject
{
    [SerializeField]
    public int levelWidth = 64;
    [SerializeField]
    public int levelHeight = 64;

    [SerializeField]
    public float time = 100;

    [SerializeField]
    public List<LevelTile> allLevelTiles = new List<LevelTile>();
}
