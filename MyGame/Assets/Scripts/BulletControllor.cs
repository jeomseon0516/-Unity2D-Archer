using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControllor : MonoBehaviour
{
    // �Ѿ��� ����
    public Vector3 Direction { get; set; }
    // ��������Ʈ ������� �޾ƿ���
    public SpriteRenderer _sprRen;
    public GameObject     _fxPrefab;
    // �Ѿ��� ���ư��� �ӵ�
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

    // �浹ü�� ���������� ���Ե� ������Ʈ�� �ٸ� �浹ü�� �浹�Ѵٸ� ����Ǵ� �Լ���.
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