using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class EnemyController : LivingObject
{
    private StateMachine<EnemyController> _state;
    protected GameObject _target;
    private float _searchDis;
    private float _attackDis;

    protected override void Init()
    {
        _target = GameObject.Find("Player").gameObject;
        _hp = 5;
        _state = new StateMachine<EnemyController>();
        _state.SetState(new IdleState());

        _searchDis = 8.0f;
        _attackDis = 1.0f;
    }
    protected override void Run()
    {
        base.Run();
    }
    protected override void CreateBullet()
    {

    }
    protected override void Die()
    {
        base.Die();
    }
    protected override void ObjUpdate() { _state.Update(this); }
    // TODO : HitAnimation재생
    protected internal override void CollisionAction(Collision2D obj)
    {
        if (LayerMask.LayerToName(obj.gameObject.layer) != "Bullet") return;
        --_hp;
    }
    private Vector3 RandomMovePosition()
    {
        int xDir = Random.Range(0, 2) == 0 ? -1 : 1;
        int yDir = Random.Range(0, 2) == 0 ? -1 : 1;

        Vector3 offset = new Vector3(Random.Range(0, 5), Random.Range(0.0f, 1.5f), 0.0f);

        return new Vector3(transform.position.x + offset.x * xDir, 0.0f + offset.y * yDir, 0.0f);
    }
}

public partial class EnemyController : LivingObject
{
    public enum ENEMY_STATE
    {
        IDLE,
        RUN,
        ATTACK,
        HIT
    }

    public class IdleState : State<EnemyController>
    {
        Vector3 _randPoint;
        bool _isMove;
        float _coolTime;
        public override void Enter(EnemyController t) 
        {
            _coolTime = Random.Range(0.0f, 3.0f);
            _isMove = false;
            base.Enter(t);
        } 
        public override void Update(EnemyController t) 
        {
            float distance = Constants.GetDistance(t._target.transform.position, t.transform.position);

            if (distance <= t._searchDis)
            {
                t._state.SetState(new TargetingState());
                return;
            }

            if (_coolTime > 0.0f)
            {
                _coolTime -= Time.deltaTime;
                return;
            }

            if (!_isMove)
            {
                _isMove = true;
                _randPoint = t.RandomMovePosition();
            }
            else
            {
                t._direction = (_randPoint - t.transform.position).normalized;
                float movePointDistance = Constants.GetDistance(_randPoint, t.transform.position);

                if (movePointDistance <= 1.0f)
                {
                    t._direction = Vector3.zero;
                    _coolTime = Random.Range(0.0f, 3.0f);
                    _isMove = false;
                }
            }
        }
        public override void Exit(EnemyController t) {}
        public IdleState() {}
    }
    public class TargetingState : State<EnemyController>
    {
        public override void Update(EnemyController t) 
        {
            Vector3 dir = (t._target.transform.position - t.transform.position).normalized;
            t._lookAt = dir;
            t._direction = dir;
        }
        public override void Exit(EnemyController t) { }
        public TargetingState() {}
    }
    public class AttackState : State<EnemyController>
    {
        public override void Enter(EnemyController t) { }
        public override void Update(EnemyController t) { }
        public override void Exit(EnemyController t) { }
        public AttackState() {}
    }
    public class HitState : State<EnemyController>
    {
        public override void Enter(EnemyController t) { }
        public override void Update(EnemyController t) { }
        public override void Exit(EnemyController t) { }
        public HitState() {}
    }
}