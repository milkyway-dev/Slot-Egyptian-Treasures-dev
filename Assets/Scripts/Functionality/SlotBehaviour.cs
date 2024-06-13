using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using System;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;
    [SerializeField]
    private List<SlotImage> Tempimages;

    [Header("Slots Objects")]
    [SerializeField]
    private GameObject[] Slot_Objects;
    [Header("Slots Elements")]
    [SerializeField]
    private LayoutElement[] Slot_Elements;

    [Header("Slots Transforms")]
    [SerializeField]
    private Transform[] Slot_Transform;

    [Header("Line Button Objects")]
    [SerializeField]
    private List<GameObject> StaticLine_Objects;

    [Header("Line Button Texts")]
    [SerializeField]
    private List<TMP_Text> StaticLine_Texts;

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Symbol1;
    [SerializeField]
    private Sprite[] Symbol2;
    [SerializeField]
    private Sprite[] Symbol3;
    [SerializeField]
    private Sprite[] Symbol4;
    [SerializeField]
    private Sprite[] Symbol5;
    [SerializeField]
    private Sprite[] Symbol6;
    [SerializeField]
    private Sprite[] Symbol7;
    [SerializeField]
    private Sprite[] Symbol8;
    [SerializeField]
    private Sprite[] Symbol9;
    [SerializeField]
    private Sprite[] Symbol10;
    [SerializeField]
    private Sprite[] Symbol11;
    [SerializeField]
    private Sprite[] Symbol12;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private ImageAnimation UppeLogo_Anim;
    [SerializeField]
    private ImageAnimation UppeLogo2_Anim;
    [SerializeField]
    private int[] Lines_num;
    [SerializeField]
    private Button Lines_Button;
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private TMP_Text Lines_text;
    [SerializeField]
    private TMP_Text TotalWin_text;
    [SerializeField] private Button AutoSpin_Button;
    [SerializeField] private Button AutoSpinStop_Button;
    [SerializeField] private Button MaxBet_Button;
    [SerializeField] private Button Betone_button;
    [SerializeField] private Button gamble_button;
    [SerializeField] private Button increase_autoSpin;
    [SerializeField] private Button decrease_autospin;
    [SerializeField] private TMP_Text autoSpinCountText;

    Coroutine tweenroutine = null;
    private int LineCounter = 0;
    private int BetCounter = 0;


    int tweenHeight = 0;

    [SerializeField]
    private GameObject Image_Prefab;

    [SerializeField]
    private PayoutCalculation PayCalculator;

    [SerializeField]
    private List<Tweener> alltweens = new List<Tweener>();

    [SerializeField]
    private List<ImageAnimation> TempList;

    [SerializeField]
    private int IconSizeFactor = 100;

    private int numberOfSlots = 5;

    [SerializeField]
    int verticalVisibility = 3;

    [SerializeField]
    private SocketIOManager SocketManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private BonusLevelCalculation _bonusManager;
    [SerializeField]
    private List<int> TempLineIds;
    [SerializeField]
    private List<string> x_animationString;
    [SerializeField]
    private List<string> y_animationString;
    [SerializeField]
    private Sprite[] Box_Sprites;


    private bool IsSpinning = false;
    [SerializeField]internal bool CheckPopups = false;
    private bool IsFreeSpin;
    bool IsAutoSpin = false;

    bool SlotRunning = false;
    [SerializeField] private AudioController audioController;
    [SerializeField] private GambleController gambleController;

    double bet = 0;
    double balance = 0;

    Coroutine AutoSpinRoutine = null;
    Coroutine FreeSpinRoutine = null;
    private Coroutine SlotAnimRoutine = null;
    [SerializeField]private int autospinCount=0;
    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (Lines_Button) Lines_Button.onClick.RemoveAllListeners();
        if (Lines_Button) Lines_Button.onClick.AddListener(ToggleLine);

        if (Lines_text) Lines_text.text = Lines_num[0].ToString();
        LineCounter = 0;

        if (Betone_button) Betone_button.onClick.RemoveAllListeners();
        if (Betone_button) Betone_button.onClick.AddListener(OnBetOne);

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

        if (increase_autoSpin) increase_autoSpin.onClick.RemoveAllListeners();
        if (increase_autoSpin) increase_autoSpin.onClick.AddListener(delegate { SetAutoSpinCounter(true); });

        if (decrease_autospin) decrease_autospin.onClick.RemoveAllListeners();
        if (decrease_autospin) decrease_autospin.onClick.AddListener(delegate { SetAutoSpinCounter(false); });


        //numberOfSlots = 5;
        //PopulateInitalSlots(numberOfSlots);
        //FetchLines();
    }

    private void GenerateSampleLines(TMP_Text LineID_Text)
    {
        int LineID = 1;
        try
        {
            LineID = int.Parse(LineID_Text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Exception while parsing " + e.Message);
        }
        PayCalculator.GeneratePayoutLinesBackend(LineID - 1);
    }

    internal void SetInitialUI()
    {
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        LineCounter = SocketManager.initialData.LinesCount.Count - 1;
        if (Lines_text) Lines_text.text = SocketManager.initialData.LinesCount[LineCounter].ToString();
        PayCalculator.SetButtonActive(SocketManager.initialData.LinesCount[LineCounter]);
        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.haveWon.ToString();
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();
        uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines, SocketManager.initUIData.spclSymbolTxt);
        PayCalculator.LineList = SocketManager.initialData.LinesCount;

    }

    private void AutoSpin()
    {
        if (audioController) audioController.PlayWLAudio("spin");
        if (autospinCount == 0 || autospinCount == 25)
            return;


        if (!IsAutoSpin)
        {
            IsAutoSpin = true;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(true);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(false);

            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                AutoSpinRoutine = null;
            }
            AutoSpinRoutine = StartCoroutine(AutoSpinCoroutine());
        }
    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseWebSocket();
    }

    private void StopAutoSpin()
    {
        if (audioController) audioController.PlayWLAudio("spin");
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            autospinCount = 0;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        int i = 0;
        while (i<autospinCount && IsAutoSpin)
        {
            i++;
            autoSpinCountText.text = (autospinCount - i).ToString();
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
        }
        autoSpinCountText.text = "0";
        ToggleButtonGrp(true);
        AutoSpinStop_Button.gameObject.SetActive(false);
        AutoSpin_Button.gameObject.SetActive(true);
        IsAutoSpin = false;
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }

    void SetAutoSpinCounter(bool inc) {

        if (inc)
            autospinCount++;
        else
            autospinCount--;
        if (autospinCount < 0)
            autospinCount = 0;
        if (autospinCount > 25)
            autospinCount = 25;

        autoSpinCountText.text = (autospinCount).ToString();
    }

    //internal void FreeSpin(int spins)
    //{
    //    if (!IsFreeSpin)
    //    {

    //        IsFreeSpin = true;
    //        ToggleButtonGrp(false);

    //        if (FreeSpinRoutine != null)
    //        {
    //            StopCoroutine(FreeSpinRoutine);
    //            FreeSpinRoutine = null;
    //        }
    //        FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));

    //    }
    //}

    //private IEnumerator FreeSpinCoroutine(int spinchances)
    //{
    //    int i = 0;
    //    while (i < spinchances)
    //    {
    //        StartSlots(IsAutoSpin);
    //        yield return tweenroutine;
    //        i++;
    //    }
    //    ToggleButtonGrp(true);
    //    IsFreeSpin = false;
    //}

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
    }

    void OnBetOne()
    {
        if (audioController) audioController.PlayButtonAudio();

        if (BetCounter < SocketManager.initialData.Bets.Count - 1)
        {
            BetCounter++;
        }
        else
        {
            BetCounter = 0;
        }

        if (TotalBet_text) TotalBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
    }

    private void ToggleLine()
    {
        if (audioController) audioController.PlayButtonAudio();
        PayCalculator.ToggleLine();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && SlotStart_Button.interactable)
        {
            StartSlots();
        }
    }

    internal void PopulateInitalSlots(int number, List<int> myvalues)
    {
        PopulateSlot(myvalues, number);
    }

    internal void LayoutReset(int number)
    {
        if (Slot_Elements[number]) Slot_Elements[number].ignoreLayout = true;
        if (SlotStart_Button) SlotStart_Button.interactable = true;
    }

    private void PopulateSlot(List<int> values , int number)
    {
        if (Slot_Objects[number]) Slot_Objects[number].SetActive(true);
        for(int i = 0; i<values.Count; i++)
        {
            GameObject myImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(myImg.transform.GetChild(1).GetComponent<Image>());
            images[number].slotImages[i].sprite = myImages[values[i]];
            PopulateAnimationSprites(images[number].slotImages[i].GetComponent<ImageAnimation>(), values[i]);
        }
        for (int k = 0; k < 2; k++)
        {
            GameObject mylastImg = Instantiate(Image_Prefab, Slot_Transform[number]);
            images[number].slotImages.Add(mylastImg.transform.GetChild(1).GetComponent<Image>());
            images[number].slotImages[images[number].slotImages.Count - 1].sprite = myImages[values[k]];
            PopulateAnimationSprites(images[number].slotImages[images[number].slotImages.Count - 1].GetComponent<ImageAnimation>(), values[k]);
        }
        if (mainContainer_RT) LayoutRebuilder.ForceRebuildLayoutImmediate(mainContainer_RT);
        tweenHeight = (values.Count * IconSizeFactor) - 280;
        GenerateMatrix(number);
    }

    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        switch (val)
        {
            case 0:
                for (int i = 0; i < Symbol1.Length; i++)
                {
                    animScript.textureArray.Add(Symbol1[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Symbol2.Length; i++)
                {
                    animScript.textureArray.Add(Symbol2[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Symbol3.Length; i++)
                {
                    animScript.textureArray.Add(Symbol3[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Symbol4.Length; i++)
                {
                    animScript.textureArray.Add(Symbol4[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Symbol5.Length; i++)
                {
                    animScript.textureArray.Add(Symbol5[i]);
                }
                break;
            case 5:
                for (int i = 0; i < Symbol6.Length; i++)
                {
                    animScript.textureArray.Add(Symbol6[i]);
                }
                break;
            case 6:
                for (int i = 0; i < Symbol7.Length; i++)
                {
                    animScript.textureArray.Add(Symbol7[i]);
                }
                break;
            case 7:
                for (int i = 0; i < Symbol8.Length; i++)
                {
                    animScript.textureArray.Add(Symbol8[i]);
                }
                break;
            case 8:
                for (int i = 0; i < Symbol9.Length; i++)
                {
                    animScript.textureArray.Add(Symbol9[i]);
                }
                break;
            case 9:
                for (int i = 0; i < Symbol10.Length; i++)
                {
                    animScript.textureArray.Add(Symbol10[i]);
                }
                break;
            case 10:
                for (int i = 0; i < Symbol11.Length; i++)
                {
                    animScript.textureArray.Add(Symbol11[i]);
                }
                break;
            case 11:
                for (int i = 0; i < Symbol12.Length; i++)
                {
                    animScript.textureArray.Add(Symbol12[i]);
                }
                break;
        }
    }

    private void StartSlots(bool autoSpin = false)
    {
        if (audioController) audioController.PlayWLAudio("spin");
        if (!autoSpin)
        {
            if (AutoSpinRoutine != null)
            {
                StopCoroutine(AutoSpinRoutine);
                StopCoroutine(tweenroutine);
                tweenroutine = null;
                AutoSpinRoutine = null;
            }

        }
        PayCalculator.DontDestroy.Clear();
        if (audioController) audioController.PlayWLAudio("spin");

        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        PayCalculator.ResetLines();
        tweenroutine = StartCoroutine(TweenRoutine());
    }



    //private IEnumerator TweenRoutine()
    //{

    //    for (int i = 0; i < numberOfSlots; i++)
    //    {
    //        InitializeTweening(Slot_Transform[i]);
    //        yield return new WaitForSeconds(0.1f);
    //    }

    //    SocketManager.AccumulateResult();
    //    yield return new WaitForSeconds(0.5f);
    //    List<int> resultnum = SocketManager.tempresult.StopList?.Split(',')?.Select(Int32.Parse)?.ToList();

    //    for (int i = 0; i < numberOfSlots; i++)
    //    {
    //        yield return StopTweening(resultnum[i] + 3, Slot_Transform[i], i);
    //    }
    //    yield return new WaitForSeconds(0.3f);
    //    GenerateMatrix(SocketManager.tempresult.StopList);
    //    CheckPayoutLineBackend(SocketManager.tempresult.resultLine, SocketManager.tempresult.x_animResult, SocketManager.tempresult.y_animResult);
    //    KillAllTweens();
    //    if (SlotStart_Button) SlotStart_Button.interactable = true;
    //    if (UppeLogo_Anim) UppeLogo_Anim.StopAnimation();
    //    if (UppeLogo2_Anim) UppeLogo2_Anim.StopAnimation();
    //}
    private IEnumerator TweenRoutine()
    {
        IsSpinning = true;
        PayCalculator.DontDestroy.Clear();
        PayCalculator.ResetStaticLine();
        ToggleButtonGrp(false);
        bet = 0;
        balance = 0;
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }


        try
        {
            bet = double.Parse(TotalBet_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        try
        {
            balance = double.Parse(Balance_text.text);
        }
        catch (Exception e)
        {
            Debug.Log("Error while conversion " + e.Message);
        }

        balance = balance - bet;

        if (Balance_text) Balance_text.text = balance.ToString();

        SocketManager.AccumulateResult(bet);

        yield return new WaitUntil(() => SocketManager.isResultdone);

        if (audioController) audioController.PlayWLAudio("spinStop");

        for (int j = 0; j < SocketManager.resultData.ResultReel.Count; j++)
        {
            List<int> resultnum = SocketManager.resultData.FinalResultReel[j]?.Split(',')?.Select(Int32.Parse)?.ToList();
            for (int i = 0; i < 5; i++)
            {
                if (images[i].slotImages[images[i].slotImages.Count - 5 + j]) images[i].slotImages[images[i].slotImages.Count - 5 + j].sprite = myImages[resultnum[i]];
                PopulateAnimationSprites(images[i].slotImages[images[i].slotImages.Count - 5 + j].gameObject.GetComponent<ImageAnimation>(), resultnum[i]);
            }
        }

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < numberOfSlots; i++)
        {
            yield return StopTweening(5, Slot_Transform[i], i);
        }

        yield return new WaitForSeconds(0.3f);
        CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.resultData.jackpot);
        KillAllTweens();


        CheckPopups = true;

        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.haveWon.ToString();

        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();

        if (SocketManager.resultData.jackpot > 0)
        {
            uiManager.PopulateWin(SocketManager.resultData.jackpot);
        }
        else
        {
            yield return new WaitForSeconds(1);
            CheckBonusGame();
        }

        yield return new WaitUntil(() => !CheckPopups);
        if (!IsAutoSpin)
        {
            if (SocketManager.playerdata.currentWining > 0) gambleController.toggleDoubleButton(true);
            ToggleButtonGrp(true);
            IsSpinning = false;
            //ToggleButtonGrp(true);
            //IsSpinning = false;
        }
        else
        {
            yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }
    }

    internal void updateBalance()
    {
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString();
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.haveWon.ToString();
    }

    internal void CheckBonusGame()
    {
        if (SocketManager.resultData.isBonus)
        {
            _bonusManager.startGame(SocketManager.resultData.BonusResult,(int)bet);
        }
        else
        {
            CheckPopups = false;
        }
    }

    void ToggleButtonGrp(bool toggle)
    {

        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (Lines_Button) Lines_Button.interactable = toggle;
        if (Betone_button) Betone_button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (gamble_button) gamble_button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
        if (increase_autoSpin) increase_autoSpin.interactable = toggle;
        if (decrease_autospin) decrease_autospin.interactable = toggle;

    }

    private IEnumerator slotLineAnim(List<string> x_AnimString, List<string> y_AnimString)
    {
        int n = 0;
        PayCalculator.ResetLines();
        while(n < 5)
        {
            List<int> x_anim = null;
            List<int> y_anim = null;
            for (int i = 0; i < TempLineIds.Count; i++)
            {
                x_anim = x_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
                y_anim = y_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
                PayCalculator.GeneratePayoutLinesBackend(TempLineIds[i] - 1);
                for (int k = 0; k < x_anim.Count; k++)
                {
                    Tempimages[x_anim[k]].slotImages[y_anim[k]].gameObject.GetComponent<SlotScript>().SetBg(Box_Sprites[TempLineIds[i] - 1]);
                }
                yield return new WaitForSeconds(3);
                for (int k = 0; k < x_anim.Count; k++)
                {
                    Tempimages[x_anim[k]].slotImages[y_anim[k]].gameObject.GetComponent<SlotScript>().ResetBG();
                }
                PayCalculator.ResetStaticLine();
            }
            for (int i = 0; i < TempLineIds.Count; i++)
            {
                PayCalculator.GeneratePayoutLinesBackend(TempLineIds[i] - 1);
            }
            yield return new WaitForSeconds(3);
            PayCalculator.ResetLines();
            yield return new WaitForSeconds(1);
            n++;
        }
        AnimStoppedProcess();
    }



    private void DisableLineButtons()
    {
        foreach (GameObject child in StaticLine_Objects)
        {
            child.GetComponent<ManageLineButtons>().isEnabled = false;
            child.GetComponent<Button>().interactable = false;
        }
    }

    private void AnimStoppedProcess()
    {
        StopGameAnimation();
        foreach (GameObject child in StaticLine_Objects)
        {
            child.GetComponent<ManageLineButtons>().isEnabled = true;
            child.GetComponent<Button>().interactable = true;
        }
        for (int i = 0; i < images.Count; i++)
        {
            foreach (Image child in images[i].slotImages)
            {
                child.gameObject.GetComponent<SlotScript>().ResetBG();
            }
        }
        PayCalculator.ResetLines();
        TempLineIds.Clear();
        TempLineIds.TrimExcess();
    }

    //private void StartGameAnimation(GameObject animObjects, int  LineID) 
    //{
    //    ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
    //    temp.StartAnimation();
    //    TempList.Add(temp);
    //}
    private void StartGameAnimation(GameObject animObjects)
    {
        //if (animObjects.transform.parent.GetChild(1).gameObject.transform.GetComponent<ImageAnimation>().isActiveAndEnabled)
        //{
        //    animObjects.transform.parent.GetChild(1).gameObject.SetActive(true);
        //}

        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();

        temp.StartAnimation();
        TempList.Add(temp);
    }

    private void StopGameAnimation()
    {
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }
        TempList.Clear();
        TempList.TrimExcess();
    }

    //private void CheckPayoutLineBackend(List<int> LineId, List<string> x_AnimString, List<string> y_AnimString)
    //{
    //    TempLineIds = LineId;
    //    List<int> x_anim = null;
    //    List<int> y_anim = null;

    //    if (LineId.Count > 0)
    //    {
    //        if (audioController) audioController.PlayWLAudio("win");
    //        for (int i = 0; i < x_AnimString.Count; i++)
    //        {
    //            x_anim = x_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
    //            y_anim = y_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

    //            for (int k = 0; k < x_anim.Count; k++)
    //            {
    //                StartGameAnimation(Tempimages[x_anim[k]].slotImages[y_anim[k]].gameObject, LineId[i]);
    //            }
    //        }
    //        PayCalculator.ResetStaticLine();
    //        DisableLineButtons();
    //        if (SlotAnimRoutine != null)
    //        {
    //            StopCoroutine(SlotAnimRoutine);
    //            SlotAnimRoutine = null;
    //        }
    //        SlotAnimRoutine = StartCoroutine(slotLineAnim(x_AnimString, y_AnimString));

    //    }
    //    else {

    //        if (audioController) audioController.PlayWLAudio("lose");

    //    }


    //}

    //private void GenerateMatrix(string stopList)
    //{
    //    List<int> numbers = stopList?.Split(',')?.Select(Int32.Parse)?.ToList();

    //    for (int i = 0; i < numbers.Count; i++)
    //    {
    //        for (int s = 0; s < verticalVisibility; s++)
    //        {
    //            Tempimages[i].slotImages.Add(images[i].slotImages[(images[i].slotImages.Count - (numbers[i] + 3)) + s]);
    //        }
    //    }
    //}
    private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString, double jackpot = 0)
    {
        List<int> points_anim = null;
        if (LineId.Count > 0)
        {
            if (audioController) audioController.PlayWLAudio("win");


            for (int i = 0; i < LineId.Count; i++)
            {
                PayCalculator.DontDestroy.Add(LineId[i] - 1);
                PayCalculator.GeneratePayoutLinesBackend(LineId[i] - 1);
            }

            if (jackpot > 0)
            {
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject);
                    }
                }
            }
            else
            {
                for (int i = 0; i < points_AnimString.Count; i++)
                {
                    points_anim = points_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

                    for (int k = 0; k < points_anim.Count; k++)
                    {
                        if (points_anim[k] >= 10)
                        {
                            StartGameAnimation(Tempimages[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].gameObject);
                        }
                        else
                        {
                            StartGameAnimation(Tempimages[0].slotImages[points_anim[k]].gameObject);
                        }
                    }
                }
            }
        }
        else
        {

            if (audioController) audioController.PlayWLAudio("lose");
        }


    }

    private void GenerateMatrix(int value)
    {
        for (int j = 0; j < 3; j++)
        {
            Tempimages[value].slotImages.Add(images[value].slotImages[images[value].slotImages.Count - 5 + j]);
        }
    }



    #region TweeningCode

    private void InitializeTweening(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        Tweener tweener = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener.Play();
        alltweens.Add(tweener);
    }
    private IEnumerator StopTweening(int reqpos, Transform slotTransform, int index)
    {
        alltweens[index].Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        alltweens[index] = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.5f).SetEase(Ease.OutElastic);
        yield return new WaitForSeconds(0.2f);
    }

    private void KillAllTweens()
    {
        for (int i = 0; i < numberOfSlots; i++)
        {
            alltweens[i].Kill();
        }
        alltweens.Clear();

    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

