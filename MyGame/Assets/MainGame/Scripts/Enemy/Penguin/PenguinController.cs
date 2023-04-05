using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public partial class PenguinController : EnemyBase
    {
        StateMachine<PenguinController> _state;
        BoxCollider2D _boxCol;
        Vector2 _keepBoxSize;
        Vector2 _keepBoxOffset;

        bool _useJump;
        bool _useSlide;

        float _jumpCoolTime;
        float _slideCoolTime;

        bool _isActive;
        protected override void Init()
        {
            _id = OBJECTID.PENGUIN;
            base.Init();

            _attackDis = 0.65f;
            _hp = _maxHp = 1000;

            _jumpCoolTime  = 7.5f;
            _slideCoolTime = 5.0f;

            _isActive = true;
            _atk = 5;

            _useJump = true;
            _useSlide = true;

            CheckInComponent(_attackBox.TryGetComponent(out _boxCol));

            _keepBoxSize   = _boxCol.size;
            _keepBoxOffset = _boxCol.offset;

            StartCoroutine(CheckFallingOrJumping());
            _state = new StateMachine<PenguinController>();
            _state.SetState(new IdleState());
        }
        private void CreateBullet(float power)
        {
            float angle = 360 / 8;

            for (float i = 0; i < 360; i += angle)
            {
                Transform obj = Instantiate(_bullet).transform;

                CheckInComponent(obj.Find("Body").Find("Image").TryGetComponent(out Snowball bullet));
                obj.position = new Vector2(_physics.position.x, _physics.position.y - _offsetY);
                bullet.SetSpeed(power * 0.4f);
                bullet.SetNextJump(power * 0.5f);

                float radian = Default.ConvertFromAngleToRadian(i);

                bullet.SetDirection(new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));
            }
        }
        protected override void Die()
        {
            StartCoroutine(FadeOutObject());
        }
        private IEnumerator Wait(float time)
        {
            yield return YieldCache.WaitForSeconds(time);
            _state.SetState(new IdleState());
        }
        protected override void ObjFixedUpdate()
        {
            if (!_isActive) return;
            _animator.SetFloat("JumpSpeed", _jumpValue);
            _state.Update(this);
        }
        private void FastChaseTarget(Vector2 myPos, Vector2 targetPos, float distance, float speed = 2)
        {
            float radian = Default.GetPositionToRadian(targetPos, myPos);

            float x = Mathf.Cos(radian) * distance * 95.0f * 0.01f * speed;
            float y = Mathf.Sin(radian) * distance * 95.0f * 0.01f * speed;

            _direction = new Vector2(x, y);
        }
        private void SetUseJump(bool useJump) { _useJump = useJump; }
        private void SetUseSlide(bool useSlide) { _useSlide = useSlide; }
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
                    _state.SetState(new JumpAttackState(this));
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
                _coolTime = Random.Range(0.0f, 3.0f);
                _isMove = false;
                base.Enter(t);
            }
            public override void Update(PenguinController t)
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
            public override void Exit(PenguinController t) {}
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

                if (t._useSlide)
                {
                    t._state.SetState(new SlideAttackWait());
                    return;
                }
                else if (t._useJump)
                {
                    t._state.SetState(new JumpAttackState(t));
                    return;
                }

                if (t.CheckAttack(t, xDir, movePoint, myPos, 0.25f))
                {
                    t._state.SetState(new AttackState());
                    return;
                }
      
                t._direction = (movePoint - myPos).normalized;
                t._lookAt    = (targetPos - myPos).normalized;
            }
        }
        public sealed class AttackState : State<PenguinController>
        {
            public override void Enter(PenguinController t)
            {
                base.Enter(t);
                t._animator.SetTrigger("Attack");

                t._direction = Vector2.zero;
                t._lookAt = (t._target.transform.position - t._physics.position).normalized;
            }
            public override void Exit(PenguinController t) { t.OnAttackBox(0); } // 공격 박스 off
        }
        public sealed class JumpAttackState : State<PenguinController>
        {
            float _power;
            int _atk;

            public override void Enter(PenguinController t) 
            { 
                base.Enter(t);
                t._direction = Vector2.zero;
                t.StartCoroutine(t.CoolTime(t.SetUseJump, t._jumpCoolTime));
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
                {
                    t.OnAttackBox(1);
                    t._boxCol.offset = new Vector2(0, -1.63f);
                    t._boxCol.size = new Vector2(1.5f, 1.02f);
                }

                if (t._body.localPosition.y < float.Epsilon)
                    t._state.SetState(new WaitState());
            }
            public override void Exit(PenguinController t) 
            {
                t.OnAttackBox(0);
                t.CreateBullet(_power);
                t._boxCol.offset = t._keepBoxOffset;
                t._boxCol.size   = t._keepBoxSize;
                t._atk = _atk;
            }
            public JumpAttackState(PenguinController t) 
            {
                t._jump = Random.Range(8.0f, 15.0f);
                _power  = Random.Range(20, 32);

                t.StartCoroutine(t.Jumping(_power)); 
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

                t.GetTargetAndMyPos(out Vector2 myPos, out _movePoint);
                t._animator.SetTrigger("Slide");
                _time = 3.0f;
                _atk = t._atk;
                t._atk = 10;

                t.OnAttackBox(1);
                t._boxCol.offset = new Vector2(0, -1.48f);
                t._boxCol.size   = new Vector2(2.36f, 1.43f);

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
                t.StartCoroutine(t.CoolTime(t.SetUseSlide, 5.0f)); 
                t.OnAttackBox(0);
                t._boxCol.offset = t._keepBoxOffset;
                t._boxCol.size   = t._keepBoxSize;
                t._atk = _atk;
            }
        }
        public sealed class ChaseAttackState : State<PenguinController>
        {
            public override void Enter(PenguinController t) { base.Enter(t); }
            public override void Update(PenguinController t)
            {
            }
            public override void Exit(PenguinController t) { }
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

                _keepTargetPos = new Vector2(targetPos.x + Random.Range(4.0f, 6.0f) * xDir,
                                             Random.Range(0.0f, 1.5f) * Random.Range(0, 2) == 0 ? 1 : -1); // 오프셋을 랜덤으로 구하고 위쪽 방향인지 아래쪽방향인지 랜덤으로 구한다.
            }
            public override void Update(PenguinController t) 
            {
                t.GetTargetAndMyPos(out Vector2 myPos, out Vector2 targetPos);
                int xDir = myPos.x - targetPos.x > 0 ? 1 : -1;

                if (Default.GetDistance(_keepTargetPos, myPos) <= 1.0f ||
                    Default.GetDistance(targetPos, myPos) <= 10.0f)
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

                t._direction = (_keepTargetPos - myPos).normalized;
                t._lookAt    = (targetPos - myPos).normalized;
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
                t.StartCoroutine(t.Wait(_time));
            }
            public WaitState(float time = 1.5f) { _time = time; } 
        }
    }
}