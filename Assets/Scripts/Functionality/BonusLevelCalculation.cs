using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using System.Linq;

public class BonusLevelCalculation : MonoBehaviour
{
    List<List<int>> Platform_List = new List<List<int>>()
    {
        new List<int>(), 
        new List<int>(), 
        new List<int>()  
    };
    [SerializeField]
    private GameObject[] Prize_Columns;
    [SerializeField]
    private TMP_Text[] Prize_Texts;
    [SerializeField]
    private Button[] Prize_Buttons;
    [SerializeField]
    private GameObject[] LeftArr_Objects;
    [SerializeField]
    private GameObject[] RightArr_Objects;
    [SerializeField]
    private GameObject SunLogoNormal;
    [SerializeField]
    private GameObject SunLogoFadeIn;
    [SerializeField]
    private GameObject SunLogoFadeOut;
    [SerializeField]
    private Sprite[] Initial_Sprites;
    [SerializeField]
    private Sprite GameOverSPrite;
    [SerializeField]
    private TMP_Text totalWinText;
    [SerializeField]
    private TMP_Text multiplierText;
    [SerializeField]
    private GameObject bonusGame;
    [SerializeField]
    private SlotBehaviour slotBehaviour;
    [SerializeField]
    private GameObject Raycast_Object;
    private int arrowNum = 2;
    private int boxesOpened = 0;
    private float multiplier = 0;
    private double currentbet;

    Coroutine SunRoutine = null;

    [SerializeField] private List<int> result_num;
    internal bool gameOn;

    private void Start()
    {
        for (int i = 8; i > 4; i--)
        {
            if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
        }
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(true);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(true);

        if (SunLogoNormal) SunLogoNormal.SetActive(true);
        if (SunLogoFadeOut) SunLogoFadeOut.SetActive(false);
        if (SunLogoFadeIn) SunLogoFadeIn.SetActive(false);
    }

    internal void BonusinitialSetup(List<string> bonusList)
    {
        for (int i = 0; i < bonusList.Count; i++)
        {
            Platform_List[i] = bonusList[i]?.Split(',')?.Select(Int32.Parse)?.ToList();
        }
    }

    private IEnumerator SunLogoRoutine()
    {
        bool isSun = true;
        while (true)
        {
            isSun = !isSun;
            if (isSun)
            {
                if (SunLogoNormal) SunLogoNormal.SetActive(false);
                if (SunLogoFadeOut) SunLogoFadeOut.SetActive(true);
                if (multiplierText) multiplierText.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                if (SunLogoFadeOut) SunLogoFadeOut.SetActive(false);
            }
            else
            {
                if (SunLogoFadeIn) SunLogoFadeIn.SetActive(true);
                yield return new WaitForSeconds(0.3f);
                if (multiplierText) multiplierText.gameObject.SetActive(true);
                if (SunLogoFadeIn) SunLogoFadeIn.SetActive(false);
                if (SunLogoNormal) SunLogoNormal.SetActive(true);
            }
            yield return new WaitForSeconds(3);
        }
    }

    internal void startGame(List<int> result, double currenBet)
    {
        if (SunRoutine != null)
        {
            StopCoroutine(SunRoutine);
            SunRoutine = null;
        }
        SunRoutine = StartCoroutine(SunLogoRoutine());
        result_num.Clear();
        result_num.TrimExcess();
        currentbet = currenBet;
        result_num = result;

        gameOn = true;
        bonusGame.SetActive(true);
        for (int i = 8; i > 4; i--)
        {
            if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
        }
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(true);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(true);

        if (SunLogoNormal) SunLogoNormal.SetActive(true);
        if (SunLogoFadeOut) SunLogoFadeOut.SetActive(false);
        if (SunLogoFadeIn) SunLogoFadeIn.SetActive(false);


    }

    public void CheckBox(int num)
    {
        if (!gameOn)
            return;

        if (Raycast_Object) Raycast_Object.SetActive(true);
        if (Prize_Columns[num]) Prize_Columns[num].GetComponent<ImageAnimation>().StartAnimation();
        if (Prize_Buttons[num]) Prize_Buttons[num].interactable = false;
        DOVirtual.DelayedCall(0.5f, () =>
        {
            int myResult = Platform_List[boxesOpened][result_num[boxesOpened]];
            if (myResult != 0)
            {
                if (Prize_Columns[num]) Prize_Columns[num].SetActive(false);
                Prize_Texts[num].text = myResult.ToString();
                Prize_Texts[num].gameObject.SetActive(true);
                totalWinText.text = (int.Parse(totalWinText.text) + (currentbet * myResult)).ToString();
                if (currentbet != 0)
                {
                    multiplier += myResult;
                    multiplierText.text = "X" + multiplier + "\n" + "MULTIPLIED";
                }
            }
            else
            {
                Prize_Columns[num].SetActive(true);
                Prize_Columns[num].GetComponent<Image>().sprite = GameOverSPrite;
                Prize_Texts[num].text = "Game Over";
            }
            boxesOpened++;

            if (myResult == 0 || boxesOpened >= 3) 
            {
                gameOn = false;
                foreach (var item in Prize_Buttons)
                {
                    item.interactable = false;
                }
                Invoke("GameOver", 1.75f);
            }
            else
            {
                if (boxesOpened == 1)
                {
                    for (int i = 8; i > 4; i--)
                    {
                        if (Prize_Buttons[i]) Prize_Buttons[i].interactable = false;
                    }
                    NextLineUp(1);
                }
                else if (boxesOpened == 2)
                {
                    for (int i = 4; i > 1; i--)
                    {
                        if (Prize_Buttons[i]) Prize_Buttons[i].interactable = false;
                    }
                    NextLineUp(0);
                }
            }
            if (Raycast_Object) Raycast_Object.SetActive(false);
        });
    }

    private void NextLineUp(int line)
    {
        if (line == 1)
        {
            for (int i = 4; i > 1; i--)
            {
                if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
            }
        }
        else
        {
            for (int i = 1; i > -1; i--)
            {
                if (Prize_Buttons[i]) Prize_Buttons[i].interactable = true;
            }
        }
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(false);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(false);
        arrowNum--;
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(true);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(true);
    }

    void GameOver()
    {
        totalWinText.text = "0";
        arrowNum = 2;
        multiplierText.text = "X 0"+ "\n" + "MULTIPLIED";
        multiplier = 0;
        for (int i = 0; i < Prize_Columns.Length; i++)
        {
            Prize_Columns[i].SetActive(true);
            ImageAnimation temp = Prize_Columns[i].GetComponent<ImageAnimation>();
            temp.StopAnimation();
            temp.rendererDelegate.sprite = Initial_Sprites[i];
            Prize_Texts[i].gameObject.SetActive(false);
            Prize_Buttons[i].interactable = false;
            if (i > 4)
            {
                Prize_Buttons[i].interactable = true;
            }
        }

        for (int i = 0; i < LeftArr_Objects.Length; i++)
        {
            LeftArr_Objects[i].SetActive(false);
            RightArr_Objects[i].SetActive(false);
        }
        if (LeftArr_Objects[arrowNum]) LeftArr_Objects[arrowNum].SetActive(true);
        if (RightArr_Objects[arrowNum]) RightArr_Objects[arrowNum].SetActive(true);
        boxesOpened = 0;
        bonusGame.SetActive(false);
        if (SunRoutine != null)
        {
            StopCoroutine(SunRoutine);
            SunRoutine = null;
        }
        slotBehaviour.CheckPopups = false;
    }
}
