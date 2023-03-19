using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingObject
{
    /*
     * 큐로 플레이어 패턴 구현
     * 점프 히트 될 수 있다
     * 점프할땐 공격 못하게
     * 
     */

    private List<GameObject> _bullets = new List<GameObject>();
    private bool             _onJump;

    protected override void Init()
    {
        base.Init();
        SetState();
        _hp    = 20;
        _id    = OBJECTID.PLAYER;
        _speed = 5.0f;
    }
    protected override void ObjUpdate()
    {
        if (Input.GetKey(KeyCode.LeftControl))
            OnAttack();
        if (Input.GetKey(KeyCode.LeftShift))
            OnHit();
        if (Input.GetKey(KeyCode.Space))
            OnJump();
    }
    private void SetState()
    {       
        _onAttack = false;
        _onHit    = false;
        _onJump   = false;       
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
    private void SetJump() { _onJump = false; }
}