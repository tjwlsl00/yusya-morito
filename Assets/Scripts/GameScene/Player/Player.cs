using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour, IDamageable
{
    #region 내부 변수 
    public float moveSpeed;
    private float InitialmoveSpeed = 3f;
    public int currentCoin; // 현재 돈
    [SerializeField] int initialCoin;
    public float maxHp;
    public float currentHp;
    private float RecoveryRegenRate = 10f; //초당 체력 회복량
    private float RecoveryDelayTimer; // 딜레이 시간 계산 내부 타이머 변수
    public float targetLocalScaleX;
    public float targetLocalScaleY;
    private Vector2 moveInput;
    // 업그레이드 변수 
    [SerializeField] int upgradeCoinRate = 100; //업그레이드 마다 100코인 차감
    private int InitialAtkLevel = 1;
    private int InitialDexLevel = 1;
    [SerializeField] private int AtkLevel;
    [SerializeField] int DexLevel;
    [SerializeField] int maxAtkLevel = 5;
    [SerializeField] int maxDexLevel = 5;
    // 공격 변수
    public float attackDuration;
    public GameObject attackHitBox;
    private float InitialAtkDamage = 30f;
    // 방어 변수
    public Collider2D myCollider; //방어 중 콜라이더 비활성화 -> 피격 무시 
    public GameObject barrier;
    public float maxStamina;
    public float currentStamina;
    private float staminaDrainRate = 25f; //초당 스태미나 소모량
    private float staminaRegenRate = 15f; //초당 스태미나 회복량
    private float staminaRegenDelay = 2f; //방어 후 스태미나 회복 시간까지의 딜레이 
    private float regenDelayTimer; // 딜레이 시간 계산 내부 타이머 변수 
    // bool
    private bool isMoving = false;
    private bool isAttacking = false;
    private bool isDefense = false;
    private bool ShieldSoundPlayed;
    public bool isDeath = false;
    // 애니메이션
    private Animator animator;
    private string WalkAnim = "Walk";
    // UI연결 
    public Image hpBarImage;
    public Image ShieldGazeBarImage;
    public TextMeshProUGUI AtkLevelText;
    public TextMeshProUGUI DexLevelText;
    public TextMeshProUGUI coinText;
    private HitBox hitBox;
    public Canvas WorldUI;
    public TextMeshProUGUI damageText; //worldCanvas
    // 사운드 
    private AudioSource audioSource;
    public AudioClip UpgradeClip;
    public AudioClip UpgradeFailClip;
    public AudioClip attackClip;
    public AudioClip ShieldClip;
    public AudioClip DeathClip;
    // 게임 매니저 연결
    public GameManager gameManager;
    #endregion

    #region 상태
    public enum PlayerDirection
    {
        Idle,
        Walk,
        Attack,
        Defense,
        Hurt,
        Death
    }
    private PlayerDirection currentDirection;
    #endregion

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        attackHitBox.SetActive(false);
        barrier.SetActive(false);
        hitBox = attackHitBox.GetComponent<HitBox>();
        WorldUI.gameObject.SetActive(false);

        #region 플레이어 상태 초기화
        AtkLevel = InitialAtkLevel;
        DexLevel = InitialDexLevel;
        currentHp = maxHp;
        currentCoin = initialCoin;
        currentStamina = maxStamina;
        moveSpeed = InitialmoveSpeed; //이동 속도 업그레이드 값 초기화 
        hitBox.damage = InitialAtkDamage; //공격 데미지 업그레이드 값 초기화

        // 플레이어 스탯 UI
        AtkLevelText.text = AtkLevel + "Lv";
        DexLevelText.text = DexLevel + "Lv";
        #endregion
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Play)
        {
            if (isDeath) return; //죽으면 아래 로직 실행 안함

            HandleInputAndActions();
            HandleMovement();
            HandleRicovery();
            HandleStamina();
            UpdateAnimator();

            coinText.text = "お金：" + currentCoin.ToString();
        }
    }

    private void HandleInputAndActions()
    {
        // 이동 입력 감지
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();
        isMoving = moveInput.sqrMagnitude > 0;

        // 행동 결정 (공격, 방어)
        if (isMoving)
        {
            isDefense = false;
        }

        // 공격 키 입력
        if (Input.GetKeyDown(KeyCode.LeftControl) && !isAttacking && !isDefense)
        {
            StartCoroutine(PerformAttack());
        }

        // 방어 키 입력
        bool wantsToDefend = Input.GetKey(KeyCode.Z);
        if (wantsToDefend && !isAttacking && currentStamina > 0)
        {
            isDefense = true;
            InDefense();
        }
        else
        {
            isDefense = false;
            OutDefense();
        }
    }

    private void HandleMovement()
    {
        if (!isMoving || isAttacking || isDefense) return; //움직이지 않거나, 공격 또는 방어 중일때 이동 불가

        transform.position += (Vector3)moveInput * moveSpeed * Time.deltaTime;

        // 플레이어 뒤집기 
        if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-targetLocalScaleX, targetLocalScaleY, 1);
        }
        else if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(targetLocalScaleX, targetLocalScaleY, 1);
        }
    }

    private void HandleRicovery()
    {
        if (RecoveryDelayTimer > 0)
        {
            RecoveryDelayTimer -= Time.deltaTime;
        }
        else if (currentHp < maxHp)
        {
            currentHp += RecoveryRegenRate * Time.deltaTime;
        }

        // 체력 UI 업데이트
        UpdateHP(currentHp, maxHp);
    }

    private void HandleStamina()
    {
        if (isDefense)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime; //초당 스태미나 감소
            regenDelayTimer = staminaRegenDelay;

            if (currentStamina <= 0)
            {
                isDefense = false;
            }

        }
        else
        {
            if (regenDelayTimer > 0)
            {
                regenDelayTimer -= Time.deltaTime; //회복 딜레이가 있으면 먼저 감소
            }
            else if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime; // 회복 딜레이 끝나고, 스태미나가 최대치가 아닐 때 회복 시작
            }
        }
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        // 스테미나 ui 업데이트
        UpdateShieldGaze(currentStamina, maxStamina);
    }

    private void UpdateAnimator()
    {
        animator.SetBool(WalkAnim, isMoving && !isAttacking);
        animator.SetBool("Defense", isDefense);

        // PlayerDirection 상태 업데이트 (필요 시)
        if (isAttacking) currentDirection = PlayerDirection.Attack;
        else if (isDefense) currentDirection = PlayerDirection.Defense;
        else if (isMoving) currentDirection = PlayerDirection.Walk;
        else currentDirection = PlayerDirection.Idle;
    }

    #region 코루틴
    IEnumerator PerformAttack() //공격
    {
        Debug.Log("공격 시작!");
        currentDirection = PlayerDirection.Attack;
        isAttacking = true; //공격 중에는 다른 행동 제한
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.1f); // 잠깐의 선딜레이 후 히트박스 활성화
        attackHitBox.SetActive(true); //히트박스 보이기

        // 공격 사운드 
        audioSource.clip = attackClip;
        audioSource.Play();

        yield return new WaitForSeconds(attackDuration);

        attackHitBox.SetActive(false);
        isAttacking = false;
        Debug.Log("공격 끝!");
    }
    #endregion

    #region 방어 효과 
    private void InDefense()
    {
        myCollider.enabled = false;
        barrier.SetActive(true);
        Debug.Log("방어 중! 현재 스태미나: " + (int)currentStamina);

        // 사운드 처리 
        if (!ShieldSoundPlayed)
        {
            ShieldSoundPlayed = true;
            audioSource.PlayOneShot(ShieldClip);
        }
    }

    private void OutDefense()
    {
        myCollider.enabled = true;
        barrier.SetActive(false);
        Debug.Log("방어 취소! 현재 스태미나: " + (int)currentStamina);

        // 사운드 리셋
        ShieldSoundPlayed = false;
    }
    #endregion

    #region 데미지 받기 / 죽음
    public void TakeDamage(float damageAmount)
    {
        if (isDeath || isDefense) return; //죽거나 방어중이면 리턴

        currentHp -= damageAmount;

        RecoveryDelayTimer = 2f; //공격 받을때마다 2초 타이머 -> 피격x시 회복 

        if (!isAttacking) //공격하지 않을때만 애니메이션 재생
        {
            currentDirection = PlayerDirection.Hurt;
            animator.SetTrigger("Hurt");
        }

        if (currentHp <= 0)
        {
            Death();
        }

        // 체력 UI 업데이트
        UpdateHP(currentHp, maxHp);
        StartCoroutine(DamageEffect(damageAmount));
    }

    IEnumerator DamageEffect(float damageAmount)
    {
        WorldUI.gameObject.SetActive(true);
        damageText.text = damageAmount.ToString();
        yield return new WaitForSeconds(0.5f);
        WorldUI.gameObject.SetActive(false);
    }

    private void Death()
    {
        currentDirection = PlayerDirection.Death;
        // 모든 행동을 막는 플래그
        isDeath = true;
        animator.SetTrigger("Death");
        StartCoroutine(WaitforDeathAnim());
    }

    //플레이어 죽음 bool 값, Enemy에게 전달
    public bool IsDeath()
    {
        return isDeath;
    }

    IEnumerator WaitforDeathAnim()
    {
        // 죽음 사운드 
        audioSource.clip = DeathClip;
        audioSource.Play();

        yield return new WaitForSeconds(1.5f);
    }
    #endregion

    #region 자금 획득 / 상점 코인 감소,증가
    public void EarnCoin(int amount)
    {
        currentCoin += amount;
        Debug.Log(amount + "몬스터 보상");
    }
    
    public void ReduceCoin(int amount)
    {
        currentCoin -= amount;
        Debug.Log(amount + "코인이 차감되었습니다.");
    }

    public void PayBackCoin(int amount)
    {
        int paybackCoin = amount / 2;
        currentCoin += paybackCoin;
    }
    #endregion

    #region UI업데이트 체력 / 실드
    private void UpdateHP(float currentHp, float maxHp)
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = currentHp / maxHp;
        }
    }

    private void UpdateShieldGaze(float currentStamina, float maxStamina)
    {
        if (ShieldGazeBarImage != null)
        {
            ShieldGazeBarImage.fillAmount = currentStamina / maxStamina;
        }
    }
    #endregion

    #region 플레이서 스탯 업그레이드 
    public void UpgradeAtk()
    {
        if (AtkLevel == maxAtkLevel)
        {
            AtkLevelText.text = "Max";
            UpgradeFailSound();
            return;
        }

        if (currentCoin >= upgradeCoinRate)
        {
            AtkLevel += 1;
            hitBox.damage += 5f;
            currentCoin -= upgradeCoinRate;
            // 오디오 
            UpgradeSound();
        }

        if (AtkLevel == maxAtkLevel)
        {
            AtkLevelText.text = "Max";
        }
        else
        {
            AtkLevelText.text = AtkLevel + "Lv";
        }
    }

    public void UpgradeDex()
    {
        if (DexLevel == maxDexLevel)
        {
            DexLevelText.text = "Max";
            UpgradeFailSound();
            return;
        }

        if (currentCoin >= upgradeCoinRate)
        {
            DexLevel += 1;
            moveSpeed += 0.5f;
            currentCoin -= upgradeCoinRate;
            // 오디오 
            UpgradeSound();
        }

        if (DexLevel == maxDexLevel)
        {
            DexLevelText.text = "Max";
        }
        else
        {
            DexLevelText.text = DexLevel + "Lv";
        }
    }

    private void UpgradeSound()
    {
        audioSource.clip = UpgradeClip;
        audioSource.Play();
    }

    private void UpgradeFailSound()
    {
        audioSource.clip = UpgradeFailClip;
        audioSource.Play();
    }
    #endregion

}