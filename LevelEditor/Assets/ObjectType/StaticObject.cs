using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjTile-00", menuName = "Custom Assets/Object")]
[Serializable]
public class StaticObject : ScriptableObject
{
    [SerializeField]
    public SpriteObject sprite;
    public Layers layer = Layers.StaticObjects;
    [SerializeField]
    public ColliderObject collider;
}
