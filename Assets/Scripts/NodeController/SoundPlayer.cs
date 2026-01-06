using UnityEngine;

namespace NodeController
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundPlayer : MonoBehaviour
    {
        public static SoundPlayer Instance { get; private set; }
    
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
}
