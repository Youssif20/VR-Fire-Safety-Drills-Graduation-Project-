using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHelper : MonoBehaviour
{
    public GameObject ControllerGO;
    public UnityEvent OnTriggerEnterEvent;
    public UnityEvent OnTriggerExitEvent;

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ControllerGO)
        {
            OnTriggerEnterEvent.Invoke();
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == ControllerGO)
        {
            OnTriggerExitEvent.Invoke();
        }
    }


}
