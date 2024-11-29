using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClosePopupOnOutSide : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
        {
            gameObject.SetActive(false);
        }
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0 && results[0].gameObject == gameObject;
    }
}
