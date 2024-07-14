using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RecommendationSystem;
using TMPro;
using UnityEngine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Networking;

public class RecommendationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public BasicTrigger[] RoadToDoor;
    public TMP_Text Debugger;

    public bool isRoadToDoor = true;
    public GameObject[] Heatmaps;
    public int NumberOfHeatmapsActive;
    public int CurrentRecommendation;

    public FireManager FireManager;
    public bool isAIAgent;
    public string Header;
    private bool isAiAgentOffline;

    public UnityEvent OnLevel3;
    public float OnLevel3Timer;

    private void OnEnable()
    {
        CurrentRecommendation = -1;
    }

    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (isAIAgent)
            {
                //Debugger.text = "AI Agent Last Check:" + true + ", Current Proposed Solution: " + CurrentRecommendation; 

                continue;
            }

/*            bool isRoadToDoor = !RoadToDoor.All(x => x.Collided);
            this.isRoadToDoor = isRoadToDoor;
  */
            var NumberOfHeatmaps = 0;
            for (int i = 0; i < Heatmaps.Length; i++)
            {
                if (Heatmaps[i].activeInHierarchy)
                    NumberOfHeatmaps++;
            }

            NumberOfHeatmapsActive = NumberOfHeatmaps;
            var Risk = FireManager.CalcuateRiskEquation();
            Debug.Log(Risk);

            if (Risk >= 0 && Risk < 40)
            {
                CurrentRecommendation = 0;
            }
            else if (Risk >= 40 && Risk < 70)
            {
                CurrentRecommendation = 1;
            }
            else if (Risk >= 70 && Risk < 140)
            {
                CurrentRecommendation = 2;
            }
            else if (Risk >= 140)
            {
                CurrentRecommendation = 3;
            }

//            Debugger.text = "Road to door is clear? " + isRoadToDoor + "\nCurrent Recommendation:" + CurrentRecommendation + "\nNumber Of Disks On Fire:" + NumberOfHeatmapsActive; 
        }
    }

    string apiUrl = "http://192.168.137.1:5000/predict";

    public void CallAI(string Object)
    {
        if (isAIAgent)
        {
            if (isAiAgentOffline)
            {
                var Risk = FireManager.CalcuateRiskEquation();
                var newRecommendation = 0;
                if (Risk >= 0 && Risk < 40)
                {
                    newRecommendation = 0;
                }
                else if (Risk >= 40 && Risk < 70)
                {
                    newRecommendation = 1;
                }
                else if (Risk >= 70 && Risk < 140)
                {
                    newRecommendation = 2;
                }
                else if (Risk >= 140)
                {
                    newRecommendation = 3;
                }

                if (CurrentRecommendation != newRecommendation)
                {
                    CurrentRecommendation = newRecommendation;
                    Debug.Log("New Recommendation :" + CurrentRecommendation);
                    AudioManager.Instance.PlaySound(CurrentRecommendation);
                }

                return;
            }
            else
            {
                StartCoroutine(SendPostRequest(Object));
            }
        }
    }

    private IEnumerator SendPostRequest(string Object)
    {
        var headers = Header.Split(',');
        var row = Object.Split(',');
        Dictionary<string, string> newData = new Dictionary<string, string>();
        for (int i = 0; i < headers.Length - 1; i++)
        {
            newData.Add(headers[i], row[i]);
        }

        // Create the JSON payload
        string postData = JsonConvert.SerializeObject(newData);
        postData = postData.Replace("True", "1");
        postData = postData.Replace("False", "2");
        
        // Create a UnityWebRequest object
        // Create a UnityWebRequest object
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        request.timeout = 2;
        
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Send the request and wait for a response
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            //Debug.LogError("POST request failed. Error: " + request.error);
            isAiAgentOffline = true;
            CallAI(Object);
        }
        else
        {
            // Request successful, retrieve the response
            string response = request.downloadHandler.text;
            var newRecommendation = Int32.Parse(response.Replace("{", "").Replace("}", "").Replace("\n", "")
                .Replace("[", "").Replace("]", ""));
            if (CurrentRecommendation != newRecommendation)
            {
                CurrentRecommendation = newRecommendation;
                Debug.Log("New Recommendation :" + CurrentRecommendation);
                AudioManager.Instance.PlaySound(CurrentRecommendation);
            }

            Debug.Log("POST request successful. Response: " + response);
        }
        
    }
    
    public IEnumerator GameEndSenario()
    {
        yield return new WaitForSeconds(OnLevel3Timer);
        OnLevel3.Invoke();
    }
}