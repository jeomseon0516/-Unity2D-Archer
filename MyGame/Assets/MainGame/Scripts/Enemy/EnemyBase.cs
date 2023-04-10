using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBSERVER;

namespace OBJECT
{
    public abstract partial class EnemyBase : LivingObject, ISubject
    {
        protected GameObject _attackBox;
        protected ObjectBase _target;
        protected float _searchDis;
        protected float _attackDis;
        protected bool _isHunting;

        protected delegate void SetSkill(bool isUse);
        protected override void Init()
        {
            base.Init();

            CheckInComponent(PlayerManager.GetInstance().GetInGamePlayer().TryGetComponent(out _target));
            CheckInComponent(_body.Find("HealthCanvas").Find("HpBar").TryGetComponent(out IEnemyObserver bar));
            RegisterObserver(bar);

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

            // 어택 박스가 꺼지면 리스트의 요소들을 모두 클리어한다.
            if (!on)
                ClearColList();
        }
        // 사용하지 않음
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
        protected void GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos)
        {
            myPos = new Vector2(_physics.position.x, _physics.position.y - _offsetY);
            Transform targetTransform = _target.GetPhysics();
            targetPos = new Vector2(targetTransform.position.x, targetTransform.position.y - _target.GetOffsetY());
        }
        protected bool CheckAttack(EnemyBase t, int xDir, Vector2 movePoint, Vector2 myPosition, float yDis = 0.085f)
        {
            if (Mathf.Abs(movePoint.x - myPosition.x) <= t._attackDis &&
                Mathf.Abs(movePoint.y - myPosition.y) <= t._attackDis * yDis)
                return true;

            return false;
        }
        protected internal override void TriggerAction(Collider2D col) 
        {
            if (LayerMask.LayerToName(col.gameObject.layer).Equals("Player") ||
                LayerMask.LayerToName(col.gameObject.layer).Equals("Bullet")) return;

            TriggerCollision(col, _colTransform.gameObject);
        }
        protected override void OnCollision(ObjectBase obj, Collider2D col) 
        {
            Vector2 force = Default.GetFromPostionToDirection(obj.GetPhysics().position, _physics.position);
            obj.TakeDamage(_atk, force * 2);
        }
        protected override void ObjUpdate() { NotifyObservers(); }
        protected override void GetDamageAction(int damage) { AddAfterResetCoroutine("Targeting", TargetingObject()); }
    }

    public abstract partial class EnemyBase : LivingObject, ISubject
    {
        List<IEnemyObserver> _observers = new List<IEnemyObserver>();

        public void NotifyObservers()
        {
            for (int i = 0; i < _observers.Count; ++i) { _observers[i].UpdateData(this); }
        }
        public void RegisterObserver(IEnemyObserver observer) 
        {
            if (_observers.Contains(observer)) return;
            _observers.Add(observer);
        }
        public void RemoveObserver(IEnemyObserver observer) 
        {
            if (!_observers.Contains(observer)) return;
            _observers.Remove(observer);
        }

        public void RegisterObserver(IObserver observer) { }
        public void RemoveObserver(IObserver observer) {}
    }
 
    public interface IEnemyObserver : IObserver
    {
        public void UpdateData(int hp, int maxHp);
        public void UpdateData(ObjectBase obj) {}
    }
}