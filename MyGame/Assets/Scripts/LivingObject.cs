using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LivingObject : ObjectBase
{
    protected GameObject _bullet;
    protected bool       _onAttack;
    protected bool       _onHit;

    protected virtual void CreateBullet() {}
    protected override void Init()
    {
        _bullet     = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet");
        _onAttack   = false;
        _onHit      = false;
    }
    protected virtual void OnHit()
    {
        if (_onHit) return;

        _onHit = true;
        _animator.SetTrigger("Hit");
    }
    protected virtual void OnAttack()
    {
        if (_onAttack) return;

        _onAttack = true;
        _animator.SetTrigger("Attack");
    }
    protected override void Run()
    {
        _animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(_direction.x), Mathf.Abs(_direction.y)));
    }
    protected void SetHit() { _onHit = false; }
}
