using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public enum PATTERN
    {
        ALLDIRECTION,
        TARGETING
    }

    public class Snowball : BulletBase
    {
        private float _nextJump;
        protected override void Init()
        {
            base.Init();
            _hp = 1;
            _jump = 0;
            _nextJump = 15.0f;
            _atk = 5;
            _direction = new Vector2(0.5f, 0.5f);
            _speed = 5.0f;
            StartCoroutine(CheckFallingOrJumping());
        }
        protected internal override void TriggerAction(Collider2D col)
        {
            if (LayerMask.LayerToName(col.gameObject.layer).Contains("Wall")) return;

            TriggerCollision(col, _colTransform.gameObject);
        }
        protected override void OnCollision(ObjectBase obj, Collider2D col)
        {
            DestroyObj();
            Vector2 force = Default.GetFromPostionToDirection(obj.GetPhysics().position, _physics.position);
            obj.TakeDamage(_atk, force * 2);
        }
        protected override void Die()
        {
            StartCoroutine(FadeOutObject());
            Destroy(_colTransform.gameObject);
        }
        protected override void BulletPattern()
        {
            base.BulletPattern();

            if (_body.localPosition.y < float.Epsilon)
            {
                _jump = _nextJump;
                AddAfterResetCoroutine("Jump", Jumping());
                _nextJump *= 0.7f;
                _speed    *= 0.8f;
            }

            if (_speed < 0.0001f)
                _hp = 0;
        }
        protected override void BulletInit() {}
        public void SetNextJump(float nextJump) { _nextJump = nextJump; }
        public void SetSpeed(float speed) { _speed = speed; }
    }
}
