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

    public float     _time; // 스피드

    private Sprite   _sprite;
    private Camera   _camera;
    private Vector3  _refPos;
    private LOCATION _loc;
    
    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
    }

    private void Start()
    {
        _camera = Camera.main;
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
        float imageX  = _sprite.bounds.size.x * numLoc + transform.position.x;

        if ((_loc == LOCATION.LEFT ? cameraX : imageX) > 
            (_loc == LOCATION.LEFT ? imageX : cameraX))
        {
            transform.position = new Vector3(
                transform.position.x + _sprite.bounds.size.x * 2.0f * numLoc,
                transform.position.y,
                transform.position.z);
        }
    }

    private void BackGroundCalc()
    {
        float x = _camera.transform.position.x - _refPos.x;
        float y = (_camera.transform.position.y - _refPos.y) * 0.5f;

        Vector3 cameraSpeed = new Vector3(x, y, 0.0f);
 
        transform.position -= cameraSpeed * _time;
        _refPos = _camera.transform.position;
    }

    private LOCATION GetDirToCameraPos(Vector3 p1, Vector3 p2) { return p1.x >= p2.x ? LOCATION.LEFT : LOCATION.RIGHT; }
}