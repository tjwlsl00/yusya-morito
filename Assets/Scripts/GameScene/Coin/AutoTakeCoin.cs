using UnityEngine;

public class AutoTakeCoin : MonoBehaviour
{
    public AudioClip EarnCoinClip;
    public int coinAmount;
    private GameObject GameManagerObj;
    private GameManager gameManager;

    void Awake()
    {
        GameManagerObj = GameObject.Find("GameManager");
        gameManager = GameManagerObj.GetComponent<GameManager>();
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Victroy || gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.GameOver)
        {
            Destroy(gameObject);
        }
    }

    #region 코인 자동 회복
    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            player.EarnCoin(coinAmount);
            Debug.Log("플레이어가 돈을 획득했습니다.");

            // 사운드 
            AudioSource.PlayClipAtPoint(EarnCoinClip, transform.position);
            Destroy(gameObject); //파괴
        }
    }
    #endregion
}
