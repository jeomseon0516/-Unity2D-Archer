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
    private Collider2D _col;     // 벽 충돌 체크 할 콜라이더
    private float _beforeLocalY; // 이전 프레임의 로컬 Y 
    protected override void Init()
    {
        base.Init();
        _hp    = 20;
        _id    = OBJECTID.PLAYER;
        _speed = 5.0f;
        _playerState = new StateMachine<PlayerController>();
        _playerState.SetState(new RunState());
        _col = GetComponent<Collider2D>();
    }
    protected override void CreateBullet()
    {
        GameObject obj = Instantiate(_bullet);
        obj.transform.position = transform.position;

        BulletController controller = obj.transform.Find("Bullet").GetComponent<BulletController>();
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
    protected IEnumerator Jumping()
    { 
        float jump        = 7.0f;
        float height      = transform.localPosition.y;
        Vector3 shadowPos = _shadow.localPosition;
        _col.isTrigger = true;

        while (transform.localPosition.y >= 0)
        {
            yield return null;

            transform.localPosition += new Vector3(0.0f, jump, 0.0f) * Time.deltaTime;
            jump -= Constants.GRAVITY;
        }

        _col.isTrigger = false;
        transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
    }
    protected float CheckFallingOrJumping()
    {
        float height   = _beforeLocalY;
        float movement = transform.localPosition.y - height;

        _animator.SetFloat("JumpSpeed", transform.localPosition.y - height);
        _beforeLocalY = transform.localPosition.y;

        return movement;
    }
    protected override void ObjUpdate() 
    { 
        _playerState.Update(this);
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
        public override void Enter(PlayerController t) { base.Enter(t); }
        public override void Update(PlayerController t)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                t._animator.SetTrigger("Attack");
                t.SetState(PLR_STATE.ATTACK);
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                t.SetState(PLR_STATE.JUMP);
            }
        }
        public RunState() {}
    }
    public sealed class HitState : State<PlayerController>
    {
        public override void Enter(PlayerController t) { base.Enter(t); }
        public override void Update(PlayerController t)
        {

        }
        public override void Exit(PlayerController t)
        {

        }
        public HitState() {}
    }
    public sealed class JumpState : State<PlayerController>
    {
        public override void Enter(PlayerController t)
        { 
            t.StartCoroutine(t.Jumping());
            base.Enter(t);
        }
        public override void Update(PlayerController t)
        {
            float movement = t.CheckFallingOrJumping();

            if (movement < 0.0f)
                t.SetState(PLR_STATE.DOWN);
        }
        public JumpState() {}
    }
    public sealed class DownState : State<PlayerController>
    {
        float _localY;
        public override void Enter(PlayerController t) 
        { 
            base.Enter(t);
            _localY = t.transform.localPosition.y;
        }
        public override void Update(PlayerController t) 
        {
            float movement = t.CheckFallingOrJumping();

            if (movement == 0)
                t.SetState(PLR_STATE.RUN);
        }
        public DownState() {}
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
        public AttackState() {}
    }
}