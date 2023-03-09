using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controllor : MonoBehaviour
{
    // ������ �Ѿ��� �������
    public List<GameObject> _bullets = new List<GameObject>();
    // ���� �������� ��������Ʈ
    public SpriteRenderer   _sprRen;
    // �Ѿ� ����
    public GameObject       _bulletPrefab;
    // �ִϸ����� ������� �޾ƿ��� ����
    public Animator         _animator;
    // ������ ����
    private Vector3         _moveMent;
    // �÷��̾� �ӵ�
    private float           _speed;
    // �������ΰ�?
    private bool            _onAttack;
    // ��Ʈ���ΰ�?
    private bool            _onHit;
    // �������ΰ�?
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
        // ** Player�� Animator�� �޾ƿ´�
        Run();
        
        // ��Ʈ��Ű�� �Է��Ѵٸ�? ����, �� ������
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            CreateBullet();
            OnAttack();
        }
        // ����ƮŰ�� �Է��Ѵٸ� ��Ʈ
        if (Input.GetKey(KeyCode.LeftShift))
            OnHit();
        // �����̽� �ٸ� ������ �����Ѵ�.
        if (Input.GetKey(KeyCode.Space))
            OnJump();
    }

    private void SetState()
    {
        // Ȱ��ȭ ��ü �ʱ�ȭ
        _onAttack = false;
        _onHit    = false;
        _onJump   = false;       
    }

    private void CreateBullet()
    {
        if (_onAttack) return;
        // �Ѿ� ������ �����Ѵ�.
        GameObject obj = Instantiate(_bulletPrefab);
        // obj.transform.name = "";
        // �Ѿ��� �������� �÷��̾��� ��ġ�� �����Ѵ�.
        obj.transform.position = transform.position;
        // �Ѿ��� BulletControllor ��ũ��Ʈ�� �޾ƿ´�.
        BulletControllor controllor =  obj.GetComponent<BulletControllor>();
        // �Ѿ��� ���ư� ������ �÷��̾��� �������� �����ش�.
        controllor.Direction = _sprRen.flipX ? -transform.right : transform.right;
        // �Ѿ��� �̹��� ���� ���¸� �÷��̾��� �̹��� ���� ���·� �����Ѵ�.
        controllor.SetFlipY(_sprRen.flipX);
        // �ҷ� ����Ʈ�� �����Ѵ�.
        _bullets.Add(obj);
    }

    private void Run()
    {
        // ** Input.GetAxis =  -1, 1�� ���� ��ȯ
        // ** �Ǽ� ���� IEEE754
        float hor = Input.GetAxis("Horizontal");

        ChangeFlipXToHor(hor);

        // �Է¹��� ������ �÷��̾ �����δ�.
        _moveMent = new Vector3(hor * _speed, 0.0f, 0.0f);
        // �÷��̾��� �����ӿ� ���� �̵� ����� ���� �Ѵ�.
        _animator.SetFloat("Speed", Mathf.Abs(hor));
        // ���� �÷��̾ �����δ�.
        transform.position += _moveMent * Time.deltaTime;
    }

    // �÷��̾ �ٶ󺸰� �ִ� ���⿡ ���� ����
    private void ChangeFlipXToHor(float hor)
    {
        if      (hor < 0) _sprRen.flipX = true;
        else if (hor > 0) _sprRen.flipX = false;
    }

    private void OnHit()
    {
        // �̹� ��Ʈ ����� ��� ���̶�� �Լ��� �����Ų��.
        if (_onHit) return;

        // �Լ��� ������� �ʾҴٸ� ��Ʈ ���¸� Ȱ��ȭ�ϰ� ����� �����Ѵ�.
        _onHit = true;
        _animator.SetTrigger("Hit");
    }

    private void OnAttack()
    {
        // �̹� ���� ����� ��� ���̶�� �Լ��� �����Ų��.
        if (_onAttack) return;

        // �Լ��� ������� �ʾҴٸ� ���� ���¸� Ȱ��ȭ��Ű�� ����� �����Ѵ�.
        _onAttack = true;
        _animator.SetTrigger("Attack");
    }

    private void OnJump()
    {
        // �̹� �������̶�� �Լ��� �����Ų��.
        if (_onJump) return;
        // �Լ��� ������� �ʾҴٸ� ���� ���¸� Ȱ��ȭ��Ű�� ����� �����Ѵ�.
        _onJump = true;
        _animator.SetTrigger("Jump");
    }
    
    private void SetJump() { SetState(); }
    private void SetAttack() { _onAttack = false; }
    private void SetHit() { SetState(); }
}