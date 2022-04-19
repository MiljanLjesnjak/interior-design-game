using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceableCategoryChange : MonoBehaviour
{
    GameObject active_category = null;

    private void Start()
    {
        active_category = GameObject.FindGameObjectWithTag("Category");

        Controller controller = GameObject.Find("Scripts").GetComponent<Controller>();

        controller.cards = active_category.transform.Find("Viewport").Find("Content");
        controller.active_parent = active_category.transform.Find("Viewport").Find("Active");
        controller.padding_card = active_category.transform.Find("Viewport").Find("Content").Find("Padding (Empty card)").gameObject;
        controller.scroll = active_category.transform.GetComponent<ScrollRect>();
    }

    public void ChangeObjectCategory()
    {
        TMP_Dropdown dropdown = GameObject.Find("Category Dropdown").GetComponent<TMP_Dropdown>();

        string category = dropdown.options[dropdown.value].text;

        active_category.SetActive(false);

        foreach (Transform child in GameObject.Find("Cards").transform)
        {
            if (child.name == category)
            {
                active_category = child.gameObject;
            }
        }

        active_category.SetActive(true);

        Controller controller = GameObject.Find("Scripts").GetComponent<Controller>();

        controller.cards = active_category.transform.Find("Viewport").Find("Content");
        controller.active_parent = active_category.transform.Find("Viewport").Find("Active");
        controller.padding_card = active_category.transform.Find("Viewport").Find("Content").Find("Padding (Empty card)").gameObject;
        controller.scroll = active_category.transform.GetComponent<ScrollRect>();
    }

}
