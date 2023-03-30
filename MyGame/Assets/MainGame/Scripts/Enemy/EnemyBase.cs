using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public abstract partial class EnemyBase : LivingObject
    {
        protected GameObject _attackBox;
        protected ObjectBase _target;
        protected float _searchDis;
        protected float _attackDis;
        protected bool _isHunting;
        protected override void Init()
        {
            _target = GameObject.Find("Player").transform.Find("Body").Find("Image").GetComponent<ObjectBase>();

            _attackBox = _body.Find("AttackBox").gameObject;
            _attackBox.gameObject.AddComponent<AttackBox>().SetObjectBase(this);

            _isHunting = false;
            _searchDis = 8.0f;
            _attackDis = 1.5f;
            _hp = 5;
        }
        // 플레이어에게 피격 후 추적 중일때 추적 쿨타임
        protected IEnumerator TargetingObject()
        {
            _isHunting = true;
            yield return new WaitForSeconds(10.0f);
            _isHunting = false;
        }
        public void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _attackBox.SetActive(on);
        }
        protected internal override void TriggerAction(Collider2D col) 
        {
            if (!TriggerCollision(col.transform.parent, col.transform.Find("Image").GetComponent<ObjectBase>())) return;

            StartCoroutine(TargetingObject()); // 플레이어에게 공격받으면 10초동안은 거리에 상관없이 무조건 플레이어를 쫒아다닌다.
        }
    }
}