using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] music_clip;
    [SerializeField] AudioClip end_clip;
    [SerializeField] AudioClip menu_click_clip;
    [SerializeField] AudioClip place_clip, card_interact;
    [SerializeField] AudioSource audio_source;
    int clip_index = 0;

    private void Start()
    {
        clip_index = Random.Range(0, music_clip.Length);

        if (GameObject.Find("Audio Source") == null)
        {
            audio_source.gameObject.SetActive(true);
            DontDestroyOnLoad(audio_source.gameObject);

            StartMusic();
        }
        else
        {
            audio_source = GameObject.Find("Audio Source").GetComponent<AudioSource>();
        }
       

    }

    public void StartMusic()
    {
        audio_source.clip = music_clip[clip_index];
        audio_source.Play();
    }

    private void Update()
    {
        if (!audio_source.isPlaying)
        {
            if (++clip_index > music_clip.Length - 1)
                clip_index = 0;

            audio_source.clip = music_clip[clip_index];
            audio_source.Play();
        }
    }

    public void PlayLevelEnd()
    {
        audio_source.PlayOneShot(end_clip);
    }

    public void PlayMenuSelect()
    {
        audio_source.PlayOneShot(menu_click_clip);
    }

    public void PlayPlaceObject()
    {
        audio_source.PlayOneShot(place_clip);
    }

    public void PlayCardInteract()
    {
        audio_source.PlayOneShot(card_interact);
    }






}