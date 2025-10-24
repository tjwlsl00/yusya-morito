using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayer : MonoBehaviour
{
    private Vector2 moveInput;
    [SerializeField] float targetLocalScaleX;
    [SerializeField] float moveSpeed;
    // bool 
    private bool isMoving;
    // 애니메이션
    private Animator animator;
    private string WalkAnim = "Walk";
    // UI연결
    public GameObject HowToPlayPanel;
    // 오디오 
    private AudioSource audioSource;
    public AudioClip WalkClip;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        HowToPlayPanel.SetActive(false);
    }

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.Normalize();
        isMoving = moveInput.sqrMagnitude > 0;
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            transform.position += (Vector3)moveInput * moveSpeed * Time.deltaTime;

            // 플레이어 뒤집기 
            if (moveInput.x < 0)
            {
                transform.localScale = new Vector3(-targetLocalScaleX, 5, 1);
            }
            else if (moveInput.x > 0)
            {
                transform.localScale = new Vector3(targetLocalScaleX, 5, 1);
            }

            // 애니메이션
            animator.SetBool(WalkAnim, true);

            // 오디오 
            if (!audioSource.isPlaying)
            {
                audioSource.clip = WalkClip;
                audioSource.loop = true;
                audioSource.pitch = 1.8f;
                audioSource.Play();
            }
        }
        else
        {
            // 애니메이션
            animator.SetBool(WalkAnim, false);
            // 오디오 
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    #region 충돌 이벤트 패널열기 / 게임 씬 이동
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HowToPlay"))
        {
            HowToPlayPanel.SetActive(true);
            HowToPlay.Instance.FirstLoadPanel();

        }
        else if (collision.gameObject.CompareTag("GameStart"))
        {
            SceneManager.LoadScene("GameScene");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("HowToPlay"))
        {
            HowToPlayPanel.SetActive(false);
        }
    }
    #endregion
}
