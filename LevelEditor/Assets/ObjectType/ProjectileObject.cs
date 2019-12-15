using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileObject : ScriptableObject
{
    [SerializeField]
    public SpriteObject sprite;
    [SerializeField]
    public float speed;
    [SerializeField]
    public CircleColliderObject projectileCollider;
}
