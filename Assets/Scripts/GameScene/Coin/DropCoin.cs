using UnityEngine;

public class DropCoin : MonoBehaviour
{
    #region 내부변수 
    public GameObject coinPrefab; // 돈 오브젝트 
    public int numberOfCoins = 1; //드랍할 코인 개수 
    #endregion

    #region 코인 드랍
    public void MakeCoin(int amount)
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            // 코인 생성하고 돈 값 전달
            GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

            AutoTakeCoin autoTakeCoin = coin.GetComponent<AutoTakeCoin>();
            autoTakeCoin.coinAmount = amount;
        }
    }
    #endregion
}
