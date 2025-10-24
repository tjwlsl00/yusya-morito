using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public enum GameMangerDireaction
    {
        Play,
        Victroy,
        GameOver
    }
    public GameMangerDireaction GameCurrentDireaction;

    #region 내부 변수 
    public Player player;
    public TimeAttack timeAttack;
    public Castle castle;
    // bool 
    private bool ResultSoundPlayed; //사운드 한번만 재생
    // 오디오 
    private AudioSource audioSource;
    public AudioClip ButtonClip;
    public AudioClip KnifeClip;
    public AudioClip VictroyClip;
    public AudioClip DefeatClip;
    //UI 연결
    public GameObject GamePanel;
    public GameObject VictroyPanel;
    public GameObject DefeatPanel;
    public Image StartImage;
    public TextMeshProUGUI StartText;
    [SerializeField] private float fadeDuration = 2f;
    #endregion

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameCurrentDireaction = GameMangerDireaction.Play;
        GamePanel.gameObject.SetActive(false);

        // 이미지, 글씨 처음 Color 값 받아오기 
        StartImage.color = new Color32(255, 255, 255, 255);
        StartText.color = new Color32(255, 255, 255, 255);
        StartCoroutine(FadeOut());

        // 전투개시 효과음 / 칼 사운드
        audioSource.clip = KnifeClip;
        audioSource.Play();
    }

    void Update()
    {
        // 타임아웃 -> 승리
        if (timeAttack.TimeRemaining == 0)
        {
            GameCurrentDireaction = GameMangerDireaction.Victroy;
            ShowGameResultPanel();
            PlayGameResultSound();
        }
        // 플레이어 사망 or 성 파괴 -> 패배
        else if (player.isDeath || castle.currentHp == 0)
        {
            GameCurrentDireaction = GameMangerDireaction.GameOver;
            ShowGameResultPanel();
            PlayGameResultSound();
        }
    }

    // 이미지 / 글씨 페이드 아웃
    IEnumerator FadeOut()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            byte newAlpha = (byte)Mathf.Lerp(255, 0, timer / fadeDuration);
            StartImage.color = new Color32(255, 255, 255, newAlpha);
            StartText.color = new Color32(255, 255, 255, newAlpha);

            yield return null;
        }

        StartImage.color = new Color32(255, 255, 255, 0);
        StartText.color = new Color32(255, 255, 255, 0);
    }

    private void ShowGameResultPanel()
    {
        GamePanel.gameObject.SetActive(true);

        if (GameCurrentDireaction == GameMangerDireaction.Victroy)
        {
            VictroyPanel.SetActive(true);
            DefeatPanel.SetActive(false);
        }
        else
        {
            VictroyPanel.SetActive(false);
            DefeatPanel.SetActive(true);
        }
    }

    private void PlayGameResultSound()
    {
        if (ResultSoundPlayed || GameCurrentDireaction == GameMangerDireaction.Play) return;

        if (GameCurrentDireaction == GameMangerDireaction.Victroy)
        {
            audioSource.clip = VictroyClip;
        }
        else if (GameCurrentDireaction == GameMangerDireaction.GameOver)
        {
            audioSource.clip = DefeatClip;
        }

        audioSource.Play();
        ResultSoundPlayed = true;
    }

    #region 게임 씬 관련(재시작 / 메뉴 이동)
    public void RestartGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        // 씬 이동
        StartCoroutine(RestartGameCoroutine(currentScene));
        // 오디오 
        audioSource.clip = ButtonClip;
        audioSource.Play();
    }

    IEnumerator RestartGameCoroutine(string SceneName)
    {
        yield return new WaitForSeconds(ButtonClip.length);
        SceneManager.LoadScene(SceneName);
    }

    public void LoadMenuScene()
    {
        // 오디오 
        audioSource.clip = ButtonClip;
        audioSource.Play();
        StartCoroutine(LoadMenuSceneCoroutine());
    }

    IEnumerator LoadMenuSceneCoroutine()
    {
        SceneManager.LoadScene(0);
        yield return null;
    }
    #endregion
}