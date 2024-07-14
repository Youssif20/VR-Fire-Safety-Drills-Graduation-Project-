using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelperFunctions : MonoBehaviour
{
    public void LoadLevel(int ID)
    {
        SceneManager.LoadScene(ID);
    }
    
    
}
