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
        private Vector3 _spawnPoint;
        private float _beforeLocalY; // 이전 프레임의 로컬 Y 
        private float _jumpValue;
   
        protected override void Init()
        {
            base.Init();
            _maxHp = _hp = 20;
            _id = OBJECTID.PLAYER;
            _speed = 5.0f;
            _atk = 2;
            _playerState = new StateMachine<PlayerController>();
            _playerState.SetState(new RunState());
            _spawnPoint = _physics.position;

            RegisterObserver(GameObject.Find("Canvas").transform.Find("HpBar").GetComponent<PrograssBar>());
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

            float radian = Default.GetPositionToRadian(_lookAt, Vector2.zero);

            BulletController controller = objTransform.Find("Body").Find("Image").GetComponent<BulletController>();
            controller.SetDirection(_lookAt != Vector2.zero ? new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)).normalized :
                                                              transform.eulerAngles.y == 180.0f ? Vector2.left : Vector2.right);

            Quaternion rotation = controller.transform.rotation;
            float angle = Default.ConvertFromRadianToAngle(radian);

            rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, rotation.eulerAngles.x, angle);
            controller.transform.rotation = rotation;

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
            _bodyCollider.isTrigger = true;

            while (_body.localPosition.y >= 0)
            {
                yield return new WaitForFixedUpdate();
                _body.localPosition += new Vector3(0.0f, jump, 0.0f) * Time.deltaTime;
                jump -= Constants.GRAVITY * Time.deltaTime;
            }

            _bodyCollider.isTrigger = false;
            _body.localPosition = new Vector2(0.0f, 0.0f);
        }
        private IEnumerator CheckFallingOrJumping()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                _jumpValue = _body.localPosition.y - _beforeLocalY;

                _animator.SetFloat("JumpSpeed", _jumpValue);
                _beforeLocalY = _body.localPosition.y;
            }
        }
        private IEnumerator Respawn() 
        {
            yield return new WaitForSeconds(5.0f);
            _playerState.SetState(new RunState());
            _animator.SetTrigger("Respawn");
            _rigidbody.position = _spawnPoint;
            _isDie = false;
            _hp = _maxHp;
        }
        protected override void ObjFixedUpdate() { _playerState.Update(this); }
        protected override void GetDamageAction(int damage) { _playerState.SetState(new HitState()); }
        protected override void ObjUpdate() 
        { 
            NotifyObservers(); 
        }
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
                if (Input.GetKey(KeyCode.UpArrow)   || Input.GetKey(KeyCode.DownArrow) || 
                    Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
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
                if (t._body.localPosition.y <= 0.0f)
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
                t._speed = 3.5f;
                _direction = t._direction;
            }
            public override void Update(PlayerController t) 
            { 
                t._lookAt = new Vector2(Input.GetAxisRaw("FireHorizontal"), Input.GetAxisRaw("FireVertical"));
            }
            public override void Exit(PlayerController t) { t._speed = 5.0f; }

            public AttackState() {}
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
            public DieState() {}
        }
    }
}