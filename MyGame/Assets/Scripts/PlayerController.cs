using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingObject
{
    // Run과 Idle은 결론적으로 같은 동작을 수행하므로 따로 처리하지 않는다.
    public sealed class RunState : State<PlayerController>
    {
        public override void Update(PlayerController t)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                t.OnHit();
                _state = new HitState();
            }
            if (Input.GetKey(KeyCode.Space))
            {
                t.OnJump();
            }
        }
        public override void Exit(PlayerController t)
        {

        }

        public RunState() : base() {}
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
    /*
     * 큐로 플레이어 패턴 구현
     * 점프 히트 될 수 있다
     * 점프할땐 공격 못하게
     * 
     */

    private List<GameObject> _bullets = new List<GameObject>();
    private State<PlayerController> _playerState;
    private bool _onJump;

    protected override void Init()
    {
        base.Init();
        _hp    = 20;
        _id    = OBJECTID.PLAYER;
        _speed = 5.0f;
        _playerState = new RunState();
    }
    protected override void ObjUpdate()
    {
        _playerState.Update(this);
        if (Input.GetKey(KeyCode.LeftControl))
            OnAttack();
        if (Input.GetKey(KeyCode.LeftShift))
            OnHit();
        if (Input.GetKey(KeyCode.Space))
            OnJump();
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
        if (_onJump) return;

        _onJump    = true;
        _animator.SetTrigger("Jump");
    }
    protected override void Run() 
    { 
        _direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        base.Run();
    }
    protected override void OnAttack() 
    {
        if (_onAttack) return;

        CreateBullet();
        _onAttack  = true;
        _animator.SetTrigger("Attack");
    }
    protected void SetJump() { _onJump = false; }
}