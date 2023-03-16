using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BulletController : ObjectBase
{
    private GameObject _fxPrefab;
    private float _time;

    protected override void Init()
    {
        _fxPrefab = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "Smoke");
        _speed = 10.0f;
        _time = 10.0f;
        _hp = 3;
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_time);
        _hp = 0;
        CheckDeadToHp();
    }

    protected override void ObjUpdate()
    {
        base.ObjUpdate();
        Fire();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        --_hp;
        EffectAfterDestroy(other.gameObject);
        ActionCamera(new GameObject("Camera Test"));

        if (other.gameObject.transform.tag != "wall")
            Destroy(other.gameObject.transform.gameObject);

        CheckDeadToHp();
    }

    private void EffectAfterDestroy(GameObject obj)
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
        EffectAfterDestroy(gameObject);
    }

    private void ActionCamera(GameObject camera) { camera.AddComponent<VibratingCamera>(); }
    private void Fire() { transform.position += _direction * _speed * Time.deltaTime; }
    public void SetFlipY(bool flipY) { _sprRen.flipY = flipY; }
    public void SetDirection(Vector3 dir) { _direction = dir; }
}