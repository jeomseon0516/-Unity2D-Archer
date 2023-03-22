using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Text text;
    private Color oldColor;

    private void Start()
    {
        text = GetComponent<Text>();
        text.text = "Game Start";
        text.rectTransform.sizeDelta = text.rectTransform.sizeDelta + new Vector2(10, 10);
        
        oldColor = text.color;
    }
    public void PushButton()
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        text.color = text.rectTransform.rect.Contains(text.transform.InverseTransformPoint(eventData.position)) ?
                     Color.white : oldColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!text.rectTransform.rect.Contains(text.transform.InverseTransformPoint(eventData.position))) 
            return;
        SceneManager.LoadScene(text.name);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        text.color = Color.white;
    }
}
