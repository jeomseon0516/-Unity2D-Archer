using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배경 움직이기
/*
 * 플레이어보다 앞에 있는 배경은 양수의 값으로 입력해야 합니다. 0 보다 큰 숫자
 * 플레이어보다 뒤에 있는 배경은 음수의 값으로 입력해야 합니다. 0 ~ -1 -1에 가까워 질수록 느려집니다.
 */

public class BackGroundsController : MonoBehaviour
{
    // 팔로우하는 객체가 어느방향에 있는지 판별한다.
    public enum LOCATION
    {
        LEFT  =  1,
        RIGHT = -1
    }
    public float     _time;

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

    private void BackGroundCalc()
    {
        Vector3 cameraSpeed = new Vector3(
            _camera.transform.position.x - _refPos.x ,
            _camera.transform.position.y - _refPos.y,
            0.0f);
 
        // 카메라의 이동속도가 이미 델타 타임을 구한 값이기 때문에 계산하지 않는다.
        transform.position -= cameraSpeed * _time;
        _refPos = _camera.transform.position;
    }

    private LOCATION GetDirToCameraPos(Vector3 p1, Vector3 p2) { return p1.x >= p2.x ? LOCATION.LEFT : LOCATION.RIGHT; }
}