using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllor : MonoBehaviour
{
    // 스프라이트 구성요소 받아오기
    public SpriteRenderer _sprRen;
    // 총알의 방향
    public Vector3 _direction { get; set; }
    // 총알이 날아가는 속도
    private float  _speed;

    private void Awake()
    {
        _sprRen = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        _speed = 10.0f;
    }

    void Update()
    {
        Fire();
    }

    // 충돌체와 물리엔진이 포함된 오브젝트가 다른 충돌체와 충돌한다면 실행되는 함수다.
    private void OnTriggerEnter2D(Collider2D other)
    {
        DestroyObject(this.gameObject);
    }

    public void SetFlipY(bool flipY)
    {
        GetComponent<SpriteRenderer>().flipY = flipY;
    }

    private void Fire()
    {
        transform.position += _direction * _speed * Time.deltaTime;
    }
}
