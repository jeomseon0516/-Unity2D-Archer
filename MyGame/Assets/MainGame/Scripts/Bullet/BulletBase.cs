using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public abstract class BulletBase : ObjectBase
    {
        protected float _time;

        protected override void Init()
        {
            _speed = 15.0f;
            _time = 10.0f;
            _atk = 2;
            _hp = 3;
        }
        private IEnumerator Start()
        {
            BulletInit();

            yield return YieldCache.WaitForSeconds(_time);
            _hp = 0;
        }
        protected override void ObjFixedUpdate() { BulletPattern(); }
        protected virtual void BulletPattern() { _lookAt = Vector2.zero; }
        protected abstract void BulletInit();
        public void SetDirection(Vector2 dir) { _direction = dir; }
    }
}
