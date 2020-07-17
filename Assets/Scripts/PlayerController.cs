using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharactarController
{

    public float initHpMax = 20.0f;
    [Range(0.1f, 100.0f)] public float initSpeed = 12.0f;

    public static float nowHpMax = 0;
    public static float nowHp = 0;
    public static int score = 0;

    public static bool checkPointEnabled = false;
    public static string checkPointSceneName = "";
    public static string checkPointLabelName = "";
    public static float checkPointHp = 0;

    public static bool initParam = true;

    public static bool itemKeyA = false;
    public static bool itemKeyB = false;
    public static bool itemKeyC = false;

    [System.NonSerialized] public float groundY = 0.0f;
    [System.NonSerialized] public bool superMode = false;

    [System.NonSerialized] public int comboCount = 0;

    [System.NonSerialized] public Vector3 enemyActiveZonePointA;
    [System.NonSerialized] public Vector3 enemyActiveZonePointB;

    /* 해시로 등호비교하는 것이 문자열로 등비교하는 것보다 처리가 빠르다 */
    public readonly static int ANISTS_Idle          = Animator.StringToHash("Base Layer.Player_Idle");
    public readonly static int ANISTS_Walk          = Animator.StringToHash("Base Layer.Player_Walk");
    public readonly static int ANISTS_Run           = Animator.StringToHash("Base Layer.Player_Run");
    public readonly static int ANISTS_Jump          = Animator.StringToHash("Base Layer.Player_Jump");
    public readonly static int ANISTS_ATTACK_A      = Animator.StringToHash("Base Layer.Player_ATK_A");
    public readonly static int ANISTS_ATTACK_B      = Animator.StringToHash("Base Layer.Player_ATK_B");
    public readonly static int ANISTS_ATTACK_C      = Animator.StringToHash("Base Layer.Player_ATK_C");
    public readonly static int ANISTS_ATTACKJUMP_A  = Animator.StringToHash("Base Layer.Player_ATKJUMP_A");
    public readonly static int ANISTS_ATTACKJUMP_B  = Animator.StringToHash("Base Layer.Player_ATKJUMP_B");
    public readonly static int ANISTS_DEAD          = Animator.StringToHash("Base Layer.Player_Dead");

    LineRenderer hudHpBar;
    TextMesh hudScore;
    TextMesh hudCombo;

    int jumpCount = 0;

    volatile bool atkInputEnabled = false;  //콤보입력 가능여부
    volatile bool atkInputNow = false;      //콤보입력 여부 

    bool breakEnabled = true;
    float groundFriction = 0.0f;

    float comboTimer = 0.0f;

    public static GameObject GetGameObject() {
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static Transform GetTransform() {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }

    public static PlayerController GetController() {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public static Animator GetAnimator() {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    protected override void Awake() {
        // 접지 오브젝트 설정하
        base.Awake();

        System.GC.Collect();

        hudHpBar = GameObject.Find("HUD_HPBar").GetComponent<LineRenderer>();
        hudScore = GameObject.Find("HUD_Score").GetComponent<TextMesh>();
        hudCombo = GameObject.Find("HUD_Combo").GetComponent<TextMesh>();

        // 초기 값 세팅하기
        speed = initSpeed;
        groundY = groundCheck_C.transform.position.y + 2.0f;

        BoxCollider2D boxCol2D = transform.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
        enemyActiveZonePointA = new Vector3(boxCol2D.offset.x - boxCol2D.size.x / 2.0f, boxCol2D.offset.y - boxCol2D.size.y / 2.0f);
        enemyActiveZonePointB = new Vector3(boxCol2D.offset.x + boxCol2D.size.x / 2.0f, boxCol2D.offset.y + boxCol2D.size.y / 2.0f);
        boxCol2D.transform.gameObject.SetActive(false);

        if (initParam)
        {
            SetHp(initHpMax, initHpMax);
            initParam = false;
        }
        if (SetHp(PlayerController.nowHp, PlayerController.nowHpMax)) {
            SetHp(1, initHpMax);
        }

        if (checkPointEnabled) {
            StageTrigger_CheckPoint[] triggerList = GameObject.Find("Stage").GetComponentsInChildren<StageTrigger_CheckPoint>();

            foreach (StageTrigger_CheckPoint trigger in triggerList) {
                if (trigger.labelName == checkPointLabelName) {
                    transform.position = trigger.transform.position;
                    groundY = transform.position.y;
                    Camera.main.GetComponent<CameraFollow>().SetCamera(trigger.cameraParam);
                    break;
                }
            }

        }

        Transform hud = GameObject.FindGameObjectWithTag("SubCamera").transform;
        hud.Find("Stage_Item_Key_A").GetComponent<SpriteRenderer>().enabled = itemKeyA;
        hud.Find("Stage_Item_Key_B").GetComponent<SpriteRenderer>().enabled = itemKeyB;
        hud.Find("Stage_Item_Key_C").GetComponent<SpriteRenderer>().enabled = itemKeyC;
    }

    public void ActionEtc() {
        Collider2D[] otherAll = Physics2D.OverlapPointAll(groundCheck_C.position);

        foreach (Collider2D other in otherAll) {
            if (other.tag == "EventTrigger")
            {
                StageTrigger_Link link = other.GetComponent<StageTrigger_Link>();
                if (link != null)
                {
                    link.Jump();
                }
            }
            else if (other.tag == "KeyDoor") {
                StageObject_KeyDoor keyDoor = other.GetComponent<StageObject_KeyDoor>();
                keyDoor.OpenDoor();
            }
        }
    }

    protected override void Update() {
        base.Update();

        hudHpBar.SetPosition(1, new Vector3(5.0f * (hp / hpMax), 0.0f, 0.0f));
        hudScore.text = string.Format("Score {0}", score);

        if (comboTimer <= 0.0f)
        {
            hudCombo.gameObject.SetActive(false);
            comboCount = 0;
            comboTimer = 0.0f;
        }
        else {
            comboTimer -= Time.deltaTime;
            if (comboTimer > 5.0f) {
                comboTimer = 5.0f;
            }

            float s = 0.3f + 0.5f * comboTimer;
            hudCombo.gameObject.SetActive(true);
            hudCombo.transform.localScale = new Vector3(s, s, 1.0f);
        }
    }

    protected override void FixedUpdateCharacter() {

        // 현재 애니메이션 상태 가져오기
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // 점프 후 접지 판정
        if (jumped) {
            // 일반적인 경우 || 천장이 낮은 경우
            if ((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
                animator.SetTrigger("Idle");
                jumped = false;
                jumpCount = 0;
                GetComponent<Rigidbody2D>().gravityScale = gravityScale;
            }

            //공중스킬 1초 후 가벼워진 중력복구
            if (Time.fixedTime > jumpStartTime + 1.0f) {
                if (stateInfo.nameHash == ANISTS_Idle ||
                    stateInfo.nameHash == ANISTS_Walk ||
                    stateInfo.nameHash == ANISTS_Run ||
                    stateInfo.nameHash == ANISTS_Jump) {
                    GetComponent<Rigidbody2D>().gravityScale = gravityScale;
                }
            }
        }
        else {
            // 접지 중 1단 점프로 대기
            jumpCount = 0;
            GetComponent<Rigidbody2D>().gravityScale = gravityScale;
        }

        //공격 중에 가로이동 불가
        if (stateInfo.nameHash == ANISTS_ATTACK_A ||
            stateInfo.nameHash == ANISTS_ATTACK_B ||
            stateInfo.nameHash == ANISTS_ATTACK_C ||
            stateInfo.nameHash == ANISTS_ATTACKJUMP_A ||
            stateInfo.nameHash == ANISTS_ATTACKJUMP_B) {

            speedVx = 0f;
        }

        // 스프라이트 방향 바꾸기 
        transform.localScale = new Vector3(dir * basScaleX, transform.localScale.y, transform.localScale.z);

        // 점프 중 가로 이동 속도 감소
        if (jumped && !grounded && groundCheck_OnMoveObject == null) {
            if (breakEnabled) {
                breakEnabled = false;
                speedVx *= 0.9f;
            }
        }

        // 이동 속도감소
        if (breakEnabled) {
            speedVx *= groundFriction;
        }

        // 케릭터 위에서 촬영
        //Camera.main.transform.position = transform.position - Vector3.forward;
    }

    public void EnableAttackInput()
    {
        atkInputEnabled = true;
    }

    public void SetNextAttack(string name)
    {
        if (atkInputNow == true)
        {
            atkInputNow = false;
            animator.Play(name);
        }
    }

    public override void ActionMove(float n) {
        if (!activeSts) {
            return;
        }

        float dirOld = dir;
        breakEnabled = false;

        // 애니메이션 설정
        float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);
        animator.SetFloat("MovSpeed", moveSpeed);

        if (n != 0.0f)
        {
            // 방향 데이터 갱
            dir = Mathf.Sign(n);

            moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;

            // 속도 정하기
            speedVx = initSpeed * moveSpeed * dir;
        }
        else {
            // 이동 정지
            breakEnabled = true;
        }

        // 방향 전환시 이동 정지
        if (dirOld != dir) {
            breakEnabled = true;
        }
    }

    public void ActionJump() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.nameHash == ANISTS_Idle ||
            stateInfo.nameHash == ANISTS_Walk ||
            stateInfo.nameHash == ANISTS_Run ||
            stateInfo.nameHash == ANISTS_Jump && GetComponent<Rigidbody2D>().gravityScale >= gravityScale)
        {
            switch (jumpCount)
            {
                // 1단 점프
                case 0:
                    if (grounded)
                    {
                        animator.SetTrigger("Jump");
                        GetComponent<Rigidbody2D>().velocity = Vector2.up * 30.0f;
                        jumpStartTime = Time.fixedTime;
                        jumped = true;
                        jumpCount++;
                    }
                    break;
                // 2단 점프
                case 1:
                    if (!grounded)
                    {
                        animator.Play("Player_Jump", 0, 0.0f);
                        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 20.0f);
                        jumped = true;
                        jumpCount++;
                    }
                    break;
            }
        }
    }

    public void ActionAttack() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
     

        if (stateInfo.nameHash == ANISTS_Idle ||
            stateInfo.nameHash == ANISTS_Walk ||
            stateInfo.nameHash == ANISTS_Run  ||
            stateInfo.nameHash == ANISTS_Jump ||
            stateInfo.nameHash == ANISTS_ATTACK_C)
        {
            animator.SetTrigger("Attack_A");
            if (stateInfo.nameHash == ANISTS_Jump ||
                stateInfo.nameHash == ANISTS_ATTACK_C) {
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().gravityScale = 0.1f;
            }
        }
        else {
            if (atkInputEnabled) {
                atkInputEnabled = false;
                atkInputNow = true;
            }
        }
    }

    public void ActionAttackJump() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (grounded &&
            (stateInfo.nameHash == ANISTS_Idle ||
             stateInfo.nameHash == ANISTS_Walk ||
             stateInfo.nameHash == ANISTS_Run ||
             stateInfo.nameHash == ANISTS_ATTACK_A ||
             stateInfo.nameHash == ANISTS_ATTACK_B))
        {
            animator.SetTrigger("Attack_C");
            jumpCount = 2;
        }
        else {
            if (atkInputEnabled) {
                atkInputEnabled = false;
                atkInputNow = true;
            }
        }

    }

    public void ActionDamage(float damage) {
        if (!activeSts) {
            return;
        }

        Debug.Log("맞음");

        animator.SetTrigger("DMG_A");
        speedVx = 0;
        GetComponent<Rigidbody2D>().gravityScale = gravityScale;

        //공중시 1.5배 피격당하기 
        if (jumped) {
            damage *= 1.5f;
        }

        if (SetHp(hp - damage, hpMax)) {
            Dead(true);
        }
    }

    public override void Dead(bool gameOver) {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.nameHash == ANISTS_DEAD) {
            return;
        }

        base.Dead(gameOver);

        SetHp(0, hpMax);
        Invoke("GameOver", 3.0f);

        if (gameOver)
        {
            SetHp(0, hpMax);
            Invoke("GameOver", 3.0f);
        }
        else {
            SetHp(hp / 2, hpMax);
            Invoke("GameReset", 3.0f);
        }

        GameObject.Find("HUD_Dead").GetComponent<MeshRenderer>().enabled = true;
        GameObject.Find("HUD_DeadShadow").GetComponent<MeshRenderer>().enabled = true;
    }

    public void GameOver() {
        PlayerController.score = 0;
        PlayerController.nowHp = PlayerController.checkPointHp;
        Application.LoadLevel(Application.loadedLevelName);
    }

    void GameReset() {
        Application.LoadLevel(Application.loadedLevelName);
    }

    public override bool SetHp(float _hp, float _hpMax)
    {
        if (_hp > _hpMax) {
            _hp = _hpMax;
        }

        nowHp = _hp;
        nowHpMax = _hpMax;
        return base.SetHp(_hp, _hpMax);
    }

    public void AddCombo() {
        comboCount++;
        comboTimer += 1.0f;
        hudCombo.text = string.Format("Combo {0}", comboCount);
    }
}
