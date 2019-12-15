using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationObject : ScriptableObject
{
    [SerializeField]
    public Texture texture;
    [SerializeField]
    public List<SpriteObject> frames;
}