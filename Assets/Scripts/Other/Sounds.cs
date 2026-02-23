using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class Sounds : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float picth1 = 0.8f, float pith2 = 1.2f)
    {
        audioSource.pitch = Random.Range(picth1, pith2);
        if (destroyed)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position);
        }
        else
            audioSource.PlayOneShot(clip, volume);
    }
}
