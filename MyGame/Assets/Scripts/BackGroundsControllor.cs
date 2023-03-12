using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� �����̱�
public class BackGroundsControllor : MonoBehaviour
{
    // �ȷο��ϴ� ��ü�� ������⿡ �ִ��� �Ǻ��Ѵ�.
    public enum LOCATION
    {
        LEFT  =  1,
        RIGHT = -1
    }

    public float    _time;
    public LOCATION _loc;

    private Sprite  _sprite;
    private Camera  _camera;
    private Vector3 _refPos;

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
        GetDirToCameraPos();
        int numLoc = (int)_loc;

        float cameraX = _camera.transform.position.x; // Left 1 
        float imageX  = _sprite.bounds.size.x * numLoc + transform.position.x;

        float maxX = _loc == LOCATION.LEFT ? cameraX : imageX;
        float minX = _loc == LOCATION.LEFT ? imageX  : cameraX;

        if (maxX > minX)
        {
            transform.position = new Vector3(
                transform.position.x + _sprite.bounds.size.x * 2.0f * numLoc,
                transform.position.y,
                transform.position.z);
        }
    }

    private void GetDirToCameraPos()
    {
        if      (_camera.transform.position.x > transform.position.x) _loc = LOCATION.LEFT;
        else if (_camera.transform.position.x < transform.position.x) _loc = LOCATION.RIGHT;
    }

    private void BackGroundCalc()
    {
        Vector3 cameraSpeed = new Vector3(
            _camera.transform.position.x - _refPos.x,
            _camera.transform.position.y - _refPos.y,
            0.0f);
        // ī�޶��� �̵��ӵ��� �̹� ��Ÿ Ÿ���� ���� ���̱� ������ ������� �ʴ´�.
 
        transform.position -= cameraSpeed * _time;
        _refPos = _camera.transform.position;
    }
}