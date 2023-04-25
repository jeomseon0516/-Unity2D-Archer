using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PUB_SUB;

/*
 * 코드 가독성을 위해 partial키워드로 분리
 */
namespace OBJECT
{
    /*
        _direction은 PlayerManager에서 받아옵니다. PlayerManager에서 PlayerCharacter를 참조하여 SetDirection으로 받아오는 식
        마찬가지로 키입력은 InputManager또는 PlayerManager에서 받아오는 식으로 구현 합니다.
    */
    public partial class PlayerController : LivingObject
    {
        // skillRountine..
        /*
            Player -> 랜덤으로 배열에 스킬 생성 0 = Q, 1 = E, 2 = R에 대응(0번 인덱스부터 순서대로 채워짐) -> 
            PlayerManager -> 플레이어가 배열에 스킬을 가지고 있는지 체크(어떤 스킬 인지까지) ->
            PlayerManager -> 스킬이 있다면 미리 캐싱해둔 스킬UI 큐에서 화면 상에 보이는 InGameSkillUI리스트에 push
            PlayerManager -> Q를 누르면 플레이어 스킬 트리거 인자 값에 0을 전달 (E : 1, R : 2) ->
            Player -> Trigger 발동 다음 프레임에 Trigger해제 -> 
            Trigger -> 상태를 다루는 클래스에서 Trigger 체크 -> 
            만약 Trigger가 true이고 현재 플레이어가 배열안에 스킬을 보유중인가? true : 스킬 발동, false : 그냥 return ->
            PlayerManager -> Player가 스킬을 사용했는지 체크, 사용했다면 해당 스킬칸에 대응하는 UI에 스킬을 사용했다고 전달
            UI -> FadeOut후 active = false ->
            ...반복
            
            해당 방식은 번거롭지만 객체지향의 규칙을 지키기위한 설계
            자동차와 자동차의 운전자를 따로 나눈 방식
            사용자는 플레이어를 운전하며 조작하고 관찰하면서 관찰한 정보를 필요한 이들에게 전달하는 매니저 클래스
         */

        private StateMachine<PlayerController> _playerState;
        private GameObject _gronudSmoke;
        private GameObject _dogSkill;
        private Vector2 _spawnPoint;
        private Vector2 _fireDirection;
        private float _defaultSpeed;
        private float _attackSpeed;
        private int _stamina, _maxStamina;
        private int _jumpCount;
        private bool _onRadialForm;

        // 트리거는 매프레임마다 갱신되고 다음프레임까지 트리거에 해당하는 동작이 수행되지 않으면 자동으로 false 전환
        private Dictionary<string, bool> _actionTrigger = new Dictionary<string, bool>();
        private string[] _skillList = new string[4];
        private int _skillIndex;

        protected override void Init()
        {
            _id = OBJECTID.PLAYER;
            base.Init();

            _stamina = _maxStamina = 30;
            _maxHp = _hp = 500;
            _attackSpeed = 4;
            _defaultSpeed = _speed = 5.0f;
            _atk = 2;

            for (int i = 0; i < 3; ++i)
                _skillList[i] = "";

            _playerState = new StateMachine<PlayerController>();
            _playerState.SetState(new RunState());

            _gronudSmoke = ResourcesManager.GetInstance().GetObjectToKey(_id, "GroundSmoke");
            _dogSkill = ResourcesManager.GetInstance().GetObjectToKey(_id, "DogSkill");

            _spawnPoint = _physics.position;
            _jumpCount = 0;

            _actionTrigger.Add("Jump", false);
            _actionTrigger.Add("Dash", false);
            _actionTrigger.Add("Dog", false);
            _actionTrigger.Add("Continuous", false);
            _actionTrigger.Add("RadialForm", false);

            _onRadialForm = false;

            AddAfterResetCoroutine("CheckFalling", CheckFallingOrJumping());
            AddAfterResetCoroutine("AddStamina", AddStamina());

            StartCoroutine(RandomSkillPush());
        }
        private IEnumerator RandomSkillPush()
        {
            while (true)
            {
                yield return YieldCache.WaitForSeconds(Random.Range(2.0f, 4.0f));

                for (int i = 0; i < 3; ++i)
                {
                    if (!string.IsNullOrEmpty(_skillList[i])) continue;

                    int randomSkill = Random.Range(0, 3);

                    switch (randomSkill)
                    {
                        case 0:
                            _skillList[i] = "Dog";
                            break;
                        case 1:
                            _skillList[i] = "Continuous";
                            break;
                        case 2:
                            _skillList[i] = "RadialForm";
                            break;
                    }
                    break;
                }

            }
        }
        public void FromIndexToSkillAction(int index)
        {
            if (index < 0 || index >= 3 || string.IsNullOrEmpty(_skillList[index])) return;

            string skillName = _skillList[index];

            StartCoroutineTrigger(skillName, skillName);
            // 해당 인덱스(keyCode를 저장)
            _skillIndex = index;
        }
        // 다음 프레임에 스킬의 트리거를 false로 바꿔준다. 상태에 따라서 스킬이 입력 되지 않을 수 있기 때문에
        private IEnumerator SetActionTrigger(string key)
        {
            if (!_actionTrigger.ContainsKey(key)) yield break; // 해당 키 값이 존재하지 않는다면

            _actionTrigger[key] = true;
            yield return YieldCache.WaitForFixedUpdate;
            _actionTrigger[key] = false;
        }
        private bool GetActionTrigger(string key)
        {
            if (!_actionTrigger.TryGetValue(key, out bool trigger)) return false;
            return trigger;
        }
        protected void CreateBullet(GameObject bullet, Vector2 position, Vector2 direction)
        {
            Transform objTransform = Instantiate(bullet).transform;
            objTransform.position = position;

            //....
            CheckInComponent(objTransform.Find("Body").Find("Image").TryGetComponent(out BulletBase controller));
            // 현재 방향키를 어떤 방향으로 누르고 있는지를 확인해서 쏠 방향을 구하고 이동중인 방향만큼 더 해주면 총알이 플레이어의 움직임의 영향을 받게 된다.
            controller.SetDirection(direction);
        }
        private void CreateArrow()
        {
            if (_onRadialForm)
            {
                DefaultBullet.CreateRadialForm(_bullet, _lookAt.normalized + _direction * 0.25f, _rigidbody.position, 6);
                _onRadialForm = false;
            }
            else
                CreateBullet(_bullet, _rigidbody.position, _lookAt.normalized + _direction * 0.25f);
        }
        private IEnumerator Respawn()
        {
            yield return YieldCache.WaitForSeconds(5.0f);

            _animator.SetTrigger("Respawn");
            ResetPlayer();
        }
        private IEnumerator AddStamina()
        {
            while (true)
            {
                if (_stamina < _maxStamina)
                    ++_stamina;

                yield return YieldCache.WaitForSeconds(0.5f);
            }
        }
        protected override void ObjFixedUpdate()
        {
            if (_isDie)
                _lookAt = _direction = Vector2.zero;

            _animator.SetFloat("JumpSpeed", _jumpValue);
            _playerState.Update(this);
        }
        private bool PlayDash()
        {
            if (!GetActionTrigger("Dash") || _stamina < 10) return false;

            _playerState.SetState(new DashState());
            return true;
        }
        public void ResetPlayer()
        {
            AddAfterResetCoroutine("CheckFalling", CheckFallingOrJumping());
            AddAfterResetCoroutine("AddStamina", AddStamina());
            StartCoroutine(RandomSkillPush());

            _playerState.SetState(new RunState());

            _rigidbody.position = _spawnPoint;

            _isDie = false;
            _hp = _maxHp;
        }
        public bool OnStartJump()
        {
            if (!GetActionTrigger("Jump") && _body.localPosition.y < float.Epsilon) return false; // 스페이스 바를 누르거나 공중에 띄워져있을때

            _playerState.SetState(new JumpState());
            return true;
        }
        private void OnAttack(float horizontal, float vertical)
        {
            if (Mathf.Abs(horizontal) < float.Epsilon && Mathf.Abs(vertical) < float.Epsilon) return;

            SetFromDirectionLookAt(horizontal, vertical);
            _playerState.SetState(new AttackState());
        }
        private void SetFromDirectionLookAt(float horizontal, float vertical)
        {
            Vector2 attackDir = new Vector2(horizontal, vertical);
            attackDir.x = horizontal == 0 ? _direction.x : horizontal;

            _lookAt = attackDir;
        }
        private void OnSkill(string skillName)
        {
            if (!GetActionTrigger(skillName)) return;

            FindFromIndexToSkill(skillName, _skillIndex);

            switch (skillName)
            {
                case "Dog":
                    _playerState.SetState(new DogAttackState());
                    break;
                case "Continuous":
                    _playerState.SetState(new ContinuousAttackState());
                    break;
                case "RadialForm":
                    _onRadialForm = true;
                    _playerState.SetState(new RadialFormAttackState());
                    break;
            }
        }
        private void FindFromIndexToSkill(string skillName, int index)
        {
            string skillValue = _skillList[index];

            if (!skillValue.Contains(skillName))
                return;

            skillValue = "";
            _skillList[index] = skillValue;
        }
        private void CreateGroundSmoke()
        {
            Transform groundSmoke = Instantiate(_gronudSmoke).transform;

            float xDir = _direction.x != 0 ? _direction.x < 0 ? 1 : -1 :
                                             _physics.rotation.eulerAngles.y == 180.0f ? 1 : -1;
            // Rotation
            Quaternion rotation = groundSmoke.rotation;
            rotation.eulerAngles = new Vector2(rotation.x, xDir == 1 ? 180.0f : 0.0f);
            groundSmoke.rotation = rotation;

            // Position
            Vector2 position = new Vector2(_body.position.x, _body.position.y - _offsetY);
            groundSmoke.position = position + new Vector2(0.5f * xDir, 0.0f);
        }
        private void CheckSkillsAfterAction()
        {
            OnSkill("Dog");
            OnSkill("Continuous");
            OnSkill("RadialForm");
        }
        private Vector2 GetFromAngleAndSpeedToDirection() { return CheckMovePlayer() ? _direction.normalized : GetFromAngleToDirection(); }
        private Vector2 GetFromAngleToDirection() { return _physics.rotation.eulerAngles.y == 180.0f ? Vector2.left : Vector2.right; }
        private bool CheckMovePlayer() { return Mathf.Abs(_direction.x) > float.Epsilon || Mathf.Abs(_direction.y) > float.Epsilon; }
        protected override void GetDamageAction(int damage) { _playerState.SetState(new HitState()); }
        protected override void Die() { _playerState.SetState(new DieState()); }
        public string GetFromIndexToSkillListValue(int index) { return string.IsNullOrEmpty(_skillList[index]) || index < 0 ? "" : _skillList[index]; }
        public void StartCoroutineTrigger(string name, string key) { AddAfterResetCoroutine(name, SetActionTrigger(key)); }
        public void SetFireDirection(Vector2 fireDirection) { _fireDirection = fireDirection; }
        public void SetDirection(Vector2 direction) { _direction = direction; }
        public int GetStamina() { return _stamina; }
        public int GetMaxStamina() { return _maxStamina; }
    }
    public partial class PlayerController : LivingObject
    {
        public enum PLR_STATE
        {
            RUN,
            JUMP,
            DOUBLEJUMP,
            DASH,
            ATTACK,
            CONTINUOUS_ATTACK,
            RADIAL_FORM_ATTACK,
            DOGATTACK,
            HIT,
            DIE
        }

        /* 해당 함수는 하이어라키에서 애니메이션 이벤트로 호출되는 함수 입니다. 스크립트 내에서 상태 전환이 필요한 경우 new 키워드를 사용해 초기화 합니다. */
        private void SetState(PLR_STATE state)
        {
            if (_isDie) return;

            switch (state)
            {
                case PLR_STATE.RUN:
                    _playerState.SetState(new RunState());
                    break;
                case PLR_STATE.JUMP:
                    _playerState.SetState(new JumpState());
                    break;
                case PLR_STATE.ATTACK:
                    _playerState.SetState(new AttackState());
                    break;
                case PLR_STATE.HIT:
                    _playerState.SetState(new HitState());
                    break;
                case PLR_STATE.DIE:
                    _playerState.SetState(new DieState());
                    break;
            }
        }
        // Run과 Idle은 결론적으로 같은 동작을 수행하므로 따로 처리하지 않는다.
        public sealed class RunState : State<PlayerController>
        {
            public override void Update(PlayerController t)
            {
                if (t.OnStartJump() || t.PlayDash()) return;

                float speed = t.GetFromDirectionToSpeed(t._direction);

                t._animator.speed = speed > 0.0f ? speed : 1;

                float x = t._fireDirection.x;
                float y = t._fireDirection.y;

                t.OnAttack(x, y);
                t.CheckSkillsAfterAction();
            }
            public override void Exit(PlayerController t) { t._animator.speed = 1; }
        }
        public sealed class HitState : State<PlayerController>
        {
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t._animator.SetTrigger("Hit");
            }
            public override void Update(PlayerController t) { if (t.PlayDash()) return; }
        }
        public sealed class JumpState : State<PlayerController>
        {
            float _jump;
            bool _onSpace; // 스페이스바를 누르고 있는가..
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                _jump = 8.0f;

                t._jump = t._body.localPosition.y < float.Epsilon ? _jump : t._jump;
                _onSpace = t._jump < float.Epsilon ? false : true;

                t.AddAfterResetCoroutine("Jump", t.Jumping());
            }
            public override void Update(PlayerController t)
            {
                if (t.PlayDash()) return;

                AddJumpPowerOnSpace(t);

                if (t._body.localPosition.y < float.Epsilon)
                {
                    t._playerState.SetState(new RunState());
                    t._jumpCount = 0;
                    return;
                }
                if (Input.GetKeyDown(KeyCode.Space) && t._jumpCount <= 0)
                    t._playerState.SetState(new DoubleJumpState());
            }
            private void AddJumpPowerOnSpace(PlayerController t)
            {
                if (t.GetActionTrigger("Jump") || !_onSpace || t._jumpValue < float.Epsilon) return;

                t._jump = t._jump * 0.5f;
                _onSpace = false;
            }
        }
        public sealed class DoubleJumpState : State<PlayerController>
        {
            Vector2 _saveDirection;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                _saveDirection = t.GetFromAngleAndSpeedToDirection();

                ++t._jumpCount;
                t._animator.SetTrigger("DoubleJump");

                t._jump += 6.0f;
                t._speed = 10.0f;
            }
            public override void Update(PlayerController t)
            {
                t._lookAt = t._direction = _saveDirection;

                if (t._body.localPosition.y < float.Epsilon)
                    t._playerState.SetState(new RunState());
            }
            public override void Exit(PlayerController t) { t._speed = t._defaultSpeed; }
        }
        public sealed class DashState : State<PlayerController>
        {
            Vector2 _saveDirection;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                _saveDirection = t.GetFromAngleAndSpeedToDirection();

                t._stamina -= 10;

                if (t._stamina < 0)
                    t._stamina = 0;

                t._animator.SetTrigger("Dash");
                t._speed = t._defaultSpeed * 3;
                t._jump = 0.0f;

                t.CreateGroundSmoke();
                t.FindCoroutineStop("Jump");
            }
            public override void Update(PlayerController t) { t._lookAt = t._direction = _saveDirection; }
            public override void Exit(PlayerController t) { t._speed = t._defaultSpeed; }
        }
        public class AttackState : State<PlayerController>
        {
            protected Vector2 _saveLookAt;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                t._animator.SetTrigger("Attack");

                t._animator.speed = t._attackSpeed;
                t._speed = 3.5f;

                float x = t._fireDirection.x;
                float y = t._fireDirection.y;

                Vector2 dir = new Vector2(x, y);
                Vector2 angleToDir = t.GetFromAngleToDirection();

                t._lookAt = _saveLookAt = dir != Vector2.zero ? dir : new Vector2(x != 0 ? x : angleToDir.x, y != 0 ? y : angleToDir.y);
            }
            public override void Update(PlayerController t)
            {
                if (t.OnStartJump() || t.PlayDash()) return;

                UpdateLookAt(t);
                t.CheckSkillsAfterAction();
            }
            public override void Exit(PlayerController t)
            {
                t._animator.speed = 1;
                t._speed = t._defaultSpeed;

                UpdateLookAt(t);
            }
            protected void UpdateLookAt(PlayerController t) { t._lookAt = t._fireDirection != Vector2.zero ? _saveLookAt = t._fireDirection : _saveLookAt; }
        }
        public sealed class ContinuousAttackState : State<PlayerController>
        {
            int _count;
            Vector2 _saveLookAt;

            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                t._animator.SetTrigger("Attack");
                t._animator.speed = 10;

                t._speed = 3.5f;
                _count = 5;

                t._lookAt = _saveLookAt = t._fireDirection != Vector2.zero ? t._fireDirection : t.GetFromAngleToDirection();
            }
            public override void Update(PlayerController t) { t._lookAt = _saveLookAt; }
            public override void Exit(PlayerController t)
            {
                t._lookAt = _saveLookAt = t._fireDirection != Vector2.zero ? t._fireDirection : t.GetFromAngleToDirection();

                if (--_count > 0)
                {
                    t._animator.SetTrigger("Attack");
                    t._playerState.SetState(this);
                }
                else
                {
                    t._animator.speed = 1;
                    t._lookAt = _saveLookAt;
                    t._speed = t._defaultSpeed;
                }
            }
        }
        public sealed class RadialFormAttackState : AttackState
        {
            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t._lookAt = _saveLookAt = t._fireDirection == Vector2.zero ? t.GetFromAngleToDirection() : t._fireDirection;
            }
            public override void Update(PlayerController t) { UpdateLookAt(t); }
            public override void Exit(PlayerController t)
            {
                UpdateLookAt(t);

                if (t._onRadialForm)
                    t._playerState.SetState(this);
                else
                    base.Exit(t);
            }
        }
        public sealed class DogAttackState : State<PlayerController>
        {
            Vector2 _saveLookAt;
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                t._animator.SetTrigger("DogSkill");
                _saveLookAt = t.GetFromAngleToDirection();

                t.CreateBullet(t._dogSkill, t._rigidbody.position, _saveLookAt * 0.25f);
                t._speed = t._defaultSpeed * 0.5f;
            }
            public override void Update(PlayerController t)
            {
                if (t.OnStartJump() || t.PlayDash()) return;
                t._lookAt = _saveLookAt;
            }
            public override void Exit(PlayerController t) { t._speed = 5.0f; }
        }
        public sealed class DieState : State<PlayerController>
        {
            public override void Enter(PlayerController t)
            {
                base.Enter(t);

                t._animator.SetTrigger("Die");
                t._direction = Vector2.zero;

                t.StartCoroutine(t.Respawn());
            }
            public override void Update(PlayerController t) { t._direction = Vector2.zero; }
        }
    }
}