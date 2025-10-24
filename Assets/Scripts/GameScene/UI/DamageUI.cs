using UnityEngine;

public class DamageUI : MonoBehaviour
{
    #region 내부 변수 
    private Vector3 initialLocalScale;
    #endregion

    void Start()
    {
        initialLocalScale = transform.localScale;
    }

    void LateUpdate()
    {
        Transform parent = transform.parent;
        if (parent == null) return;

        if (parent.localScale.x < 0)
        {
            transform.localScale = new Vector3(-initialLocalScale.x, initialLocalScale.y, initialLocalScale.z);
        }
        else
        {
            transform.localScale = initialLocalScale;
        }
    }
}
