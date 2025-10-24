using UnityEngine;

public class TurretBullet : MonoBehaviour
{
    #region 내부 변수 
    public float bulletSpeed;
    public float AttackForce;
    private Transform currentTarget;
    #endregion

    // 부모에서 전달 받은 타겟 위치 정보
    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }

    void Update()
    {
        if (currentTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, currentTarget.position, bulletSpeed * Time.deltaTime);
    }

    #region 충돌 / 적과 충돌 이후 사라지고 데미지 10 
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Ai enemy = other.GetComponent<Ai>();

            if (enemy != null)
            {
                enemy.TakeDamage(AttackForce);
            }
            Destroy(gameObject);
        }
    }
    #endregion
}
