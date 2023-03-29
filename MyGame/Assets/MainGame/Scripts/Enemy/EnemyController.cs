using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public partial class EnemyController : LivingObject
    {
        private StateMachine<EnemyController> _state;
        private GameObject _skill;
        private GameObject _attackBox;
        private GameObject _target;
        private float _skillMaxDistance;
        private float _searchDis;
        private float _attackDis;
        private bool _useSkill;
        private bool _isHunting;
        protected override void Init()
        {
            _skill  = ResourcesManager.GetInstance().GetObjectToKey(OBJECTID.ENEMY, "Bullet");
            _target = GameObject.Find("Player");

            _attackBox = transform.Find("AttackBox").gameObject;
            _attackBox.gameObject.AddComponent<AttackBox>().SetObjectBase(this);

            _state = new StateMachine<EnemyController>();
            _state.SetState(new IdleState());

            _skillMaxDistance = 8.0f;
            _useSkill         = true;
            _isHunting        = false;
            _searchDis        = 8.0f;
            _attackDis        = 1.5f;
            _hp               = 5;
        }
        protected override void CreateBullet()
        {
            ObjectBase targetPhysics    = _target.transform.Find(_target.name).GetComponent<ObjectBase>();
            Transform tPhysicsTransform = _target.transform;
            
            if (Default.GetDistance(tPhysicsTransform.position, transform.position) <= _skillMaxDistance)
            {
                Transform skillTransform = Instantiate(_skill).transform;
                ObjectBase skill = skillTransform.Find(_skill.name).GetComponent<ObjectBase>();
                skillTransform.position = new Vector2(
                    tPhysicsTransform.position.x + 0.15f,
                    tPhysicsTransform.position.y + skill.GetOffSetY() - targetPhysics.GetOffSetY());
            }
        }
        // TODO : HitAnimation재생
        protected internal override void CollisionAction(Collision2D obj)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "Bullet") return;
            StartCoroutine(TargetingObject()); // 플레이어에게 공격받으면 10초동안은 거리에 상관없이 무조건 플레이어를 쫒아다닌다.
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
        private Vector2 RandomMovePosition()
        {
            int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

            Vector3 offset = new Vector2(Random.Range(0, 5), Random.Range(0.0f, 1.5f));

            return new Vector2(_rigidbody.position.x + offset.x * xDir, 0.0f + offset.y * yDir);
        }
        // 플레이어에게 피격 후 추적 중일때 추적 쿨타임
        private IEnumerator TargetingObject()
        {
            _isHunting = true;
            yield return new WaitForSeconds(10.0f);
            _isHunting = false;
        }
        private IEnumerator SkillCollTime()
        {
            _useSkill = false;
            yield return new WaitForSeconds(5.0f);
            _useSkill = true;
        }
        public void OnAttackBox(float isOn) 
        {
            bool on = isOn > 0.0f ? true : false;
            _attackBox.SetActive(on);
        }
        protected override void Die() 
        {
            if (_isDie) return;

            _state.SetState(new DieState());
            Destroy(_physics.GetComponent<Collider2D>()); 
        }
        protected override void ObjFixedUpdate() { _state.Update(this); }
        protected override void GetDamageAction(int damage) { _state.SetState(new HitState()); }
        protected internal override void TriggerAction(Collider2D col) { TriggerCollision(col.transform.parent.gameObject.transform, col.transform.GetComponent<ObjectBase>()); }
    }

    // TODO : 에너미 상태 패턴 구현 
    public partial class EnemyController : LivingObject
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
            bool _isMove;
            float _coolTime;
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
                    t._direction = (_randPoint - t.transform.position).normalized;

                    if (Default.GetDistance(_randPoint, t.transform.position) <= 1.0f) // 목표 위치로 이동이 끝났다면
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
                Vector2 myPosition = new Vector2(t.transform.position.x, t.transform.position.y - t._offsetY);
                Transform targetTransform = t._target.transform;
                Vector3 targetPosition = targetTransform.position;

                if (!t._isHunting && Default.GetDistance(targetPosition, myPosition) > t._searchDis) // 플레이어가 범위 밖으로 벗어났다면 다시 Idle패턴으로 전환
                {
                    t._state.SetState(new IdleState());
                    return;
                }

                ObjectBase targetObj = targetTransform.Find("Player").GetComponent<ObjectBase>();
                int xDir = myPosition.x - targetPosition.x > 0 ? 1 : -1; // 보정해야 할 방향이 어느쪽인가?
                Vector2 movePoint = Vector2.zero;

                movePoint = HuntingAttackGetMovePoint(t, xDir, targetObj, targetPosition, myPosition);

                if (t._useSkill)
                    movePoint = SkillAndGetMovePoint(t, xDir, targetPosition, myPosition);
                
                t._direction = (movePoint - myPosition).normalized;
                t._lookAt = (targetPosition - t.transform.position).normalized;
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
                t._lookAt = (t._target.transform.position - t.transform.position).normalized;
            }
            public override void Exit(EnemyController t) { t.OnAttackBox(0); }
            public AttackState() {}
        }
        public class SkillState : State<EnemyController>
        {
            public override void Enter(EnemyController t)
            {
                base.Enter(t);
                t._animator.SetTrigger("Skill");
                t._direction = Vector2.zero;
                t._lookAt = (t._target.transform.position - t.transform.position).normalized;
            }
            public override void Exit(EnemyController t) { t.StartCoroutine(t.SkillCollTime()); }
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