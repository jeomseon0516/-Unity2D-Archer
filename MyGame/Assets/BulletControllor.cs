using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllor : MonoBehaviour
{
    // ��������Ʈ ������� �޾ƿ���
    public SpriteRenderer _sprRen;
    // �Ѿ��� ����
    public Vector3 _direction { get; set; }
    // �Ѿ��� ���ư��� �ӵ�
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

    // �浹ü�� ���������� ���Ե� ������Ʈ�� �ٸ� �浹ü�� �浹�Ѵٸ� ����Ǵ� �Լ���.
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
