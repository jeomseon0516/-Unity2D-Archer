using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public abstract class LivingObject : ObjectBase
    {
        protected GameObject _bullet;
        protected float _maxHp;

        protected virtual void CreateBullet() { }
        protected override void Init() { _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet"); }
        protected override void Run()  { _animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(_direction.x), Mathf.Abs(_direction.y))); }
    }
}
