using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharactarController : MonoBehaviour
{
    public Vector2 velocityMin = new Vector2(-100.0f, -100.0f);
    public Vector2 velocityMax = new Vector2(100.0f, 50.0f);

    public bool superArmor = false;
    public bool superArmor_jumpAttackDmg = true;

    [System.NonSerialized] public float hpMax = 10.0f;          // 최대 체력
    [System.NonSerialized] public float hp = 10.0f;             // 체력
    [System.NonSerialized] public float dir = 1.0f;             // 바라보는 방	
    [System.NonSerialized] public float speed = 6.0f;           // 이동속도
    [System.NonSerialized] public float basScaleX = 1.0f;       // 가로크기 
    [System.NonSerialized] public bool activeSts = false;       // 플레이 가능 상태
    [System.NonSerialized] public bool jumped = false;          // 현재 프레임의 점프 상태
    [System.NonSerialized] public bool grounded = false;        // 현재 프레임의 접지 상태
    [System.NonSerialized] public bool groundedPrev = false;    // 이전 프레임의 접지 상태

    [System.NonSerialized] public Animator animator;
    protected Transform groundCheck_L;
    protected Transform groundCheck_C;
    protected Transform groundCheck_R;

    protected float speedVx;
    protected float speedVxAddPower = 0.0f;

    protected GameObject groundCheck_OnRoadObject;
    protected GameObject groundCheck_OnMoveObject;
    protected GameObject groundCheck_OnEnemyObject;

    protected float gravityScale = 10.0f;
    protected float jumpStartTime = 0.0f;

    protected bool addForceVxEnabled = false;
    protected float addForceVxStartTime = 0.0f;

    protected bool addVelocityEnabled = false;
    protected float addVelocityVx = 0.0f;
    protected float addVelocityVy = 0.0f;

    protected bool setVelocityVxEnabled = false;
    protected bool setVelocityVyEnabled = false;
    protected float setVelocityVx = 0.0f;
    protected float setVelocityVy = 0.0f;

    public GameObject[] fireObjectList;

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
                    } else if (groundCheck.tag == "EnemyObject") {
                        groundCheck_OnEnemyObject = groundCheck.gameObject;
                    }
                }
            }
        }

        FixedUpdateCharacter();
        
        if (grounded) {
            speedVxAddPower = 0.0f;
       
            if (groundCheck_OnMoveObject != null) { 
                speedVxAddPower = groundCheck_OnMoveObject.GetComponentInParent<Rigidbody2D>().velocity.x;
            }
        }

        // 강제로 0.5초간 x축으로 힘주기
        if (addForceVxEnabled)
        {
            if (Time.fixedTime - addForceVxStartTime > 0.5f)
            {
                addForceVxEnabled = false;
            }
        }
        else {
            GetComponent<Rigidbody2D>().velocity = new Vector2(speedVx + speedVxAddPower, GetComponent<Rigidbody2D>().velocity.y);
        }

        //가속도 더하기
        if (addVelocityEnabled)
        {
            addVelocityEnabled = false;

            GetComponent<Rigidbody2D>().velocity =
                new Vector2(GetComponent<Rigidbody2D>().velocity.x + addVelocityVx, GetComponent<Rigidbody2D>().velocity.y + addVelocityVy);
        }

        // x축 속도 강제지정
        if (setVelocityVxEnabled) {
            setVelocityVxEnabled = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(setVelocityVx, GetComponent<Rigidbody2D>().velocity.y);
        }

        //y축 속도 강제지정
        if (setVelocityVyEnabled) {
            setVelocityVxEnabled = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, setVelocityVy);
        }

        /* 최댓값 최솟값 이내로 보정  */
        float vx = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.x, velocityMin.x, velocityMax.x);
        float vy = Mathf.Clamp(GetComponent<Rigidbody2D>().velocity.y, velocityMin.y, velocityMax.y);

        GetComponent<Rigidbody2D>().velocity = new Vector2(vx, vy);
    }

    protected virtual void FixedUpdateCharacter() {

    }

    public virtual void AddForceAnimatorVx(float vx)
    {
        if (vx != 0.0f)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(vx * dir, 0.0f));
            addForceVxEnabled = true;
            addForceVxStartTime = Time.fixedTime;
        }
    }

    public virtual void AddForceAnimatorVy(float vy)
    {
        if (vy != 0.0f)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0.0f, vy));
            jumped = true;
            jumpStartTime = Time.fixedTime;
        }
    }

    public virtual void AddVelocityVx(float vx)
    {
        addVelocityEnabled = true;
        addVelocityVx = vx * dir;
    }

    public virtual void AddVelocityVy(float vy)
    {
        addVelocityEnabled = true;
        addVelocityVy = vy;
    }

    public virtual void SetVelocityVx(float vx)
    {
        setVelocityVxEnabled = true;
        setVelocityVx = vx * dir;
    }

    public virtual void SetVelocityVy(float vy)
    {
        setVelocityVyEnabled = true;
        setVelocityVy = vy;
    }

    public virtual void SetLightGravity()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, 0.0f);
        GetComponent<Rigidbody2D>().gravityScale = 0.1f;
    }

    // 움직이기 
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

    public void ActionFire() {
        Transform goFire = transform.Find("Muzzle");

        foreach (GameObject fireObject in fireObjectList) {
            GameObject go = Instantiate(fireObject, goFire.position, Quaternion.identity);
            go.GetComponent<FireBullet>().owner = transform;
        }
    }

    public virtual void Dead(bool gameOver) {
        if (!activeSts) {
            return;
        }

        activeSts = false;
        animator.SetTrigger("Dead");
    }

    //hp가 없으면 false, 있으면 true
    public virtual bool SetHp(float _hp, float _hpMax) {
        hp = _hp;
        hpMax = _hpMax;
        return (hp <= 0);
    }

    public void EnableSuperArmor() {
        superArmor = true;
    }

    public void DisableSuperArmor() {
        superArmor = false;
    }

    public bool ActionLookup(GameObject go, float near) {
        //near 보다 거리가 멀 때 바라본다
        if (Vector3.Distance(transform.position, go.transform.position) > near) {
            dir = (transform.position.x < go.transform.position.x) ? 1 : -1;
            return true;
        }

        return false;
    }

    public bool ActionMoveToNear(GameObject go, float near) {
        //near 보다 거리가 멀면 다가간다
        if (Vector3.Distance(transform.position, go.transform.position) > near)
        {
            ActionMove((transform.position.x < go.transform.position.x) ? 1.0f : -1.0f);
            return true;
        }

        return false;
    }

    public bool ActionMoveToFar(GameObject go, float far)
    {
        // far 보다 거리가 가까우면 도망간다
        if (Vector3.Distance(transform.position, go.transform.position) < far)
        {
            ActionMove((transform.position.x > go.transform.position.x) ? 1.0f : -1.0f);
            return true;
        }

        return false;
    }
}
