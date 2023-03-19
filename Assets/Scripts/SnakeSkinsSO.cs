using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class SnakeSkinsSO : ScriptableObject
{
    public List<Sprite> SnakeSkins { get; private set; } = new();
}
