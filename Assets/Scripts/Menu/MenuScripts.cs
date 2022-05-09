using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScripts : MonoBehaviour
{
    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void PlayClickSound()
    {
        GameObject.Find("Sound Manager").GetComponent<SoundManager>().PlayMenuSelect();
    }


    public void NextLevel()
    {
        int level_index = PlayerPrefs.GetInt("level_index", 0);
        PlayerPrefs.SetInt("level_index", level_index + 1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void PlayButtonPush()
    {
        GameObject.Find("Play Button").GetComponent<Animator>().Play("ButtonPush");
    }


    public void RestartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        GameObject.Find("Scripts").GetComponent<LevelControl>().ResetLevel();
    }

    public void RoomPreviewStart()
    {
        GameObject.Find("Scripts").GetComponent<LevelControl>().RoomPreview();
    }

    public void RoomPreviewEnd()
    {
        GameObject.Find("Scripts").GetComponent<LevelControl>().RoomPreviewEnd();
    }

    public void StartLevelUI()
    {
        GameObject.Find("Scripts").GetComponent<LevelControl>().StartLevel();
    }

}
