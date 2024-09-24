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
    [SerializeField] private Button exit_button;

    [Header("Popus UI")]
    [SerializeField]
    private GameObject MainPopup_Object;

    [Header("Win Popup")]
    [SerializeField]
    private GameObject WinPopup_Object;
    [SerializeField]
    private TMP_Text Win_Text;

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
    [SerializeField] private GameObject[] pageList;
    [SerializeField] private TMP_Text[] SymbolsText;
    [SerializeField] private TMP_Text[] SpecialSymbolsText;

    private bool isSOundOn=true;

    [SerializeField] private AudioController audioController;
    [SerializeField] private SlotBehaviour slotBehaviour;


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

        if (exit_button) exit_button.onClick.RemoveAllListeners();
        if (exit_button) exit_button.onClick.AddListener(CallOnExitFunction);
    }

    internal void PopulateWin(double amount)
    {
        int initAmount = 0;
        if (WinPopup_Object) WinPopup_Object.SetActive(true);
        if (MainPopup_Object) MainPopup_Object.SetActive(true);

        DOTween.To(() => initAmount, (val) => initAmount = val, (int)amount, 5f).OnUpdate(() =>
        {
            if (Win_Text) Win_Text.text = initAmount.ToString();
        });

        DOVirtual.DelayedCall(6f, () =>
        {
            if (WinPopup_Object) WinPopup_Object.SetActive(false);
            if (MainPopup_Object) MainPopup_Object.SetActive(false);
            //slotManager.CheckBonusGame();
        });
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
        for (int i = 0; i < pageList.Length; i++)
        {
            pageList[i].SetActive(false);
        }
        if (type)
        {
            if (Previous_Button) Previous_Button.interactable = true;
            paginationCounter++;
            //if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];
            pageList[paginationCounter-1].SetActive(true);
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
            pageList[paginationCounter - 1].SetActive(true);

            //if (Info_Image) Info_Image.sprite = Info_Sprites[paginationCounter - 1];
            if (paginationCounter == 1)
            {
                if (Previous_Button) Previous_Button.interactable = false;
            }
            if (Pagination_Text) Pagination_Text.text = paginationCounter + "  3";
        }
    }
    internal void InitialiseUIData(string SupportUrl, string AbtImgUrl, string TermsUrl, string PrivacyUrl, Paylines symbolsText)
    {
        PopulateSymbolsPayout(symbolsText);
    }

    internal void LowBalPopup()
    {
        //OpenPopup(LBPopup_Object);
    }

    private void PopulateSymbolsPayout(Paylines paylines)
    {
        for (int i = 0; i < SymbolsText.Length; i++)
        {
            string text = null;
            if (paylines.symbols[i].Multiplier[0][0] != 0)
            {
                text += "5x - " + paylines.symbols[i].Multiplier[0][0];
            }
            if (paylines.symbols[i].Multiplier[1][0] != 0)
            {
                text += "\n4x - " + paylines.symbols[i].Multiplier[1][0];
            }
            if (paylines.symbols[i].Multiplier[2][0] != 0)
            {
                text += "\n3x - " + paylines.symbols[i].Multiplier[2][0];
            }
            if (SymbolsText[i]) SymbolsText[i].text = text;
        }

        //for (int i = 0; i < paylines.symbols.Count; i++)
        //{
        //    if (paylines.symbols[i].Name.ToUpper() == "BONUS")
        //    {
        //        if (Bonus_Text) Bonus_Text.text = paylines.symbols[i].description.ToString();
        //    }
        //}
    }

    private void CallOnExitFunction()
    {
        slotBehaviour.CallCloseSocket();
        Application.ExternalCall("window.parent.postMessage", "onExit", "*");
    }
}
