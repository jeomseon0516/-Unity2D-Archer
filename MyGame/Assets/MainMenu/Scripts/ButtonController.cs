using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private Text  _text;
    private Color _oldColor;

    private void Start()
    {
        TryGetComponent(out _text);
        _text.text = "Game Start";
        _text.rectTransform.sizeDelta = _text.rectTransform.sizeDelta + new Vector2(10, 10);
        _oldColor = _text.color;
    }
    public void PushButton()
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        _text.color = _text.rectTransform.rect.Contains(_text.transform.InverseTransformPoint(eventData.position)) ?
                      Color.white : _oldColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_text.rectTransform.rect.Contains(_text.transform.InverseTransformPoint(eventData.position))) 
            return;
        SceneManager.LoadScene(_text.name);
    }
    public void OnPointerDown(PointerEventData eventData) { _text.color = Color.white; }
}
