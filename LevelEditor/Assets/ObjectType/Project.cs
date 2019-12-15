using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Project : ScriptableObject
{
    [SerializeField]
    public List<Level> allLevels;
    [SerializeField]
    public GameAssets allGameAssets;
}
