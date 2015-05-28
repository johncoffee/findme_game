using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sounds : MonoBehaviour, ITimeupEvent {

	public AudioSource timeoutBuzz = null;
	public List<AudioSource> ambientSounds = new List<AudioSource>();



	
	public List<AudioSource> blueSounds = new List<AudioSource>();
	public List<AudioSource> redSounds = new List<AudioSource>();

	public AudioSource enterRoom = null;
	public AudioSource homebase = null;
	public AudioSource foundPiece = null;
	public AudioSource foundEnemy = null;

	public NetConnector netConnector;

	public void Timeup ()
	{
		if (timeoutBuzz != null)
		timeoutBuzz.Play();
		StopAmbient ();
	}


	public void Found ()
	{
		StopAmbient ();
		foundPiece.Play ();
	}


	public void CheckedIn (int roomId)
	{
		enterRoom.Play ();
		PlayAmbient (roomId);
	}

	public void PlayAmbient(int roomID) {
		ambientSounds [(roomID - 1) % ambientSounds.Count].Play ();
		if (netConnector.PlayerColor == NetConnector.PlayerColors.Blue) {
			blueSounds [(roomID - 1) % blueSounds.Count].Play ();
		} else {
			redSounds [(roomID - 1) % redSounds.Count].Play ();
		}
	}

	public void StopAmbient() {
		foreach (AudioSource source in ambientSounds) {
			source.Stop();
		}
		foreach (AudioSource source in redSounds) {
			source.Stop();
		}
		foreach (AudioSource source in blueSounds) {
			source.Stop();
		}
	}
	public void UnlockEvent ()
	{
		homebase.Play ();
		timeoutBuzz.Stop();
	}
	public void FoundEnemyPiece ()
	{
		foundEnemy.Play ();
	}

}


