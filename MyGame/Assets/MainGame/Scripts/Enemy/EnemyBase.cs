using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PUB_SUB;

namespace OBJECT
{
    public abstract partial class EnemyBase : LivingObject
    {
        protected GameObject _attackBox;
        protected EnemyStatsPublisher _statsPublisher;
        protected ObjectBase _target;
        protected float _searchDis;
        protected float _attackDis;
        protected bool _isHunting;

        protected delegate void SetSkill(bool isUse);
        protected override void Init()
        {
            base.Init();
            
            CheckInComponent(PlayerManager.GetInstance().GetInGamePlayer().TryGetComponent(out _target));

            _attackBox = _body.Find("AttackBox").gameObject;
            _attackBox.gameObject.AddComponent<AttackBox>().SetObjectBase(this);

            _isHunting = false;
            _searchDis = 8.0f;
            _attackDis = 1.5f;
            _hp = _maxHp = 5;
        }
        private void Start()
        {
            CheckInComponent(_body.Find("HealthCanvas").Find("HpBar").TryGetComponent(out IEnemyStatsSubscriber bar));

            _statsPublisher = new EnemyStatsPublisher();
            _statsPublisher.RegisterSubscriber(bar);
            _statsPublisher.UpdateMaxHp(_maxHp);
        }
        // 플레이어에게 피격 후 추적 중일때 추적 쿨타임
        protected IEnumerator TargetingObject()
        {
            _isHunting = true;
            yield return YieldCache.WaitForSeconds(10.0f);
            _isHunting = false;
        }
        public override void OnAttackBox(float isOn)
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
            return Mathf.Abs(movePoint.x - myPosition.x) <= t._attackDis &&
                   Mathf.Abs(movePoint.y - myPosition.y) <= t._attackDis * yDis;
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
        protected void RunStateMethod(ref float coolTime, ref bool isMove, ref Vector3 randPoint)
        {
            if (coolTime > 0.0f) // 쿨타임일땐 처리하지 않음
            {
                coolTime -= Time.deltaTime;
                return;
            }
            if (!isMove) // 쿨타임이 끝났고 현재 랜덤한 위치로 이동중이지 않으면?
            {
                randPoint = RandomMovePosition();
                isMove = true;
                return;
            }

            _direction = (randPoint - _physics.position).normalized;

            if (Default.GetDistance(randPoint, _physics.position) > 1.0f)
                return; // 목표 위치로 이동이 끝났다면

            _direction = Vector2.zero;
            coolTime = Random.Range(0.0f, 3.0f);
            isMove = false;
        }
        protected void SetAttackState()
        {
            _animator.SetTrigger("Attack");
            _direction = Vector2.zero;
            _lookAt = (_target.transform.position - _physics.position).normalized;
        }
        protected void SetLookAtAndDirection(Vector2 movePoint, Vector2 targetPos, Vector2 myPos)
        {
            _direction = (movePoint - myPos).normalized;
            _lookAt    = (targetPos - myPos).normalized;
        }
        protected override void ObjUpdate() { _statsPublisher.UpdateHpAndAngle(_hp, _physics.rotation.eulerAngles.y); }
        protected override void GetDamageAction(int damage) { AddAfterResetCoroutine("Targeting", TargetingObject()); }
    }

    public class EnemyStatsPublisher
    {
        List<IEnemyStatsSubscriber> _statsSubscribers = new List<IEnemyStatsSubscriber>();
        int _hp, _maxHp;
        float _angle;

        public void UpdateHp(int hp)
        {
            _hp = hp;

            for (int i = 0; i < _statsSubscribers.Count; ++i)
                _statsSubscribers[i].OnUpdateHp(_hp);
        }
        public void UpdateMaxHp(int maxHp)
        {
            _maxHp = maxHp;

            for (int i = 0; i < _statsSubscribers.Count; ++i)
                _statsSubscribers[i].OnUpdateMaxHp(_maxHp);
        }
        public void UpdateHpAndAngle(int hp, float angle)
        {
            _hp = hp;
            _angle = angle;

            for (int i = 0; i < _statsSubscribers.Count; ++i)
            {
                _statsSubscribers[i].OnUpdateHp(_hp);
                _statsSubscribers[i].OnUpdateAngle(_angle);
            }
        }
        public void RegisterSubscriber(IEnemyStatsSubscriber statsSubscriber) 
        {
            if (_statsSubscribers.Contains(statsSubscriber)) return;
            _statsSubscribers.Add(statsSubscriber);
        }
        public void RemoveSubscriber(IEnemyStatsSubscriber statsSubscriber) 
        {
            if (!_statsSubscribers.Contains(statsSubscriber)) return;
            _statsSubscribers.Remove(statsSubscriber);
        }
    }

    public interface IAngleSubscriber
    {
        public void OnUpdateAngle(float angle);
    }
    public interface IEnemyStatsSubscriber : IHpSubscriber, IAngleSubscriber {}
}