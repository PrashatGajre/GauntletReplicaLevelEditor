using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layers { Environment, StaticObjects, Enemies, Players /*, Effects, UI, UIEffects*/ };

[Serializable]
public struct Layer
{
    public List <Vector2Int> position;
    public List <ScriptableObject> objectOnPosition;
}

[CreateAssetMenu(fileName = "Level-00", menuName = "Custom Assets/Level")]
[Serializable]
public class Level : ScriptableObject
{
    [SerializeField]
    public int levelWidth = 64;
    [SerializeField]
    public int levelHeight = 64;

    [SerializeField]
    public float time = 300;

    [SerializeField]
    public Layer[] layers = new Layer[4];
}
