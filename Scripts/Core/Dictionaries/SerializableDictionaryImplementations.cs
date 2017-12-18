using System;
 
using UnityEngine;
 
// ---------------
//  String => Int
// ---------------
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}
 
// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}

// ---------------
//  String => Bool
// ---------------
[Serializable]
public class StringBoolDictionary : SerializableDictionary<string, bool> {}