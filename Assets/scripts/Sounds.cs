using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sounds : MonoBehaviour, ITimeupEvent {

	public AudioSource timeoutBuzz = null;
	public List<AudioSource> ambientSounds = new List<AudioSource>();

	public void Timeup ()
	{
		if (timeoutBuzz != null)
		timeoutBuzz.Play();
		StopAmbient ();
	}
	public void Found ()
	{
		StopAmbient ();
	}


	public void CheckedIn (int roomId)
	{
		PlayAmbient (roomId);
	}

	public void PlayAmbient(int roomID) {
		ambientSounds [roomID - 1].Play ();
	}

	public void StopAmbient() {
		foreach (AudioSource source in ambientSounds) {
			source.Stop();
		}
	}

}


