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
     * ??곕탵???紐껊즲甕? ????�뗫�?��?PI / 180;
     * ??곕탵???紐껊즲甕? ??= ???�뼏?브쑬??* Constants.PI / 180.0f
     * ???�뼏?브쑬???? ??= ??곕탵????* 180 / ??�쎈?????곕탵???�뱽 ?袁⑥???곗쨮 ?�쏅???? ??
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
    // 椰꾧??�?��???�뗫�??
    private float GetDistance(Vector3 p1, Vector3 p2)
    {
        float x = p1.x - p2.x;
        float y = p1.y - p2.y;

        return Mathf.Sqrt(x * x + y * y);
    }

    // ????�쏄????��???곕탵??????�뗫�????�쎈????�쏅????��??�궰???
    private float GetAngleToPosition(Vector3 p1, Vector3 p2) { return -Mathf.Atan2(p1.y - p2.y, p1.x - p2.x) * 180.0f * Constants.M_DEG; }
    private float GetDirectionToAngle(float angle) { return angle * Constants.PI / 180.0f; }
}