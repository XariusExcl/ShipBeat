using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class KeyValue
{
    [SerializeField] public string key;
    [SerializeField] public GameObject value;
}

[CreateAssetMenu(fileName = "StoryboardList", menuName = "Scriptable Objects/StoryboardList")]
public class StoryboardList : ScriptableObject
{ 
    [SerializeField] public List<KeyValue> storyboards;
}