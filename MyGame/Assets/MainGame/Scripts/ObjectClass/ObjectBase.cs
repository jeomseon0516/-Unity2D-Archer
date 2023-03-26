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
        protected Transform      _shadow;
        protected Transform      _physics;
        protected Vector3        _direction;
        protected Vector3        _lookAt;
        protected Vector3        _shadowPos;
        protected float          _heightOffset;
        protected float          _offsetY;
        protected float          _speed;
        protected int            _hp;
        protected int            _beforeHp;

        // default 값 세팅
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _sprRen   = GetComponent<SpriteRenderer>();

            _physics = transform.parent;
            _physics.gameObject.AddComponent<ObjectPhysics>();

            _shadow = _physics.Find("Shadow");
            _shadowSprRen = _shadow.GetComponent<SpriteRenderer>();

            _shadowPos = _shadow.localPosition;
            _lookAt = new Vector3(0.0f, 0.0f, 0.0f);

            _offsetY = transform.position.y - _shadow.transform.position.y;
            _heightOffset = _shadow.gameObject.GetComponent<RectTransform>().rect.height * _shadow.transform.localScale.y;

            _hp    = 10;
            _speed = 2;

            Init();
        }
        private void OnTriggerEnter2D(Collider2D col) { TriggerAction(col); }
        protected internal virtual void TriggerAction(Collider2D col) {}
        protected virtual void Init() {}
        protected virtual void Run() {}
        protected internal virtual void CollisionAction(Collision2D obj) {}
        protected virtual void GetDamageAction(int damage) {}
        protected virtual void ObjUpdate() {}
        protected virtual void Die() 
        {
            Destroy(_physics.GetComponent<Collider2D>());
            if   (_animator == null) { DestroyObj(); }
            else { _animator.SetTrigger("Die"); }
        }
        private bool CheckDeadToHp()
        {
            if (_hp > 0) return false;

            Die();
            return true;
        }
        private void Update()
        {
            if (CheckDeadToHp()) return;

            if (_hp < _beforeHp)
                GetDamageAction(_beforeHp - _hp);

            _beforeHp = _hp;
            _lookAt = _direction;
            Run();
            ObjUpdate();
            Move(_direction.x, _direction.y);
            CheckHeight();
            ChangeFlipXToHor(_lookAt.x);
            SettingZNode();
        }
        private void Move(float moveX, float moveY)
        {
            if (moveX == 0.0f && moveY == 0.0f) return;
            _physics.position += new Vector3(moveX * _speed, moveY * (_speed * 0.5f), 0.0f) * Time.deltaTime;
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
        protected void DestroyObj() { Destroy(_physics.gameObject); }
        public void TakeDamage(int damage) { _hp -= damage; }
        private void CheckHeight() { _shadow.localPosition = new Vector3(_shadowPos.x, _shadowPos.y - transform.localPosition.y * 0.5f, 0.0f); }

        public float GetHeightOffSet() { return _heightOffset; }
        public float GetOffSetY()      { return _offsetY; }
    }
}