using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controllor : MonoBehaviour
{
    // 복제된 총알의 저장공간
    public List<GameObject> _bullets = new List<GameObject>();
    // 현재 적용중인 스프라이트
    public SpriteRenderer   _sprRen;
    // 총알 원본
    public GameObject       _bulletPrefab;
    // 애니메이터 구성요소 받아오기 위해
    public Animator         _animator;
    // 움직일 방향
    private Vector3         _moveMent;
    // 플레이어 속도
    private float           _speed;
    // 공격중인가?
    private bool            _onAttack;
    // 히트중인가?
    private bool            _onHit;
    // 점프중인가?
    private bool            _onJump;

    public GameObject[] stageBacknew = new GameObject[7];

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _sprRen   = GetComponent<SpriteRenderer>();        
    }

    private void Start() //Init()
    {
        _speed = 5.0f;
        SetState();
    }

    private void Update()
    {
        // ** Player의 Animator를 받아온다
        Run();
        
        // 컨트롤키를 입력한다면? 공격, 삽 던지기
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CreateBullet();
            OnAttack();
        }
        // 쉬프트키를 입력한다면 히트
        if (Input.GetKey(KeyCode.LeftShift))
            OnHit();
        // 스페이스 바를 누르면 점프한다.
        if (Input.GetKey(KeyCode.Space))
            OnJump();
    }

    private void SetState()
    {
        // 활성화 전체 초기화
        _onAttack = false;
        _onHit    = false;
        _onJump   = false;       
    }

    private void CreateBullet()
    {
        if (_onAttack) return;
        // 총알 원본을 복제한다.
        GameObject obj = Instantiate(_bulletPrefab);
        // obj.transform.name = "";
        // 총알의 포지션을 플레이어의 위치로 변경한다.
        obj.transform.position = transform.position;
        // 총알의 BulletControllor 스크립트를 받아온다.
        BulletControllor controllor =  obj.GetComponent<BulletControllor>();
        // 총알이 날아갈 방향을 플레이어의 방향으로 정해준다.
        controllor.Direction = _sprRen.flipX ? -transform.right : transform.right;
        // 총알의 이미지 반전 상태를 플레이어의 이미지 반전 상태로 설정한다.
        controllor.SetFlipY(_sprRen.flipX);
        // 불렛 리스트에 보관한다.
        _bullets.Add(obj);
    }

    private void Run()
    {
        // ** Input.GetAxis =  -1, 1의 값을 반환
        // ** 실수 연산 IEEE754
        float hor = Input.GetAxis("Horizontal");

        ChangeFlipXToHor(hor);

        // 입력받은 값으로 플레이어를 움직인다.
        _moveMent = new Vector3(hor * _speed, 0.0f, 0.0f);
        // 플레이어의 움직임에 따라 이동 모션을 실행 한다.
        _animator.SetFloat("Speed", Mathf.Abs(hor));
        // 실제 플레이어를 움직인다.
        transform.position += _moveMent * Time.deltaTime;
    }

    // 플레이어가 바라보고 있는 방향에 따라 설정
    private void ChangeFlipXToHor(float hor)
    {
        if      (hor < 0) _sprRen.flipX = true;
        else if (hor > 0) _sprRen.flipX = false;
    }

    private void OnHit()
    {
        // 이미 히트 모션이 재생 중이라면 함수를 종료시킨다.
        if (_onHit) return;

        // 함수가 종료되지 않았다면 히트 상태를 활성화하고 모션을 실행한다.
        _onHit = true;
        _animator.SetTrigger("Hit");
    }

    private void OnAttack()
    {
        // 이미 공격 모션이 재생 중이라면 함수를 종료시킨다.
        if (_onAttack) return;

        // 함수가 종료되지 않았다면 공격 상태를 활성화시키고 모션을 실행한다.
        _onAttack = true;
        _animator.SetTrigger("Attack");
    }

    private void OnJump()
    {
        // 이미 점프중이라면 함수를 종료시킨다.
        if (_onJump) return;
        // 함수가 종료되지 않았다면 공격 상태를 활성화시키고 모션을 실행한다.
        _onJump = true;
        _animator.SetTrigger("Jump");
    }
    
    private void SetJump() { SetState(); }
    private void SetAttack() { _onAttack = false; }
    private void SetHit() { SetState(); }
}