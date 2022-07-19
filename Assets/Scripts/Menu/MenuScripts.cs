using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScripts : MonoBehaviour
{
    public void EnableCardContainer()
    {
        GameObject.Find("Scripts").GetComponent<CardsInstantiation>().EnableCardContainer();
    }
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

        GameObject.Find("Scripts").GetComponent<LevelControl>().ResetLevel();
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RoomPreview()
    {
        GameObject.Find("Scripts").GetComponent<LevelControl>().RoomPreview();
    }

    public void StartLevelUI()
    {
        GameObject.Find("Scripts").GetComponent<LevelControl>().StartLevel();
    }

}
