using UnityEngine;
using System.Collections;
using RAIN.Action;
using RAIN.Core;
using RAIN.Representation;
using RAIN.Entities.Aspects;
using RAIN.Entities;
using RAIN.Navigation.Graph;
using RAIN.Navigation.Pathfinding;
using RAIN.Motion;					//For MoveLookTarget
using RAIN.Serialization;
using RAIN.Perception.Sensors;

[RAINAction]
public class AIEnemyUpdate : RAINAction {

	//who to update
	public Expression EnemyEntityToDetect = new Expression();
//	public Expression FriendlyEntity = new Expression ();
	public Expression SensorToUse = new Expression();
	public Expression EnemyStoreVariable = new Expression();
	//what sensor to use
	private VisualSensor UpdateSensor = new VisualSensor ();

	public override ActionResult Execute(RAIN.Core.AI ai)
	{
		UpdateAI (ai);
		return ActionResult.SUCCESS;
	}
	private void UpdateAI(AI ai) {
		//get the right sensor to use
		for (int i=0; i<=ai.Senses.Sensors.Count; i++) {
			if (ai.Senses.Sensors [i].SensorName == SensorToUse.VariableName) {
				UpdateSensor = ai.Senses.Sensors [i] as VisualSensor;
				break;
			}
		}
		//use found sensor to get closest 1 match
		UpdateSensor.Sense(EnemyEntityToDetect.VariableName, RAINSensor.MatchType.BEST);
		if (UpdateSensor.Matches.Count > 0) {
			//store the found enemy location
			Vector3 enemyLoc = UpdateSensor.Matches [0].Position;
			//Save stored value in current ai memory
			ai.WorkingMemory.SetItem (EnemyStoreVariable.VariableName, enemyLoc);
		}
	}
}
