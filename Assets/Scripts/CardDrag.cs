using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardDrag : MonoBehaviour, IPointerDownHandler
{
    Vector3 init_scale;
    float scale_lerp_duration = 0.15f;

    LayerMask raycastMask;

    private void Start()
    {
        raycastMask = 1 << LayerMask.NameToLayer("Placement Raycast");

        init_scale = transform.localScale;
    }

    
    public void OnPointerDown(PointerEventData eventData)
    {
        //Controller.selected_card = eventData.pointerCurrentRaycast.gameObject;
        GameObject.Find("Scripts").GetComponent<Controller>().SelectCard(eventData.pointerCurrentRaycast.gameObject);

        Controller.init_card_pos = transform.position;

        Controller.begin_drag_pos = Input.mousePosition;

        StartCoroutine(LerpScale(1f, 1.15f));
    }

    public void ReturnCard()
    {

        if (Controller.selected_card == null)
            return;

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
