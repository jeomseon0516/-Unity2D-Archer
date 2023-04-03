using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public partial class PenguinController : EnemyBase
    {
        StateMachine<PenguinController> _state;

        bool _useJump;
        bool _useSlide;

        float _jumpCoolTime;
        float _slideCoolTime;

        protected override void Init()
        {
            base.Init();
            _id = OBJECTID.PENGUIN;
            _bullet = ResourcesManager.GetInstance().GetObjectToKey(_id, "Bullet");

            _attackDis = 0.65f;
            _hp = _maxHp = 1000;

            _jumpCoolTime  = 7.5f;
            _slideCoolTime = 5.0f;

            _useJump = true;
            _useSlide = true;

            StartCoroutine(CheckFallingOrJumping());
            _state = new StateMachine<PenguinController>();
            _state.SetState(new IdleState());
        }
        private void CreateBullet(float power)
        {
            float force = power * 0.25f;
            float angle = 360 / 32;

            for (float i = 0; i < 360; i += angle)
            {
                Transform obj = Instantiate(_bullet).transform;

                CheckInComponent(obj.Find("Body").Find("Image").TryGetComponent(out Snowball bullet));
                obj.position = new Vector2(_physics.position.x, _physics.position.y - _offsetY);
                bullet.SetSpeed(force * 2);
                bullet.SetNextJump(force);

                float radian = Default.ConvertFromAngleToRadian(i);

                bullet.SetDirection(new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)));
            }
        }
        private IEnumerator Wait(float time)
        {
            yield return YieldCache.WaitForSeconds(time);
            _state.SetState(new IdleState());
        }
        protected override void ObjFixedUpdate()
        {
            _animator.SetFloat("JumpSpeed", _jumpValue);
            _state.Update(this);
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
                /*            if (t._useSkill)
                                movePoint = SkillAndGetMovePoint(t, xDir, targetPosition, myPosition);*/
                if (t._useJump)
                    t._state.SetState(new JumpAttackState(t));

                t._direction = (movePoint - myPosition).normalized;
                t._lookAt = (targetPosition - myPosition).normalized;
            }
            private Vector2 SkillAndGetMovePoint(PenguinController t, int xDir, Vector2 targetPosition, Vector2 myPosition)
            {
                Vector2 movePoint = new Vector2(targetPosition.x + Random.Range(4.0f, 6.0f) * xDir, _yTemp);

                if (Default.GetDistance(movePoint, myPosition) <= 1.0f)
                    t._state.SetState(new JumpAttackState(t));

                return movePoint;
            }
            private Vector2 HuntingAttackGetMovePoint(PenguinController t, int xDir, ObjectBase targetObj, Vector2 targetPosition, Vector2 myPosition)
            {
                Vector2 movePoint = new Vector2(targetPosition.x + 1.5f * xDir, targetPosition.y - targetObj.GetOffSetY());

                if (Mathf.Abs(movePoint.x - myPosition.x) <= t._attackDis &&
                    Mathf.Abs(movePoint.y - myPosition.y) <= t._attackDis * 0.25f)
                    t._state.SetState(new AttackState());

                return movePoint;
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

            public override void Enter(PenguinController t) 
            { 
                base.Enter(t);
                t._direction = Vector2.zero;
                t.StartCoroutine(t.CoolTime(t.SetUseJump, t._jumpCoolTime));
            }
            public override void Update(PenguinController t) 
            {
                Vector2 myPos = t.GetPhysics().position;
                Vector2 targetPos = t._target.GetPhysics().position;
                targetPos = new Vector2(targetPos.x, targetPos.y + t._offsetY - t._target.GetOffSetY());

                t._lookAt = (targetPos - myPos).normalized;

                float distance = Default.GetDistance(targetPos, myPos);
                float radian   = Default.GetPositionToRadian(targetPos, myPos);

                float x = Mathf.Cos(radian) * distance * 95.0f * 0.01f * t._speed * 2;
                float y = Mathf.Sin(radian) * distance * 95.0f * 0.01f * t._speed * 2;

                t._rigidbody.MovePosition(t._rigidbody.position + new Vector2(x, y) * Time.deltaTime);

                if (t._body.localPosition.y < float.Epsilon)
                    t._state.SetState(new WaitState());
            }
            public override void Exit(PenguinController t) { t.CreateBullet(_power); }
            public JumpAttackState(PenguinController t) 
            {
                t._jump = Random.Range(8.0f, 15.0f);
                _power = Random.Range(20, 32);
                t.StartCoroutine(t.Jumping(_power)); 
            }
        }
        public sealed class SlideAttackState : State<PenguinController>
        {
            public override void Enter(PenguinController t) 
            { 
                
                base.Enter(t); 
            }

            public override void Update(PenguinController t) { }
            public override void Exit(PenguinController t) { }
        }
        public sealed class ChaseAttackState : State<PenguinController>
        {
            public override void Enter(PenguinController t) { base.Enter(t); }
            public override void Update(PenguinController t) { }
            public override void Exit(PenguinController t) { }
        }
        public sealed class DieState : State<PenguinController>
        {
            public override void Enter(PenguinController t) { base.Enter(t); }
            public override void Update(PenguinController t) { }
            public override void Exit(PenguinController t) { }
        }
        public sealed class WaitState : State<PenguinController>
        {
            public override void Enter(PenguinController t) 
            { 
                base.Enter(t);
                t.StartCoroutine(t.Wait(1.5f));
            }
        }
    }
}