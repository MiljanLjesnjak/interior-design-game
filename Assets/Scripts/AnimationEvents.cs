using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationEvents : MonoBehaviour
{
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }

}

