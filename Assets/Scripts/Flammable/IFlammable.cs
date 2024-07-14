using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class IFlammable : MonoBehaviour
{
    public FireManager FireManager;
    public List<IFlammable> CloseFlammables;
    public bool canStartFire;
    public int Ignite;
    public int RandomIgnitationToFire;
    public bool OnFire = false;
    public GameObject[] FireDesigns;
    public GameObject MyFire;
    public float EstimatedArea;
    public GameObject Heatmap;
    public float DistanceToFireExt;
    
    private void Awake()
    {
        FireManager = GameObject.Find("FireManager").GetComponent<FireManager>();
    }

    private void Start()
    {
        FireManager.Register(this);
        RandomIgnitationToFire = Random.Range(8, 15);
    }

    public void AddFire()
    {
        MyFire.transform.localScale *= 1.0000000000000000000000000000001f;
    }

    public bool Fire(bool Force)
    {
        if (OnFire)
        {
            AddFire();
            return false;
        }
        
        if (Force)
        {
            StartFire();
            return true;
        }

        Ignite++;
        if (Ignite > +RandomIgnitationToFire)
        {
            StartFire();
            return true;
        }
        else
        {
            return false;
        }
        
        Debug.Log("I Started Fire");
    }

    public void StartFire()
    {
        OnFire = true;
        MyFire = Instantiate(FireDesigns[Random.Range(0, FireDesigns.Length)], this.transform);
        MyFire.transform.position = new Vector3(MyFire.transform.position.x, MyFire.transform.position.y + 0.1f,
            MyFire.transform.position.z);
        if(Heatmap)
            Heatmap.SetActive(true);
    }

    private void OnDestroy()
    {
        FireManager.UnRegister(this);
    }
}
