using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace BehaviorDesigner.Runtime
{
    [System.Serializable]
    public class SharedParticleSystem : SharedVariable<ParticleSystem>
    {
        public static implicit operator SharedParticleSystem(ParticleSystem value) { return new SharedParticleSystem { Value = value }; }
    }
}