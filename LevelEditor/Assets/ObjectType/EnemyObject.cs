using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyObject : ScriptableObject
{
    //[SerializeField]
    //public Texture texture;
    //[SerializeField]
    public Layers layer = Layers.Enemies;
    [SerializeField]
    public SpriteObject sprite;
    //[SerializeField]
    //public List<AnimationObject> animations;
    [SerializeField]
    public float health;
    [SerializeField]
    public float speeed = 10.0f;
    [SerializeField]
    public float attack;
    [SerializeField]
    public CircleColliderObject enemyCollider;
}
