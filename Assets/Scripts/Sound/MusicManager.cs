using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Background Music")]
    [SerializeField] private AudioClip backgroundMusic;
    
    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern - garante que só existe uma instância
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre cenas
            
            // Configura o AudioSource
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true; // Loop infinito
            audioSource.playOnAwake = false;
            audioSource.volume = musicVolume;
            
            // Inicia a música
            PlayBackgroundMusic();
        }
        else
        {
            // Se já existe uma instância, destrói este objeto duplicado
            Destroy(gameObject);
        }
    }

    private void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && audioSource != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.Play();
            Debug.Log("?? Música de fundo iniciada!");
        }
        else
        {
            Debug.LogWarning("?? MusicManager: Nenhuma música de fundo configurada!");
        }
    }

    // Métodos públicos para controlar a música
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    public void PauseMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }

    public void StopMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    public void ChangeMusicTrack(AudioClip newTrack)
    {
        if (audioSource != null && newTrack != null)
        {
            audioSource.Stop();
            audioSource.clip = newTrack;
            audioSource.Play();
        }
    }
}
