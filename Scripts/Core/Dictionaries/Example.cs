using System.Collections.Generic;
 
using UnityEngine;
 
[CreateAssetMenu(menuName = "Example Asset")]
public class Example : ScriptableObject {
 
    [SerializeField]
    private StringIntDictionary stringIntegerStore = StringIntDictionary.New<StringIntDictionary>();
    private Dictionary<string, int> stringIntegers {
        get { return stringIntegerStore.dictionary; }
    }
 
    [SerializeField]
    private GameObjectFloatDictionary gameObjectFloatStore = GameObjectFloatDictionary.New<GameObjectFloatDictionary>();
    private Dictionary<GameObject, float> screenshots {
        get { return gameObjectFloatStore.dictionary; }
    }
}
