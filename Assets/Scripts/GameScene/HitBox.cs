using UnityEngine;

public class HitBox : MonoBehaviour
{
    #region 내부 변수 
    public float damage;
    public LayerMask targetLayer;
    private Collider2D collider2D;
    #endregion

    void Awake()
    {
        collider2D = GetComponent<Collider2D>();
    }

    // 오브젝트 활성화 시 자동으로 
    void OnEnable()
    {
        CheckForInitialOverlap();
    }

    #region 히트박스 내 존재할 경우 데미지 처리  
    private void CheckForInitialOverlap()
    {
        Collider2D[] overlappingColliders = new Collider2D[10]; // 현재 트리거랑 겹쳐있는 콜라이더 확인
        ContactFilter2D filter = new ContactFilter2D().NoFilter();
        filter.SetLayerMask(targetLayer); //설정한 타겟 레이어만 검사

        int count = collider2D.Overlap(filter, overlappingColliders);

        // 겹쳐있는 콜라이더에게 각각 로직 적용
        for (int i = 0; i < count; i++)
        {
            ApplyEffect(overlappingColliders[i]);
        }
    }

    private void ApplyEffect(Collider2D target)
    {
        IDamageable damageable = target.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
    }
    #endregion
}
