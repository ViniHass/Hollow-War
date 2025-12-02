using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volume Settings")]
    [Range(0f, 10f)]
    [SerializeField] private float masterVolume = 1f; // Volume geral de todos os sons

    [Range(0f, 10f)]
    [SerializeField] private float sfxVolume = 1f; // Volume específico para efeitos sonoros (ataques, etc)

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
    {
        if (clip != null && audioSource != null)
        {
            float finalVolume = masterVolume * sfxVolume * volumeMultiplier;
            audioSource.volume = finalVolume;
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlaySound(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (clip != null && audioSource != null)
        {
            float finalVolume = masterVolume * sfxVolume * volumeMultiplier;
            audioSource.volume = finalVolume;
            audioSource.PlayOneShot(clip);
        }
    }

    // Métodos para ajustar volumes em tempo real
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp(volume, 0f, 10f);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp(volume, 0f, 10f);
    }
}