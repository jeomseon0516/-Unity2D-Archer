using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public class DefaultBullet : BulletBase
    {
        private GameObject _smoke;
        private GameObject _hitEffect;
        private bool _makeSmoke;
        protected override void Init()
        {
            base.Init();
            _smoke     = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "Smoke");
            _hitEffect = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.FX, "HitEffect");
            _makeSmoke = true;
        }
        protected override void BulletInit()
        {
            Quaternion rotation = transform.rotation;
            float radian = Default.GetPositionToRadian(_direction, Vector2.zero);
            float angle  = Default.ConvertFromRadianToAngle(radian);

            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.x, angle);
            transform.rotation = rotation;

            UpdateCollider();
            _shadow.transform.eulerAngles = transform.eulerAngles;
            _heightOffset = (_bodyCollider.bounds.max.y - _bodyCollider.bounds.min.y) * 2;
        }
        protected internal override void TriggerAction(Collider2D col)
        {
            if (LayerMask.LayerToName(col.gameObject.layer).Contains("Wall"))
            {
                _hp = 0;
                _makeSmoke = false;
                return;
            }

            /*TODO : 모듈화*/
            TriggerCollision(col, _colTransform.gameObject);
        }
        protected override void OnCollision(ObjectBase obj, Collider2D col)
        {
            --_hp;
            base.OnCollision(obj, col);
            CreateEffect(new Vector2(col.transform.position.x, transform.position.y), _hitEffect);
            ActionCamera(Camera.main.gameObject);
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
        private void CreateEffect(Vector2 pos, GameObject effect) { if (_makeSmoke) Instantiate(effect).transform.position = pos; }
        private void ActionCamera(GameObject camera) { camera.AddComponent<VibratingCamera>(); } // 카메라 매니저를 만들어주는게 좋지않을까?
        public void SetAtk(int atk) { _atk = atk; }
    }
}