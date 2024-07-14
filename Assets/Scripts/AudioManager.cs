using System;
using System.Collections;
using System.Collections.Generic;
using AdvancedMonoBehaviour.Scripts.Patterns;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : AdvancedSingletonPresent<AudioManager>
{
    private AudioSource _audioSource;
    public AudioClip[] Clips;

    public override void Awake()
    {
        base.Awake();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
    }

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        _audioSource.Stop();
        
    }

    public void PlaySound(int ID)
    {
        _audioSource.Stop();
        _audioSource.clip = Clips[ID];
        _audioSource.Play();
    }
}
