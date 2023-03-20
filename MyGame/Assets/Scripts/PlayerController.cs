using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : LivingObject
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
                t.OnAttack();
                t.SetState(PLR_STATE.ATTACK);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                t.OnJump();
                t.SetState(PLR_STATE.JUMP);
            }
        }

        public RunState() {}
    }

    public sealed class HitState : State<PlayerController>
    {
        public override void Update(PlayerController t)
        {

        }
        public override void Exit(PlayerController t)
        {

        }

        public HitState() : base() {}
    }

    public sealed class JumpState : State<PlayerController>
    {
        public override void Enter(PlayerController t)
        {
            
        }
        public override void Update(PlayerController t)
        {

        }
        public override void Exit(PlayerController t)
        {
            t._animator.SetTrigger("Down");
            base.Exit(t);
        }

        public JumpState() : base() {}
    }
    public sealed class DownState : State<PlayerController>
    {
        public override void Enter(PlayerController t) { }
        public override void Update(PlayerController t) { }
        public override void Exit(PlayerController t) { }

        public DownState() : base() {}
    }
    public sealed class AttackState : State<PlayerController>
    {
        public override void Enter(PlayerController t) 
        {
            t.CreateBullet();
            base.Enter(t);
        }
        public override void Update(PlayerController t) 
        {

        }
        public override void Exit(PlayerController t) {}

        public AttackState() {}
    }

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
    private void OnJump()
    {
        _animator.SetTrigger("Jump");
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