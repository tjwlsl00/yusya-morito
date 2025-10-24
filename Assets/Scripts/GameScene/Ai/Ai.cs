using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// 캐릭터 타입 설정
public enum AiType { Ally, Enemy };

public class Ai : MonoBehaviour, IDamageable
{
    #region 내부 변수 
    public AiType aiType; //아군, 적군 죽음 로직 다르게 하기 위해 -> 적군 사망시 코인 드랍
    public float moveSpeed;
    public float maxHp;
    public float currentHp;
    public float targetLocalScaleX;
    public float targetLocalScaleY;
    [SerializeField] int coinPrice;
    // 애니메이션
    private Animator animator;
    // 공격(히트박스/시간)
    private float timer = 0f;
    public float attackinterval;
    public float attackAnim;
    public GameObject attackHitBox;
    // bool
    private bool isChase = true;
    private bool isAttacking = false;
    public bool isDeath = false;
    // 범위(순찰/공격)
    private Transform target; //타겟 위치
    public string targetTagname;
    public float ChaseRange;
    public float attackRange;
    // 플레이어 할당(사망 확인 수단)
    public GameObject playerObj;
    // UI연결
    public GameObject WorldUI;
    public Image hpBarImage;
    public TextMeshProUGUI damageText;
    private Rigidbody2D rb;
    // 사운드
    private AudioSource audioSource;
    public AudioClip DeathClip;
    // 게임 매니저 연결 
    private GameManager gameManager;
    #endregion

    #region 상태
    public enum AiDirection
    {
        Chase,
        Attack,
        Hurt,
        Death
    }
    private AiDirection currentDirection;
    #endregion

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        currentHp = maxHp;
        currentDirection = AiDirection.Chase;
        attackHitBox.SetActive(false);
        damageText.gameObject.SetActive(false);

        // 플레이어 자동 할당
        Player player = FindAnyObjectByType<Player>();
        if (player != null)
        {
            playerObj = player.gameObject;
        }

        // 게임 매니저 연결
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Play && !isDeath)
        {
            FindTargetAndChase();
        }

        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Victroy || gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.GameOver)
        {
            Destroy(gameObject);
        }
    }

    float CalcultatePlayerDistance()
    {
        if (target == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, target.position);
    }

    #region 플레이어 죽음 매 프레임 마다 확인 / 본격적이 타겟 추적 시작 / 자동 공격 로직 실행
    private void FindTargetAndChase()
    {
        #region 타겟 찾기 / 거리 계산
        if (target == null)
        {
            FindNewTarget();
            if (target == null) //더 이상 타겟이 없으면 함수 종료
                return;
        }

        //1. 타겟과의 거리 계산
        float distanceToTarget = CalcultatePlayerDistance();

        //2. 타겟과의 거리에 따라서 추적 유무 결정(True, False)
        isChase = distanceToTarget > attackRange;
        #endregion

        #region 추적 / 타겟과의 거리가 공격 범위보다 클때
        if (isChase)
        {
            // 타겟 위치에 따라 적 캐릭터 바라보는 방향 조정
            if (target.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-targetLocalScaleX, targetLocalScaleY, 1); // 왼쪽 바라보기
            }
            else
            {
                transform.localScale = new Vector3(targetLocalScaleX, targetLocalScaleY, 1); // 오른쪽 바라보기
            }
        }
        #endregion

        #region 자동 공격 / 타겟과의 거리가 공격 범위보다 작거나 같을때
        if (!isChase)
        {
            if (!isAttacking)
            {
                timer += Time.deltaTime;

                if (timer >= attackinterval)
                {
                    StartCoroutine(PerformAttack());
                    timer = 0f; //타이머 리셋(다음 공격을 위해)
                }
            }
        }
        #endregion
    }
    #endregion

    private void FixedUpdate()
    {
        if (isDeath || !target) return;

        if (isChase)
        {
            Vector2 newPosition = Vector2.MoveTowards(rb.position, target.position, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);
        }
    }

    private void FindNewTarget()
    {
        GameObject[] potentialTargets = GameObject.FindGameObjectsWithTag(targetTagname); //씬에서 해당 태그를 가진 모든 오브젝트 검색
        Transform closestTarget = null;
        float minDistance = Mathf.Infinity; //최소 거리 증가

        if (potentialTargets.Length == 0) //찾은 타겟이 없으면 함수 종료
        {
            return;
        }

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
    }

    IEnumerator PerformAttack()
    {
        // 공격 모션
        currentDirection = AiDirection.Attack;
        animator.SetTrigger("Attack");

        // AttackHitBox 활성화 / 비활성화
        Debug.Log(attackinterval + "초 마다 공격합니다.");
        attackHitBox.SetActive(true);
        isAttacking = true;
        yield return new WaitForSeconds(attackAnim);
        attackHitBox.SetActive(false);
        isAttacking = false;
    }

    #region 데미지 받기 / 죽음 
    public void TakeDamage(float damageAmount)
    {
        if (isDeath) return;
        currentHp -= damageAmount;

        if (!isAttacking) //공격하지 않을때만 애니메이션 재생
        {
            currentDirection = AiDirection.Hurt;
            animator.SetTrigger("Hurt");
        }

        if (currentHp <= 0)
        {
            Death();
        }

        FindNewTarget(); //피격 받으면 가장 가까운거 검색

        //체력 UI 업데이트
        UpdateHP(currentHp, maxHp);
        StartCoroutine(DamageEffect(damageAmount));
    }

    IEnumerator DamageEffect(float damageAmount)
    {
        damageText.gameObject.SetActive(true);
        damageText.text = damageAmount.ToString();
        yield return new WaitForSeconds(0.5f);
        damageText.gameObject.SetActive(false);
    }

    private void Death()
    {
        currentDirection = AiDirection.Death;
        isDeath = true;
        animator.SetTrigger("Death");
        StartCoroutine(WaitforDeathAnim());
    }

    IEnumerator WaitforDeathAnim()
    {
        if (aiType == AiType.Enemy) //적군이 죽었을때만 코인 드랍
        {
            DropCoin dropCoin = GetComponent<DropCoin>();
            dropCoin.MakeCoin(coinPrice);
        }
        /*
            적군, 아군 동일한 죽음 로직 
        */
        // 사운드 
        audioSource.clip = DeathClip;
        audioSource.Play();

        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject); //오브젝트 파괴
    }
    #endregion

    #region UI업데이트 
    private void UpdateHP(float currentHp, float maxHp)
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = currentHp / maxHp;
        }
    }
    #endregion

    #region 디버그 시각화(Gizmo) / 순찰, 공격 범위
    void OnDrawGizmosSelected()
    {
        DrawChaseRange();
        DrawAttackRange();
    }

    void DrawChaseRange()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ChaseRange);
    }

    void DrawAttackRange()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(attackRange * 1, attackRange * 1, 1));
    }
    #endregion
}
