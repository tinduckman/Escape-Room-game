using UnityEngine;

public class AmbientZone : MonoBehaviour
{
    public AudioClip flightSound;
    public float volume = 0.5f;

    AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = flightSound;
        audioSource.loop = true;
        audioSource.volume = 0f;
        audioSource.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            audioSource.volume = volume;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            audioSource.volume = 0f;
    }
}