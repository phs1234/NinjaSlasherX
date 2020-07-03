using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ENEMYAISTS
{
    ACTIONSELECT,
    WAIT,
    RUNTOPLAYER,
    JUMPTOPLAYER,
    ESCAPE,
    RETURNTODOGPILE,
    ATTACKONSIGTH,
    FREEZ
}

public class EnemyMain : MonoBehaviour
{
    public bool cameraSwitch = true;
    public bool inActiveZoneSwitch = false;
    public bool combatAIOrder = true;
    public float dogPileReturnLength = 10.0f;

    public int debug_SelectRandomAIState = -1;

    [System.NonSerialized] public bool cameraEnabled = false;
    [System.NonSerialized] public bool inActiveZone = false;
    [System.NonSerialized] public ENEMYAISTS aiState = ENEMYAISTS.ACTIONSELECT;
    [System.NonSerialized] public GameObject dogPile;

    protected EnemyController enemyCtrl;
    protected GameObject player;
    protected PlayerController playerCtrl;

    protected float aiActionTimeLength = 0.0f;
    protected float aiActionTimeStart = 0.0f;
    protected float distanceToPlayer = 0.0f;
    protected float distanceToPlayerPrev = 0.0f;

    public virtual void Awake() {
        enemyCtrl = GetComponent<EnemyController>();
        player = PlayerController.GetGameObject();
        playerCtrl = player.GetComponent<PlayerController>();
    }

    public virtual void Start()
    {
        StageObject_DogPile[] dogPileList = GameObject.FindObjectsOfType<StageObject_DogPile>();
        foreach (StageObject_DogPile findDogPile in dogPileList) {
            foreach (GameObject go in findDogPile.enemyList) {
                if (gameObject == go) {
                    dogPile = findDogPile.gameObject;
                    break;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (enemyCtrl.name == "EnemyJumpTrigger_L")
        {
            if (enemyCtrl.ActionJump())
            {
                enemyCtrl.ActionMove(1.0f);
            }
        }
        else if (other.name == "EnemyJumpTrigger_R")
        {
            if (enemyCtrl.ActionJump())
            {
                enemyCtrl.ActionMove(-1.0f);
            }
        }
        else if (other.name == "EnemyJumpTrigger") {
            enemyCtrl.ActionJump();
        }
    }

    public virtual void Update()
    {
        cameraEnabled = false;
    }

    public virtual void FixedUpdate() {
        if (BeginEnemyCommonWork()) {
            FixedUpdateAI();
            EndEnemyCommonWork();
        }
    }

    public virtual void FixedUpdateAI() {

    }

    /* AI가 생각해도 되는지를 검사
     * hp가 없거나 당하거나 죽거나 공격하면 false 아니면 true */
    public bool BeginEnemyCommonWork() {
        if (enemyCtrl.hp <= 0) {
            return false;
        }

        if (inActiveZoneSwitch) {
            inActiveZone = false;
            Vector3 vecA = player.transform.position + playerCtrl.enemyActiveZonePointA;
            Vector3 vecB = player.transform.position + playerCtrl.enemyActiveZonePointB;

            if (transform.position.x > vecA.x && transform.position.x < vecB.x &&
                transform.position.y > vecA.y && transform.position.y < vecB.y) {
                inActiveZone = true;
            }
        }

        if (enemyCtrl.grounded)
        {
            if (cameraSwitch && !cameraEnabled && !inActiveZoneSwitch)
            {
                enemyCtrl.ActionMove(0.0f);
                enemyCtrl.cameraRendered = false;
                enemyCtrl.animator.enabled = false;
                GetComponent<Rigidbody2D>().Sleep();
                return false;
            }
        }
         
        enemyCtrl.animator.enabled = true;
        enemyCtrl.cameraRendered = true;

        //공격하거나 당할 때
        if (!CheckAction()) {
            return false;
        }

        if (dogPile != null) {
            if (GetDistanceDogPile() > dogPileReturnLength) {
                aiState = ENEMYAISTS.RETURNTODOGPILE;
            }
        }

        return true;
    }

    // AI가 일정 시간마다 주기적으로 다음 행동을 생각한다
    public void EndEnemyCommonWork() {
        float time = Time.fixedTime - aiActionTimeStart;

        if (time > aiActionTimeLength) {
            aiState = ENEMYAISTS.ACTIONSELECT;
        }
    }

    // 죽거나 공격하거나 당할 때 false 아니면 true
    public bool CheckAction() {
        AnimatorStateInfo stateInfo = enemyCtrl.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.tagHash == EnemyController.ANITAG_ATTACK ||
            stateInfo.nameHash == EnemyController.ANISTS_DMG_A ||
            stateInfo.nameHash == EnemyController.ANISTS_DMG_B ||
            stateInfo.nameHash == EnemyController.ANISTS_Dead) {
            return false;
        }

        return true;
    }

    // 0에서 100 사이의 고유한 번호를 부여한다
    public int SelectRandomAIState() {

#if UNITY_EDITOR
        if (debug_SelectRandomAIState >= 0)
        {
            return debug_SelectRandomAIState;
        }
#endif
        return Random.Range(0, 100 + 1);

    }


    // 강제로 AI 상황을 바꾸고 AI가 생각하는 주기를 바꾼다
    public void SetAIState(ENEMYAISTS sts, float t) {
        aiState = sts;
        aiActionTimeStart = Time.fixedTime;
        aiActionTimeLength = t;
    }

    // 강제로 AI 상황을 바꾸고 제자리에 멈춘다
    public virtual void SetCombatAIState(ENEMYAISTS sts) {
        aiState = sts;
        aiActionTimeStart = Time.fixedTime;
        enemyCtrl.ActionMove(0.0f);
    }

    // 플레이어와의 거리를 반환하는 함수
    public float GetDistancePlayer() {
        distanceToPlayerPrev = distanceToPlayer;
        distanceToPlayer = Vector3.Distance(transform.position, playerCtrl.transform.position);
        return distanceToPlayer;
    }

    // 플레이어가 움직이지 않았나 검사하는 함수
    public bool IsChangeDistancePlayer(float l) {
        return (Mathf.Abs(distanceToPlayer - distanceToPlayerPrev) > l);
    }

    // 플레이어와의 X축 거리를 반환
    public float GetDistancePlayerX() {
        Vector3 posA = transform.position;
        Vector3 posB = playerCtrl.transform.position;

        posA.y = 0; posA.z = 0;
        posB.y = 0; posB.z = 0;

        return Vector3.Distance(posA, posB);
    }

    // 플레이어와의 Y축 거리를 반환
    public float GetDistancePlayerY()
    {
        Vector3 posA = transform.position;
        Vector3 posB = playerCtrl.transform.position;

        posA.x = 0; posA.z = 0;
        posB.x = 0; posB.z = 0;

        return Vector3.Distance(posA, posB);
    }

    public float GetDistanceDogPile() {
        return Vector3.Distance(transform.position, dogPile.transform.position);
    }
}
