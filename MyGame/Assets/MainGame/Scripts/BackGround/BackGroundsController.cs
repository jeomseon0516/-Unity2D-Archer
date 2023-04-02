using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundsController : MonoBehaviour
{
    public enum LOCATION
    {
        LEFT  =  1,
        RIGHT = -1
    }

    private float    _speed; // 스피드
    private Sprite   _sprite;
    private Camera   _camera;
    private Vector3  _refPos;
    private float    _imageSizeX;
    private float    _xSize;
    private LOCATION _loc;

    // TODO : 완료
  
    private void Awake()
    {
        transform.Find("Image").TryGetComponent(out SpriteRenderer leftImage);
        SpriteRenderer rightImage = leftImage;
        name = Default.GetRemoveSelectString(name, "(Clone)");

        leftImage.sortingOrder = Default.GetIntParseString(name) + 1;
        _sprite = leftImage.sprite;
        _camera = Camera.main;

        _imageSizeX = _sprite.bounds.size.x * leftImage.transform.localScale.x;
        float cameraWidth = ((_camera.orthographicSize * 2) * _camera.aspect) * 0.5f;

        float leftX, rightX;
        for (leftX = _sprite.bounds.min.x * leftImage.transform.localScale.x; leftX > -cameraWidth;)
        {
            leftImage = Instantiate(leftImage, transform);
            leftImage.sortingOrder = Default.GetIntParseString(name) + 1;
            leftImage.transform.position -= new Vector3(_imageSizeX, 0.0f, 0.0f);
            leftX -= _imageSizeX;
        }

        for (rightX = _sprite.bounds.max.x * rightImage.transform.localScale.x; rightX < cameraWidth;)
        {
            rightImage = Instantiate(rightImage, transform);
            rightImage.sortingOrder = Default.GetIntParseString(name) + 1;
            rightImage.transform.position += new Vector3(_imageSizeX, 0.0f, 0.0f);
            rightX += _imageSizeX;
        }

        float minX = leftImage.transform.position.x - _imageSizeX * 0.5f;
        float maxX = rightImage.transform.position.x + _imageSizeX * 0.5f;

        _xSize = maxX - minX;
    }
    private void Start()
    {
        _refPos = _camera.transform.position;
    }
    private void Update()
    {
        BackGroundUpdate();
        BackGroundCalc();
    }
    private void BackGroundUpdate()
    {
        _loc = GetDirToCameraPos(_camera.transform.position, transform.position);
        int numLoc = (int)_loc;

        float cameraX = _camera.transform.position.x; // Left 1 
        float imageX  = _xSize * numLoc + transform.position.x;

        if ((_loc == LOCATION.LEFT ? cameraX : imageX) > 
            (_loc == LOCATION.LEFT ? imageX  : cameraX))
        {
            transform.position = new Vector3(transform.position.x + _xSize * 2.0f * numLoc,
                                             transform.position.y,
                                             transform.position.z);
        }
    }
    private void BackGroundCalc()
    {
        float x =  _camera.transform.position.x - _refPos.x;
        float y = (_camera.transform.position.y - _refPos.y) * 0.85f;

        Vector3 cameraSpeed = new Vector3(x, y, 0.0f);
 
        transform.position -= cameraSpeed * _speed;
        _refPos = _camera.transform.position;
    }
    private LOCATION GetDirToCameraPos(Vector3 p1, Vector3 p2) { return p1.x >= p2.x ? LOCATION.LEFT : LOCATION.RIGHT; }
    public float GetSizeX() { return _xSize; }
    public void SetSpeed(float speed) { _speed = speed; }
}