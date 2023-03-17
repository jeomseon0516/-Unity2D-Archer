using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : LivingObject
{
    private enum STATE 
    {
        IDLE = 0,
        RUN,
        JUMP,
        DOWN,
        ATTACK,
        HIT,
        ACROBATIC,
        ROPE,
        DIE
    }

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
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CreateBullet();
            OnAttack();
        }
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
        if (_onAttack) return;

        GameObject obj = Instantiate(_bullet);
        obj.transform.position = transform.position;

        BulletController controller = obj.GetComponent<BulletController>();
        controller.SetDirection(transform.rotation.y == 180.0f ? -transform.right : transform.right);

        _bullets.Add(obj);
    }
    protected override void Run()
    {
        _direction = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
        _animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(_direction.x), Mathf.Abs(_direction.y)));
    }
    private void OnJump()
    {
        if (_onJump) return;

        _onJump = true;
        _animator.SetTrigger("Jump");
    }
    private void SetJump() { SetState(); }
    private void SetHit() { SetState(); }
}