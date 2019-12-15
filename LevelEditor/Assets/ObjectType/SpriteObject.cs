using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sprite-00", menuName = "Custom Assets/SpriteObject")]
[Serializable]
public class SpriteObject : ScriptableObject
{
    [SerializeField]
    public Sprite sprite;
    public Layers layer;
    //[SerializeField]
    public int x;
    //[SerializeField]
    public int y;
    //[SerializeField]
    public int width;
    public int height;
}