using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

public class ManageLineButtons : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerUpHandler,IPointerDownHandler
{


	[SerializeField]
	private TMP_Text num_text;
	[SerializeField]
	internal bool isEnabled = false;
	[SerializeField] private int num;
	[SerializeField] private PayoutCalculation payoutManager;
	[SerializeField] private Button btn;


	private void Start()
	{
		btn = this.GetComponent<Button>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
<<<<<<< HEAD
		//if (Application.platform == RuntimePlatform.WebGLPlayer && !Application.isMobilePlatform)
		//{
		if (isEnabled)
		{
			if (_ConnectedLine) _ConnectedLine.SetActive(true);
		}
        //}
    }
=======

			if (num < payoutManager.LineList[payoutManager.currentLineIndex] + 1)
			{
				isEnabled = true;
				btn.interactable = true;
			}
			else
			{
				isEnabled = false;
				btn.interactable = false;
			}


        //Debug.Log("run on pointer enter");

			if (isEnabled)
				payoutManager.GeneratePayoutLinesBackend(num);


		//slotManager.GenerateStaticLine(num);
	}
>>>>>>> feature/BackendIntegration
	public void OnPointerExit(PointerEventData eventData)
	{
        try
        {
			if (isEnabled)
				payoutManager.ResetStaticLine();
		}
        catch (System.Exception ex)
        {
			Debug.Log(ex);
        }

		
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform)
		{
			this.gameObject.GetComponent<Button>().Select();
			Debug.Log("run on pointer down");
			payoutManager.GeneratePayoutLinesBackend(num);
			//slotManager.GenerateStaticLine(num);
		}
	}
	public void OnPointerUp(PointerEventData eventData)
	{
		if (Application.platform == RuntimePlatform.WebGLPlayer && Application.isMobilePlatform)
		{
			Debug.Log("run on pointer up");
			payoutManager.ResetStaticLine();
			//slotManager.DestroyStaticLine();
			DOVirtual.DelayedCall(0.1f, () =>
			{
				this.gameObject.GetComponent<Button>().spriteState = default;
				EventSystem.current.SetSelectedGameObject(null);
			});
		}
	}
}
