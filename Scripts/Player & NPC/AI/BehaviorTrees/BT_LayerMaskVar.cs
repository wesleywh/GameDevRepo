using UnityEngine;

namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedLayerMask : SharedVariable < LayerMask >
    {
        public static implicit operator SharedLayerMask(LayerMask value) { return new SharedLayerMask { Value = value }; }
    }
}