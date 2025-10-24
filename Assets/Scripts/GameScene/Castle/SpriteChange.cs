using System.Collections;
using UnityEngine;

public class SpriteChange : MonoBehaviour
{
    #region 내부 변수 
    public Castle castle;
    private SpriteRenderer spriteRenderer;
    public Sprite[] sprites;
    private int currenSpriteIndex;
    #endregion

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (castle.currentHp > 0)
        {
            UnBrockedSprite();
        }
        else
        {
            BrockedSprite();
        }
    }

    private void UnBrockedSprite()
    {
        if (sprites != null)
        {
            currenSpriteIndex = 0;
            spriteRenderer.sprite = sprites[currenSpriteIndex];
        }
    }

    private void BrockedSprite()
    {
        currenSpriteIndex = 1;
        spriteRenderer.sprite = sprites[currenSpriteIndex];
    }
}
