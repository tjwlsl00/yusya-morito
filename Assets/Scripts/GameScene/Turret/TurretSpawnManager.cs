using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class TurretSpawnManager : MonoBehaviour
{
    #region 내부 변수 
    public static TurretSpawnManager instance;
    public GameObject[] turretPrefabs; //터렛 프리팹 오브젝트 / 현재 : 2개 
    private GameObject selectedTurrentPrefab; // 선택된 터렛 아이템 
    public List<TurretSpawnPoint> spawnPoints;// 터렛 설치 할 장소
    //bool 
    private bool isPurchased;
    //플레이어 자금 정보 가져오기 (포탑 사면 돈 차감)
    public Player player;
    public Shop shop;
    // 성이랑 연결 체력 0되면 스폰 포인트 안보이게 처리 
    // 오디오 
    private AudioSource audioSource;
    public AudioClip ButtonClip;
    public AudioClip PurchaseClip;
    public AudioClip SellClip;
    #endregion

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // 오디오 
        audioSource = GetComponent<AudioSource>();

        // TurretSpawnPoint라는 스크립트를 가지고 있는 모드 오브젝트를 가져와서 새로운 리스트에 추가
        spawnPoints = new List<TurretSpawnPoint>(FindObjectsOfType<TurretSpawnPoint>());
    }

    #region 어떤 포탑을 구매할지 선택 / 플레이어 코인 감소
    public void SelectPurchaseTurret(int turretIndex)
    {
        if (turretIndex >= 0 && turretIndex < turretPrefabs.Length)
        {
            selectedTurrentPrefab = turretPrefabs[turretIndex]; //현재 선택된 프리팹은 해당 배열의 몇번째 것이다.
            Debug.Log("선택된 프리팹" + selectedTurrentPrefab);
        }
        else
        {
            selectedTurrentPrefab = null;
        }

        // 오디오 
        audioSource.clip = ButtonClip;
        audioSource.Play();
    }
    #endregion

    #region 스폰 포인트 클릭(포탑 선택 상태) / 터렛 생성 
    public void OnSpawnPointClicked(TurretSpawnPoint clickedPoint)
    {
        int clickedIndex = spawnPoints.IndexOf(clickedPoint);//TurretSpawnPoint 로부터 정보 전달 받아온 인덱스 

        if (clickedIndex != -1)
        {
            // 클릭된 스폰 포인트의 위치
            Transform spawnLocation = spawnPoints[clickedIndex].transform;

            if (!shop.isSell)
            {
                // 해당 스폰 포인트에 자식이 없는지 확인
                if (spawnLocation.childCount == 0)
                {
                    //해당 스폰 포인트의 자식으로 터렛 생성 
                    Instantiate(selectedTurrentPrefab, spawnLocation.position, Quaternion.identity, spawnLocation);

                    //설치된 터렛에서 가격 정보 가져오기 
                    Turret turret = selectedTurrentPrefab.GetComponent<Turret>();
                    int turretPrice = turret.price;

                    //플레이어 코인 차감
                    player.ReduceCoin(turretPrice);

                    //설치하고 선택해제 
                    selectedTurrentPrefab = null;

                    // 오디오 
                    audioSource.clip = PurchaseClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.Log("이미 해당 스폰 포인트에 포탑이 설치 되어있습니다.");
                }
            }
            // 만약 스폰포인트를 선택했는데 이미 자식에 오브젝트가 있을 경우 자식 오브젝트 삭제
            else
            {
                if (spawnLocation.childCount > 0)
                {
                    foreach (Transform child in spawnLocation.transform)
                    {
                        Turret turret = child.GetComponent<Turret>();
                        int turretPrice = turret.price;
                        player.PayBackCoin(turretPrice); //일정 터렛 값 반환 후 파괴 

                        // 오디오 
                        audioSource.clip = SellClip;
                        audioSource.Play();

                        Destroy(child.gameObject);
                    }
                }
                shop.isSell = false;
            }
        }
    }
    #endregion

}
