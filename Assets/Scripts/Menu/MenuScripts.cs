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

    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void PlayButtonPush()
    {
        GameObject.Find("Play Button").GetComponent<Animator>().Play("ButtonPush");
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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