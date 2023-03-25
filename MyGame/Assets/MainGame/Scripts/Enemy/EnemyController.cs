using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public partial class EnemyController : LivingObject
    {
        private StateMachine<EnemyController> _state;
        protected GameObject _target;
        private float _searchDis;
        private float _attackDis;
        private bool _isHunting;

        protected override void Init()
        {
            _target = GameObject.Find("Player").gameObject;
            _hp = 5;
            _state = new StateMachine<EnemyController>();
            _state.SetState(new IdleState());

            _isHunting = false;
            _searchDis = 8.0f;
            _attackDis = 1.0f;
        }
        protected override void Run()
        {
            base.Run();
        }
        protected override void CreateBullet()
        {

        }
        // TODO : HitAnimation재생
        protected internal override void CollisionAction(Collision2D obj)
        {
            if (LayerMask.LayerToName(obj.gameObject.layer) != "Bullet") return;

            StartCoroutine(TargetingObject()); // 플레이어에게 공격받으면 10초동안은 거리에 상관없이 무조건 플레이어롤 쫒아다닌다.
            _state.SetState(new HitState());
            _isHunting = true;
            --_hp;
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
        private Vector3 RandomMovePosition()
        {
            int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

            Vector3 offset = new Vector3(Random.Range(0, 5), Random.Range(0.0f, 1.5f), 0.0f);

            return new Vector3(transform.position.x + offset.x * xDir, 0.0f + offset.y * yDir, 0.0f);
        }
        private IEnumerator TargetingObject()
        {
            yield return new WaitForSeconds(10.0f);
            _isHunting = false;
        }
        protected override void ObjUpdate() { _state.Update(this); }
    }

    // TODO : 에너미 상태 패턴 구현 
    public partial class EnemyController : LivingObject
    {
        public enum ENEMY_STATE
        {
            IDLE,
            TARGETING,
            ATTACK,
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
                float distance = Constants.GetDistance(t._target.transform.position, t.transform.position);

                if (distance <= t._searchDis || t._isHunting) // 타겟이 범위 안에 들어오면 타겟팅 패턴으로 전환
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
                    float movePointDistance = Constants.GetDistance(_randPoint, t.transform.position);

                    if (movePointDistance <= 1.0f) // 목표 위치로 이동이 끝났다면
                    {
                        t._direction = Vector3.zero;
                        _coolTime = Random.Range(0.0f, 3.0f);
                        _isMove = false;
                    }
                }
            }
            public override void Exit(EnemyController t) { }
            public IdleState() {}
        }
        /* -----------------------------------------------------------Targeting-------------------------------------------------- */
        public class TargetingState : State<EnemyController>
        {
            public override void Update(EnemyController t)
            {
                float distance = Constants.GetDistance(t._target.transform.position, t.transform.position);

                if (distance > t._searchDis && !t._isHunting) // 플레이어가 범위 밖으로 벗어났다면 다시 Idle패턴으로 전환
                {
                    t._state.SetState(new IdleState());
                    return;
                }
                if (distance <= t._attackDis)
                {
                    t._state.SetState(new AttackState());
                    return;
                }

                Vector3 dir = (t._target.transform.position - t.transform.position).normalized; //플레이어를 항상 쫒아온다.
                t._lookAt = dir;
                t._direction = dir;
            }
            public override void Exit(EnemyController t) {}
            public TargetingState() {}
        }
        /* -----------------------------------------------------------Attack-------------------------------------------------- */
        public class AttackState : State<EnemyController>
        {
            public override void Enter(EnemyController t) 
            {
                base.Enter(t);
                t._animator.SetTrigger("Attack");
                t._direction = Vector3.zero;
            }
            public override void Update(EnemyController t) {}
            public override void Exit(EnemyController t) {}
            public AttackState() { }
        }
        /* -----------------------------------------------------------Hit-------------------------------------------------- */
        public class HitState : State<EnemyController>
        {
            public override void Enter(EnemyController t) 
            {
                base.Enter(t);
                t._animator.SetTrigger("Hit");
                t._direction = Vector3.zero;
            }
            public override void Update(EnemyController t) {}
            public override void Exit(EnemyController t) {}
            public HitState() { }
        }
        public class DieState : State<EnemyController>
        {
            public override void Enter(EnemyController t)
            {
                base.Enter(t);
                t._direction = Vector3.zero;
            }
            public override void Update(EnemyController t) { }
            public override void Exit(EnemyController t) { }
            public DieState() {}
        }
    }
}