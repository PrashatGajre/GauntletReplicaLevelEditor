using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawnerObject : ScriptableObject
{
    [SerializeField]
    public SpriteObject sprite;
    //[SerializeField]
    public Layers layer = Layers.Enemies;
    [SerializeField]
    public float spawnTime;
    [SerializeField]
    public EnemyObject enemy;
    [SerializeField]
    public CircleColliderObject spawnerCollider;
}
