using UnityEngine;
using System.Collections;

public class CurrentObjectiveText : MonoBehaviour {

	public UnityEngine.UI.Text Text;
	public Animator Animator;
	
	public void Display(string content)
	{
		Text.text = content;
		Animator.SetBool("Display", true);
	}
	
	public void Dismiss()
	{
		Animator.SetBool("Display", false);
	}
}
