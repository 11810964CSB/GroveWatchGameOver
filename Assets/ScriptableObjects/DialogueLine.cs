using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public Sprite image;
    [TextArea(3, 10)] public string text;
}