using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public Vector3 Direction { get; set; }

    private GameObject _fxPrefab;
    private SpriteRenderer _sprRen;
    private float _speed;
    private int _hp;
    private void Awake()
    {
        _sprRen = GetComponent<SpriteRenderer>();
        _fxPrefab = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "Smoke");
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        --_hp;
        CollisionWall(other.gameObject);
        ActionCamera(new GameObject("Camera Test"));

        if (other.gameObject.transform.tag != "wall")
            Destroy(other.gameObject.transform.gameObject);

        CheckDeadToHp();
    }
    private void CollisionWall(GameObject obj)
    {
        CreateEffect(obj.transform.position);
        Destroy(obj);
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
    private void ActionCamera(GameObject camera) { camera.AddComponent<VibratingCamera>(); }
    private void Fire() { transform.position += Direction * _speed * Time.deltaTime; }
    public void SetFlipY(bool flipY) { _sprRen.flipY = flipY; }
}