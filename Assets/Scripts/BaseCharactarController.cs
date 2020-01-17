using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharactarController : MonoBehaviour
{
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(100.0f, 50.0f);

    [System.NonSerialized] public float hpMax = 10.0f;          // 최대 체력
    [System.NonSerialized] public float hp = 10.0f;             // 체력
    [System.NonSerialized] public float dir = 1.0f;             // 바라보는 방	
    [System.NonSerialized] public float speed = 6.0f;           // 이동속도
    [System.NonSerialized] public float basScaleX = 1.0f;       // 가로크
    [System.NonSerialized] public bool activeSts = false;       // 플레이 가능 상태
    [System.NonSerialized] public bool jumped = false;          // 현재 프레임의 점프 상태
    [System.NonSerialized] public bool grounded = false;        // 현재 프레임의 접지 상태
    [System.NonSerialized] public bool groundedPrev = false;    // 이전 프레임의 접지 상태

    [System.NonSerialized] public Animator animator;
    protected Transform groundCheck_L;
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    protected float speedVx = 0.0f; 
    protected float speedVxAddPower = 0.0f;
    protected float gravityScale = 10.0f;      
    protected float jumpStartTime = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;

    protected virtual void Awake() {
        animator = GetComponent<Animator>();

        groundCheck_L = transform.Find("GroundCheck_L");
        groundCheck_C = transform.Find("GroundCheck_C");
        groundCheck_R = transform.Find("GroundCheck_R");

        dir = (transform.localScale.x > 0.0f) ? 1 : -1;
        basScaleX = transform.localScale.x * dir;
        transform.localScale = new Vector3(basScaleX, transform.localScale.y, transform.localScale.z);

        activeSts = true;
        gravityScale = GetComponent<Rigidbody2D>().gravityScale;
    }

    protected virtual void Start() {

    }

    protected virtual void Update()
    {

    }

    protected virtual void FixedUpdate()
    {
        //떨어지면 죽
        if (transform.position.y < -30.0f) {
            Dead(false);
        }

        groundedPrev = grounded;
        grounded = false;

        groundCheck_OnEnemyObject = null;
        groundCheck_OnMoveObject = null;
        groundCheck_OnRoadObject = null;

        Collider2D[][] groundCheckCollider = new Collider2D[3][];

        groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
        groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
        groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

        foreach (Collider2D[] groundCheckList in groundCheckCollider) {
            foreach (Collider2D groundCheck in groundCheckList) {
                if (groundCheck != null) {
                    grounded = true;
                    if (groundCheck.tag == "Road") {
                        groundCheck_OnRoadObject = groundCheck.gameObject;
                    } else if (groundCheck.tag == "MoveObject") {
                        groundCheck_OnMoveObject = groundCheck.gameObject;
                    } else if (groundCheck.tag == "Enemy") {
                        groundCheck_OnEnemyObject = groundCheck.gameObject;
                    }
                }
            }
        }

        FixedUpdateCharacter();

        // 속도 최댓값 최솟값 보
        GetComponent<Rigidbody2D>().velocity = new Vector2(speedVx, GetComponent<Rigidbody2D>().velocity.y);

        float vx = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.y, velocityMin.y, velocityMax.y);

        GetComponent<Rigidbody2D>().velocity = new Vector2(vx, vy);
    }

    protected virtual void FixedUpdateCharacter() {

    }

    public virtual void ActionMove(float n) {
        if (n != 0.0f)
        {
            dir = Mathf.Sign(n);
            speedVx = speed * n;

            animator.SetTrigger("Run");
        }
        else {
            speedVx = 0;
            animator.SetTrigger("Idle");
        }
    }

    public virtual void Dead(bool gameOver) {
        if (!activeSts) {
            return;
        }

        activeSts = false;
        animator.SetTrigger("Dead");
    }

    public virtual bool SetHp(float _hp, float _hpMax) {
        hp = _hp;
        hpMax = _hpMax;
        return (hp <= 0);
    }
}
