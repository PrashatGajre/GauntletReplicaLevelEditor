using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[Serializable]
public class GameAssets : ScriptableObject
{
    [SerializeField]
    public List<Texture> gameTextures = new List<Texture>();
    [SerializeField]
    public List<AudioClip> gameAudioClips = new List<AudioClip>();
    [SerializeField]
    public List<Font> gameFonts = new List<Font>();
}