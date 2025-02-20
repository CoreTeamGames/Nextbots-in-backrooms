using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Nextbot")]
public class Nextbot : ScriptableObject
{
    [SerializeField] private Texture2D _texture;

    public Texture2D Texture {get { return _texture; } set { _texture = value; }}
}