using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class FireManager : MonoBehaviour
{
    public List<IFlammable> Flammables = new List<IFlammable>();
    public List<IFlammable> OnFire = new List<IFlammable>();
    public float IgnitionRange = 50f;
    public int Ignitions;
    public float GameSpeed = 5f;
    public TMP_Text DebugText;
    public RecommendationManager _RecommendationManager;
    public GameObject FireExt;

    [Header("Debug Settings")] private string fileName = "data.csv";
    private string filePath;
    public Camera DebugCamera;
    public Camera HeatMapCamera;
    private int CameraScreenshotCounter;
    public bool isDataSheetGenerator;
    public bool isGameEnded;

    public void Register(IFlammable _flammable)
    {
        if (!Flammables.Contains(_flammable))
            Flammables.Add(_flammable);
    }

    public void UnRegister(IFlammable _flammable)
    {
        if (Flammables.Contains(_flammable))
            Flammables.Remove(_flammable);
    }

    public void DumpSessionData()
    {
        var csvheader = "";
        csvheader += _RecommendationManager.isRoadToDoor + ",";
        for (var i = 0; i < Flammables.Count; i++) csvheader += Flammables[i].OnFire + ",";
        _RecommendationManager.CallAI(csvheader);
        csvheader += _RecommendationManager.CurrentRecommendation;

        WriteCSV(csvheader);
        if (isDataSheetGenerator)
        {
            var filePathCamera = filePath.Replace(".csv", "") + "/" + "Screenshot_Record_Default_" +
                                 CameraScreenshotCounter + ".png";
            DebugCamera.targetTexture = new RenderTexture(1920, 1080, 24);
            var screenshot = new Texture2D(DebugCamera.targetTexture.width, DebugCamera.targetTexture.height,
                TextureFormat.RGB24, false);
            DebugCamera.Render();
            RenderTexture.active = DebugCamera.targetTexture;
            screenshot.ReadPixels(new Rect(0, 0, DebugCamera.targetTexture.width, DebugCamera.targetTexture.height), 0,
                0);
            screenshot.Apply();
            var bytes = screenshot.EncodeToPNG();
            File.WriteAllBytes(filePathCamera, bytes);
            DebugCamera.targetTexture = null;
            RenderTexture.active = null;
        }

        /*filePathCamera = filePath.Replace(".csv","") + "/" + "Screenshot_Record_Heatmap_" + CameraScreenshotCounter + ".png";
        HeatMapCamera.targetTexture = new RenderTexture(1920,1080, 24);
        Texture2D screenshot1 = new Texture2D(HeatMapCamera.targetTexture.width, HeatMapCamera.targetTexture.height, TextureFormat.RGB24, false);
        HeatMapCamera.Render();
        RenderTexture.active = HeatMapCamera.targetTexture;
        screenshot1.ReadPixels(new Rect(0, 0, HeatMapCamera.targetTexture.width, HeatMapCamera.targetTexture.height), 0, 0);
        screenshot1.Apply();
        bytes = screenshot1.EncodeToPNG();
        System.IO.File.WriteAllBytes(filePathCamera, bytes);
        HeatMapCamera.targetTexture = null;
        RenderTexture.active = null;*/
        CameraScreenshotCounter++;
    }

    private void WriteCSV(string text)
    {
        using (var writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(text);
        }
    }


    public void CalucateNewFire()
    {
        if (OnFire.Count == 0)
        {
            var StartFireWith = Flammables.FindAll(x => x.canStartFire);
            var Object = StartFireWith[Random.Range(0, StartFireWith.Count())];
            OnFire.Add(Object);
            Object.Fire(true);
            return;
        }

        for (var i = 0; i < OnFire.Count; i++)
        {
            var Destroy = true;

            for (var j = 0; j < Flammables.Count; j++)
            {
                if (OnFire.Contains(Flammables[j])) continue;

                //Debug.Log(OnFire[i] + " Firing " + Flammables[j]);   
                var dist = Vector3.Distance(OnFire[i].transform.position, Flammables[j].transform.position);
                if (dist < IgnitionRange)
                {
                    Destroy = false;
                    Debug.DrawLine(OnFire[i].transform.position, Flammables[j].transform.position, Color.green, 5f);
                    var CaughtFire = Flammables[j].Fire(false);
                    if (CaughtFire) OnFire.Add(Flammables[j]);
                }
                else
                {
                    Debug.DrawLine(OnFire[i].transform.position, Flammables[j].transform.position, Color.red, 5f);
                }
            }

            if (Destroy)
                //Object.Destroy(OnFire[i].gameObject);
                OnFire.Remove(OnFire[i]);
        }
    }

    private void CreateCSVFile()
    {
        if (!File.Exists(filePath))
        {
            var csvheader = "";
            csvheader += "isRoadToDoorClear,";
            for (var i = 0; i < Flammables.Count; i++) csvheader += "Element " + i + " Status,";

            csvheader += "Sol";
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine(csvheader);
                _RecommendationManager.Header = csvheader;
            }

            Directory.CreateDirectory(filePath.Replace(".csv", ""));
        }
    }

    public void CalcuateDisToFireExt()
    {
        var position = FireExt.transform.position;
        for (var j = 0; j < Flammables.Count; j++)
        {
            var dist = Vector3.Distance(position, Flammables[j].transform.position);
            Flammables[j].DistanceToFireExt = dist;
            Debug.DrawLine(position, Flammables[j].transform.position, Color.yellow, 5f);
        }
    }

    public float CalcuateRiskEquation()
    {
        float Eqn = 0;
        for (var i = 0; i < OnFire.Count; i++) Eqn += OnFire[i].DistanceToFireExt;

        return Eqn;
    }

    public IEnumerator Start()
    {
        yield return new WaitForSeconds(0.2f);
        fileName = "Data_Session_" + DateTime.Now.ToString().Replace("/", "_").Replace(":", "_") + "_Game.csv";
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        CreateCSVFile();
        CalcuateDisToFireExt();
        while (true)
        {
            if (isGameEnded)
            {
                break;
            }
            yield return new WaitForSeconds(GameSpeed);
            var area = 0.0f;
            for (var i = 0; i < OnFire.Count; i++) area += OnFire[i].EstimatedArea;

            DumpSessionData();

            CalucateNewFire();
            Ignitions++;
            if (Ignitions % 5 == 0) IgnitionRange += 0.01f;
            if (_RecommendationManager.CurrentRecommendation == 3)
            {
                EndGame();
                break;
            }

            if (isDataSheetGenerator)


                if ((float)OnFire.Count / Flammables.Count >= 0.75f)
                {
                    Debug.Log("GameEnded");
                    yield return new WaitForSeconds(1f);
                    DumpSessionData();
                    {
                        SceneManager.LoadScene(1);
                    }
                    break;
                }
                else
                {
 
                }

            // Debug
        }
    }

    public void EndGame()
    {
        if (_RecommendationManager.CurrentRecommendation == 0)
        {
            _RecommendationManager.CurrentRecommendation = 1;
        }

        StartCoroutine(_RecommendationManager.GameEndSenario());


        var Score = 0.7f * Math.Abs((_RecommendationManager.CurrentRecommendation - 3.0f) / 2.0f) +
                    0.3f * ((10.0f * this.GameSpeed) / (Ignitions * GameSpeed));
        
        if (_RecommendationManager.CurrentRecommendation == 3)
        {
            Score = 0;
        }
        
        Debug.Log(Score);
        var totalScore = PlayerPrefs.GetFloat("Score", 0f);
        totalScore += Score;
        PlayerPrefs.SetFloat("Score",totalScore);
        PlayerPrefs.Save();
        Debug.Log("Game Ended");
        for (int i = 0; i < OnFire.Count; i++)
        {
            Destroy(OnFire[i].MyFire);
        }
        isGameEnded = true;
    }

    public void ResetGame()
    {
        if(isGameEnded)
            SceneManager.LoadScene(0);
    }
    
    
    
    
}