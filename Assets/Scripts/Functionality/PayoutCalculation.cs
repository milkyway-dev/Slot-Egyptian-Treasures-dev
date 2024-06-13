using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;
using TMPro;

public class PayoutCalculation : MonoBehaviour
{
    [SerializeField]
    private int x_Distance;
    [SerializeField]
    private int y_Distance;

    [SerializeField]
    private Transform LineContainer;
    [SerializeField]
    private GameObject[] Lines_Object;

    [SerializeField] private TMP_Text line_text;
    internal int LineCount;

    internal int currrentLineIndex;

    [SerializeField] internal List<int> DontDestroy = new List<int>();
    [SerializeField] internal List<int> LineList;

    [SerializeField] private Button[] buttons;
    //[SerializeField] private Button[] right_buttons;
    [SerializeField] internal List<int> DontDestroyLines = new List<int>();

    GameObject TempObj = null;
    [SerializeField]internal int currentLineIndex;

    internal int CurrentLines;
    internal int LineIndex;


    private void Awake()
    {
        LineCount = Lines_Object.Length;
    }
    private void Start()
    {
        CurrentLines = LineList[LineList.Count - 1];
        LineIndex = LineList.Count - 1;

        currentLineIndex = LineList.Count - 1;
        //inActiveLineButtons[LineIndex].SetActive(false);
        //activeLineButtons[LineIndex].SetActive(true);
    }
    //internal void GeneratePayoutLinesBackend(int lineIndex = -1, int linecounter = 0, bool isStatic = false)
    //{
    //    ResetLines();
    //    if (lineIndex >= 0)
    //    {
    //        if (Lines_Object[lineIndex]) Lines_Object[lineIndex].SetActive(true);
    //        return;
    //    }

    //    if (isStatic)
    //    {
    //        TempObj = Lines_Object[lineIndex];
    //    }
    //    for (int i = 0; i < linecounter; i++)
    //    {
    //        Lines_Object[i].SetActive(true);
    //    }
    //}

    internal void GeneratePayoutLinesBackend(int index = -1, bool DestroyFirst = true)
    {

        if (DestroyFirst)
            ResetStaticLine();

        if (index >= 0)
        {
            Lines_Object[index].SetActive(true);
            return;
        }
        DontDestroyLines.Clear();
        for (int i = 0; i < LineList[currentLineIndex]; i++)
        {
            Lines_Object[i].SetActive(true);


        }


    }

    internal void ToggleLine()
    {
        print("line current index " + currentLineIndex);
        currentLineIndex++;
        if (currentLineIndex == LineList.Count)
        {
            currentLineIndex = 0;
        }
        line_text.text = LineList[currentLineIndex].ToString();
        DontDestroyLines.Clear();
        ResetStaticLine();
        GeneratePayoutLinesBackend(-1);
        SetButtonActive(LineList[currentLineIndex]);

    }

    internal void ResetStaticLine()
    {
        for (int i = 0; i < Lines_Object.Length; i++)
        {
            if (DontDestroyLines.Contains(i))
                continue;
            else
                Lines_Object[i].SetActive(false);
        }
    }

    internal void SetButtonActive(int LineCounter)
    {
       

        for (int i = 0; i < LineCounter; i++)
        {
            Lines_Object[i].SetActive(true);

            buttons[i].interactable = true;
        }


        for (int j = LineCounter; j < buttons.Length; j++)
        {
            buttons[j].interactable = false;
        }
    }

    internal void ResetLines()
    {
        for (int i = 0; i < Lines_Object.Length; i++)
        {
            if (DontDestroy.IndexOf(i) >= 0)
                continue;
            else
                Lines_Object[i].SetActive(false);
        }
    }
}
