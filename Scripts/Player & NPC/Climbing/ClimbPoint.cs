using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevRepo {
	namespace Climbing {
		public class ClimbPoint : MonoBehaviour {
			public ClimbIKPositions[] iks = null;
			public ClimbHintIKPositions[] hints = null;
			public ClimbNeighbor[] neighbors = null;
		}
		[System.Serializable]
		public class ClimbNeighbor {
			public ClimbTransitionType type = ClimbTransitionType.step;
			public ClimbPoint target;
            public ClimbDirection direction = ClimbDirection.Left;
		}
		[System.Serializable]
		public class ClimbIKPositions {
			public AvatarIKGoal ikType = AvatarIKGoal.LeftHand;
			public Transform target;
		}
		[System.Serializable]
		public class ClimbHintIKPositions {
			public AvatarIKHint hintType = AvatarIKHint.LeftElbow;
			public Transform target;
		}
        [System.Serializable]
        public enum ClimbDirection {
            Left,
            Right,
            Up,
            Down
        }
		[System.Serializable]
		public enum ClimbTransitionType {
			step,
			jump,
            dismount
		}
	}
}