using UnityEngine;
using UnityEngine.UI;

public class CastleUI : MonoBehaviour
{
    #region 내부 변수 
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.2f, 0);
    private RectTransform rectTransform;
    private Camera camera;
    public Image hpBarImage;
    public Castle castle;
    #endregion

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        camera = Camera.main;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector2 screenPosition = camera.WorldToScreenPoint(target.position + offset);
        rectTransform.position = screenPosition;
    }

    public void UpdateHP(float currentHp, float maxHp)
    {
        if (hpBarImage != null)
        {
            hpBarImage.fillAmount = currentHp / maxHp;
            // Debug.Log("현재 체력" + hpBarImage.fillAmount);
        }
    }
}
