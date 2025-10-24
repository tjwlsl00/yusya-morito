using UnityEngine;
using System.Collections.Generic;

public class coinMagnet : MonoBehaviour
{
    #region 내부 변수 
    public float magnetForce = 10f; //자석 끌어당기는 힘
    private List<Transform> attractedObjects = new List<Transform>(); //자석 범위에 들어온 아이템들을 저장할 리스트 
    #endregion

    void Update()
    {
        for (int i = attractedObjects.Count - 1; i >= 0; i--)
        {
            Transform item = attractedObjects[i];

            if (item == null)
            {
                attractedObjects.RemoveAt(i); //아이템이 획득 된 상태면 제거 
                continue;
            }

            item.position = Vector3.MoveTowards(item.position, transform.position, magnetForce * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            if (!attractedObjects.Contains(other.transform))
            {
                attractedObjects.Add(other.transform); //리스트에 추가 
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            if (attractedObjects.Contains(other.transform))
            {
                attractedObjects.Remove(other.transform); //리스트에서 제거 
            }
        }
    }

}
