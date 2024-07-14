using System;
using System.Collections.Generic;
using UnityEngine;

namespace RecommendationSystem
{
    public class BasicTrigger : MonoBehaviour
    {
        public bool Collided;

        private void OnParticleTrigger()
        {
            Collided = true;

        }

    }
}