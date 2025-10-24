using UnityEngine;

public class TurretSpawnPoint : MonoBehaviour
{
    // 클릭되면 해당 정보를 TurretSpawnManager에게 [Index] 전달
    private void OnMouseDown()
    {
        TurretSpawnManager.instance.OnSpawnPointClicked(this);
    }

}
