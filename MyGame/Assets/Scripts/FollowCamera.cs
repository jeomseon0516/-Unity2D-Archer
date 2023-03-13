using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    static class Constants
    {
        public const float PI    = 3.141592653f;
        public const float M_DEG = 0.318309886f;
    }

    private Camera _camera;
    private GameObject _player;
    private float _offsetY;
    private float _maxSpeed;

    /*
     * 라디안(호도법) 값 구하기 PI / 180;
     * 라디안(호도법) 값 = 육십분법 * Constants.PI / 180.0f
     * 육십분법(도) 값 = 라디안 값 * 180 / 디그리(라디안을 도법으로 바꿀때) 값
     */

    private void Start()
    {
        _camera = Camera.main;
        _player = GameObject.Find("Player").gameObject;
        _offsetY = _camera.transform.position.y;
        _maxSpeed = 2.0f;
    }

    private void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        Vector3 pos = new Vector3(this.transform.position.x, this.transform.position.y - _offsetY, 10.0f);

        float distance  = GetDistance(_player.transform.position, pos);
        float angle     = GetAngleToPosition(_player.transform.position, pos);
        float direction = GetDirectionToAngle(angle);

        float x =  Mathf.Cos(direction) * distance * _maxSpeed;
        float y = -Mathf.Sin(direction) * distance * _maxSpeed;

        _camera.transform.position += new Vector3(x, y, 0.0f) * Time.deltaTime;
    }
    //
    // 거리를 구한다.
    private float GetDistance(Vector3 p1, Vector3 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }

    // 상대각으로 라디안 값 구한후 디그리 값으로 변경
    private float GetAngleToPosition(Vector3 p1, Vector3 p2) { return -Mathf.Atan2(p1.y - p2.y, p1.x - p2.x) * 180.0f * Constants.M_DEG; }
    private float GetDirectionToAngle(float angle) { return angle * Constants.PI / 180.0f; }
}