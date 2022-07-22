using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScript : MonoBehaviour
{
    [SerializeField] GameObject sound_toggle, music_toggle;
    public static bool sound_mute, music_mute;
    bool grid;

    [SerializeField] GameObject grid_toggle;
    [SerializeField] Material grid_mat;

    private void Awake()
    {
        //Sound
        sound_mute = PlayerPrefs.GetInt("sound_mute", 0) == 1;
        sound_toggle.GetComponent<Toggle>().isOn = sound_mute;

        //Music
        music_mute = PlayerPrefs.GetInt("music_mute", 0) == 1;
        music_toggle.GetComponent<Toggle>().isOn = music_mute;

        //Grid
        grid = PlayerPrefs.GetInt("grid", 0) == 1;
        grid_toggle.GetComponent<Toggle>().isOn = grid;
        
        Color col = grid_mat.color;
        col.a = grid ? 0 : 0.1f;
        grid_mat.color = col;
    }

    public void SetGrid()
    {
        grid = grid_toggle.GetComponent<Toggle>().isOn;

        int pref = grid ? 1 : 0;
        PlayerPrefs.SetInt("grid", pref);

        //Set grid mat
        Color col = grid_mat.color;
        col.a = grid ? 0 : 0.1f;
        grid_mat.color = col;
    }

    public void SetSound()
    {
        sound_mute = sound_toggle.GetComponent<Toggle>().isOn;

        int pref = sound_mute ? 1 : 0;
        PlayerPrefs.SetInt("sound_mute", pref);
    }

    public void SetMusic()
    {
        music_mute = music_toggle.GetComponent<Toggle>().isOn;

        int pref = music_toggle.GetComponent<Toggle>().isOn ? 1 : 0;
        PlayerPrefs.SetInt("music_mute", pref);

        if (music_mute)
            GameObject.Find("Sound Manager").GetComponent<SoundManager>().StopMusic();
    }

   
}
