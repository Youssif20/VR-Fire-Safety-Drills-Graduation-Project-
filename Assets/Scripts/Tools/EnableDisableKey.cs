using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisableKey : MonoBehaviour
{
    public KeyCode Key;
    public GameObject GameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            GameObject.SetActive(!GameObject.activeInHierarchy);
        }
    }
}
