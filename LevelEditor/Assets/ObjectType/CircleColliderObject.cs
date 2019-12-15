using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CirCollider-00", menuName = "Custom Assets/Circle Collider")]
[Serializable]
public class CircleColliderObject : ColliderObject
{
    [SerializeField]
    public float radius = 32.0f;
}