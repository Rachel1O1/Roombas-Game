using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelMenu", menuName = "Scriptable Objects/New Level", order = 1)]
public class LevelScriptableObject : ScriptableObject
{
    public int level;
    public string title;
}
