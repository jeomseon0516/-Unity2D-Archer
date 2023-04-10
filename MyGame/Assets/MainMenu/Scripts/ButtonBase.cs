using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public abstract class ButtonBase : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerMoveHandler
{
    private Text _text;
    private Image _image;
    private Color _oldTextColor;
    private Color _oldImageColor;

    protected abstract void PushButton();
    private void Start()
    {
        transform.Find("Text").TryGetComponent(out _text);
        TryGetComponent(out _image);
        _oldTextColor = _text.color;
        _oldImageColor = _image.color;
    } 
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!MouseOnButton(eventData.position))
        {
            _text.color  = _oldTextColor;
            _image.color = _oldImageColor;
        }
        else
            PushButton();
    }
    private bool MouseOnButton(Vector2 point)
    {
        return _image.rectTransform.rect.Contains(transform.InverseTransformPoint(point));
    }
    public void OnPointerDown(PointerEventData eventData) 
    {
        if (!MouseOnButton(eventData.position)) return;
        _image.color = Color.gray;
    }
    public void OnPointerMove(PointerEventData eventData)
    {
        _text.color = MouseOnButton(eventData.position) ? Color.white : _oldTextColor;
    }
}
