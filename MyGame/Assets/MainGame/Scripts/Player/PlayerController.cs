using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OBSERVER;

/*
 * 코드 가독성을 위해 partial키워드로 분리
 */
namespace OBJECT
{
    public partial class PlayerController : LivingObject
    {
        private StateMachine<PlayerController> _playerState;
        private GameObject _deathSmoke;
        private Vector2 _spawnPoint;
        private float _attackSpeed;
        private int _jumpCount;
        private bool _isDash;
        protected override void Init()
        {
            _id = OBJECTID.PLAYER;
            base.Init();
            _maxHp = _hp = 50;
            _attackSpeed = 4;
            _speed = 5.0f;
            _atk = 2;
            _playerState = new StateMachine<PlayerController>();
            _playerState.SetState(new RunState());
            _deathSmoke = ResourcesManager.GetInstance().GetObjectToKey(_id, "DeathSmoke");
            _spawnPoint = _physics.position;
            _jumpCount = 0;
            _isDash = false;

            AddAfterResetCoroutine("CheckFalling", CheckFallingOrJumping());
        }
        protected override void Run()
        {
            if (_isDie) return;

            _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            base.Run();
        }
        protected override void CreateBullet()
        {
            Transform objTransform = Instantiate(_bullet).transform;
            objTransform.position = _rigidbody.position;

            CheckInComponent(objTransform.Find("Body").Find("Image").TryGetComponent(out DefaultBullet controller));
            // 현재 방향키를 어떤 방향으로 누르고 있는지를 확인해서 쏠 방향을 구하고 이동중인 방향만큼 더 해주면 총알이 플레이어의 움직임의 영향을 받게 된다.
            Vector2 dir = _lookAt.normalized + _direction * 0.25f;
            controller.SetDirection(dir);
        }
        /* 해당 함수는 하이어라키에서 애니메이션 이벤트로 호출되는 함수 입니다. 스크립트 내에서 상태 전환이 필요한 경우 new 키워드를 사용해 초기화 합니다. */
        private IEnumerator Respawn()
        {
            yield return YieldCache.WaitForSeconds(5.0f);
            _animator.SetTrigger("Respawn");
            ResetPlayer();
        }
        protected override void ObjFixedUpdate() 
        {
            _animator.SetFloat("JumpSpeed", _jumpValue);
            _playerState.Update(this);
        }
        private bool PlayDash()
        {
            if (!Input.GetKeyDown(KeyCode.LeftShift)) return false;

            _playerState.SetState(new DashState());
            return false;
        }

        public void ResetPlayer()
        {
            AddAfterResetCoroutine("CheckFalling", CheckFallingOrJumping());
            _playerState.SetState(new RunState());
            _rigidbody.position = _spawnPoint;
            _isDie = false;
            _hp = _maxHp;
        }
        public bool OnStartJump()
        {
            if (!Input.GetKeyDown(KeyCode.Space) && _body.localPosition.y < float.Epsilon) return false; // 스페이스 바를 누르거나 공중에 띄워져있을때

            _playerState.SetState(new JumpState());
            return true;
        }
        
        private Vector2 GetFromAngleAndSpeedToDirection()
        {
            return Mathf.Abs(_direction.x) > float.Epsilon || Mathf.Abs(_direction.y) > float.Epsilon ?
                             _direction.normalized : _physics.rotation.eulerAngles.y == 180.0f ? Vector2.left : Vector2.right;
        }
        protected override void GetDamageAction(int damage) { _playerState.SetState(new HitState()); }
        protected override void Die() { _playerState.SetState(new DieState()); }
    }
    public partial class PlayerController : LivingObject
    {
        public enum PLR_STATE
        {
            RUN,
            JUMP,
            DOUBLEJUMP,
            DASH,
            ATTACK,
            HIT,
            ACROBATIC,
            DIE
        }
        private void SetState(PLR_STATE state)
        {
            switch (state)
            {
                case PLR_STATE.RUN:
                    _playerState.SetState(new RunState());
                    break;
                case PLR_STATE.JUMP:
                    _playerState.SetState(new JumpState());
                    break;
                case PLR_STATE.ATTACK:
                    _playerState.SetState(new AttackState());
                    break;
                case PLR_STATE.HIT:
                    _playerState.SetState(new HitState());
                    break;
                case PLR_STATE.DIE:
                    _playerState.SetState(new DieState());
                    break;
            }
        }
        // Run과 Idle은 결론적으로 같은 동작을 수행하므로 따로 처리하지 않는다.
        public sealed class RunState : State<PlayerController>
        {
            public override void Update(PlayerController t)
            { 
                if (t.OnStartJump() || t.PlayDash()) return;

                float speed = t.GetFromDirectionToSpeed(t._direction);

                t._animator.speed = speed > 0.0f ? speed : 1;

                float x = Input.GetAxisRaw("FireHorizontal");
                float y = Input.GetAxisRaw("FireVertical");

                Vector2 attackDir = new Vector2(x, y);

                if (attackDir != Vector2.zero)
                {
                    attackDir.x = x == 0 ? t._direction.x : x;
                    t._lookAt = attackDir;
                    t._animator.SetTrigger("Attack");
                    t._playerState.SetState(new AttackState(t._lookAt));
                }
            }
            public override void Exit(PlayerController t) { t._animator.speed = 1; }
            public RunState() { }
        }
        public sealed class HitState : State<PlayerController>
        {
            public override void Enter(PlayerController t)
            {
                t._animator.SetTrigger("Hit");
                base.Enter(t);
            }
            public override void Update(PlayerController t)
            {
                if (t.PlayDash()) return;
            }
            public HitState() { }
        }
        public sealed class JumpState : State<PlayerController>
        {
            float _jump;
            bool _onSpace; // 스페이스바를 누르고 있는가..
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                _jump = 8.0f;
                t._jump = t._body.localPosition.y < float.Epsilon ? _jump : t._jump;
                _onSpace = t._jump < float.Epsilon ? false : true;
                t.AddAfterResetCoroutine("Jump", t.Jumping());
            }
            public override void Update(PlayerController t)
            {
                if (t.PlayDash()) return;

                AddJumpPowerOnSpace(t);

                if (t._body.localPosition.y < float.Epsilon)
                {
                    t._playerState.SetState(new RunState());
                    t._jumpCount = 0;
                    return;
                }

                if (Input.GetKeyDown(KeyCode.Space) && t._jumpCount <= 0)
                    t._playerState.SetState(new DoubleJumpState());
            }
            private void AddJumpPowerOnSpace(PlayerController t)
            {
                if (Input.GetKey(KeyCode.Space) || !_onSpace || t._jumpValue < float.Epsilon) return;

                 t._jump = t._jump * 0.5f;
                 _onSpace = false;
            }
            public JumpState() {}
        }
        public sealed class DoubleJumpState : State<PlayerController>
        {
            Vector2 _saveDirection;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                _saveDirection = t.GetFromAngleAndSpeedToDirection();
                ++t._jumpCount;
                t._animator.SetTrigger("DoubleJump");
                t._jump += 6.0f;
                t._speed = 10.0f;
            }
            public override void Update(PlayerController t)
            {
                t._direction = _saveDirection;

                if (t._body.localPosition.y < float.Epsilon)
                    t._playerState.SetState(new RunState());
            }
            public override void Exit(PlayerController t) { t._speed = 5.0f; }

            public DoubleJumpState() {}
        }
        public sealed class DashState : State<PlayerController>
        {
            Vector2 _saveDirection;
            float _keepSpeed;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                _saveDirection = t.GetFromAngleAndSpeedToDirection();
                t._animator.SetTrigger("Dash");
                _keepSpeed = t._speed;
                t._speed *= 4;
            }
            public override void Update(PlayerController t) { t._direction = _saveDirection; }
            public override void Exit(PlayerController t)
            {
                t._speed = _keepSpeed;
            }
            public DashState() {}
        }
        public sealed class AttackState : State<PlayerController>
        {
            Vector2 _saveLookAt;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                t._animator.speed = t._attackSpeed;
                t._speed = 3.5f;

                if (_saveLookAt == Vector2.zero)
                {
                    float x = Input.GetAxisRaw("FireHorizontal");
                    float y = Input.GetAxisRaw("FireVertical");

                    Vector2 dir = new Vector2(x, y);
                    _saveLookAt = dir != Vector2.zero ? dir : new Vector2(x != 0 ? x : t._lookAt.x, y != 0 ? y : t._lookAt.y);
                }

                t._lookAt = _saveLookAt;
            }
            public override void Update(PlayerController t)
            {
                if (t.OnStartJump() || t.PlayDash()) return;

                Vector2 dir = new Vector2(Input.GetAxisRaw("FireHorizontal"), Input.GetAxisRaw("FireVertical"));
                t._lookAt = dir != Vector2.zero ? _saveLookAt = dir : _saveLookAt;
            }
            public override void Exit(PlayerController t)
            {
                t._animator.speed = 1;
                t._lookAt = _saveLookAt;
                t._speed = 5.0f;
            }
            public AttackState() { _saveLookAt = Vector2.zero; }
            public AttackState(Vector2 dir) { _saveLookAt = dir; }
        }
        public sealed class DieState : State<PlayerController>
        {
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                t._animator.SetTrigger("Die");
                t._direction = Vector2.zero;
                t.StartCoroutine(t.Respawn());
            }
            public override void Update(PlayerController t) { t._direction = Vector2.zero; }
            public DieState() { }
        }
    }
}