using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 배경 움직이기
public class BackGroundsControllor : MonoBehaviour
{
    public enum LOCATION
    {
        LEFT  = -1,
        RIGHT =  1
    }

    public float _time;
    public LOCATION _loc;

    private Sprite _sprite;
    private Camera _camera;
    private Vector3 _refPos;
    private GameObject _cloneImage;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>().sprite;
    }
    void Start()
    {
        // 이름의 길이가 1보다 크면 
        // _cloneImage = GameObject.Find(this.gameObject.name + "Clone").gameObject;
        _cloneImage = (this.gameObject.name.Length > 1) ?
            GameObject.Find(this.gameObject.name.Substring(0, 1)).gameObject :
            GameObject.Find(this.gameObject.name + "Clone").gameObject;

        //print(this.gameObject.name.Substring(0, 1));

        _camera = Camera.main;
        _refPos = _camera.transform.position;
    }

    void Update()
    {
        BackGroundCalc();
        BackGroundUpdate();
    }

    private void BackGroundUpdate()
    {
        float cameraX = (_camera.transform.position.x + (_sprite.bounds.size.x * 0.5f) *  (int)_loc);
        float imageX  = (_sprite.bounds.size.x * 0.5f) * (int)_loc + transform.position.x;

        float maxX = _loc == LOCATION.LEFT ? imageX  : cameraX;
        float minX = _loc == LOCATION.LEFT ? cameraX : imageX;

        if (maxX > minX)
        {
            _cloneImage.transform.position = new Vector3(
                    transform.position.x + _sprite.bounds.size.x * (int)_loc,
                    transform.position.y,
                    transform.position.z);
            _loc = (LOCATION)((int)_loc * -1);
        }
    }

    private void BackGroundCalc()
    {
        // 카메라의 프레임당 이동속도
        Vector3 cameraSpeed = new Vector3(
            _camera.transform.position.x - _refPos.x,
            _camera.transform.position.y - _refPos.y,
            0.0f);

        // 카메라의 이동속도가 이미 델타 타임을 구한 값이기 때문에 계산하지 않는다.
        _refPos = _camera.transform.position;
        transform.position -= cameraSpeed * _time;
    }
}