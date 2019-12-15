using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PolyCollider-00", menuName = "Custom Assets/PolyCollider")]
[Serializable]
public class PolygonColliderObject : ColliderObject
{
    [SerializeField]
    public float height = 64.0f;
    [SerializeField]
    public float width = 64.0f;
}