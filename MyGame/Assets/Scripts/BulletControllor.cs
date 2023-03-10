using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllor : MonoBehaviour
{
    // 총알의 방향
    public Vector3 Direction { get; set; }
    // 스프라이트 구성요소 받아오기
    public SpriteRenderer _sprRen;
    public GameObject     _fxPrefab;
    // 총알이 날아가는 속도
    private float  _speed;
    private int _hp;

    private void Awake()
    {
        _sprRen = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _speed = 10.0f;
        _hp = 3;
    }

    void Update()
    {
        Fire();
    }

    // 충돌체와 물리엔진이 포함된 오브젝트가 다른 충돌체와 충돌한다면 실행되는 함수다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        --_hp;
        GameObject colObj = other.gameObject;
        Destroy(colObj);
        CreateEffect(colObj.transform.position);

        GameObject camera = new GameObject("Camera Test");
        camera.AddComponent<VibratingCamera>();

        CheckDeadToHp();
    }

    private void CreateEffect(Vector3 pos)
    {
        GameObject obj = Instantiate(_fxPrefab);
        obj.transform.position = pos;
    }

    private void CheckDeadToHp()
    {
        if (_hp > 0) return;

        Destroy(this.gameObject);
    }

    private void Fire() { transform.position += Direction * _speed * Time.deltaTime; }
    public void SetFlipY(bool flipY) { _sprRen.flipY = flipY; }
}