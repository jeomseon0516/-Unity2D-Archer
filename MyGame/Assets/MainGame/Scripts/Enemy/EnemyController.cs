using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public partial class EnemyController : EnemyBase<EnemyController>
    {
        private GameObject _skill;
        private float _skillMaxDistance;
        private bool _useSkill;
        protected override void Init()
        {
            base.Init();
            _skill = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Bullet");

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
                    tPhysicsTransform.position.y + skill.GetOffsetY() - _target.GetOffsetY());
            }
        }
        // 플레이어에게 피격 후 추적 중일때 추적 쿨타임
        protected override void Die()
        {
            _state.SetState(new DieState());
            Destroy(_colTransform.gameObject);
        }
        protected override void GetDamageAction(int damage) { _state.SetState(new HitState()); }
        protected override void ObjFixedUpdate() { _state.Update(this); }
        private void SetUseSkill(bool useSkill) { _useSkill = useSkill; }

        protected override bool TargetingMethod()
        {
            if (!_useSkill) return false;

            _state.SetState(new SkillWait());
            return true;
        }

        protected override void SetSkillState() { _state.SetState(new SkillState()); }
    }

    // TODO : 에너미 상태 패턴 구현 
    public partial class EnemyController : EnemyBase<EnemyController>
    {
        public enum ENEMY_STATE
        {
            IDLE,
            TARGETING,
            ATTACK,
            SKILL,
            SKILL_WAIT,
            HIT,
            DIE
        }
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
        /* -----------------------------------------------------------Idle-------------------------------------------------- */

        public sealed class SkillState : State<EnemyController>
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
        public sealed class HitState : State<EnemyController>
        {
            public override void Enter(EnemyController t) 
            {
                base.Enter(t);
                t._animator.SetTrigger("Hit");
                t._direction = Vector2.zero;
            }
            public HitState() { }
        }

        public sealed class SkillWait : State<EnemyController>
        {
            float _yTemp;
            public override void Enter(EnemyController t)
            {
                base.Enter(t);
                _yTemp = Random.Range(0.0f, 1.5f) * Random.Range(0, 2) == 0 ? 1 : -1;
            }
            public override void Update(EnemyController t)
            {
                if (t._target.GetIsDie())
                {
                    t._state.SetState(new IdleState());
                    return;
                }

                t.SkillWaitMethod

                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);

                int xDir = myPos.x - targetPos.x > 0 ? 1 : -1; // 보정해야 할 방향이 어느쪽인가?
                Vector2 movePoint = new Vector2(targetPos.x + Random.Range(4.0f, 6.0f) * xDir, _yTemp);

                if (Default.GetDistance(movePoint, myPos) <= 1.0f)
                {
                    t._state.SetState(new SkillState());
                    return;
                }

                if (t.CheckAttack(t, new Vector2(targetPos.x + 1.5f * xDir, targetPos.y), myPos))
                {
                    t._state.SetState(new AttackState());
                    return;
                }

                t.SetLookAtAndDirection(movePoint, targetPos, myPos);
            }
            public SkillWait() { }
        }
    }
}