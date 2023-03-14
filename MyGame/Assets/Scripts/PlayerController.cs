using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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
    private GameObject       _bulletPrefab;
    private Animator         _animator;
    private SpriteRenderer   _sprRen;
    private Vector3          _moveMent;
    private float            _speed;
    private bool             _onAttack;
    private bool             _onHit;
    private bool             _onJump;

    private void Awake()
    {
        _animator     = GetComponent<Animator>();
        _sprRen       = GetComponent<SpriteRenderer>();
        _bulletPrefab = Resources.Load("Prefabs/Bullet") as GameObject;
    }
    private void Start() //Init()
    {
        _speed = 5.0f;
        SetState();
    }
    private void Update()
    {
        Run();
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
    private void CreateBullet()
    {
        if (_onAttack) return;

        GameObject obj = Instantiate(_bulletPrefab);
        obj.transform.position = transform.position;

        BulletController controllor = obj.GetComponent<BulletController>();
        controllor.Direction = _sprRen.flipX ? -transform.right : transform.right;
        controllor.SetFlipY(_sprRen.flipX);

        _bullets.Add(obj);
    }
    private void Run()
    {
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        ChangeFlipXToHor(hor);

        _moveMent = new Vector3(hor * _speed, ver * (_speed * 0.5f), 0.0f);
        _animator.SetFloat("Speed", Mathf.Max(Mathf.Abs(hor), Mathf.Abs(ver)));

        transform.position += _moveMent * Time.deltaTime;
    }
    private void ChangeFlipXToHor(float hor)
    {
        if      (hor < 0) _sprRen.flipX = true;
        else if (hor > 0) _sprRen.flipX = false;
    }
    private void OnHit()
    {
        if (_onHit) return;

        _onHit = true;
        _animator.SetTrigger("Hit");
    }
    private void OnAttack()
    {
        if (_onAttack) return;

        _onAttack = true;
        _animator.SetTrigger("Attack");
    }
    private void OnJump()
    {
        if (_onJump) return;

        _onJump = true;
        _animator.SetTrigger("Jump");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print("Coll");
    }

    private void SetJump() { SetState(); }
    private void SetAttack() { _onAttack = false; }
    private void SetHit() { SetState(); }
}