using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MessageBoxController : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    private bool _isLoad;
    private Text _text;

    private void Awake()
    {
        transform.Find("Message").TryGetComponent(out _text);
        _isLoad = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isLoad) return;
        _isLoad = false;
        _text.text = "Wait..";
        _text.color = Color.black;
        gameObject.SetActive(false);
    }

    public void SetIsLoad(bool isLoad) { _isLoad = isLoad; }
    public void SetText(string text) { _text.text = text; }
    public void SetTextColor(Color color) { _text.color = color; }

    static public void SetMgBox(MessageBoxController mgBox, string text, Color color, bool isLoad)
    {
        mgBox._isLoad = isLoad;
        mgBox._text.text = text;
        mgBox._text.color = color;
    }

    public void OnPointerDown(PointerEventData eventData) {}
}
