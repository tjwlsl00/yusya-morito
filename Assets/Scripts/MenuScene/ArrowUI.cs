using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ArrowUI : MonoBehaviour
{
    public Image ArrowPanelLeft;
    public Image ArrowPanelRight;
    [SerializeField] float ArrowActiveTime = 1f;

    void Awake()
    {
        ArrowPanelLeft.gameObject.SetActive(false);
        ArrowPanelRight.gameObject.SetActive(false);
    }

    void Start()
    {
        Arrowvisible();
    }

    private void Arrowvisible()
    {
        ArrowPanelLeft.gameObject.SetActive(true);
        ArrowPanelRight.gameObject.SetActive(true);
        StartCoroutine(ArrowUnvisible());
    }

    IEnumerator ArrowUnvisible()
    {
        yield return new WaitForSeconds(ArrowActiveTime);
        ArrowPanelLeft.gameObject.SetActive(false);
        ArrowPanelRight.gameObject.SetActive(false);
        yield return new WaitForSeconds(ArrowActiveTime);
        Arrowvisible();
    }
}
