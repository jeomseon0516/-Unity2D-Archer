using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��� �����̱�
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
        // ī�޶��� �̵��ӵ�
        Vector3 cameraSpeed = new Vector3(
            _camera.transform.position.x - _refPos.x,
            _camera.transform.position.y - _refPos.y,
            0.0f);

        // ī�޶��� �̵��ӵ��� �̹� ��Ÿ Ÿ���� ���� ���̱� ������ ������� �ʴ´�.
        _refPos = _camera.transform.position;
        transform.position -= cameraSpeed * _time;
    }
}