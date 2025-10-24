using UnityEngine;

public class HowToPlay : MonoBehaviour
{
    public GameObject[] Panels;
    private int currentIndex = 0;
    // 오디오
    private AudioSource audioSource;
    public AudioClip ClickBtnClip;
    // 싱글톤
    public static HowToPlay Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();

    }

    // 패널 첫 로드 / 마지막 로드(꺼질 때)
    public void FirstLoadPanel()
    {
        ShowPanel(currentIndex);
    }
    
    // 버튼 
    public void ShowPanel(int index)
    {
        for (int i = 0; i < Panels.Length; i++)
        {
            Panels[i].SetActive(i == index);
        }
        audioSource.clip = ClickBtnClip;
        audioSource.Play();
    }

    public void ShowNextPanel()
    {
        currentIndex++;

        if (currentIndex >= Panels.Length)
        {
            currentIndex = 0;
        }

        ShowPanel(currentIndex);
    }

}
