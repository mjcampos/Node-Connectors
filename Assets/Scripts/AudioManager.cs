using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    [SerializeField] AudioClip hoverOverSound;
    [SerializeField] AudioClip clickSound;

    AudioSource _audioSource;
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        _audioSource.PlayOneShot(clickSound);
    }

    public void PlayHoverSound()
    {
        _audioSource.PlayOneShot(hoverOverSound);
    }
}
