using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배경 움직이기
public class BackGroundsControllor : MonoBehaviour
{
    public float _time;
    private Camera _camera;
    private Vector3 _refPos;

    void Start()
    {
        _camera = Camera.main;
        _refPos = _camera.transform.position;
    }

    void Update()
    {
        BackGroundCalc();
    }

    private void BackGroundCalc()
    {
        // 카메라의 이동속도
        Vector3 cameraSpeed = new Vector3(
            _camera.transform.position.x - _refPos.x,
            _camera.transform.position.y - _refPos.y,
            0.0f);

        // 카메라의 이동속도가 이미 델타 타임을 구한 값이기 때문에 계산하지 않는다.
        _refPos = _camera.transform.position;
        transform.position -= cameraSpeed * _time;
    }
}