using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public abstract class ButtonBase : MonoBehaviour, IPointerUpHandler, IPointerMoveHandler
{
    private Text _text;
    private Image _image;
    private Color _oldTextColor;

    protected abstract void PushButton();
    private void Start()
    {
        transform.Find("Text").TryGetComponent(out _text);
        TryGetComponent(out _image);
        _oldTextColor = _text.color;
    } 
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!MouseOnButton(eventData.position))
            _text.color = _oldTextColor;
        else
            PushButton();
    }
    private bool MouseOnButton(Vector2 point)
    {
        return _image.rectTransform.rect.Contains(transform.InverseTransformPoint(point));
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        _text.color = MouseOnButton(eventData.position) ? Color.white : _oldTextColor;
    }
}
