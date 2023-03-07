using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controllor : MonoBehaviour
{
    private float _speed;
    private Vector3 _moveMent;
    private bool _onAttack;
    private bool _onHit;
    private bool _onJump;

    public Animator _animator;
    void Start() //Init()
    {
        _speed = 5.0f;
        SetState();
    }

    void Update()
    {
        // ** Player의 Animator를 받아온다
        _animator = this.GetComponent<Animator>();
        Run();

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
        _onHit = false;
        _onJump = false;       
    }

    private void Run()
    {
        // ** Input.GetAxis =  -1, 1의 값을 반환
        // ** 실수 연산 IEEE754
        float hor = Input.GetAxis("Horizontal");
        float ver = Input.GetAxis("Vertical");

        _moveMent = new Vector3(
             hor * _speed,
             ver * _speed,
             0.0f);

        _animator.SetFloat("Speed", Mathf.Abs(hor));
        this.transform.position += _moveMent * Time.deltaTime;
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
    
    private void SetJump()
    {
        SetState();
    }
    private void SetAttack()
    {
        _onAttack = false;
    }

    private void SetHit()
    {
        SetState();
    }
}
