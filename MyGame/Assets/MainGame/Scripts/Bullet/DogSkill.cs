using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public class DogSkill : BulletBase
    {
        Vector2 _keepLookAt;
        protected override void BulletInit()
        {
            _speed = 0;
            _hp = 10;
            _atk = 10;
            _keepLookAt = _direction;
            AddAfterResetCoroutine("DogAttack", DogAttack()); // 코루틴으로 등록해두고..
        }
        private IEnumerator DogAttack()
        {
            while (true)
            {
                AddForce(_direction);
                yield return YieldCache.WaitForFixedUpdate;
            }
        }
        private void BreakDog() { _direction *= -0.75f; }
        protected override void BulletPattern() { _lookAt = _keepLookAt; }
    }
}
