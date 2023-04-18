using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PUB_SUB;

namespace OBJECT
{
    public abstract partial class EnemyBase<T> : LivingObject where T : EnemyBase<T>
    {
        protected GameObject _attackBox;
        protected EnemyStatsPublisher _statsPublisher;

        protected ObjectBase _target;
        protected float _searchDis;
        protected float _attackDis;
        protected float _attackYDis;

        protected bool _isHunting;

        protected StateMachine<T> _state;

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
            _attackYDis = 0.085f;

            _hp = _maxHp = 5;

            _state = new StateMachine<T>();
            _state.SetState(new IdleState());
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
        protected bool CheckAttack(EnemyBase<T> t, Vector2 movePoint, Vector2 myPosition, float yDis = 0.085f)
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
        public override void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _attackBox.SetActive(on);

            // 어택 박스가 꺼지면 리스트의 요소들을 모두 클리어한다.
            if (!on)
                ClearColList();
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
        protected Vector2 GetFromTargetPosToMovePoint(Vector2 myPos, Vector2 targetPos)
        {
            int xDir = myPos.x - targetPos.x > 0 ? 1 : -1;
            return new Vector2(targetPos.x + 1.5f * xDir, targetPos.y);
        }
        // TODO : 수정할 것
        protected void SkillWaitMethod(Vector2 destination, float yTemp)
        {
            GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);

            int xDir = myPos.x - targetPos.x > 0 ? 1 : -1; // 보정해야 할 방향이 어느쪽인가?
            Vector2 movePoint = new Vector2(targetPos.x + Random.Range(4.0f, 6.0f) * xDir, yTemp);

            if (Default.GetDistance(destination, myPos) <= 1.0f ||
                Default.GetDistance(targetPos, myPos) >= 20.0f)
            {
                SetSkillState();
                return;
            }

            if (CheckAttack(this, new Vector2(targetPos.x + 1.5f * xDir, targetPos.y), myPos, _attackYDis))
            {
                _state.SetState(new AttackState());
                return;
            }

            SetLookAtAndDirection(destination, targetPos, myPos);
        }
        protected bool CheckTarget()
        {
            return _target.GetIsDie() ? false : _isHunting || Default.GetDistance(_target.transform.position, transform.position) <= _searchDis;
        }
        // 타겟팅 상태일때 상속받는 객체 별로 할 수 있는 행동이 다르기 때문에
        protected override void ObjUpdate() { _statsPublisher.UpdateHpAndAngle(_hp, _physics.rotation.eulerAngles.y); }
        protected override void GetDamageAction(int damage) { AddAfterResetCoroutine("Targeting", TargetingObject()); }
        protected abstract bool TargetingMethod();
        protected abstract void SetSkillState();
    }
    // 적들이 가져야할 기본적인 상태는 제네릭을 사용해서 EnemyBase클래스에서 구현한다. 이렇게 사용하면 EnemyBase를 상속받는 클래스마다 Idle, Targeting, Attack를 따로 구현할 필요가 없어진다.
    public abstract partial class EnemyBase<T> : LivingObject where T : EnemyBase<T>
    {
        public sealed partial class IdleState : State<T>
        {
            Vector3 _randPoint;
            float _coolTime;
            bool _isMove;
            public override void Enter(T t)
            {
                base.Enter(t);

                _coolTime = Random.Range(0.0f, 3.0f);
                _isMove = false;
            }
            public override void Update(T t)
            {
                if (t.CheckTarget()) // 타겟이 범위 안에 들어오면 타겟팅 패턴으로 전환
                {
                    t._state.SetState(new TargetingState());
                    return;
                }

                t.RunStateMethod(ref _coolTime, ref _isMove, ref _randPoint);
            }
        }
        public sealed class TargetingState : State<T>
        {
            public override void Update(T t)
            {

                if (!t.CheckTarget()) // 플레이어가 범위 밖으로 벗어났다면 다시 Idle패턴으로 전환
                {
                    t._state.SetState(new IdleState());
                    return;
                }

                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);

                Vector2 movePoint = t.GetFromTargetPosToMovePoint(myPos, targetPos);

                if (t.TargetingMethod())
                    return;

                if (t.CheckAttack(t, movePoint, myPos, t._attackYDis))
                {
                    t._state.SetState(new AttackState());
                    return;
                }

                t.SetLookAtAndDirection(movePoint, targetPos, myPos);
            }
        }
        public sealed class AttackState : State<T>
        {
            public override void Enter(T t)
            {
                base.Enter(t);
                t.SetAttackState();
            }
        }
        public sealed class DieState : State<T>
        {
            public override void Enter(T t)
            {
                t._animator.SetTrigger("Die");
                t._direction = Vector2.zero;
            }
            public DieState() { }
        }
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
    public interface IAngleSubscriber { public void OnUpdateAngle(float angle); }
    public interface IEnemyStatsSubscriber : IHpSubscriber, IAngleSubscriber {}
}