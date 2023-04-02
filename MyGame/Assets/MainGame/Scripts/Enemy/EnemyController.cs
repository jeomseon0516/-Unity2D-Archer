using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public partial class EnemyController : EnemyBase
    {
        private StateMachine<EnemyController> _state;
        private GameObject _skill;
        private float _skillMaxDistance;
        private bool _useSkill;
        protected override void Init()
        {
            base.Init();
            _skill = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Bullet");

            _state = new StateMachine<EnemyController>();
            _state.SetState(new IdleState());

            _skillMaxDistance = 8.0f;
            _useSkill = true;
        }
        protected override void CreateBullet()
        {
            Transform tPhysicsTransform = _target.GetPhysics();

            if (Default.GetDistance(tPhysicsTransform.position, transform.position) <= _skillMaxDistance)
            {
                Transform skillTransform = Instantiate(_skill).transform;
                CheckInComponent(skillTransform.Find("Body").Find("Image").TryGetComponent(out ObjectBase skill));
                skillTransform.position = new Vector2(
                    tPhysicsTransform.position.x + 0.15f,
                    tPhysicsTransform.position.y + skill.GetOffSetY() - _target.GetOffSetY());
            }
        }
        // TODO : HitAnimation재생
        private void SetState(ENEMY_STATE state)
        {
            switch (state)
            {
                case ENEMY_STATE.IDLE:
                    _state.SetState(new IdleState());
                    break;
                case ENEMY_STATE.TARGETING:
                    _state.SetState(new TargetingState());
                    break;
                case ENEMY_STATE.HIT:
                    _state.SetState(new HitState());
                    break;
                case ENEMY_STATE.ATTACK:
                    _state.SetState(new AttackState());
                    break;
                case ENEMY_STATE.DIE:
                    _state.SetState(new DieState());
                    break;
            }
        }
        // 플레이어에게 피격 후 추적 중일때 추적 쿨타임
        protected override void Die()
        {
            _state.SetState(new DieState());
            Destroy(_bodyCollider);
        }
        protected override void ObjFixedUpdate() { _state.Update(this); }
        protected override void GetDamageAction(int damage) { _state.SetState(new HitState()); }
        private void SetUseSkill(bool useSkill) { _useSkill = useSkill; }
    }

    // TODO : 에너미 상태 패턴 구현 
    public partial class EnemyController : EnemyBase
    {
        public enum ENEMY_STATE
        {
            IDLE,
            TARGETING,
            ATTACK,
            SKILL,
            HIT,
            DIE
        }
        /* -----------------------------------------------------------Idle-------------------------------------------------- */
        public class IdleState : State<EnemyController>
        {
            Vector3 _randPoint;
            float   _coolTime;
            bool    _isMove;
            public override void Enter(EnemyController t)
            {
                _coolTime = Random.Range(0.0f, 3.0f);
                _isMove = false;
                base.Enter(t);
            }
            public override void Update(EnemyController t)
            {
                if (t._isHunting || Default.GetDistance(t._target.transform.position, t.transform.position) <= t._searchDis) // 타겟이 범위 안에 들어오면 타겟팅 패턴으로 전환
                {
                    t._state.SetState(new TargetingState());
                    return;
                }
                if (_coolTime > 0.0f) // 쿨타임일땐 처리하지 않음
                {
                    _coolTime -= Time.deltaTime;
                    return;
                }
                if (!_isMove) // 쿨타임이 끝났고 현재 랜덤한 위치로 이동중이지 않으면?
                {
                    _isMove = true;
                    _randPoint = t.RandomMovePosition();
                }
                else // 랜덤한 위치로 이동중일때
                {
                    t._direction = (_randPoint - t._physics.position).normalized;

                    if (Default.GetDistance(_randPoint, t._physics.position) <= 1.0f) // 목표 위치로 이동이 끝났다면
                    {
                        t._direction = Vector2.zero;
                        _coolTime = Random.Range(0.0f, 3.0f);
                        _isMove = false;
                    }
                }
            }
            public IdleState() {}
        }
        /* -----------------------------------------------------------Targeting-------------------------------------------------- */
        public class TargetingState : State<EnemyController>
        {
            float _yTemp;
            public override void Enter(EnemyController t)
            {
                _yTemp = Random.Range(0.0f, 1.5f) * Random.Range(0, 2) == 0 ? 1 : -1;
                base.Enter(t);
            }
            public override void Update(EnemyController t)
            {
                Vector2 myPosition = new Vector2(t._physics.position.x, t._physics.position.y - t._offsetY);
                Transform targetTransform = t._target.transform;
                Vector2 targetPosition = targetTransform.position;

                if (!t._isHunting && Default.GetDistance(targetPosition, myPosition) > t._searchDis) // 플레이어가 범위 밖으로 벗어났다면 다시 Idle패턴으로 전환
                {
                    t._state.SetState(new IdleState());
                    return;
                }

                t.CheckInComponent(targetTransform.TryGetComponent(out ObjectBase targetObj));

                int xDir = myPosition.x - targetPosition.x > 0 ? 1 : -1; // 보정해야 할 방향이 어느쪽인가?
                Vector2 movePoint = Vector2.zero;

                movePoint = HuntingAttackGetMovePoint(t, xDir, targetObj, targetPosition, myPosition);

                if (t._useSkill)
                    movePoint = SkillAndGetMovePoint(t, xDir, targetPosition, myPosition);
                
                t._direction = (movePoint - myPosition).normalized;
                t._lookAt    = (targetPosition - myPosition).normalized;
            }
            private Vector2 SkillAndGetMovePoint(EnemyController t, int xDir, Vector2 targetPosition, Vector2 myPosition)
            {
                Vector2 movePoint = new Vector2(targetPosition.x + Random.Range(4.0f, 6.0f) * xDir, _yTemp);

                if (Default.GetDistance(movePoint, myPosition) <= 1.0f)
                    t._state.SetState(new SkillState());

                return movePoint;
            }
            private Vector2 HuntingAttackGetMovePoint(EnemyController t, int xDir, ObjectBase targetObj, Vector2 targetPosition, Vector2 myPosition)
            {
                Vector2 movePoint = new Vector2(targetPosition.x + 1.5f * xDir, targetPosition.y - targetObj.GetOffSetY());

                if (Mathf.Abs(movePoint.x - myPosition.x) <= t._attackDis &&
                    Mathf.Abs(movePoint.y - myPosition.y) <= t._attackDis * 0.15f)
                    t._state.SetState(new AttackState());

                return movePoint;
            }
            public TargetingState() {}
        }
        /* -----------------------------------------------------------Attack-------------------------------------------------- */
        public class AttackState : State<EnemyController>
        {
            public override void Enter(EnemyController t) 
            {
                base.Enter(t);
                t._animator.SetTrigger("Attack");
                t._direction = Vector2.zero;
                t._lookAt = (t._target.transform.position - t._physics.position).normalized;
            }
            public override void Exit(EnemyController t) { t.OnAttackBox(0); } // 공격 박스 off
            public AttackState() {}
        }
        public class SkillState : State<EnemyController>
        {
            public override void Enter(EnemyController t)
            {
                base.Enter(t);
                t._animator.SetTrigger("Skill");
                t._direction = Vector2.zero;
                t._lookAt = (t._target.transform.position - t._physics.position).normalized;
            }
            public override void Exit(EnemyController t) { t.StartCoroutine(t.CoolTime(t.SetUseSkill, 5.0f)); }
            public SkillState() {}
        }
        /* -----------------------------------------------------------Hit-------------------------------------------------- */
        public class HitState : State<EnemyController>
        {
            public override void Enter(EnemyController t) 
            {
                base.Enter(t);
                t._animator.SetTrigger("Hit");
                t._direction = Vector2.zero;
            }
            public HitState() { }
        }
        /* -----------------------------------------------------------Die-------------------------------------------------- */
        public class DieState : State<EnemyController>
        {
            public override void Enter(EnemyController t)
            {
                t._animator.SetTrigger("Die");
                t._direction = Vector2.zero;
            }
            public DieState() {}
        }
    }
}