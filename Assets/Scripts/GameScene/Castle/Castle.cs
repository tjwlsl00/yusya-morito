using UnityEngine;

public class Castle : MonoBehaviour, IDamageable
{
    #region 내부 변수 
    public float currentHp;
    public float MaxHp;
    public GameObject CastleUIPrefab;
    private CastleUI castleUI;
    #endregion

    void Awake()
    {
        // 상태 초기화 
        currentHp = MaxHp;

        #region  UI연결
        Transform canvasTransfrom = GameObject.Find("UI_Canvas").transform;
        GameObject castleUIObj = Instantiate(CastleUIPrefab, canvasTransfrom); // 프리팹 생성
        castleUI = castleUIObj.GetComponent<CastleUI>(); //해당 프리팹에서 PlayerUI 스크립트 가져오기 
        castleUI.target = this.transform; //프리팹에 붙어 있는 스크립트 찾아서 target 자신으로 설정
        castleUI.castle = this;
        #endregion
    }

    void Update()
    {
        if (currentHp <= 0)
        {
            DestroyCastle();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHp -= damageAmount;

        if (currentHp <= 0)
        {
            DestroyCastle();
        }
        castleUI.UpdateHP(currentHp, MaxHp);
    }

    private void DestroyCastle()
    {
        Debug.Log("성이 파괴되었습니다.");
    }

}
