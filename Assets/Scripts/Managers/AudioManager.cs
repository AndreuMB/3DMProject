using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    public List<AudioClip> audios = new List<AudioClip>();

    // Start is called before the first frame update
    void Start()
    {
        musicSource.clip = FindAudioClipByName("music");
        musicSource.Play();
    }

    AudioClip FindAudioClipByName(string name)
    {
        return audios.Find((audio) => audio.name == name);
    }

    public void PlaySFX(string name)
    {
        SFXSource.PlayOneShot(FindAudioClipByName(name));
    }
}
