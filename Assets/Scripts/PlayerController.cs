using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseCharactarController
{
    /* 해시로 등호비교하는 것이 문자열로 등비교하는 것보다 처리가 빠르다 */
    public readonly static int ANISTS_Idle = Animator.StringToHash("Base Layer.Player_Idle");
    public readonly static int ANISTS_Walk = Animator.StringToHash("Base Layer.Player_Walk");
    public readonly static int ANISTS_Run = Animator.StringToHash("Base Layer.Player_Run");
    public readonly static int ANISTS_Jump = Animator.StringToHash("Base Layer.Player_Jump");
    public readonly static int ANISTS_ATTACK_A = Animator.StringToHash("Base Layer.Player_ATK_A");
    public readonly static int ANISTS_ATTACK_B = Animator.StringToHash("Base Layer.Player_ATK_B");
    public readonly static int ANISTS_ATTACK_C = Animator.StringToHash("Base Layer.Player_ATK_C");
    public readonly static int ANISTS_ATTACKJUMP_A = Animator.StringToHash("Base Layer.Player_ATKJUMP_A");
    public readonly static int ANISTS_ATTACKJUMP_B = Animator.StringToHash("Base Layer.Player_ATKJUMP_B");

    public float initHpMax = 20.0f;
    [Range(0.1f, 100.0f)] public float initSpeed = 12.0f;

    int jumpCount = 0;

    volatile bool atkInputEnabled = false;  //콤보입력 가능여부
    volatile bool atkInputNow = false;      //콤보입력 여

    bool breakEnabled = true;
    float groundFriction = 0.0f;

    protected override void Awake() {
        // 접지 오브젝트 설정하
        base.Awake();

        // 초기 값 세팅하기
        speed = initSpeed;
        SetHp(initHpMax, initHpMax);
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
            }
        }

        // 접지 중 1단 점프로 대기
        if (!jumped) {
            jumpCount = 0;
        }

        //공격 중에 가로이동 불가
        if (stateInfo.nameHash == ANISTS_ATTACK_A ||
            stateInfo.nameHash == ANISTS_ATTACK_B ||
            stateInfo.nameHash == ANISTS_ATTACK_C ||
            stateInfo.nameHash == ANISTS_ATTACKJUMP_A ||
            stateInfo.nameHash == ANISTS_ATTACKJUMP_B) {

            speedVx = 0f;
        }

        // 점프 중 가로 이동 속도 감소
        if (jumped && !grounded) {
            if (breakEnabled) {
                breakEnabled = false;
                speedVx *= 0.9f;
            }
        }

        // 이동 정 
        if (breakEnabled) {
            speedVx *= groundFriction;
        }

        // 케릭터 위에서 촬영
        Camera.main.transform.position = transform.position - Vector3.forward;
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


            // 스프라이트 방향 바꾸ㄱ
            transform.localScale = new Vector3(dir * basScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    public void ActionJump() {
        switch (jumpCount) {
            // 1단 점프
            case 0:
                if (grounded) {
                    animator.SetTrigger("Jump");
                    GetComponent<Rigidbody2D>().velocity = Vector2.up * 30.0f;
                    jumpStartTime = Time.fixedTime;
                    jumped = true;
                    jumpCount++;
                }
                break;
            // 2단 점프
            case 1:
                if (!grounded) {
                    animator.Play("Player_Jump", 0, 0.0f);
                    GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 20.0f);   
                    jumped = true;
                    jumpCount++;
                }
                break;
        }
    }

    public void EnableAttackInput() {
        atkInputEnabled = true;
    }

    public void SetNextAttack(string name) {
        if (atkInputNow == true) {
            atkInputNow = false;
            animator.Play(name);
        }
    }

    public void ActionAttack() {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.nameHash == ANISTS_Idle ||
            stateInfo.nameHash == ANISTS_Walk ||
            stateInfo.nameHash == ANISTS_Run ||
            stateInfo.nameHash == ANISTS_Jump)
        {
            animator.SetTrigger("Attack_A");
        }
        else {
            if (atkInputEnabled) {
                atkInputEnabled = false;
                atkInputNow = true;
            }
        }
    }
}
