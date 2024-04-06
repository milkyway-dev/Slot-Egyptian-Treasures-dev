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
    [SerializeField]
    private Button AutoSpin_Button;
    [SerializeField]
    private Button MaxBet_Button;
    private int LineCounter = 0;


    int tweenHeight = 0;

    [SerializeField]
    private GameObject Image_Prefab;

    [SerializeField]
    private PayoutCalculation PayCalculator;

    private Tweener tweener1;
    private Tweener tweener2;
    private Tweener tweener3;
    private Tweener tweener4;
    private Tweener tweener5;

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
    private Sprite[] Box_Sprites;


    //Coroutine AutoSpinRoutine = null;

    private void Start()
    {

        if (SlotStart_Button) SlotStart_Button.onClick.RemoveAllListeners();
        if (SlotStart_Button) SlotStart_Button.onClick.AddListener(StartSlots);

        if (MaxBet_Button) MaxBet_Button.onClick.RemoveAllListeners();
        if (MaxBet_Button) MaxBet_Button.onClick.AddListener(MaxBet);

        if (Lines_Button) Lines_Button.onClick.RemoveAllListeners();
        if (Lines_Button) Lines_Button.onClick.AddListener(ToggleLine);

        if (Lines_text) Lines_text.text = Lines_num[0].ToString();
        LineCounter = 0;

        //if (AutoSpin_Button) AutoSpin_Button.onClick.RemoveAllListeners();
        //if (AutoSpin_Button) AutoSpin_Button.onClick.AddListener(AutoSpin);
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
        PayCalculator.GeneratePayoutLinesBackend(LineID - 1, true);
    }

    bool IsAutoSpin = false;

    bool SlotRunning = false;

    private void MaxBet()
    {
        if (TotalBet_text) TotalBet_text.text = "99999";
    }

    private void ToggleLine()
    {
        LineCounter++;
        if(LineCounter == Lines_num.Length)
        {
            LineCounter = 0;
        }
        if (Lines_text) Lines_text.text = Lines_num[LineCounter].ToString();
        PayCalculator.ResetLines();
        for (int i = 0; i < StaticLine_Objects.Count; i++)
        {
            if (i < Lines_num[LineCounter])
            {
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<Button>().interactable = true;
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<ManageLineButtons>().isEnabled = true;
                GenerateSampleLines(StaticLine_Texts[i]);
            }
            else
            {
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<Button>().interactable = false;
                if (StaticLine_Objects[i]) StaticLine_Objects[i].GetComponent<ManageLineButtons>().isEnabled = false;
            }
        }
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
    }

    private void PopulateAnimationSprites(ImageAnimation animScript, int val)
    {
        switch(val)
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

    private void StartSlots()
    {
        if (UppeLogo_Anim) UppeLogo_Anim.StartAnimation();
        if (UppeLogo2_Anim) UppeLogo2_Anim.StartAnimation();
        if (SlotAnimRoutine != null)
        {
            StopCoroutine(SlotAnimRoutine);
            SlotAnimRoutine = null;
        }
        AnimStoppedProcess();
        if (SlotStart_Button) SlotStart_Button.interactable = false;
        if (TempList.Count > 0) 
        {
            StopGameAnimation();
        }
        for (int i = 0; i < Tempimages.Count; i++)
        {
            Tempimages[i].slotImages.Clear();
            Tempimages[i].slotImages.TrimExcess();
        }
        PayCalculator.ResetLines();
        StartCoroutine(TweenRoutine());
    }

    [SerializeField]
    private List<int> TempLineIds;
    [SerializeField]
    private List<string> x_animationString;
    [SerializeField]
    private List<string> y_animationString;

    private IEnumerator TweenRoutine()
    {
        if (numberOfSlots >= 1)
        {
            InitializeTweening1(Slot_Transform[0]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 2)
        {
            InitializeTweening2(Slot_Transform[1]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 3)
        {
            InitializeTweening3(Slot_Transform[2]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 4)
        {
            InitializeTweening4(Slot_Transform[3]);
        }
        yield return new WaitForSeconds(0.1f);

        if (numberOfSlots >= 5)
        {
            InitializeTweening5(Slot_Transform[4]);
        }
        SocketManager.AccumulateResult();
        yield return new WaitForSeconds(0.5f);
        List<int> resultnum = SocketManager.tempresult.StopList?.Split(',')?.Select(Int32.Parse)?.ToList();
        if (numberOfSlots >= 1)
        {
            yield return StopTweening1(resultnum[0]+3, Slot_Transform[0]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 2)
        {
            yield return StopTweening2(resultnum[1]+3, Slot_Transform[1]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 3)
        {
            yield return StopTweening3(resultnum[2]+3, Slot_Transform[2]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 4)
        {
            yield return StopTweening4(resultnum[3]+3, Slot_Transform[3]);
        }
        yield return new WaitForSeconds(0.5f);
        if (numberOfSlots >= 5)
        {
            yield return StopTweening5(resultnum[4]+3, Slot_Transform[4]);
        }
        yield return new WaitForSeconds(0.3f);
        GenerateMatrix(SocketManager.tempresult.StopList);
        CheckPayoutLineBackend(SocketManager.tempresult.resultLine, SocketManager.tempresult.x_animResult, SocketManager.tempresult.y_animResult);
        KillAllTweens();
        if (SlotStart_Button) SlotStart_Button.interactable = true;
        if (UppeLogo_Anim) UppeLogo_Anim.StopAnimation();
        if (UppeLogo2_Anim) UppeLogo2_Anim.StopAnimation();
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
                PayCalculator.GeneratePayoutLinesBackend(TempLineIds[i] - 1, true);
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

    private Coroutine SlotAnimRoutine = null;

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

    private void StartGameAnimation(GameObject animObjects, int  LineID) 
    {
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

    private void CheckPayoutLineBackend(List<int> LineId, List<string> x_AnimString, List<string> y_AnimString)
    {
        TempLineIds = LineId;
        List<int> x_anim = null;
        List<int> y_anim = null;

        for (int i = 0; i < x_AnimString.Count; i++)
        {
            x_anim = x_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
            y_anim = y_AnimString[i]?.Split(',')?.Select(Int32.Parse)?.ToList();

            for (int k = 0; k < x_anim.Count; k++)
            {
                StartGameAnimation(Tempimages[x_anim[k]].slotImages[y_anim[k]].gameObject, LineId[i]);
            }
        }
        PayCalculator.ResetStaticLine();
        DisableLineButtons();
        if(SlotAnimRoutine != null)
        {
            StopCoroutine(SlotAnimRoutine);
            SlotAnimRoutine = null;
        }
        SlotAnimRoutine = StartCoroutine(slotLineAnim(x_AnimString, y_AnimString));
    }

    private void GenerateMatrix(string stopList)
    {
        List<int> numbers = stopList?.Split(',')?.Select(Int32.Parse)?.ToList();

        for (int i = 0; i < numbers.Count; i++)
        {
            for (int s = 0; s < verticalVisibility; s++)
            {
                Tempimages[i].slotImages.Add(images[i].slotImages[(images[i].slotImages.Count - (numbers[i] + 3)) + s]);
            }
        }
    }

    #region TweeningCode
    private void InitializeTweening1(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener1 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener1.Play();
    }
    private void InitializeTweening2(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener2 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener2.Play();
    }
    private void InitializeTweening3(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener3 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener3.Play();
    }
    private void InitializeTweening4(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener4 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener4.Play();
    }
    private void InitializeTweening5(Transform slotTransform)
    {
        slotTransform.localPosition = new Vector2(slotTransform.localPosition.x, 0);
        tweener5 = slotTransform.DOLocalMoveY(-tweenHeight, 0.2f).SetLoops(-1, LoopType.Restart).SetDelay(0);
        tweener5.Play();
    }

    private IEnumerator StopTweening1(int reqpos, Transform slotTransform)
    {
        tweener1.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener1 = slotTransform.DOLocalMoveY(-tweenpos , 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener1 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening2(int reqpos, Transform slotTransform)
    {
        tweener2.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener2 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener2 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening3(int reqpos, Transform slotTransform)
    {
        tweener3.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener3 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener3 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening4(int reqpos, Transform slotTransform)
    {
        tweener4.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener4 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener4 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }
    private IEnumerator StopTweening5(int reqpos, Transform slotTransform)
    {
        tweener5.Pause();
        int tweenpos = (reqpos * IconSizeFactor) - IconSizeFactor;
        tweener5 = slotTransform.DOLocalMoveY(-tweenpos, 0.2f);
        yield return new WaitForSeconds(0.2f);
        tweener5 = slotTransform.DOLocalMoveY(-tweenpos + 100, 0.2f);
    }

    private void KillAllTweens()
    {
        tweener1.Kill();
        tweener2.Kill();
        tweener3.Kill();
        tweener4.Kill();
        tweener5.Kill();
    }
    #endregion

}

[Serializable]
public class SlotImage
{
    public List<Image> slotImages = new List<Image>(10);
}

