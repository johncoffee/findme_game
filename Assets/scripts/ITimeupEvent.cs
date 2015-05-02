using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public interface ITimeupEvent : IEventSystemHandler {

	void Timeup();
	void Found();
	void CheckedIn(int roomId);
	void UnlockEvent();
	void FoundEnemyPiece();

}
