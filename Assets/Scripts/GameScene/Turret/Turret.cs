using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour
{
    #region 내부 변수 
    public Transform target;
    public Transform turretPosition;
    public float attackRange; //터렛 적 인식 범위 
    private Animator animator; // 애니메이션    
    public GameObject BulletPrefab; //총알 오브젝트 
    private float timer = 0f; //총알 무한생성 방지(일정시간 마다 총알 발사 가능하게 -> 추후 interval 값에 따라 변동)
    public int price;
    public float rotationSpeed;
    public float shootInterval;
    private float shootAnim = 0.25f;
    // 사운드 
    private AudioSource audioSource;
    public AudioClip ShootClip;
    // 게임 매니저 연결
    private GameManager gameManager;
    #endregion

    // 터렛 상태 
    public enum miniTurretDirection
    {
        Idle,
        Attack
    }
    private miniTurretDirection currentDirection; //현재 상태 

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentDirection = miniTurretDirection.Idle;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Play)
        {
            if (target == null)
            {
                FindNewTarget();
                if (target == null)
                {
                    return;
                }
            }

            float distanceToTarget = CalcultateEnemyDistance();

            if (distanceToTarget < attackRange)
            {
                Debug.Log("터렛이 적을 인식했습니다. 사격을 시작합니다.");
                currentDirection = miniTurretDirection.Attack;

                // 타겟 바라보기 
                Vector2 direction = target.position - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


                // 총알 무한 생성 방지 
                timer += Time.deltaTime;
                if (timer >= shootInterval)
                {
                    StartCoroutine(MakeBullet());
                    timer = 0f;
                }
            }
            else if (distanceToTarget > attackRange)
            {
                Debug.Log("적이 터렛 공격 범위에 없습니다. 사격을 중지합니다.");
                currentDirection = miniTurretDirection.Idle;
            }
        }
    }

    #region 거리 계산 / 타겟 찾기
    float CalcultateEnemyDistance()
    {
        if (target == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, target.position);
    }

    private void FindNewTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag("Enemy");

        if (potentialTargets.Length == 0) return;

        Transform closestTarget = null;
        float minDistance = Mathf.Infinity; //최소 거리 증가

        // 모든 잠재적 타겟을 순회해서 가장 가까운거 검색
        foreach (GameObject potentialTarget in potentialTargets)
        {
            float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestTarget = potentialTarget.transform;
            }
        }

        target = closestTarget; //가장 가까운 타겟을 새로운 타겟으로 설정
        Debug.Log("새로운 타겟 설정");
    }
    #endregion

    #region 총알 생성 
    IEnumerator MakeBullet()
    {
        animator.SetBool("Shoot", true);
        // 애니메이션 시간 대기 -> 애니메이션 보다 총알 먼저 발사되는거 방지
        yield return new WaitForSeconds(shootAnim);
        animator.SetBool("Shoot", false);
        //부모 위치에 총알 생성 
        Debug.Log(shootInterval + "초마다 총알을 발사합니다");
        GameObject bullet = Instantiate(BulletPrefab, turretPosition.position, turretPosition.rotation);
        TurretBullet turretBullet = bullet.GetComponent<TurretBullet>();

        // 총알에게 타겟 정보 전달
        if (turretBullet != null)
        {
            turretBullet.SetTarget(target);
        }

        // 총알 사운드 
        audioSource.PlayOneShot(ShootClip);
    }
    #endregion

    #region Gizmo
    void OnDrawGizmosSelected()
    {
        DrawAttackRange();
    }

    void DrawAttackRange()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
    #endregion

}
