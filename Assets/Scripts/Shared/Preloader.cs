using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Preloader : MonoBehaviour
{

    public UnityEngine.UI.Slider ProgressSlider;

	IEnumerator Start ()
	{
	    var animator = GetComponent<Animator>();
	    animator.Play("PreloaderFadeIn");

	    yield return new WaitForSeconds(0.5f);

	    var loadOp = Application.LoadLevelAsync(1);

	    while (!loadOp.isDone)
	    {
	        ProgressSlider.value = loadOp.progress;
	        yield return null;
	    }
	}
}
