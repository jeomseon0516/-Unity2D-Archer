using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBJECT.SNOWBALL; 

namespace OBJECT
{
    public partial class PenguinController : EnemyBase
    {
        StateMachine<PenguinController> _state;
        BoxCollider2D _boxCol;
        Vector2 _keepBoxSize;
        Vector2 _keepBoxOffset;

        Queue<State<PenguinController>> _skillQueue = new Queue<State<PenguinController>>(); // 큐로 스킬을 쌓아둔다

        float _jumpCoolTime;
        float _slideCoolTime;
        float _chaseCoolTime;

        bool _isActive;
        protected override void Init()
        {
            _id = OBJECTID.PENGUIN;
            base.Init();

            _attackDis = 0.65f;
            _hp = _maxHp = 500;

            _jumpCoolTime = 12.5f;
            _chaseCoolTime = 8.0f;
            _slideCoolTime = 6.0f;

            _isActive = true;
            _atk = 5;

            CheckInComponent(_attackBox.TryGetComponent(out _boxCol));

            _keepBoxSize   = _boxCol.size;
            _keepBoxOffset = _boxCol.offset;

            StartCoroutine(CheckFallingOrJumping());

            /*쿨타임이 있는 스킬 패턴들은 큐로 관리 쿨타임이 다 되면 큐에 넣고 호출*/
            AddAfterResetCoroutine("Jump", SetUseSkill(Random.Range(_jumpCoolTime - 2.5f, _jumpCoolTime + 2.5f), new JumpAttackState()));
            StartCoroutine(SetUseSkill(Random.Range(_chaseCoolTime - 1.5f, _chaseCoolTime + 1.5f), new ChaseAttackState()));
            StartCoroutine(SetUseSkill(Random.Range(_slideCoolTime - 1.5f, _slideCoolTime + 1.5f), new SlideAttackWait()));

            _state = new StateMachine<PenguinController>();
            _state.SetState(new IdleState());
        }
        protected override void ObjFixedUpdate()
        {
            if (!_isActive) return;

            _animator.SetFloat("JumpSpeed", _jumpValue);
            _state.Update(this);
        }
        private IEnumerator Wait(float time)
        {
            yield return YieldCache.WaitForSeconds(time);
            _state.SetState(new IdleState());
        }
        private void FastChaseTarget(Vector2 myPos, Vector2 targetPos, float distance, float speed = 2)
        {
            float radian = Default.GetPositionToRadian(targetPos, myPos);

            float x = Mathf.Cos(radian) * distance * 95.0f * 0.01f * speed;
            float y = Mathf.Sin(radian) * distance * 95.0f * 0.01f * speed;

            _direction = new Vector2(x, y);
        }
        private IEnumerator SetUseSkill(float time, State<PenguinController> state)
        {
            yield return YieldCache.WaitForSeconds(time);
            _skillQueue.Enqueue(state);
        }
        private void SetAttackBox(float onBox, Vector2 offset, Vector2 size)
        {
            OnAttackBox(onBox);
            _boxCol.offset = offset;
            _boxCol.size  = size;
        }
        protected override void Die() 
        { 
            StartCoroutine(FadeOutObject());
            Destroy(_colTransform.gameObject);
        }
        protected override void OnCollision(ObjectBase obj, Collider2D col)
        {
            Vector2 force = Default.GetFromPostionToDirection(obj.GetPhysics().position, _physics.position);
            obj.TakeDamage(_atk, force * 2);
        }
    }
    public partial class PenguinController : EnemyBase
    {
        enum PENGUIN_STATE
        {
            IDLE,
            TARGETING,
            ATTACK,
            JUMPATTACK,
            SLIDEATTACK,
            CHASEATTACK,
            SLIDEATTACK_WAIT,
            WAIT,
            DIE
        }
        void SetState(PENGUIN_STATE state)
        {
            switch (state)
            {
                case PENGUIN_STATE.IDLE:
                    _state.SetState(new IdleState());
                    break;
                case PENGUIN_STATE.TARGETING:
                    _state.SetState(new TargetingState());
                    break;
                case PENGUIN_STATE.ATTACK:
                    _state.SetState(new AttackState());
                    break;
                case PENGUIN_STATE.JUMPATTACK:
                    _state.SetState(new JumpAttackState());
                    break;
                case PENGUIN_STATE.SLIDEATTACK:
                    _state.SetState(new SlideAttackState());
                    break;
                case PENGUIN_STATE.CHASEATTACK:
                    _state.SetState(new ChaseAttackState());
                    break;
                case PENGUIN_STATE.DIE:
                    _state.SetState(new DieState());
                    break;
                case PENGUIN_STATE.WAIT:
                    _state.SetState(new WaitState());
                    break;
            }
        }
        public sealed class IdleState : State<PenguinController>
        {
            Vector3 _randPoint;
            float _coolTime;
            bool _isMove;
            public override void Enter(PenguinController t)
            {
                base.Enter(t);
                t.ZeroForce();

                _coolTime = Random.Range(0.0f, 3.0f);
                _isMove = false;
            }
            public override void Update(PenguinController t)
            {
                if (t._isHunting || Default.GetDistance(t._target.transform.position, t.transform.position) <= t._searchDis) // 타겟이 범위 안에 들어오면 타겟팅 패턴으로 전환
                {
                    t._state.SetState(new TargetingState());
                    return;
                }

                t.RunStateMethod(ref _coolTime, ref _isMove, ref _randPoint);
            }
        }
        public sealed class TargetingState : State<PenguinController>
        {
            float _yTemp;
            public override void Enter(PenguinController t)
            {
                _yTemp = Random.Range(0.0f, 1.5f) * Random.Range(0, 2) == 0 ? 1 : -1;
                base.Enter(t);
            }
            public override void Update(PenguinController t)
            {
                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);

                if (!t._isHunting && Default.GetDistance(targetPos, myPos) > t._searchDis) // 플레이어가 범위 밖으로 벗어났다면 다시 Idle패턴으로 전환
                {
                    t._state.SetState(new IdleState());
                    return;
                }

                int xDir = myPos.x - targetPos.x > 0 ? 1 : -1; // 보정해야 할 방향이 어느쪽인가?
                Vector2 movePoint = new Vector2(targetPos.x + 1.5f * xDir, targetPos.y);

                if (t._skillQueue.Count > 0)
                {
                    t._state.SetState(t._skillQueue.Dequeue());
                    return;
                }

                if (t.CheckAttack(t, xDir, movePoint, myPos, 0.25f))
                {
                    t._state.SetState(new AttackState());
                    return;
                }

                t.SetLookAtAndDirection(movePoint, targetPos, myPos);
            }
        }
        public sealed class AttackState : State<PenguinController>
        {
            public override void Enter(PenguinController t)
            {
                base.Enter(t);
                t.SetAttackState();
            }
        }
        public sealed class JumpAttackState : State<PenguinController>
        {
            float _power;
            int _atk;
            public override void Enter(PenguinController t) 
            { 
                base.Enter(t);

                t._jump = Random.Range(8.0f, 15.0f);
                _power = Random.Range(20, 32);
                t.StartCoroutine(t.Jumping(_power));

                t._direction = Vector2.zero;
                _atk = t._atk;
                t._atk = 7;
            }
            public override void Update(PenguinController t) 
            {
                base.Enter(t);
                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);
                float distance = Default.GetDistance(targetPos, myPos);

                t._lookAt = (targetPos - myPos).normalized;
                t.FastChaseTarget(myPos, targetPos, distance);

                if (!t._attackBox.activeSelf && t._jumpValue < float.Epsilon) //낙하하는 타이밍에 어택박스를 켜준다.
                    t.SetAttackBox(1, new Vector2(0, -1.63f), new Vector2(1.5f, 1.02f));

                if (t._body.localPosition.y < float.Epsilon)
                    t._state.SetState(new WaitState());
            }
            public override void Exit(PenguinController t) 
            {
                CreateSnowBall.AllDirection(t._bullet, new Vector2(t._physics.position.x, t._physics.position.y - t._offsetY), _power);
                t.AddAfterResetCoroutine("Jump", t.SetUseSkill(
                    Random.Range(t._jumpCoolTime - 2.5f, t._jumpCoolTime + 2.5f), 
                    new JumpAttackState()));
                t.SetAttackBox(0, t._keepBoxOffset, t._keepBoxSize);
                t._atk = _atk;
            }
        }
        public sealed class SlideAttackState : State<PenguinController>
        {
            float _time;
            int _atk;
            Vector2 _movePoint;
            public override void Enter(PenguinController t)
            {
                base.Enter(t);

                t.SetAttackBox(1, new Vector2(0, -1.48f), new Vector2(2.36f, 1.43f));
                t.GetTargetAndMyPos(out Vector2 myPos, out _movePoint);
                t._animator.SetTrigger("Slide");
                _time = 3.0f;
                _atk = t._atk;
                t._atk = 10;

                float distance = Default.GetDistance(_movePoint, myPos);
                _movePoint += (_movePoint - myPos).normalized * distance * 60.0f * 0.01f;
            }
            public override void Update(PenguinController t) 
            {
                _time -= Time.deltaTime;
                Vector2 myPos = new Vector2(t._physics.position.x, t._physics.position.y - t._offsetY);

                float distanceX = Mathf.Abs(_movePoint.x - myPos.x);
                float distance  = Default.GetDistance(_movePoint, myPos);

                t.FastChaseTarget(myPos, _movePoint, distance, 1.25f);

                if (distanceX < 0.1f || _time <= 0.0f)
                    t._state.SetState(new WaitState(1.0f));
            }
            public override void Exit(PenguinController t) 
            {
                t.StartCoroutine(t.SetUseSkill(Random.Range(t._slideCoolTime - 1.5f, t._slideCoolTime + 1.5f), new SlideAttackWait()));
                t.SetAttackBox(0, t._keepBoxOffset, t._keepBoxSize);
                t._atk = _atk;
            }
        }
        /*
         * 플레이어를 추격하며 계속 쫒아다닌다
         */
        public sealed class ChaseAttackState : State<PenguinController>
        {
            float _chaseTime;
            Vector3 _beforePosition;
            int _count;
            public override void Enter(PenguinController t) 
            {
                base.Enter(t);
                _chaseTime = 2.0f;
                _count = 3;

                _beforePosition = t._physics.position;
                t._direction = Vector2.zero;
            }
            public override void Update(PenguinController t)
            {
                _chaseTime -= Time.fixedDeltaTime;

                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);
                float radian = Default.GetPositionToRadian(targetPos, myPos);

                Vector3 moveDir = (t._physics.position - _beforePosition).normalized;
                float distance = Default.GetDistance(t._physics.position, _beforePosition);

                if (_chaseTime < float.Epsilon)
                {
                    t._animator.SetTrigger("Attack");
                    _chaseTime = 2.0f;
                    CreateSnowBall.Targeting(t._bullet, moveDir * distance * 10, myPos, radian);
                }

                _beforePosition = t._physics.position;
                t._direction = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
                t._lookAt = (targetPos - myPos).normalized;

                float x = t._direction.x * 10 * 0.01f;
                float y = t._direction.y * 10 * 0.01f;

                Vector2 movement = new Vector2(x, y);

                t.AddForce(movement);
            }
            public override void Exit(PenguinController t) 
            {
                if (--_count <= 0)
                {
                    t._state.SetState(Random.Range(0, 2) == 0 ? new WaitState(2.0f) : new JumpAttackState());
                    t.StartCoroutine(t.SetUseSkill(Random.Range(t._chaseCoolTime - 1.5f, t._chaseCoolTime + 1.5f), new ChaseAttackState()));
                }
                else
                {
                    t.FindCoroutineStop("Wait"); // 어택 후 WaitState호출후 코루틴으로 패턴을 전환하기 때문에 해당 코루틴을 찾아 정지
                    t._state.SetState(this);
                }
            }
        }
        public sealed class DieState : State<PenguinController>
        {
            public override void Enter(PenguinController t) { base.Enter(t); }
            public override void Update(PenguinController t) { }
            public override void Exit(PenguinController t) { }
        }
        public sealed class SlideAttackWait : State<PenguinController>
        {
            Vector2 _keepTargetPos;
            public override void Enter(PenguinController t) 
            { 
                base.Enter(t);

                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);
                int xDir = myPos.x - targetPos.x > 0 ? 1 : -1; // 보정해야 할 방향이 어느쪽인가?
                int yDir = Random.Range(0, 2) == 0 ? 1 : -1;

                _keepTargetPos = new Vector2(targetPos.x + Random.Range(4.0f, 6.0f) * xDir,
                                                           Random.Range(0.0f, 1.5f) * yDir); // 오프셋을 랜덤으로 구하고 위쪽 방향인지 아래쪽방향인지 랜덤으로 구한다.
            }
            public override void Update(PenguinController t) 
            {
                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);
                int xDir = myPos.x - targetPos.x > 0 ? 1 : -1;

                if (Default.GetDistance(_keepTargetPos, myPos) <= 1.0f ||
                    Default.GetDistance(targetPos, myPos) >= 20.0f)
                {
                    t._state.SetState(new SlideAttackState());
                    return;
                }

                Vector2 movePoint = new Vector2(targetPos.x + 1.5f * xDir, targetPos.y);

                if (t.CheckAttack(t, xDir, movePoint, myPos, 0.25f))
                {
                    t._state.SetState(new AttackState());
                    return;
                }

                t.SetLookAtAndDirection(_keepTargetPos, targetPos, myPos);
            }
            public override void Exit(PenguinController t) { }
        }
        public sealed class WaitState : State<PenguinController>
        {
            float _time;
            public override void Enter(PenguinController t) 
            {
                base.Enter(t);
                t._direction = Vector2.zero;
                t.AddAfterResetCoroutine("Wait", t.Wait(_time));
            }
            public WaitState(float time = 1.5f) { _time = time; } 
        }
    }
}