using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OBJECTID
{
    PLAYER,
    ENEMY,
    BACKGROUND,
    FX
}

namespace OBJECT
{
    public abstract class ObjectBase : MonoBehaviour
    {
        protected OBJECTID       _id;
        protected Animator       _animator;
        protected SpriteRenderer _sprRen;
        protected SpriteRenderer _shadowSprRen;
        protected Rigidbody2D    _rigidbody;
        protected Collider2D     _bodyCollider;
        protected Transform      _colTransform;
        protected Transform      _physics;
        protected Transform      _body;
        protected Transform      _shadow;
        protected Vector2        _originalColSize;
        protected Vector2        _originalShadowScale;
        protected Vector2        _direction;
        protected Vector2        _lookAt;
        protected Vector2        _shadowPos;
        protected float          _heightOffset;
        protected float          _offsetY;
        protected float          _speed;
        protected int            _hp;
        protected int            _beforeHp;
        protected int            _atk;
        protected bool           _isDie;

        // default 값 세팅
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprRen   = GetComponent<SpriteRenderer>();

            _body = transform.parent;

            _colTransform = _body.Find("Collider");
            _originalColSize = _body.localScale;

            _bodyCollider = _colTransform.GetComponent<Collider2D>();
            _bodyCollider.gameObject.AddComponent<ObjectPhysics>();

            _physics = _body.parent; // 가장 상위 트랜스폼
            _rigidbody = _physics.GetComponent<Rigidbody2D>(); // 

            _shadow       = _physics.Find("Shadow");
            _shadowSprRen = _shadow.GetComponent<SpriteRenderer>();
            _originalShadowScale = _shadow.localScale;

            _shadowPos = _shadow.localPosition;
            _lookAt    = new Vector2(0.0f, 0.0f);

            _offsetY = transform.position.y - _shadow.transform.position.y;
            _heightOffset = _shadow.GetComponent<RectTransform>().rect.height * _shadow.transform.localScale.y;

            _isDie = false;
            _hp    = 10;
            _atk   = 2;
            _speed = 2;

            Init();
        }
        private void Update() { ObjUpdate(); }
        private void OnTriggerEnter2D(Collider2D col) { TriggerAction(col); }
        protected internal virtual void TriggerAction(Collider2D col) {}
        protected internal virtual void CollisionAction(Collision2D obj) {}
        protected virtual void GetDamageAction(int damage) {}
        protected virtual void ObjFixedUpdate() {}
        protected virtual void ObjUpdate() {}
        protected virtual void Init() {}
        protected virtual void Run() {}
        protected virtual void Die() { DestroyObj(); }
        private void FixedUpdate()
        {
            if (_hp < _beforeHp && !_isDie)
                GetDamageAction(_beforeHp - _hp);

            _beforeHp = _hp;
            _lookAt   = _direction;

            UpdateShadowAndCollider();
            CheckDeadToHp();
            ObjFixedUpdate();
            Run();
            Move(_direction.x, _direction.y);
            CheckHeight();
            ChangeFlipXToHor(_lookAt.x);
            SettingZNode();
        }
        // TODO : 점프때문에 GetPhysics사용해야함
        protected internal bool TriggerCollision(Transform targetPhysics, ObjectBase obj)
        {
            float targetPosY = targetPhysics.position.y - obj.GetOffSetY();
            float myPosY     = _physics.position.y - _offsetY;

            if (targetPosY + obj.GetHeightOffset() > myPosY - _heightOffset && 
                targetPosY - obj.GetHeightOffset() < myPosY + _heightOffset)
                return true;

            return false;
        }
        private void CheckDeadToHp()
        {
            if (_hp > 0) return;
            Die();
            _isDie = true;
        }
        private void Move(float moveX, float moveY)
        {
            if (moveX == 0.0f && moveY == 0.0f) return;
            _rigidbody.MovePosition(_rigidbody.position + new Vector2(moveX * _speed, moveY * (_speed * 0.5f)) * Time.deltaTime);
        }
        private void ChangeFlipXToHor(float hor)
        {
            Quaternion rotation = _physics.rotation;

            if      (hor < 0.0f) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 180.0f, rotation.eulerAngles.z);
            else if (hor > 0.0f) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 0.0f,   rotation.eulerAngles.z);
                                                                                                     
            _physics.rotation = rotation;
        }
        private void SettingZNode() 
        { 
            _sprRen.sortingOrder = (int)((_shadow.position.y) * 10) * -1;
            _shadowSprRen.sortingOrder = _sprRen.sortingOrder - 1;
        }
        private void UpdateShadowAndCollider()
        {
            float average = (_sprRen.bounds.max.x - _sprRen.bounds.min.x) / (_sprRen.localBounds.size.x * _sprRen.transform.lossyScale.x);

            _colTransform.localScale = new Vector2(_originalColSize.x * average, _originalColSize.y);
            _colTransform.localEulerAngles = transform.localEulerAngles;
            _shadow.localScale = new Vector2(_originalShadowScale.x * average, _originalShadowScale.y);
        }
        private void CheckHeight() { _shadow.localPosition = new Vector2(_shadowPos.x, _shadowPos.y - _body.localPosition.y * 0.5f); }
        protected void DestroyObj() { Destroy(_physics.gameObject); }
        public void TakeDamage(int damage) { _hp -= damage; }
        public Transform GetPhysics() { return _physics; }
        public float GetHeightOffset() { return _heightOffset; }
        public float GetOffSetY() { return _offsetY; }
        public int GetHp() { return _hp; }
        public int GetAtk() { return _atk; }
    }
}