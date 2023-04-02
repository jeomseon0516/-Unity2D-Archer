using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public class BulletController : ObjectBase
    {
        private GameObject _smoke;
        private GameObject _hitEffect;
        private float _time;
        private bool _makeSmoke;
        protected override void Init()
        {
            _smoke = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "Smoke");
            _hitEffect = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "HitEffect");
            _speed = 15.0f;
            _atk = 2;
            _time = 10.0f;
            _hp = 3;
            _makeSmoke = true;
        }
        private IEnumerator Start()
        {
            yield return YieldCache.WaitForSeconds(_time);
            _hp = 0;
        }
        protected internal override void TriggerAction(Collider2D col)
        {
            if (LayerMask.LayerToName(col.gameObject.layer).Contains("Wall"))
            {
                _hp = 0;
                _makeSmoke = false;
                return;
            }

            CheckInComponent(col.transform.parent.Find("Image").TryGetComponent(out ObjectBase obj));
            if (TriggerCollision(obj.GetPhysics(), obj))
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
        protected override void Die()
        {
            Destroy(_shadowSprRen);
            Destroy(_colTransform.gameObject);
            CreateEffect(transform.position, _smoke);
            _animator.SetTrigger("Die");
        }
        protected override void ObjFixedUpdate()
        {
            if (_isDie) return;

            UpdateShadowAndCollider();
            _lookAt = Vector3.zero;
            _shadow.transform.eulerAngles = transform.eulerAngles;
            _heightOffset = (_bodyCollider.bounds.max.y - _bodyCollider.bounds.min.y) * _body.lossyScale.y;
        }
        private void CreateEffect(Vector2 pos, GameObject effect) { if (_makeSmoke) Instantiate(effect).transform.position = pos; }
        private void ActionCamera(GameObject camera) { camera.AddComponent<VibratingCamera>(); } // 카메라 매니저를 만들어주는게 좋지않을까?
        public void SetDirection(Vector2 dir) { _direction = dir; }
        public void SetAtk(int atk) { _atk = atk; }
    }
}