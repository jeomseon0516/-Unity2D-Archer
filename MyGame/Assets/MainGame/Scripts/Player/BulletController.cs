using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT;
public class BulletController : ObjectBase
{
    private GameObject _smoke;
    private GameObject _hitEffect;
    private float      _time;
    protected override void Init()
    {
        _smoke     = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "Smoke");
        _hitEffect = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "HitEffect");
        _speed     = 10.0f;
        _atk       = 2;
        _time      = 10.0f;
        _hp        = 3;
    }
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(_time);
        _hp = 0;
    }
    protected internal override void TriggerAction(Collider2D col)
    {
        ObjectBase obj = col.transform.Find("Image").GetComponent<ObjectBase>();

        if (TriggerCollision(col.transform.parent, obj))
        {
            --_hp;
            obj.TakeDamage(_atk);
            CreateEffect(new Vector2(col.transform.position.x, transform.position.y), _hitEffect);
            ActionCamera(Camera.main.gameObject);
        }
    }
    private void EffectAfterDestroy(GameObject obj, GameObject effect)
    {
        CreateEffect(obj.transform.position, effect);
        base.Die();
    }
    private void CreateEffect(Vector2 pos, GameObject effect) { Instantiate(effect).transform.position = pos; }
    protected override void Die() { EffectAfterDestroy(gameObject, _smoke); }
    private void ActionCamera(GameObject camera) { camera.AddComponent<VibratingCamera>(); } // 카메라 매니저를 만들어주는게 좋지않을까?
    public void SetDirection(Vector2 dir) { _direction = dir; }
    public void SetAtk(int atk) { _atk = atk; }
}