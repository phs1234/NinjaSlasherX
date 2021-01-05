using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FIREBULLET
{
    ANGLE,
    HOMING,
    HOMING_Z
}

public class FireBullet : MonoBehaviour
{
    public FIREBULLET fireType = FIREBULLET.HOMING;

    public float attackDamage = 1;
    public Vector2 attackNockBackVector;

    public bool penetration = false;                            // 벽에 부딫힌 여부

    public float lifeTime = 3.0f;
    public float speedV = 10.0f;                                // 속도
    public float speedA = 0.0f;                                 // 가속도
    public float angle = 0.0f;                                  // 쏘는 각도

    public float homingTime = 0.0f;                             // 떠오르는 시간
    public float homingAngleV = 180.0f;                         // 떠오르는 속도
    public float homingAngleA = 20.0f;                          // 떠오르는 가속도

    public Vector3 bulletScaleV = Vector3.zero;                 
    public Vector3 bulletScaleA = Vector3.zero;

    public Sprite hitSprite;
    public Vector3 hitEffectScale = Vector3.one;
    public float rotateVt = 360.0f;                             //초당 회전수

    [System.NonSerialized] public Transform owner;
    [System.NonSerialized] public GameObject targetObject;
    [System.NonSerialized] public bool attackEnabled;

    float fireTime;
    Vector3 posTarget;
    float homingAngle;
    Quaternion homingRotate;
    float speed;

    void Start()
    {
        if (!owner) {
            return;
        }

        targetObject = PlayerController.GetGameObject();
        posTarget = targetObject.transform.position + new Vector3(0.0f, 0.1f, 0.0f);

        switch (fireType) {
            case FIREBULLET.ANGLE:
                speed = (owner.localScale.x < 0.0f) ? -speedV : speedV;
                break;
            case FIREBULLET.HOMING:
                speed = speedV;
                homingRotate = Quaternion.LookRotation(posTarget - transform.position);
                break;

            case FIREBULLET.HOMING_Z:
                speed = speedV;
                break;

        }

        fireTime = Time.fixedTime;
        homingAngle = angle;
        attackEnabled = true;

        Destroy(this.gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!owner) {
            return;
        }

        /* 추가 other.tag?? */
        if (other.isTrigger || 
            (owner.tag == "Player" && other.tag == "PlayerBody") ||
            (owner.tag == "Player" && other.tag == "PlayerArm") ||
            (owner.tag == "Player" && other.tag == "PlayerArmBullet") ||
            (owner.tag == "Enemy" && other.tag == "EnemyBody") ||
            (owner.tag == "Enemy" && other.tag == "EnemyArm") ||
            (owner.tag == "Enemy" && other.tag == "EnemyArmBullet")) {
                return;
            }

        if (!penetration) {
            GetComponent<SpriteRenderer>().sprite = hitSprite;
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
            transform.localScale = hitEffectScale;
            //(this.gameObject, 0.1f);
        }
    }

    void Update()
    {
        // 스프라이트 회전 처리
        transform.Rotate(0.0f, 0.0f, Time.deltaTime * rotateVt);
    }

    void FixedUpdate()
    {
        bool homing = (Time.fixedTime - fireTime) > homingTime;

        // 날아가는 동안 타깃 좌표 수정
        if (homing) {
            //if (targetObject == null) {
            //    Debug.Log("targetObject is null");
            //}

//            posTarget = targetObject.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
        }

        switch (fireType) {
            case FIREBULLET.ANGLE:              // 지정한 각도로 발사
                GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0.0f, 0.0f, angle)
                    * new Vector3(speed, 0.0f,0.0f);
                break;

          
            case FIREBULLET.HOMING:             // 가만히 있을 것 같다? 
                if (homing) {
                    homingRotate = Quaternion.LookRotation(posTarget - transform.position);
                }

                Vector3 vecMove = (homingRotate * Vector3.forward) * speed;
                GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0.0f, 0.0f, angle) * vecMove;

                break;

            case FIREBULLET.HOMING_Z:   //오히려 더 멀어질 것 같다 
                if (homing) {
                    float targetAngle = Mathf.Atan2(
                        posTarget.y - transform.position.y,
                        posTarget.x - transform.position.x) * Mathf.Rad2Deg;
                    float deltaAngle = Mathf.DeltaAngle(targetAngle, homingAngle);      //현재 앵글과 이전 앵글 차이 
                    float deltaHomingAngle = homingAngleV * Time.fixedDeltaTime;        //원래 호밍되어야할 각도

                    //호밍할 각도 계산
                    if (Mathf.Abs(deltaAngle) >= deltaHomingAngle) {
                        homingAngle += (deltaAngle < 0.0f) ? deltaHomingAngle : -deltaHomingAngle;
                    }

                    //호밍 가속도 연산
                    homingAngleV += (homingAngleA * Time.fixedDeltaTime);


                    homingRotate = Quaternion.Euler(0.0f, 0.0f, homingAngle);
                }

                GetComponent<Rigidbody2D>().velocity = (homingRotate * Vector3.right) * speed;
                break;
        }

        speed += speedA * Time.fixedDeltaTime;

        transform.localScale += bulletScaleV;
        bulletScaleV += bulletScaleA * Time.fixedDeltaTime;

        if (transform.localScale.x < 0.0f || transform.localScale.y < 0.0f || transform.localScale.z < 0.0f) {
            Destroy(this.gameObject);
        }
    }
}
