using UnityEngine;
using System.Collections;

public class IfVarSet : MonoBehaviour {
	[Header("Disable if this variable doesn't equal 'isAtState'")]
	[SerializeField] private string variableName = "";
	private enum dataType {Boolean, Int, Float};
	[SerializeField] private dataType Type = dataType.Boolean;
	[SerializeField] private string isAtState = "false";
	private bool output;
	// Use this for initialization
	void Start () {
		AreaManager manager = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<AreaManager> ();
		var value = manager.GetType ().GetField (variableName).GetValue (manager);
		switch (Type) {
			case dataType.Boolean:
				output = ((bool)value == bool.Parse (isAtState));
				break;
			case dataType.Float:
				output = ((float)value == float.Parse(isAtState));
				break;
			case dataType.Int:
				output = ((int)value == int.Parse(isAtState));
				break;
		} 
		if (output == false) {
			this.gameObject.SetActive (false);
		}
	}
}
