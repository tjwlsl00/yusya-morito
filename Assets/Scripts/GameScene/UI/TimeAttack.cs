using UnityEngine;
using TMPro;

public class TimeAttack : MonoBehaviour
{
    #region 내부 변수 
    public TextMeshProUGUI timeText;
    public float TimeInLimit = 3.0f;
    public float TimeRemaining;
    public SpawnManager spawnManager;
    private float timer = 0f;
    private float timeToDecrease = 25f;
    // 게임 매니저 연결
    public GameManager gameManager;
    #endregion

    void Start()
    {
        TimeRemaining = TimeInLimit * 60;
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Play)
        {
            if (TimeRemaining > 0)
            {
                TimeRemaining -= Time.deltaTime;
                UpdateUI(TimeRemaining);
            }
            else
            {
                TimeRemaining = 0;
                UpdateUI(TimeRemaining);
            }

            #region 일정 주기 마다 스폰 주기 감소
            timer += Time.deltaTime;
            if (timer >= timeToDecrease)
            {
                Debug.Log("시간 감소 로직 실행");
                spawnManager.DecreaseSpawnInterval();
                timer = 0f;
            }
            #endregion
        }

    }

    private void UpdateUI(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60f);
        int seconds = Mathf.FloorToInt(timeToDisplay) % 60;

        timeText.text = "時間 " + string.Format("{0:00}：{1:00}", minutes, seconds);
    }
}
