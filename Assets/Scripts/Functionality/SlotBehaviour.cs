using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotBehaviour : MonoBehaviour
{
    [SerializeField]
    private RectTransform mainContainer_RT;

    [Header("Sprites")]
    [SerializeField]
    private Sprite[] myImages;  //images taken initially

    [Header("Slot Images")]
    [SerializeField]
    private List<SlotImage> images;     //class to store total images
    [SerializeField]
    private List<SlotImage> Tempimages;     //class to store the result matrix
    [SerializeField]
    private List<BoxScript> TempBoxScripts;
    [SerializeField]
    private List<Sprite> Box_Sprites; 

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

    [Header("Buttons")]
    [SerializeField]
    private Button SlotStart_Button;
    [SerializeField]
    private Button MaxBet_Button;
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField]
    private Button AutoSpinStop_Button;
    [SerializeField]
    private Button BetOne_button;
    [SerializeField]
    private Button AutoStartPlus_Button;
    [SerializeField]
    private Button AutoStartMinus_Button;
    [SerializeField]
    private Button TBPlus_Button;
    [SerializeField]
    private Button TBMinus_Button;

    [Header("Animated Sprites")]
    [SerializeField]
    private Sprite[] Symbol1_Sprite;
    [SerializeField]
    private Sprite[] Symbol2_Sprite;
    [SerializeField]
    private Sprite[] Symbol3_Sprite;
    [SerializeField]
    private Sprite[] Symbol4_Sprite;
    [SerializeField]
    private Sprite[] Symbol5_Sprite;
    [SerializeField]
    private Sprite[] Symbol6_Sprite;
    [SerializeField]
    private Sprite[] Symbol7_Sprite;
    [SerializeField]
    private Sprite[] Symbol8_Sprite;
    [SerializeField]
    private Sprite[] Symbol9_Sprite;
    [SerializeField]
    private Sprite[] Symbol10_Sprite;
    [SerializeField]
    private Sprite[] Symbol11_Sprite;
    [SerializeField]
    private Sprite[] Symbol12_Sprite;

    [Header("Miscellaneous UI")]
    [SerializeField]
    private TMP_Text Balance_text;
    [SerializeField]
    private TMP_Text TotalBet_text;
    [SerializeField]
    private TMP_Text LineBet_text;
    [SerializeField]
    private TMP_Text TotalWin_text;
    [SerializeField]
    private TMP_Text AutoSpin_Text;
    private int[] AutoSpinsValue = { 0, 5, 10, 25, 50, 100 };
    private int AutoSpinCounter = 0;
    private int AutoSpinNum = 0;

    [Header("Audio Management")]
    [SerializeField] private AudioController audioController;

    int tweenHeight = 0;  //calculate the height at which tweening is done

    [SerializeField]
    private GameObject Image_Prefab;

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private List<Tweener> alltweens = new List<Tweener>();

    private Tweener WinTween = null;

    [SerializeField]
    private List<ImageAnimation> TempList;  //stores the sprites whose animation is running at present 

    [SerializeField]
    private int IconSizeFactor = 100;       //set this parameter according to the size of the icon and spacing

    private int numberOfSlots = 5;          //number of columns

    [SerializeField]
    private SocketIOManager SocketManager;
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private BonusLevelCalculation _bonusManager;

    [SerializeField]
    private GameObject Gamble;
    [SerializeField]
    private GambleController gambleController;

    Coroutine AutoSpinRoutine = null;
    Coroutine tweenroutine;
    Coroutine FreeSpinRoutine = null;
    Coroutine BoxAnimRoutine = null;
    bool IsFreeSpin = false;
    bool IsAutoSpin = false;
    bool IsSpinning = false;
    private bool CheckSpinAudio = false;
    internal bool CheckPopups = false;
    private int BetCounter = 0;
    private double currentBalance = 0;
    private double currentTotalBet = 0;
    internal double currentBet = 0;

    protected int Lines = 20;

    private void Start()
    {
        IsAutoSpin = false;
        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(delegate { StartSlots(); });

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (BetOne_button) BetOne_button.onClick.RemoveAllListeners();
        if (BetOne_button) BetOne_button.onClick.AddListener(ChangeBet);

        if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);

        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.RemoveAllListeners();
        if (AutoSpinStop_Button) AutoSpinStop_Button.onClick.AddListener(StopAutoSpin);

        if (AutoStartPlus_Button) AutoStartPlus_Button.onClick.RemoveAllListeners();
        if (AutoStartPlus_Button) AutoStartPlus_Button.onClick.AddListener(delegate { ToggleAutoSpins(true); });

        if (AutoStartMinus_Button) AutoStartMinus_Button.onClick.RemoveAllListeners();
        if (AutoStartMinus_Button) AutoStartMinus_Button.onClick.AddListener(delegate { ToggleAutoSpins(false); });

        if (TBPlus_Button) TBPlus_Button.onClick.RemoveAllListeners();
        if (TBPlus_Button) TBPlus_Button.onClick.AddListener(delegate { ToggleTotalBet(true); });

        if (TBMinus_Button) TBMinus_Button.onClick.RemoveAllListeners();
        if (TBMinus_Button) TBMinus_Button.onClick.AddListener(delegate { ToggleTotalBet(false); });

        tweenHeight = (15 * IconSizeFactor) - 280;
    }

    private void ToggleAutoSpins(bool isIncrement)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (isIncrement) 
        {
            if (AutoSpinCounter < AutoSpinsValue.Length - 1)
            {
                AutoSpinCounter++;
                AutoSpinNum = AutoSpinsValue[AutoSpinCounter];
                if (AutoSpin_Text) AutoSpin_Text.text = AutoSpinNum.ToString();
                if (AutoStartMinus_Button) AutoStartMinus_Button.interactable = true;
                if (AutoSpin_Button) AutoSpin_Button.interactable = true;
            }
            if (AutoSpinCounter >= AutoSpinsValue.Length - 1)
            {
                if (AutoStartPlus_Button) AutoStartPlus_Button.interactable = false;
                if (AutoStartMinus_Button) AutoStartMinus_Button.interactable = true;
            }
        }
        else
        {
            if (AutoSpinCounter > 0)
            {
                AutoSpinCounter--;
                AutoSpinNum = AutoSpinsValue[AutoSpinCounter];
                if (AutoSpin_Text) AutoSpin_Text.text = AutoSpinNum.ToString();
                if (AutoStartPlus_Button) AutoStartPlus_Button.interactable = true;
                if (AutoSpin_Button) AutoSpin_Button.interactable = true;
            }
            if (AutoSpinCounter <= 0)
            {
                if (AutoStartPlus_Button) AutoStartPlus_Button.interactable = true;
                if (AutoStartMinus_Button) AutoStartMinus_Button.interactable = false;
                if (AutoSpin_Button) AutoSpin_Button.interactable = false;
            }
        }
    }

    private void AutoSpin()
    {
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

    internal void GambleCollect()
    {
        SocketManager.GambleCollectCall();
    }

    internal void FreeSpin(int spins)
    {
        if (!IsFreeSpin)
        {

            IsFreeSpin = true;
            ToggleButtonGrp(false);

            if (FreeSpinRoutine != null)
            {
                StopCoroutine(FreeSpinRoutine);
                FreeSpinRoutine = null;
            }
            FreeSpinRoutine = StartCoroutine(FreeSpinCoroutine(spins));

        }
    }

    internal void SetInitialUI(List<string> BonusList)
    {
        BetCounter = 0;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString("f2");
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("f2");
        if (TotalWin_text) TotalWin_text.text = "0.00";
        currentBalance = SocketManager.playerdata.Balance;
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        uiManager.InitialiseUIData(SocketManager.initUIData.AbtLogo.link, SocketManager.initUIData.AbtLogo.logoSprite, SocketManager.initUIData.ToULink, SocketManager.initUIData.PopLink, SocketManager.initUIData.paylines);
        _bonusManager.BonusinitialSetup(BonusList);
    }

    private void StopAutoSpin()
    {
        if (IsAutoSpin)
        {
            IsAutoSpin = false;
            if (AutoSpinStop_Button) AutoSpinStop_Button.gameObject.SetActive(false);
            if (AutoSpin_Button) AutoSpin_Button.gameObject.SetActive(true);
            StartCoroutine(StopAutoSpinCoroutine());
        }
    }

    private IEnumerator AutoSpinCoroutine()
    {
        while (IsAutoSpin)
        {
            if (AutoSpinNum > 0)
            {
                StartSlots(IsAutoSpin);
                AutoSpinNum--;
                if (AutoSpin_Text) AutoSpin_Text.text = AutoSpinNum.ToString();
                yield return tweenroutine;
            }
            else
            {
                StopAutoSpin();
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private IEnumerator FreeSpinCoroutine(int spinchances)
    {
        int i = 0;
        while (i < spinchances)
        {
            StartSlots(IsAutoSpin);
            yield return tweenroutine;
            i++;
        }
        ToggleButtonGrp(true);
        if (AutoSpinNum <= 0)
        {
            AutoStartMinus_Button.interactable = false;
            AutoSpin_Button.interactable = false;
        }
        IsFreeSpin = false;
    }

    private IEnumerator StopAutoSpinCoroutine()
    {
        yield return new WaitUntil(() => !IsSpinning);
        ToggleButtonGrp(true);
        if (AutoSpin_Button) AutoSpin_Button.interactable = false;
        if (AutoStartMinus_Button) AutoStartMinus_Button.interactable = false;
        AutoSpinCounter = 0;
        AutoSpinNum = 0;
        if (AutoSpin_Text) AutoSpin_Text.text = AutoSpinNum.ToString();
        if (AutoSpinRoutine != null || tweenroutine != null)
        {
            StopCoroutine(AutoSpinRoutine);
            StopCoroutine(tweenroutine);
            tweenroutine = null;
            AutoSpinRoutine = null;
            StopCoroutine(StopAutoSpinCoroutine());
        }
    }

    private void MaxBet()
    {
        if (audioController) audioController.PlayButtonAudio();
        BetCounter = SocketManager.initialData.Bets.Count - 1;
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString("f2");
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    private void ChangeBet()
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
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString("f2");
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    private void ToggleTotalBet(bool IncDec)
    {
        if (audioController) audioController.PlayButtonAudio();
        if (IncDec)
        {
            if (BetCounter < SocketManager.initialData.Bets.Count - 1)
            {
                BetCounter++;
            }
            else
            {
                BetCounter = 0;
            }
        }
        else
        {
            if (BetCounter > 0)
            {
                BetCounter--;
            }
            else
            {
                BetCounter = SocketManager.initialData.Bets.Count - 1;
            }
        }
        if (LineBet_text) LineBet_text.text = SocketManager.initialData.Bets[BetCounter].ToString();
        if (TotalBet_text) TotalBet_text.text = (SocketManager.initialData.Bets[BetCounter] * Lines).ToString("f2");
        currentTotalBet = SocketManager.initialData.Bets[BetCounter] * Lines;
        CompareBalance();
    }

    private void CompareBalance()
    {
        if (currentBalance < currentTotalBet)
        {
            uiManager.LowBalPopup();
            if (AutoSpin_Button) AutoSpin_Button.interactable = false;
            if (SlotStart_Button) SlotStart_Button.interactable = false;
        }
        else
        {
            if (AutoSpin_Button) AutoSpin_Button.interactable = true;
            if (SlotStart_Button) SlotStart_Button.interactable = true;
            if (AutoSpinNum <= 0)
            {
                AutoStartMinus_Button.interactable = false;
                AutoSpin_Button.interactable = false;
            }
        }
    }

    //function to populate animation sprites accordingly
    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        animScript.textureArray.Clear();
        animScript.textureArray.TrimExcess();
        switch (val)
        {
            case 0:
                for (int i = 0; i < Symbol1_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol1_Sprite[i]);
                }
                break;
            case 1:
                for (int i = 0; i < Symbol2_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol2_Sprite[i]);
                }
                break;
            case 2:
                for (int i = 0; i < Symbol3_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol3_Sprite[i]);
                }
                break;
            case 3:
                for (int i = 0; i < Symbol4_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol4_Sprite[i]);
                }
                break;
            case 4:
                for (int i = 0; i < Symbol5_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol5_Sprite[i]);
                }
                break;
            case 5:
                for (int i = 0; i < Symbol6_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol6_Sprite[i]);
                }
                break;
            case 6:
                for (int i = 0; i < Symbol7_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol7_Sprite[i]);
                }
                break;

            case 7:
                for (int i = 0; i < Symbol8_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol8_Sprite[i]);
                }
                break;
            case 8:
                for (int i = 0; i < Symbol9_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol9_Sprite[i]);
                }
                break;
            case 9:
                for (int i = 0; i < Symbol10_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol10_Sprite[i]);
                }
                break;
            case 10:
                for (int i = 0; i < Symbol11_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol11_Sprite[i]);
                }
                break;
            case 11:
                for (int i = 0; i < Symbol12_Sprite.Length; i++)
                {
                    animScript.textureArray.Add(Symbol12_Sprite[i]);
                }
                break;
        }
    }
    //starts the spin process
    private void StartSlots(bool autoSpin = false)
    {
        if (audioController) audioController.PlaySpinButtonAudio();
        if (gambleController) gambleController.toggleDoubleButton(false);
        if (gambleController) gambleController.GambleTweeningAnim(false);
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
        PayCalculator.DontDestroyLines.Clear();
        WinningsAnim(false);
        if (TempList.Count > 0)
        {
            StopGameAnimation();
        }
        PayCalculator.ResetStaticLine();
        tweenroutine = StartCoroutine(TweenRoutine());
    }

    private void OnApplicationFocus(bool focus)
    {
        audioController.CheckFocusFunction(focus, CheckSpinAudio);
    }

    internal void shuffleInitialMatrix()
    {
        for (int i = 0; i < Tempimages.Count; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int randomIndex = UnityEngine.Random.Range(0, myImages.Length);
                Tempimages[i].slotImages[j].sprite = myImages[randomIndex];
            }
        }
    }


    private IEnumerator TweenRoutine()
    {
        if (currentBalance < currentTotalBet && !IsFreeSpin)
        {
            CompareBalance();
            StopAutoSpin();
            yield return new WaitForSeconds(1);
            yield break;
        }
        if (uiManager) uiManager.StartSunAnim();
        if (audioController) audioController.PlayWLAudio("spin");
        CheckSpinAudio = true;
        IsSpinning = true;
        ToggleButtonGrp(false);
        for (int i = 0; i < numberOfSlots; i++)
        {
            InitializeTweening(Slot_Transform[i]);
            yield return new WaitForSeconds(0.1f);
        }

        if (!IsFreeSpin)
        {
            BalanceDeduction();
        }
        SocketManager.AccumulateResult(BetCounter);
        yield return new WaitUntil(() => SocketManager.isResultdone);

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
        if (uiManager) uiManager.StopSunAnim();

        CheckPayoutLineBackend(SocketManager.resultData.linesToEmit, SocketManager.resultData.FinalsymbolsToEmit, SocketManager.resultData.jackpot);
        KillAllTweens();


        CheckPopups = true;

        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("f2");

        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("f2");

        currentBalance = SocketManager.playerdata.Balance;

        currentBet = SocketManager.initialData.Bets[BetCounter];

        yield return new WaitForSeconds(0.5f);

        if (SocketManager.resultData.jackpot > 0)
        {
            uiManager.PopulateWin(4, SocketManager.resultData.jackpot);
            yield return new WaitUntil(() => !CheckPopups);
            CheckPopups = true;
        }

        if (SocketManager.resultData.isBonus)
        {
            CheckBonusGame();
        }
        else
        {
            CheckWinPopups();
        }

        yield return new WaitUntil(() => !CheckPopups);
        if (!IsAutoSpin)
        {
            ActivateGamble();
            ToggleButtonGrp(true);
            if (AutoSpinNum <= 0)
            {
                AutoStartMinus_Button.interactable = false;
                AutoSpin_Button.interactable = false;
            }
            IsSpinning = false;
        }
        else
        {
            ActivateGamble();
            yield return new WaitForSeconds(2f);
            IsSpinning = false;
        }
    }

    internal void CheckWinPopups()
    {
        if (SocketManager.resultData.WinAmout >= currentTotalBet * 10 && SocketManager.resultData.WinAmout < currentTotalBet * 15)
        {
            uiManager.PopulateWin(1, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 15 && SocketManager.resultData.WinAmout < currentTotalBet * 20)
        {
            uiManager.PopulateWin(2, SocketManager.resultData.WinAmout);
        }
        else if (SocketManager.resultData.WinAmout >= currentTotalBet * 20)
        {
            uiManager.PopulateWin(3, SocketManager.resultData.WinAmout);
        }
        else
        {
            CheckPopups = false;
        }
    }

    private void ActivateGamble()
    {
        if (SocketManager.playerdata.currentWining > 0 && SocketManager.playerdata.currentWining <= SocketManager.GambleLimit)
        {
            gambleController.GambleTweeningAnim(true);
            gambleController.toggleDoubleButton(true);
        }
    }

    internal void DeactivateGamble()
    {
        StopAutoSpin();
    }


    private void BalanceDeduction()
    {
        double bet = 0;
        double balance = 0;
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
        double initAmount = balance;

        balance = balance - bet;

        DOTween.To(() => initAmount, (val) => initAmount = val, balance, 0.8f).OnUpdate(() =>
        {
            if (Balance_text) Balance_text.text = initAmount.ToString("f2");
        });
    }

    private void WinningsAnim(bool IsStart)
    {
        if (IsStart)
        {
            WinTween = TotalWin_text.gameObject.GetComponent<RectTransform>().DOScale(new Vector2(1.5f, 1.5f), 1f).SetLoops(-1, LoopType.Yoyo).SetDelay(0);
        }
        else
        {
            WinTween.Kill();
            TotalWin_text.gameObject.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    internal void CallCloseSocket()
    {
        SocketManager.CloseSocket();
    }

    internal void CheckBonusGame()
    {
        if (SocketManager.resultData.isBonus)
        {
            _bonusManager.startGame(SocketManager.resultData.BonusResult, currentBet);
        }
        else
        {
            CheckPopups = false;
        }
    }

    void ToggleButtonGrp(bool toggle)
    {
        if (AutoStartMinus_Button) AutoStartMinus_Button.interactable = toggle;
        if (AutoStartPlus_Button) AutoStartPlus_Button.interactable = toggle;
        if (SlotStart_Button) SlotStart_Button.interactable = toggle;
        if (MaxBet_Button) MaxBet_Button.interactable = toggle;
        if (AutoSpin_Button) AutoSpin_Button.interactable = toggle;
        if (BetOne_button) BetOne_button.interactable = toggle;
        if (TBPlus_Button) TBPlus_Button.interactable = toggle;
        if (TBMinus_Button) TBMinus_Button.interactable = toggle;
    }

    internal void updateBalance()
    {
        if (Balance_text) Balance_text.text = SocketManager.playerdata.Balance.ToString("f2");
        if (TotalWin_text) TotalWin_text.text = SocketManager.playerdata.currentWining.ToString("f2");
    }

    //start the icons animation
    private void StartGameAnimation(GameObject animObjects, BoxScripting boxscript)
    {
        ImageAnimation temp = animObjects.GetComponent<ImageAnimation>();
        temp.StartAnimation();
        TempList.Add(temp);
        boxscript.isAnim = true;
    }

    //stop the icons animation
    private void StopGameAnimation()
    {
        if (BoxAnimRoutine != null)
        {
            StopCoroutine(BoxAnimRoutine);
            BoxAnimRoutine = null;
        }
        for (int i = 0; i < TempBoxScripts.Count; i++)
        {
            foreach (BoxScripting b in TempBoxScripts[i].boxScripts)
            {
                b.isAnim = false;
                b.ResetBG();
            }
        }
        for (int i = 0; i < TempList.Count; i++)
        {
            TempList[i].StopAnimation();
        }
        TempList.Clear();
        TempList.TrimExcess();
    }

    private void CheckPayoutLineBackend(List<int> LineId, List<string> points_AnimString, double jackpot = 0)
    {
        List<int> points_anim = null;
        if (LineId.Count > 0 || points_AnimString.Count > 0)
        {
            if (audioController) audioController.PlayWLAudio("win");

            if (jackpot > 0)
            {
                for (int i = 0; i < Tempimages.Count; i++)
                {
                    for (int k = 0; k < Tempimages[i].slotImages.Count; k++)
                    {
                        StartGameAnimation(Tempimages[i].slotImages[k].gameObject, TempBoxScripts[i].boxScripts[k]);
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
                            StartGameAnimation(Tempimages[(points_anim[k] / 10) % 10].slotImages[points_anim[k] % 10].gameObject, TempBoxScripts[(points_anim[k] / 10) % 10].boxScripts[points_anim[k] % 10]);
                        }
                        else
                        {
                            StartGameAnimation(Tempimages[0].slotImages[points_anim[k]].gameObject, TempBoxScripts[0].boxScripts[points_anim[k]]);
                        }
                    }
                }
            }
            WinningsAnim(true);
        }
        else
        {
            if (audioController) audioController.StopWLAaudio();
        }

        if (LineId.Count > 0)
        {
            BoxAnimRoutine = StartCoroutine(BoxRoutine(LineId));
        }

        CheckSpinAudio = false;
    }

    private IEnumerator BoxRoutine(List<int> LineIDs)
    {
        while(true)
        {
            for (int i = 0; i < LineIDs.Count; i++)
            {
                PayCalculator.GeneratePayoutLinesBackend(LineIDs[i]);
                PayCalculator.DontDestroyLines.Add(LineIDs[i]);
                for (int s = 0; s < 5; s++)
                {
                    if (TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isAnim)
                    {
                        TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].SetBG(Box_Sprites[LineIDs[i]]);
                    }
                }
                if (LineIDs.Count < 2)
                {
                    yield break;
                }
                yield return new WaitForSeconds(2f);
                for (int s = 0; s < 5; s++)
                {
                    if (TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].isAnim)
                    {
                        TempBoxScripts[s].boxScripts[SocketManager.LineData[LineIDs[i]][s]].ResetBG();
                    }
                }
                PayCalculator.DontDestroyLines.Clear();
                PayCalculator.DontDestroyLines.TrimExcess();
                PayCalculator.ResetStaticLine();
            }
            for (int i = 0; i < LineIDs.Count; i++)
            {
                PayCalculator.GeneratePayoutLinesBackend(LineIDs[i]);
                PayCalculator.DontDestroyLines.Add(LineIDs[i]);
            }
            yield return new WaitForSeconds(2f);
            PayCalculator.DontDestroyLines.Clear();
            PayCalculator.DontDestroyLines.TrimExcess();
            PayCalculator.ResetStaticLine();
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

[Serializable]
public class BoxScript
{
    public List<BoxScripting> boxScripts = new List<BoxScripting>(10);
}

