using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardsInstantiation : MonoBehaviour
{
    [SerializeField] GameObject card_prefab;
    [SerializeField] GameObject objects_parent, cards_parent;

    [SerializeField] Camera cam;
    public Transform photo_shoot;
    [SerializeField] GameObject light;


    void Start()
    {
        InstantiateCards();
    }

    public void EnableCardContainer()
    {
        cards_parent.SetActive(true);
        cards_parent.transform.parent.parent.Find("Scrollbar Horizontal").GetChild(0).GetChild(0).GetComponent<Image>().enabled = true;
    }

    public void InstantiateCards()
    {
        RenderTexture render_tex;
        Texture2D ss;
        Vector3 init_pos;
        Quaternion init_rot;
        int cell_count;
        float cell_x, cell_y;

        bool level_editor = GameObject.Find("Scripts").GetComponent<Controller>().level_editor;

        foreach (Transform obj in objects_parent.transform)
        {
            if (level_editor)
            {
                string category = Regex.Replace(obj.name, @"(?=.*)\s\d+", "");
                cards_parent = GameObject.Find(category).transform.GetChild(0).Find("Content").gameObject;
            }

            //Set object position and rotation
            obj.gameObject.SetActive(true);

            init_pos = obj.position;
            init_rot = obj.rotation;

            cell_count = 0;
            cell_x = 0;
            cell_y = 0;
            foreach (Transform child in obj.transform)
            {
                if (child.tag != "ObjectCell")
                    continue;

                cell_x += child.localPosition.x;
                cell_y += child.localPosition.y;
                cell_count++;
            }

            obj.position = photo_shoot.position + new Vector3(-cell_x / cell_count, -cell_y / cell_count, 0);
            obj.rotation = Quaternion.Euler(0, 0, 0);

            //Set camera ortho size based on object width
            cam.orthographicSize = (cell_x >= 8) ? 2.5f : 2;


            //Render to the render texture
            render_tex = new RenderTexture(512, 512, 8);
            cam.targetTexture = render_tex;
            cam.Render();

            //Convert render tex to 2D texture
            ss = new Texture2D(512, 512, TextureFormat.ARGB32, false);

            RenderTexture.active = render_tex;
            ss.ReadPixels(new Rect(0, 0, render_tex.width, render_tex.height), 0, 0);
            ss.Apply();


            //Apply 2D tex
            GameObject card = Instantiate(card_prefab, cards_parent.transform);
            card.transform.GetChild(2).GetComponent<RawImage>().texture = ss;


            //Set card text and name
            card.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = obj.name;
            card.name = obj.name;

            render_tex = null;
            ss = null;
            cam.targetTexture = null;
            RenderTexture.active = null;

            obj.position = init_pos;
            obj.rotation = init_rot;

            obj.gameObject.SetActive(false);
        }

        cards_parent.SetActive(false);   //It gets enabled later in canvas animation
       
        cam.gameObject.SetActive(false);
        light.SetActive(false);

        if (level_editor)
        {
            GameObject[] categories = GameObject.FindGameObjectsWithTag("Category");
            for (int i = 1; i < categories.Length; i++)
            {
                categories[i].SetActive(false);
            }
        }
    }



}
