using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OBJECT
{
    public abstract class ObjectBase : MonoBehaviour
    {
        protected Vector2 _size, _shadowPos, _direction, _lookAt, _originalColSize, _originalShadowScale;
        protected Transform _physics, _colTransform, _body, _shadow;
        protected SpriteRenderer _sprRen, _shadowSprRen;
        protected float _heightOffset, _offsetY, _speed; // _heightOffset : 그림자 오프셋입니다.
        protected Collider2D _bodyCollider, _collider;
        protected int _beforeHp, _maxHp, _hp, _atk;
        protected Rigidbody2D _rigidbody;
        protected Animator _animator;
        protected OBJECTID _id;
        protected bool _isDie;

        protected float _beforeLocalY; // 이전 프레임의 로컬 Y 
        protected float _jumpValue; // 이전 프레임과 현재 프레임의 Y좌표 차
        protected float _jump; //현재 점프값

        private Vector2 _addForce;
        private Dictionary<string, IEnumerator> _coroutineList = new Dictionary<string, IEnumerator>();
        private List<GameObject> _colList = new List<GameObject>();

        // default 값 세팅
        protected virtual void Awake()
        {
            CheckInComponent(TryGetComponent(out _animator));
            CheckInComponent(TryGetComponent(out _sprRen));

            _body = transform.parent;

            _colTransform    = _body.Find("Size");
            _originalColSize = _body.localScale;

            CheckInComponent(_body.Find("Collider").TryGetComponent(out _collider));

            CheckInComponent(_colTransform.TryGetComponent(out _bodyCollider));
            _bodyCollider.gameObject.AddComponent<ObjectPhysics>();

            _physics = _body.parent; // 가장 상위 트랜스폼
            CheckInComponent(_physics.TryGetComponent(out _rigidbody));

            _shadow = _physics.Find("Shadow");
            CheckInComponent(_shadow.TryGetComponent(out _shadowSprRen));
            _originalShadowScale = _shadow.localScale;

            _shadowPos = _shadow.localPosition;
            _lookAt = new Vector2(0.0f, 0.0f);

            _offsetY = _physics.position.y - _shadow.position.y;
            _heightOffset = _shadowSprRen.size.y;

            _addForce = Vector2.zero;
            Transform size = _body.Find("Size");

            CheckInComponent(size.TryGetComponent(out BoxCollider2D box));
            _size = box.size;

            _isDie = false;
            _hp = 10;
            _atk = 2;
            _speed = 2;

            Init();
        }
        private void Update() { ObjUpdate(); }
        private void OnTriggerEnter2D(Collider2D col) { TriggerAction(col); }
        protected internal virtual void TriggerAction(Collider2D col) { TriggerCollision(col, _colTransform.gameObject); }
        protected internal virtual void CollisionAction(Collision2D obj) {}
        protected virtual void OnCollision(ObjectBase obj, Collider2D col) {}
        protected virtual void GetDamageAction(int damage) {}
        protected virtual void ObjFixedUpdate() {}
        protected virtual void ObjUpdate() {}
        protected virtual void Init() {}
        protected virtual void Run() {}
        protected virtual void Die() { DestroyObj(); }
         /*
            해당 함수 안에서 호출되는 함수들은 오브젝트 베이스를 상속받는 객체들이 기본적으로 동작해야하는 함수들을 넣어놨습니다. 
            해당 함수들은 상속받은 객체에서는 따로 호출이 불가능합니다.
         */
        private void FixedUpdate()
        {
            if (_hp < _beforeHp && !_isDie)
                GetDamageAction(_beforeHp - _hp);

            _beforeHp = _hp;
            _lookAt = _direction;
            CheckDeadToHp();
            Run();
            ObjFixedUpdate();
            Move(_direction.x, _direction.y);
            CheckHeight();
            ChangeFlipXToHor(_lookAt.x);
            SettingZNode();
        }
        protected internal void TriggerCollision(Collider2D col, GameObject colTransform) 
        {
            if (CheckCollision(col.gameObject)) return;

            CheckInComponent(col.transform.parent.Find("Image").TryGetComponent(out ObjectBase obj));

            if (Collision(obj.GetPhysics(), obj))
            {
                OnCollision(obj, col);
                AddColList(col.gameObject);
            }
            else
                colTransform.SetActive(false); // 껐다 키면 TriggerEnter 재호출

            colTransform.SetActive(true);
        }
        protected bool Collision(Transform targetPhysics, ObjectBase obj)
        {
            float targetPosY = targetPhysics.position.y - obj.GetOffsetY();
            float myPosY = _physics.position.y - _offsetY;

            float targetHeightOffset = obj.GetHeightOffset() * 0.5f;
            float myHeightOffset     = _heightOffset * 0.5f;

            bool isCollision = targetPosY + targetHeightOffset > myPosY - myHeightOffset &&
                               targetPosY - targetHeightOffset < myPosY + myHeightOffset;

            return isCollision;
        }
        protected IEnumerator Jumping(float gravity = Constants.GRAVITY)
        {
            _collider.isTrigger = true;

            while (_body.localPosition.y >= 0)
            {
                _body.localPosition += new Vector3(0.0f, _jump, 0.0f) * Time.fixedDeltaTime;
                _jump -= gravity * Time.fixedDeltaTime;
                yield return YieldCache.WaitForFixedUpdate;
            }

            _jump = 0.0f;
            _collider.isTrigger = false;
            _body.localPosition = Vector2.zero;
        }
        /* 점프를 하지 않는 객체가 있으니 객체별로 따로 호출 해줍니다. */
        public virtual void OnAttackBox(float isOn)
        {
            bool on = isOn > 0.0f ? true : false;
            _colTransform.gameObject.SetActive(on);
        }
        protected IEnumerator CheckFallingOrJumping()
        {
            while (true)
            {
                yield return YieldCache.WaitForFixedUpdate;
                _jumpValue = _body.localPosition.y - _beforeLocalY;
                _beforeLocalY = _body.localPosition.y;
            }
        }
        protected IEnumerator FadeOutObject()
        {
            Color keepColor = _sprRen.color;

            while (_sprRen.color.a > float.Epsilon)
            {
                _sprRen.color = new Color(keepColor.r, keepColor.g, keepColor.b, _sprRen.color.a - 0.01f);
                yield return YieldCache.WaitForFixedUpdate;
            }

            DestroyObj();
        }
        /*
         * 해당 함수를 통해서만 _addForce에 접근 할 수 있습니다.
         */
        private IEnumerator CoroutineAddForce(Vector2 force, float limit)
        {
            _addForce += limit < float.Epsilon ? force : 
                         new Vector2(Mathf.Abs(_addForce.x) > limit ? 0.0f : force.x, 
                                     Mathf.Abs(_addForce.y) > limit ? 0.0f : force.y); // addForce는 중첩되서 들어올 수 있으므로

            while (Mathf.Abs(_addForce.x) > float.Epsilon ||
                   Mathf.Abs(_addForce.y) > float.Epsilon)
            {
                // 빼야할 값
                float xDir = _addForce.x > float.Epsilon ? 1 : -1;
                float yDir = _addForce.y > float.Epsilon ? 1 : -1;

                float addForceX = Mathf.Abs(_addForce.x);
                float addForceY = Mathf.Abs(_addForce.y);

                addForceX -= 2.0f * Time.fixedDeltaTime;
                addForceY -= 2.0f * Time.fixedDeltaTime;

                _addForce = new Vector2(addForceX * xDir, addForceY * yDir);

                yield return YieldCache.WaitForFixedUpdate;
            }
        }
        protected void AddForce(Vector2 force, float limit = 0.0f)
        {
            AddAfterResetCoroutine("AddForce", CoroutineAddForce(force, limit));
        }
        // 하나의 객체에 공격이 여러번 호출 되는 것을 위한 처리
        protected bool CheckCollision(GameObject colObj)
        {
            for (int i = 0; i < _colList.Count; ++i)
            {
                // ColList에 있는 오브젝트가 죽어서 삭제되었다면 제거 해준다.
                if (ReferenceEquals(_colList[i], null) || !_colList[i])
                {
                    _colList.RemoveAt(i);
                    --i;
                    continue;
                }
                if (_colList[i].Equals(colObj)) 
                    return true;
            }

            return false;
        }
        /* 
           리스트로 관리해야 하는 코루틴을 해당 함수로 호출합니다.
           하나의 코루틴이 여러개가 호출되는 걸 방지하기 위한 함수입니다.
           스크립트 내에서 하나씩만 동작해야하는 코루틴은 해당 함수를 통해 호출 합니다.
         */
        protected void AddAfterResetCoroutine(string key, IEnumerator coroutine)
        {
            if (_coroutineList.ContainsKey(key))
            {
                StopCoroutine(_coroutineList[key]);
                _coroutineList[key] = coroutine;
            }
            else
                _coroutineList.Add(key, coroutine);

            StartCoroutine(_coroutineList[key]);
        }
        /*
         * 정지만 시켜주어야 하는 코루틴을 호출합니다.
         */
        protected void FindCoroutineStop(string key)
        {
            if (!_coroutineList.TryGetValue(key, out IEnumerator coroutine)) return;
            StopCoroutine(coroutine);
        }
        /*
        * 중간에 회전이 되는 객체들에서 사용하는 함수입니다. 플레이어 총알에 사용됩니다. 
        * 그림자와 콜라이더를 객체가 회전하는 만큼 함께 회전 시켜줍니다.
        */
        protected void UpdateCollider()
        {
            float average = (_sprRen.bounds.max.x - _sprRen.bounds.min.x) / (_sprRen.localBounds.size.x * _sprRen.transform.lossyScale.x);

            _colTransform.localScale = new Vector2(_originalColSize.x * average, _originalColSize.y);
            _colTransform.localEulerAngles = transform.localEulerAngles;
        }
        private void CheckDeadToHp()
        {
            if (_hp > 0 || _isDie) return;
            Die();
            _isDie = true;
        }
        /* 
         * 오브젝트 베이스를 상속받는 모든 객체는 해당 함수로 움직입니다.. _direction에 정규화 시킨 벡터를 넣어주면 해당 방향으로 움직입니다.
         * 정규화가 아닌 벡터를 넣어주면 해당 크기만큼 빠르게 움직입니다.
         * AddForce를 통해서 힘을 받은 방향으로 밀립니다.
         */
        private void Move(float moveX, float moveY)
        {
            _rigidbody.MovePosition(_rigidbody.position + (new Vector2(moveX * _speed, moveY * (_speed * 0.5f)) + 
                                                           new Vector2(_addForce.x, _addForce.y * 0.5f)) * Time.deltaTime);
        }
        /*
         * 방향에 맞게 플립시켜줍니다.
         */
        private void ChangeFlipXToHor(float hor)
        {
            Quaternion rotation = _physics.rotation;

            if      (hor < 0.0f) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 180.0f, rotation.eulerAngles.z);
            else if (hor > 0.0f) rotation.eulerAngles = new Vector3(rotation.eulerAngles.x, 0.0f,   rotation.eulerAngles.z);

            _physics.rotation = rotation;
        }
        /*
         * 어떤 객체를 먼저 드로우 할지 결정하는 함수입니다. y좌표로 구합니다.
         */
        private void SettingZNode()
        {
            if (ReferenceEquals(_shadowSprRen, null) || !_shadowSprRen) return;

            _sprRen.sortingOrder = (int)((_physics.position.y - _offsetY) * 10) * -1;
            _shadowSprRen.sortingOrder = _sprRen.sortingOrder - 1;
        }
        public void TakeDamage(int damage, Vector2 force)
        {
            _hp -= damage;

            if (_physics.name.Contains("Boss"))
                return;

            ZeroForce();
            AddForce(force);
        }
        public void TakeDamage(int damage) { _hp -= damage; }
        /*
            공중에 띄워진만큼 그림자를 멀어지게 합니다.
         */
        private void CheckHeight() { _shadow.localPosition = new Vector2(_shadowPos.x, _shadowPos.y - _body.localPosition.y * 0.5f); }
        /*
         * TryGetComponent로 불러와지는 것들을 위해 만든 함수.
         */
        protected void CheckInComponent(bool inCom) { if (!inCom) print("해당 컴포넌트는 존재 하지 않습니다." + this.transform.root.name); }
        protected void AddColList(GameObject obj) { _colList.Add(obj); }
        protected void DestroyObj() { Destroy(_physics.gameObject); }
        protected void ClearColList() { _colList.Clear(); }
        protected void ZeroForce() { _addForce = Vector2.zero; }
        public Transform GetPhysics() { return _physics; }
        public Transform GetBody() { return _body; }
        public Vector2 GetSize() { return _size; }
        public float GetHeightOffset() { return _heightOffset; }
        public float GetOffsetY() { return _offsetY; }
        public int GetMaxHp() { return _maxHp; }
        public int GetHp() { return _hp; }
        public int GetAtk() { return _atk; }
        public bool GetIsDie() { return _isDie; }
    }
}