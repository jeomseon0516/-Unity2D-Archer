using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ?�쏄?�瑗???筌욊??졿묾?
/*
 * ??????곷선?�귣?????롫퓠 ???�뮉 ?�쏄?�瑗?? ??��?????�쏅????��????�젾??곷튊 ??몃빍?? 0 ?�귣??????????
 * ??????곷선?�귣??????�퓠 ???�뮉 ?�쏄?�瑗?? ???????�쏅????��????�젾??곷튊 ??몃빍?? 0 ~ -1 -1???�쎛?繹먮???筌욌??붹에?????�筌?�쵎???
 */

public class BackGroundsController : MonoBehaviour
{
    // ??�얠�??�좊�???�쏆빘猿?�첎? ????�?��?븍샨?????�뮉筌왖? ??????뺣뼄.
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
        Vector3 cameraSpeed = new Vector3(
            _camera.transform.position.x - _refPos.x ,
            _camera.transform.position.y - _refPos.y,
            0.0f);
 
        // ?�삳?�??깆벥 ???????�즲?�쎛? ???? ?�? ???袁⑹�???�뗫�??�쏅??졿묾????????�쑴�??? ???�뮉??
        transform.position -= cameraSpeed * _time;
        _refPos = _camera.transform.position;
    }

    private LOCATION GetDirToCameraPos(Vector3 p1, Vector3 p2) { return p1.x >= p2.x ? LOCATION.LEFT : LOCATION.RIGHT; }
}