using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    #region 내부 변수 
    public Button miniTurretButton;
    public Button bigTurretButton;
    public Button sellButton;
    private Image miniTurretBtnImage;
    private Image bigTurretBtnImage;
    private int miniTurretPrice = 50;
    private int bigTurretPrice = 250;
    private float activeAlpha = 1.0f; //활성화 Alpha값
    private float inactiveAlpha = 0.5f; //비활성화 Alpha값
    public Player player; //플레이어 자금 상황 확인 하기 위해서 
    // bool
    public bool isSell = false;
    // 게임 매니저 연결
    public GameManager gameManager;
    // 오디오 
    private AudioSource audioSource;
    public AudioClip ButtonClip;
    #endregion

    void Awake()
    {
        if (miniTurretButton != null)
        {
            miniTurretBtnImage = miniTurretButton.GetComponent<Image>();
        }

        if (bigTurretButton != null)
        {
            bigTurretBtnImage = bigTurretButton.GetComponent<Image>();
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        SetButtonState(); //초기 상태 점검
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Play)
        {
            SetButtonState(); // 플레이어 자금 변할 때마다 버튼 업데이트 
        }
    }

    #region 포탑 메뉴 버튼 활성화 / 비활성화 
    public void SetButtonState()
    {
        // null이면 return 
        if (miniTurretButton == null || miniTurretBtnImage == null || bigTurretButton == null || bigTurretBtnImage == null) return;

        // 각 이미지 컬러 값 변수 선언
        Color miniColor = miniTurretBtnImage.color;
        Color bigColor = bigTurretBtnImage.color;

        int currentCoin = player.currentCoin;

        //miniTurret값 이상 이면 해당 이미지 활성화
        if (currentCoin >= miniTurretPrice)
        {
            miniColor.a = activeAlpha;
            miniTurretButton.interactable = true;
        }
        else
        {
            miniColor.a = inactiveAlpha;
            miniTurretButton.interactable = false;
        }

        //bigTurret값 이상 이면 해당 이미지 활성화
        if (currentCoin >= bigTurretPrice)
        {
            bigColor.a = activeAlpha;
            bigTurretButton.interactable = true;
        }
        else
        {
            bigColor.a = inactiveAlpha;
            bigTurretButton.interactable = false;
        }

        miniTurretBtnImage.color = miniColor;
        bigTurretBtnImage.color = bigColor;
    }
    #endregion

    #region 판매 버튼 클릭
    public void SellButtonClicked()
    {
        if (gameManager.GameCurrentDireaction != GameManager.GameMangerDireaction.Play) return;

        isSell = true;

        // 오디오 
        audioSource.clip = ButtonClip;
        audioSource.Play();
    }
    #endregion
}
