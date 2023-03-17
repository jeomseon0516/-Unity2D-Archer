using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingObject : ObjectBase
{
    protected GameObject _bullet;
    protected bool       _onAttack;
    protected bool       _onHit;

    protected virtual void CreateBullet() {}
    protected override void Init()
    {
        _bullet   = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet");
        _onAttack = false;
        _onHit    = false;
    }
    protected void OnHit()
    {
        if (_onHit) return;

        _onHit = true;
        _animator.SetTrigger("Hit");
    }
    protected void OnAttack()
    {
        if (_onAttack) return;

        _onAttack = true;
        _animator.SetTrigger("Attack");
    }
    protected void SetAttack() { _onAttack = false; }
}
