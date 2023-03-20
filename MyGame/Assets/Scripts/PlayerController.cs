using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 코드 가독성을 위해 partial키워드로 분리
 */
public partial class PlayerController : LivingObject
{
    private List<GameObject> _bullets = new List<GameObject>();
    private StateMachine<PlayerController> _playerState;
    protected override void Init()
    {
        base.Init();
        _hp    = 20;
        _id    = OBJECTID.PLAYER;
        _speed = 5.0f;
        _playerState = new StateMachine<PlayerController>();
        _playerState.SetState(new RunState());
    }
    protected override void CreateBullet()
    {
        GameObject obj = Instantiate(_bullet);
        obj.transform.position = transform.position;

        BulletController controller = obj.GetComponent<BulletController>();
        controller.SetDirection(transform.rotation.y == 180.0f ? -transform.right : transform.right);

        _bullets.Add(obj);
    }

    protected override void Run() 
    { 
        _direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        base.Run();
    }
    public void SetState(PLR_STATE state)
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
        }
    }
    protected override void ObjUpdate() { _playerState.Update(this); }
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
        public override void Enter(PlayerController t)
        {
            base.Enter(t);
        }
        public override void Update(PlayerController t)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                t._animator.SetTrigger("Attack");
                t.SetState(PLR_STATE.ATTACK);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                t._animator.SetTrigger("Jump");
                t.SetState(PLR_STATE.JUMP);
            }
        }

        public RunState() { }
    }
    public sealed class HitState : State<PlayerController>
    {
        public override void Update(PlayerController t)
        {

        }
        public override void Exit(PlayerController t)
        {

        }

        public HitState() : base() { }
    }
    public sealed class JumpState : State<PlayerController>
    {
        private float _jump;
        private float _height;
        private Vector3 _shadowPos;
        private Collider2D _col;
        private Vector2 _colPos;
        public override void Enter(PlayerController t)
        {
            _jump = 10.0f;
            _height = t.transform.position.y;
            _shadowPos = t._shadow.position;
            _col = t.GetComponent<CapsuleCollider2D>();
            base.Enter(t);
        }
        public override void Update(PlayerController t)
        {
            t.transform.position += new Vector3(0.0f, _jump, 0.0f) * Time.deltaTime;
            float colDis = Mathf.Abs(_col.offset.y - _colPos.y);
            print(colDis);
            float distance = t.transform.position.y - _height;
            t._shadow.position = new Vector3(_shadowPos.x, _shadowPos.y - (distance * 0.5f), 0.0f);
            _col.offset = new Vector2(_col.offset.x, _colPos.y - colDis);

            _jump -= Constants.GRAVITY;
        }
        public override void Exit(PlayerController t)
        {
            t._animator.SetTrigger("Down");
            base.Exit(t);
        }

        public JumpState() : base() { }
    }
    public sealed class DownState : State<PlayerController>
    {
        public override void Enter(PlayerController t) { }
        public override void Update(PlayerController t) { }
        public override void Exit(PlayerController t) { }

        public DownState() : base() { }
    }
    public sealed class AttackState : State<PlayerController>
    {
        public override void Enter(PlayerController t)
        {
            base.Enter(t);
            t.CreateBullet();
        }
        public override void Update(PlayerController t)
        {

        }
        public override void Exit(PlayerController t) { }

        public AttackState() { }
    }
}