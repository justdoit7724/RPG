

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraShakeSimpleScript : MonoBehaviour {

	private bool isRunning = false;
	private Animation anim;
    private Coroutine corValue=null;

    private float amount = 0;
    private float duration = 0;

	void Start () {
		anim = GetComponent<Animation> ();
	}

	public void ShakeCamera() {	
		if (anim != null)
			anim.Play (anim.clip.name);
		else
			ShakeCaller (0.25f, 0.1f);
	}

	//other shake option
	public void ShakeCaller (float amount, float duration){

        this.amount = amount;
        this.duration = duration;

        if(!isRunning)
		    StartCoroutine (Shake());
	}

	IEnumerator Shake (){
		isRunning = true;

		int counter = 0;

		while (duration > 0.01f) {
			counter++;

			var x = Random.Range (-1f, 1f) * (amount/counter);
			var y = Random.Range (-1f, 1f) * (amount/counter);

			transform.position = Vector3.Lerp (transform.position, transform.position+new Vector3 (x, y, 0), 0.5f);

			duration -= 0.02f;
			
			yield return new WaitForSeconds (0.02f);
		}

		isRunning = false;
	}
}
