using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OculusControllerHelper : MonoBehaviour
{
    public OVRInput.Controller Controller;
    public OVRInput.Button Button;
    public OVRInput.Axis1D Axis;
    public OVRInput.Axis2D Axis2D;
    public int SelectType;

    public UnityEvent OnClicked;
    public UnityEvent<Vector2> OnData;

    public bool ActiveOnlyOnControllerTrigger;
    public GameObject ControllerGO;
    private bool isControllerTrigged;

    public bool isDebuggable;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ControllerGO)
        {
            isControllerTrigged = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == ControllerGO)
        {
            isControllerTrigged = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDebuggable)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                OnClicked.Invoke();
            }
        }
        if (ActiveOnlyOnControllerTrigger)
        {
            if (!isControllerTrigged)
            {
                return;
            }
        }
        switch (SelectType)
        {
            case 0:
                if (OVRInput.Get(Button, Controller))
                {
                    OnClicked.Invoke();
                }
                break;
            case 1:
                if (OVRInput.Get(Axis, Controller) >= 0.9f)
                {
                    OnClicked.Invoke(); 
                }  
                break;
            case 2:
                OnData.Invoke(OVRInput.Get(Axis2D, Controller));
                break;
        }
    }
}
