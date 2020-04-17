using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonDeselector : MonoBehaviour, IPointerEnterHandler {

	public void OnPointerEnter(PointerEventData pEvent)
	{
		EventSystem.current.SetSelectedGameObject (gameObject, pEvent);
	}

}
