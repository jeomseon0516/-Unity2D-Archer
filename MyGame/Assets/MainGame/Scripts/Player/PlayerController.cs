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
        private Collider2D _col;     // 벽 충돌 체크 할 콜라이더
        private float _beforeLocalY; // 이전 프레임의 로컬 Y 
        private float _jumpValue;
        private Vector3 _spawnPoint;
   
        protected override void Init()
        {
            base.Init();
            _maxHp = _hp = 1;
            _id = OBJECTID.PLAYER;
            _speed = 5.0f;
            _atk = 2;
            _playerState = new StateMachine<PlayerController>();
            _playerState.SetState(new RunState());
            _col = GetComponent<Collider2D>();
            _spawnPoint = _rigidbody.position;
        }
        protected override void Run()
        {
            _direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            base.Run();
        }
        protected override void CreateBullet()
        {
            GameObject obj = Instantiate(_bullet);
            obj.GetComponent<Rigidbody2D>().position = _rigidbody.position;

            BulletController controller = obj.transform.Find("Bullet").GetComponent<BulletController>();
            controller.SetDirection(transform.rotation.y == 180.0f ? -transform.right : transform.right);

            _bullets.Add(obj);
        }
        protected override void Die() 
        { 
            _playerState.SetState(new DieState());
            _playerState.Update(this);
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
                case PLR_STATE.DOWN:
                    _playerState.SetState(new DownState());
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
        private IEnumerator Jumping()
        {
            float jump = 7.0f;
            _col.isTrigger = true;

            while (transform.localPosition.y >= 0)
            {
                yield return null;
                transform.localPosition += new Vector3(0.0f, jump, 0.0f) * Time.deltaTime;
                jump -= Constants.GRAVITY * Time.deltaTime;
            }

            _col.isTrigger = false;
            transform.localPosition = new Vector2(0.0f, 0.0f);
        }
        private float CheckFallingOrJumping()
        {
            float movement = transform.localPosition.y - _beforeLocalY;

            _animator.SetFloat("JumpSpeed", movement);
            _beforeLocalY = transform.localPosition.y;

            return movement;
        }
        private IEnumerator Respawn() 
        {
            yield return new WaitForSeconds(5.0f);
            _animator.SetTrigger("Respawn");
            _rigidbody.position = _spawnPoint;
            _isDie = false;
            _hp = _maxHp;
        }
        protected override void ObjFixedUpdate() { _playerState.Update(this); }
        protected override void GetDamageAction(int damage) { _playerState.SetState(new HitState()); }
        protected override void ObjUpdate() { _jumpValue = CheckFallingOrJumping(); }
    }
    public partial class PlayerController : LivingObject
    {
        public enum PLR_STATE
        {
            RUN,
            JUMP,
            DOWN,
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

                if (Input.GetKeyDown(KeyCode.Space) || Mathf.Abs(t._jumpValue) > float.Epsilon)
                {
                    t._playerState.SetState(new JumpState());
                    return;
                }
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    t._animator.SetTrigger("Attack");
                    t._playerState.SetState(new AttackState());
                }
            }
            public override void Exit(PlayerController t) { t._animator.speed = 1; }
            public RunState() {}
        }
        public sealed class HitState : State<PlayerController>
        {
            public override void Enter(PlayerController t) 
            {
                t._animator.SetTrigger("Hit");
                base.Enter(t); 
            }
            public HitState() {}
        }
        public sealed class JumpState : State<PlayerController>
        {
            public override void Enter(PlayerController t)
            {
                if (t.transform.localPosition.y <= 0.0f)
                    t.StartCoroutine(t.Jumping());
                base.Enter(t);
            }
            public override void Update(PlayerController t)
            {
                if (t._jumpValue < 0.0f)
                    t._playerState.SetState(new DownState());
            }
            public JumpState() {}
        }
        public sealed class DownState : State<PlayerController>
        {
            public override void Update(PlayerController t)
            {
                if (Mathf.Abs(t._jumpValue) < float.Epsilon)
                    t._playerState.SetState(new RunState());
            }
            public DownState() {}
        }
        public sealed class AttackState : State<PlayerController>
        {
            Vector2 _direction;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                _direction = t._direction;
            }
            public override void Update(PlayerController t) { t._lookAt = _direction; }
            public AttackState() {}
        }
        public sealed class DieState : State<PlayerController>
        {
            public override void Enter(PlayerController t)
            {
                print("asdf");
                base.Enter(t);
                t._animator.SetTrigger("Die");
                t.StartCoroutine(t.Respawn());
            }
            public DieState() {}
        }
    }
}