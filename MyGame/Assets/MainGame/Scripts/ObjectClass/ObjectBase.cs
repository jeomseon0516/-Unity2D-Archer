using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OBJECTID
{
    PLAYER,
    ENEMY,
    PENGUIN,
    BACKGROUND,
    FX
}

namespace OBJECT
{
    public abstract class ObjectBase : MonoBehaviour
    {
        protected OBJECTID _id;
        protected Animator _animator;
        protected SpriteRenderer _sprRen;
        protected SpriteRenderer _shadowSprRen;
        protected Rigidbody2D _rigidbody;
        protected Collider2D _bodyCollider;
        protected Collider2D _collider;
        protected Transform _colTransform;
        protected Transform _physics;
        protected Transform _body;
        protected Transform _shadow;
        protected Vector2 _originalColSize;
        protected Vector2 _originalShadowScale;
        protected Vector2 _direction;
        protected Vector2 _lookAt;
        protected Vector2 _shadowPos;
        protected Vector2 _size;
        protected float _heightOffset;
        protected float _offsetY;
        protected float _speed;
        protected int _beforeHp;
        protected int _maxHp;
        protected int _hp;
        protected int _atk;
        protected bool _isDie;

        protected float _beforeLocalY; // 이전 프레임의 로컬 Y 
        protected float _jumpValue;
        protected float _jump; //현재 점프값

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

            CheckInComponent(_shadow.TryGetComponent(out RectTransform rectT));
            _heightOffset = _shadowSprRen.size.y;

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
        protected internal virtual void CollisionAction(Collision2D obj) {}
        protected internal virtual void OnCollision(ObjectBase obj, Collider2D col) {}
        protected virtual void GetDamageAction(int damage) {}
        protected virtual void ObjFixedUpdate() {}
        protected virtual void ObjUpdate() {}
        protected virtual void Init() {}
        protected virtual void Run() {}
        protected virtual void Die() { DestroyObj(); }
        protected internal virtual void TriggerAction(Collider2D col) { TriggerCollision(col, _colTransform.gameObject); }
        private void FixedUpdate()
        {
            if (_hp < _beforeHp && !_isDie)
                GetDamageAction(_beforeHp - _hp);

            _beforeHp = _hp;
            _lookAt = _direction;
            CheckDeadToHp();
            ObjFixedUpdate();
            Run();
            Move(_direction.x, _direction.y);
            CheckHeight();
            ChangeFlipXToHor(_lookAt.x);
            SettingZNode();
        }
        protected internal void TriggerCollision(Collider2D col, GameObject colTransform) 
        {
            CheckInComponent(col.transform.parent.Find("Image").TryGetComponent(out ObjectBase obj));

            if (!CheckCollision(col.gameObject))
            {
                if (TriggerCollision(obj.GetPhysics(), obj))
                {
                    OnCollision(obj, col);
                    AddColList(col.gameObject); ;
                }
                else
                {
                    colTransform.SetActive(false);
                }
            }

            colTransform.SetActive(true);
        }
        // TODO : 점프때문에 GetPhysics사용해야함
        protected internal bool TriggerCollision(Transform targetPhysics, ObjectBase obj)
        {
            float targetPosY = targetPhysics.position.y - obj.GetOffsetY();
            float myPosY = _physics.position.y - _offsetY;

            float targetHeightOffset = obj.GetHeightOffset() * 0.5f;
            float myHeightOffset     = _heightOffset * 0.5f;

            if (targetPosY + targetHeightOffset > myPosY - myHeightOffset &&
                targetPosY - targetHeightOffset < myPosY + myHeightOffset)
            {
                //해당 타겟이 같은 y 좌표에 있는지 체크한다.
                //float jumptarGetPosY = obj.GetBody().localPosition.y - obj.GetOffsetY();
                //float jumpMyPosY     = _body.localPosition.y - _offsetY;

                //float targetSizeY = obj.GetSize().y * 0.5f;
                //float mySizeY     = _size.y * 0.5f;

                //if (jumptarGetPosY + targetSizeY > jumpMyPosY - mySizeY &&
                //    jumptarGetPosY - targetSizeY < jumpMyPosY + mySizeY) //해당 타겟이 점프중이며 같은 높이에 있는지 체크한다.
                    return true;
            }
            return false;
        }
        protected IEnumerator Jumping(float gravity = Constants.GRAVITY)
        {
            _collider.isTrigger = true;

            while (_body.localPosition.y >= 0)
            {
                _body.localPosition += new Vector3(0.0f, _jump, 0.0f) * Time.deltaTime;
                _jump -= gravity * Time.deltaTime;
                yield return YieldCache.WaitForFixedUpdate;
            }

            _jump = 0;
            _collider.isTrigger = false;
            _body.localPosition = new Vector2(0.0f, 0.0f);
        }
        /* 점프를 하지 않는 객체가 있으니 객체별로 따로 호출 해줍니다. */
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
        // 공격이 여러번 호출 되는 것을 위한 처리
        protected bool CheckCollision(GameObject colObj)
        {
            for (int i = 0; i < _colList.Count; ++i)
            {
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
        // 공격이 끝났을 때 호출
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
        private void CheckDeadToHp()
        {
            if (_hp > 0 || _isDie) return;
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
            if (ReferenceEquals(_shadowSprRen, null) || !_shadowSprRen) return;

            _sprRen.sortingOrder = (int)((_physics.position.y - _offsetY) * 10) * -1;
            _shadowSprRen.sortingOrder = _sprRen.sortingOrder - 1;
        }
        protected void UpdateShadowAndCollider()
        {
            float average = (_sprRen.bounds.max.x - _sprRen.bounds.min.x) / (_sprRen.localBounds.size.x * _sprRen.transform.lossyScale.x);

            _colTransform.localScale = new Vector2(_originalColSize.x * average, _originalColSize.y);
            _colTransform.localEulerAngles = transform.localEulerAngles;
        }
        private void CheckHeight() { _shadow.localPosition = new Vector2(_shadowPos.x, _shadowPos.y - _body.localPosition.y * 0.5f); }
        protected void CheckInComponent(bool inCom) { if (!inCom) print("해당 컴포넌트는 존재 하지 않습니다." + this.transform.root.name); }
        protected void AddColList(GameObject obj) { _colList.Add(obj); }
        protected void DestroyObj() { Destroy(_physics.gameObject); }
        protected void ClearColList() { _colList.Clear(); }
        public void TakeDamage(int damage) { _hp -= damage; }
        public Transform GetPhysics() { return _physics; }
        public Transform GetBody() { return _body; }
        public Vector2 GetSize() { return _size; }
        public float GetHeightOffset() { return _heightOffset; }
        public float GetOffsetY() { return _offsetY; }
        public int GetMaxHp() { return _maxHp; }
        public int GetHp() { return _hp; }
        public int GetAtk() { return _atk; }
    }
}