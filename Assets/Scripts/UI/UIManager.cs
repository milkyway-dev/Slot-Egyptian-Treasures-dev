using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;


public class UIManager : MonoBehaviour
{

    [Header("Menu UI")]
    [SerializeField]
    private Button Info_Button;

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Paytable Popup")]
    [SerializeField]
    private GameObject PaytablePopup_Object;
    [SerializeField]
    private Button PaytableExit_Button;
    [SerializeField]
    private Button Next_Button;
    [SerializeField]
    private Button Previous_Button;
    [SerializeField]
    private TMP_Text Pagination_Text;
    [SerializeField]
    private Image Info_Image;
    [SerializeField]
    private Sprite[] Info_Sprites;
    private int paginationCounter = 1;


    private void Start()
    {
        if (PaytableExit_Button) PaytableExit_Button.onClick.RemoveAllListeners();
        if (PaytableExit_Button) PaytableExit_Button.onClick.AddListener(delegate { ClosePopup(PaytablePopup_Object); });

        if (Info_Button) Info_Button.onClick.RemoveAllListeners();
        if (Info_Button) Info_Button.onClick.AddListener(delegate { OpenPopup(PaytablePopup_Object); });

        paginationCounter = 1;
        if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];

        if (Next_Button) Next_Button.onClick.RemoveAllListeners();
        if (Next_Button) Next_Button.onClick.AddListener(delegate { TurnPage(true); });

        if (Previous_Button) Previous_Button.onClick.RemoveAllListeners();
        if (Previous_Button) Previous_Button.onClick.AddListener(delegate { TurnPage(false); });

        if (Previous_Button) Previous_Button.interactable = false;

        if (Pagination_Text) Pagination_Text.text = paginationCounter + "  3";
    }


    private void OpenPopup(GameObject Popup)
    {
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (Popup) Popup.SetActive(false);
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }

    private void TurnPage(bool type)
    {
        if(type)
        {
            if (Previous_Button) Previous_Button.interactable = true;
            paginationCounter++;
            if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];
            if(paginationCounter == 3)
            {
                if (Next_Button) Next_Button.interactable = false;
            }
            if (Pagination_Text) Pagination_Text.text = paginationCounter + "  3";

        }
        else
        {
            if (Next_Button) Next_Button.interactable = true;
            paginationCounter--;
            if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];
            if (paginationCounter == 1)
            {
                if (Previous_Button) Previous_Button.interactable = false;
            }
            if (Pagination_Text) Pagination_Text.text = paginationCounter + "  3";
        }
    }
}
