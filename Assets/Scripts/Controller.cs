using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//KARTICE KAD GLEDA KOLIKO PADDING DA STAVI I OSTALE KALKULACIJE ZA UI ZAVISE OD REZOLUCIJE!!!! POPRAVI
//I ONDA PONOVO UKLJUCI TARGET DPI??

public class Controller : MonoBehaviour
{
    LayerMask placementRaycast;

    Transform wall_grid;

    public bool level_editor = false;

    [Header("Placeables")]
    public Transform placeables;
    public static GameObject selected_object = null;
    public static Vector3 object_drag_offset;

    [Header("Cards")]
    public Transform cards, active_parent;
    public static GameObject selected_card = null;
    public static Vector3 init_card_pos, begin_drag_pos;

    [Header("Padding and scroll")]
    public GameObject padding_card;   //Empty card
    public ScrollRect scroll;
    public float scroll_speed = 5f;



    private void Start()
    {
        placementRaycast = 1 << LayerMask.NameToLayer("Placement Raycast");

        wall_grid = GetComponent<Grid>().wall_grid.transform;
    }

    //Input
    private void Update()
    {
        MoveObject();
        MoveCard();

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (selected_object)
                selected_object.GetComponent<ObjectDrag>().PlaceObject();


            if (selected_card)
                selected_card.GetComponent<CardDrag>().DeselectCard();

        }

    }

    //Placeable is selected
    void MoveObject()
    {
        if (selected_object == null)
            return;

        //Rotate wall objects while moving
        if (Types.Equals(selected_object.GetComponent<ObjectDrag>().GetType(), typeof(WallDrag)))
        {
            int min = 0;
            for (int i = 1; i < wall_grid.childCount; i++)
            {
                if (Vector3.Magnitude(selected_object.transform.position - wall_grid.GetChild(i).position) < Vector3.Magnitude(selected_object.transform.position - wall_grid.GetChild(min).position))
                    min = i;
            }

            selected_object.transform.rotation = wall_grid.GetChild(min).rotation;
            selected_object.transform.Rotate(90, 90, 90);
        }

        //Move to raycast hit point
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 1000, placementRaycast))
        {
            selected_object.transform.position = hit.point - object_drag_offset;
        }

        else
        {
            ObjectToCard();
            return;
        }

        //Keep entire object in grid while moving
        foreach (Transform child in selected_object.transform)
        {
            if (child.tag != "ObjectCell")
                continue;

            Vector3 cell_pos = child.transform.position;

            Vector3 offset = Vector3.zero;

            float horizontal_bound, vertical_bound;

            if (Equals(selected_object.GetComponent<ObjectDrag>().GetType(), typeof(FurnitureDrag)))
            {
                horizontal_bound = Grid.furniture_grid_bound;
            }
            else
            {
                horizontal_bound = Grid.furniture_grid_bound + 0.4f;
            }

            if (cell_pos.x > horizontal_bound)
            {
                offset.x = cell_pos.x - horizontal_bound;
            }

            if (cell_pos.x < -horizontal_bound)
            {
                offset.x = cell_pos.x + horizontal_bound;

            }

            if (cell_pos.z > horizontal_bound)
            {
                offset.z = cell_pos.z - horizontal_bound;
            }

            if (cell_pos.z < -horizontal_bound)
            {
                offset.z = cell_pos.z + horizontal_bound;
            }

            if (cell_pos.y - 40f < 0.5f)
            {
                offset.y = cell_pos.y - 40f - 0.5f;
            }

            if (cell_pos.y - 40f > Grid.wall_grid_bound)
            {
                offset.y = cell_pos.y - 40f - Grid.wall_grid_bound;
            }



            if (offset != Vector3.zero)
            {
                Vector3 pos = selected_object.transform.position - offset;
                selected_object.transform.position = pos;
            }

        }



    }

    void ObjectToCard()
    {
        if (level_editor)
        {
            selected_object.GetComponent<Animator>().Play("ObjectPickup");
            Destroy(selected_object);
            selected_object = null;

            return;
        }

        foreach (Transform card in active_parent)
        {
            if (card.name == selected_object.name)
            {
                selected_card = card.gameObject;
                break;
            }
        }

        selected_card.SetActive(true);
        selected_card.transform.position = Input.mousePosition;

        selected_card.GetComponent<Animator>().Play("OpenAnim");

        selected_object.GetComponent<Animator>().Play("ObjectPickup");
        selected_object.SetActive(false);

        selected_object = null;
    }
    //---


    //Card is selected
    void MoveCard()
    {
        if (selected_card == null)
            return;

        //Move card
        selected_card.transform.position = init_card_pos + Input.mousePosition - begin_drag_pos;


        //Scroll rect manually if card is dragged to either side
        if (selected_card.transform.localPosition.x <= -200f && scroll.horizontalNormalizedPosition >= 0)
        {
            scroll.horizontalNormalizedPosition -= Time.deltaTime * scroll_speed / -200f * selected_card.transform.localPosition.x;

            padding_card.GetComponent<RectTransform>().sizeDelta = new Vector2(-50, 185);
            LayoutRebuilder.MarkLayoutForRebuild(cards.GetComponent<RectTransform>());
            return;
        }
        else if (selected_card.transform.localPosition.x >= 200f && scroll.horizontalNormalizedPosition <= 1)
        {
            scroll.horizontalNormalizedPosition += Time.deltaTime * scroll_speed / 200f * selected_card.transform.localPosition.x;

            padding_card.GetComponent<RectTransform>().sizeDelta = new Vector2(-50, 185);
            LayoutRebuilder.MarkLayoutForRebuild(cards.GetComponent<RectTransform>());
            return;
        }


        //Set padding card SiblingIndex (insert padding in correct place)
        padding_card.transform.SetParent(active_parent);

        if (cards.childCount == 0 || selected_card.transform.position.x < cards.GetChild(0).position.x)
        {
            padding_card.transform.SetParent(cards);
            padding_card.transform.SetSiblingIndex(0);
        }
        else if (selected_card.transform.position.x > cards.GetChild(cards.childCount - 1).position.x)
        {
            padding_card.transform.SetParent(cards);
            padding_card.transform.SetSiblingIndex(cards.childCount - 1);
        }
        else
        {
            for (int i = 1; i < cards.childCount; i++)
            {

                if (selected_card.transform.position.x > cards.GetChild(i - 1).position.x
                && selected_card.transform.position.x < cards.GetChild(i).position.x)
                {
                    padding_card.transform.SetParent(cards);
                    padding_card.transform.SetSiblingIndex(cards.GetChild(i).GetSiblingIndex());
                    break;
                }
            }
        }


        //Set padding card width
        float width = 300 - 0.35f * selected_card.transform.position.y;
        width = Mathf.Clamp(width, -50, 125); //-50 because spacing in layout is 
        padding_card.GetComponent<RectTransform>().sizeDelta = new Vector2(width, 185);


        LayoutRebuilder.MarkLayoutForRebuild(cards.GetComponent<RectTransform>());

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //Place card (mouse if over grid)
        if (Physics.Raycast(ray, out hit, 1000, placementRaycast))
        {
            CardToObject(hit);
        }
    }

    void CardToObject(RaycastHit hit)
    {
        //GameObject prefab = prefabs.Find(selected_card.name).gameObject;
        GameObject placeable = null;
        foreach (Transform p in placeables)
        {
            if (p.name == selected_card.name && !p.gameObject.activeSelf)
            {
                if (!level_editor)
                    placeable = p.gameObject;
                else
                {
                    placeable = Instantiate(p.gameObject);
                    placeable.name = p.gameObject.name;
                    placeable.transform.parent = GameObject.Find("Placed Objects").transform;
                }
            }
        }


        if (!level_editor)
            selected_card.GetComponent<Animator>().Play("CloseAnim");
        else
            selected_card.GetComponent<CardDrag>().DeselectCard();

        //selected_card.SetActive(false);

        placeable.SetActive(true);

        placeable.transform.position = hit.point;

        selected_object = placeable;

        selected_object.SetActive(true);

        selected_object.GetComponent<Animator>().Play("ObjectPlace");

        selected_card = null;
    }
    //---


    public void AddCardPadding()
    {
        padding_card.transform.SetSiblingIndex(selected_card.transform.GetSiblingIndex());
        padding_card.GetComponent<RectTransform>().sizeDelta = new Vector2(125f, 185);
    }

    public void RemoveCardPadding()
    {
        selected_card.transform.SetSiblingIndex(padding_card.transform.GetSiblingIndex());
        padding_card.GetComponent<RectTransform>().sizeDelta = new Vector2(-50, 185);
    }


}
