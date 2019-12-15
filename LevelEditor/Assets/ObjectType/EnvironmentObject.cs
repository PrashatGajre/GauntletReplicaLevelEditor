using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnvTile-00", menuName = "Custom Assets/Environment")]
[Serializable]
public class EnvironmentObject : ScriptableObject
{
    [SerializeField]
    public SpriteObject sprite;
    public Layers layer = Layers.Environment;
    [SerializeField]
    public ColliderObject collider;
}
