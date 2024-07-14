using System;
using System.Collections;
using System.Collections.Generic;
using RecommendationSystem;
using UnityEngine;

public class BasicTriggerParticle : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("EventsColliders"))
        {
            other.GetComponent<BasicTrigger>().Collided = true;
        }
    }
}
