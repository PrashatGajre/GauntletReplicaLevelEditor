using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerObject : ScriptableObject
{
    [SerializeField]
    public Texture texture;
    [SerializeField]
    public Layers layer = Layers.Players;
    [SerializeField]
    public SpriteObject sprite;
    [SerializeField]
    public List<AnimationObject> animations;
    [SerializeField]
    public float health;
    [SerializeField]
    public int attack;
    [SerializeField]
    public ScriptableObject projectile;
    [SerializeField]
    public CircleColliderObject playerCollider;
}