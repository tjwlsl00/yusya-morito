using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawnData //데이터
{
    public GameObject spawnObjPrefab; //실제 소환되는 오브젝트
    public float spawnInterval; // 소환 주기 
    public float spawnTimer; // 각 몬스터 별로 타미어 카운팅
}

public class SpawnManager : MonoBehaviour
{
    #region 내부 변수 
    public List<SpawnData> SpawnList; //SpawnData정보 여러개 담는 리스트 
    public Transform[] spawnPoints; //오브젝트 스폰될 위치 
    public GameManager gameManager;
    #endregion

    void Start()
    {
        foreach (var data in SpawnList)
        {
            data.spawnTimer = 0f; //게임 시작하자마자 모두 소환되는 거 방지 
        }
    }

    void Update()
    {
        if (gameManager.GameCurrentDireaction == GameManager.GameMangerDireaction.Play)
        {
            foreach (var data in SpawnList) //등록된 모든 오브젝트 정보 하나씩 확인
            {
                data.spawnTimer += Time.deltaTime; //각 오브젝트별로 개인 타이머 

                if (data.spawnTimer >= data.spawnInterval) //해당 오브젝트의 타이머가 설정된 스폰주기를 넘어섰다면 Spawn로직 실행 
                {
                    Spawn(data.spawnObjPrefab);
                    data.spawnTimer = 0f;
                }
            }
        }
    }

    void Spawn(GameObject spawnObjPrefab)
    {
        if (spawnPoints.Length == 0 || spawnObjPrefab == null)
        {
            Debug.LogError("스폰 위치(Spawn Points) 또는 몬스터 프리팹이 설정되지 않았습니다.");
            return;
        }

        // 스폰 위치 중 한 곳을 무작위로 선택 
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];

        // 해당 위치에 몬스터 생성 
        Instantiate(spawnObjPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void DecreaseSpawnInterval()
    {
        foreach (var data in SpawnList)
        {
            float decreaseAmount = data.spawnInterval * 0.3f;

            data.spawnInterval -= decreaseAmount;
            data.spawnTimer -= decreaseAmount;

            Debug.Log(data.spawnInterval + " / " + data.spawnTimer + "로 초기화");

            if (data.spawnInterval < 0.5f)
            {
                data.spawnInterval = 0.5f;
            }

        }
    }

    void OnDrawGizmosSelected() //spawnPoint 위치 확인 
    {
        Gizmos.color = Color.blue;

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point == null)
                {
                    continue;
                }

                Gizmos.DrawWireSphere(point.position, 0.5f);
            }
        }
    }

}