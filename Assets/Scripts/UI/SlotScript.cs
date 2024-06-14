using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotScript : MonoBehaviour
{
    [SerializeField]
    private GameObject MyBG;
    [SerializeField]
    private Image MyImg;

    internal void SetBg(Sprite MySprite)
    {
        if (MyBG) MyBG.SetActive(true);
        if (MyImg) MyImg.sprite = MySprite;
    }

    internal void ResetBG()
    {
        if (MyBG) MyBG.SetActive(false);
    }
}
