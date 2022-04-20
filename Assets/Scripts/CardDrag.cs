using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour, IPointerDownHandler
{
    Vector3 init_scale;
    float scale_lerp_duration = 0.15f;

    private void Start() => init_scale = transform.localScale;

    public void OnPointerDown(PointerEventData eventData) => SelectCard(eventData.pointerCurrentRaycast.gameObject);


    public void SelectCard(GameObject card)
    {
        Controller controller = GameObject.Find("Scripts").GetComponent<Controller>();
        Controller.selected_card = card;
        Controller.init_card_pos = transform.position;
        Controller.begin_drag_pos = Input.mousePosition;
        Controller.init_siblingindex = card.transform.GetSiblingIndex();

        GameObject.Find("Scripts").GetComponent<Controller>().AddCardPadding();

        controller.scroll.enabled = false;

        card.transform.SetParent(controller.active_parent);


        StartCoroutine(LerpScale(1f, 1.15f));

        controller.sound_manager.PlayCardInteract();
    }


    public void DeselectCard()
    {
        if (Controller.selected_card == null)
            return;

        Controller controller = GameObject.Find("Scripts").GetComponent<Controller>();

        Controller.selected_card.transform.SetParent(controller.cards);


        controller.RemoveCardPadding();

        controller.scroll.enabled = true;


        Controller.selected_card = null;

        GetComponent<LayoutElement>().ignoreLayout = true;
        GetComponent<LayoutElement>().ignoreLayout = false;

        StartCoroutine(LerpScale(1.15f, 1f));
    }

    IEnumerator LerpScale(float startModifier, float endModifier)
    {
        float time = 0;

        float scaleModifier;


        while (time < scale_lerp_duration)
        {
            scaleModifier = Mathf.Lerp(startModifier, endModifier, time / scale_lerp_duration);

            transform.localScale = init_scale * scaleModifier;

            time += Time.deltaTime;

            yield return null;
        }

        transform.localScale = init_scale * endModifier;
    }



}
