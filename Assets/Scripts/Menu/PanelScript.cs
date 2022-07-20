using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    public void Toggle()
    {
        bool state = gameObject.activeSelf;

        if (state)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
    }

    public void ClosePanel()
    {
        GetComponent<Animator>().Play("PanelClose");
    }

    public void DisablePanel()
    {
        gameObject.SetActive(false);
    }



}
