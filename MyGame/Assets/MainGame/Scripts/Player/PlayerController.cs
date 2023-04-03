using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 코드 가독성을 위해 partial키워드로 분리
 */
namespace OBJECT
{
    public partial class PlayerController : LivingObject
    {
        private List<GameObject> _bullets = new List<GameObject>();
        private StateMachine<PlayerController> _playerState;
        private Vector2 _spawnPoint;
        private float _attackSpeed;
        protected override void Init()
        {
            base.Init();
            _maxHp = _hp = 10000;
            _id = OBJECTID.PLAYER;
            _attackSpeed = 4;
            _speed = 5.0f;
            _atk = 2;
            _playerState = new StateMachine<PlayerController>();
            _playerState.SetState(new RunState());
            _spawnPoint = _physics.position;

            CheckInComponent(GameObject.Find("Canvas").transform.Find("HpBar").TryGetComponent(out PrograssBar bar));
            RegisterObserver(bar);
            StartCoroutine(CheckFallingOrJumping());
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

            CheckInComponent(objTransform.Find("Body").Find("Image").TryGetComponent(out BulletController controller));
            // 현재 방향키를 어떤 방향으로 누르고 있는지를 확인해서 쏠 방향을 구하고
            Vector2 dir = _lookAt.normalized + _direction * 0.25f;
            controller.SetDirection(dir);

            _bullets.Add(objTransform.gameObject);
        }
        protected override void Die()
        {
            if (_isDie) return;
            _playerState.SetState(new DieState());
        }
        /* 해당 함수는 하이어라키에서 애니메이션 이벤트로 호출되는 함수 입니다. 스크립트 내에서 상태 전환이 필요한 경우 new 키워드를 사용해 초기화 합니다. */
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
        private IEnumerator Respawn()
        {
            yield return YieldCache.WaitForSeconds(5.0f);
            _playerState.SetState(new RunState());
            _animator.SetTrigger("Respawn");
            _rigidbody.position = _spawnPoint;
            _isDie = false;
            _hp = _maxHp;
        }
        protected override void GetDamageAction(int damage) { _playerState.SetState(new HitState()); }
        protected override void ObjFixedUpdate() { _playerState.Update(this); }
        protected override void ObjUpdate() { NotifyObservers(); }
    }
    public partial class PlayerController : LivingObject
    {
        public enum PLR_STATE
        {
            RUN,
            JUMP,
            ATTACK,
            HIT,
            ACROBATIC,
            DIE
        }
        // Run과 Idle은 결론적으로 같은 동작을 수행하므로 따로 처리하지 않는다.
        public sealed class RunState : State<PlayerController>
        {
            public override void Update(PlayerController t)
            {
                float speed = Mathf.Max(Mathf.Abs(t._direction.x), Mathf.Abs(t._direction.y));

                t._animator.speed = speed > 0.0f ? speed : 1;

                if (Input.GetKeyDown(KeyCode.Space) || t._body.localPosition.y > float.Epsilon) // 스페이스 바를 누르거나 공중에 띄워져있을때
                {
                    t._playerState.SetState(new JumpState());
                    return;
                }

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
            public HitState() { }
        }
        public sealed class JumpState : State<PlayerController>
        {
            float _jump;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StartCoroutine(t.Jumping(_jump));
            }
            public override void Update(PlayerController t)
            {
                if (t._body.localPosition.y < float.Epsilon)
                    t._playerState.SetState(new RunState());
            }
            public JumpState(float jump = 7.0f) { _jump = jump; }
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