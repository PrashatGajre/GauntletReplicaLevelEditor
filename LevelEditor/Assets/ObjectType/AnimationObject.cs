using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimTile-00", menuName = "Custom Assets/AnimationObject")]
[Serializable]
public class AnimationObject : ScriptableObject
{
    //[SerializeField]
    public Texture texture;
    [SerializeField]
    public List<SpriteObject> frames;
}