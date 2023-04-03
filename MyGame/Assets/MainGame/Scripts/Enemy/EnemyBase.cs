using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public abstract class EnemyBase : LivingObject
    {
        protected GameObject _attackBox;
        protected ObjectBase _target;
        protected float _searchDis;
        protected float _attackDis;
        protected bool _isHunting;

        protected delegate void SetSkill(bool isUse);
        protected override void Init()
        {
            CheckInComponent(GameObject.Find("Player").transform.Find("Body").Find("Image").TryGetComponent(out _target));

            _attackBox = _body.Find("AttackBox").gameObject;
            _attackBox.gameObject.AddComponent<AttackBox>().SetObjectBase(this);

            _isHunting = false;
            _searchDis = 8.0f;
            _attackDis = 1.5f;
            _hp = _maxHp = 5;
        }
        // 플레이어에게 피격 후 추적 중일때 추적 쿨타임
        protected IEnumerator TargetingObject()
        {
            _isHunting = true;
            yield return YieldCache.WaitForSeconds(10.0f);
            _isHunting = false;
        }
        public void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _attackBox.SetActive(on);
        }
        protected IEnumerator CoolTime(SetSkill skill, float time)
        {
            skill(false);
            yield return YieldCache.WaitForSeconds(time);
            skill(true);
        }
        protected Vector2 RandomMovePosition()
        {
            int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

            Vector3 offset = new Vector2(Random.Range(0, 5), Random.Range(0.0f, 1.5f));

            return new Vector2(_rigidbody.position.x + offset.x * xDir, 0.0f + offset.y * yDir);
        }
        protected internal override void TriggerAction(Collider2D col) 
        {
            if (LayerMask.LayerToName(col.gameObject.layer).Equals("Player")) return;
            CheckInComponent(col.transform.parent.Find("Image").TryGetComponent(out ObjectBase obj));

            if (TriggerCollision(obj.GetPhysics(), obj))
                AddAfterResetCoroutine("Targeting", TargetingObject()); // 플레이어에게 공격받으면 10초동안은 거리에 상관없이 무조건 플레이어를 쫒아다닌다.
        }
    }
}