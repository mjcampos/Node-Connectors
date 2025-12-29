using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
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
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (clickSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(clickSound);
        }
    }

    public void PlayHoverSound()
    {
        if (hoverOverSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(hoverOverSound);
        }
    }
}
