using UnityEngine;
using System.Collections;

public class Sounds : MonoBehaviour, ITimeupEvent {

	public AudioSource timeoutBuzz = null;

	public void Timeup ()
	{
		if (timeoutBuzz != null)
		timeoutBuzz.Play();
	}

}


