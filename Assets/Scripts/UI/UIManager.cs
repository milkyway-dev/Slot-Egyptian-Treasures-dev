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
    [SerializeField] private Button SoundButton;

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

    private bool isSOundOn=true;

    [SerializeField] private AudioController audioController;


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

        if (SoundButton) SoundButton.onClick.RemoveAllListeners();
        if (SoundButton) SoundButton.onClick.AddListener(ToggleSound);
    }

    void ToggleSound() {
        isSOundOn =!isSOundOn;
        audioController.ToggleMute(isSOundOn);
    }

    private void OpenPopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (Popup) Popup.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);
    }

    private void ClosePopup(GameObject Popup)
    {
        if (audioController) audioController.PlayButtonAudio();

        if (Popup) Popup.SetActive(false);
        if (MainPopup_Object) MainPopup_Object.SetActive(false);
    }

    private void TurnPage(bool type)
    {
        if (audioController) audioController.PlayButtonAudio();

        if (type)
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
