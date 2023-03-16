using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingObject : ObjectBase
{
    protected GameObject _bullet;
    protected bool       _onAttack;
    protected bool       _onHit;

    protected virtual void CreateBullet() {}
    protected override void Awake()
    {
        base.Awake();
        _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet");
    }
    protected override void ObjUpdate()
    {
        base.ObjUpdate();
    }

    protected override void Init()
    {
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
    protected void ChangeFlipXToHor(float hor)
    {
        if      (hor < 0) _sprRen.flipX = true;
        else if (hor > 0) _sprRen.flipX = false;
    }
    protected void Move(float moveX, float moveY)
    {
        transform.position += new Vector3(moveX * _speed, moveY * (_speed * 0.5f), 0.0f) * Time.deltaTime;
    }
    protected void SetAttack() { _onAttack = false; }
}
