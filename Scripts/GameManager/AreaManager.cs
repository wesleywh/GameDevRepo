using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AreaManager : MonoBehaviour {
    [Header("Can call \"SetValue\"")]
    [SerializeField]
    private StringBoolDictionary valuesStore = StringBoolDictionary.New<StringBoolDictionary>();
    private Dictionary<string, bool> values {
        get { return valuesStore.dictionary; }
    }

    public void SetValue(string name, bool value) {
        values[name] = value;
    }

    public object GetTargetValue(string name) {
        if (values.ContainsKey(name))
        {
            return values[name];
        }
        Debug.LogError("The value: "+name+" has not been added to AreaManager, defaulted to returning 'false'");
        return false;
    }

}
