using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class DungeonCreator : MonoBehaviour {

	struct Constants {
		public static Size DungeonSize = new Size (width: 32, height: 32);
	}

	public GameObject dungeonContainerObject;
	public GameObject roomObject;
	public GameObject hallwayObject;

	void Start () {
		CreateNewDungeon ();
	}

	public void CreateNewDungeon () {
		UnitySystemConsoleRedirector.Redirect ();

		var transform = GetComponent<Transform> ();

		//clear out any previous dungeon
		Debug.Log ("Destroying old objects");
		foreach (Transform child in transform) {
			GameObject.DestroyImmediate (child.gameObject);
		}

		var container = GameObject.Instantiate (dungeonContainerObject, transform); 
		var containerTransform = container.GetComponent<Transform>();

		var generator = new DungeonGenerator<DungeonRoom, DungeonHallway> ();
		generator.dungeonSize = Constants.DungeonSize;
		generator.creationBounds = Constants.DungeonSize;
		generator.minimumRoomWidth = 5;
		generator.minimumRoomHeight = 5;
		generator.maximumRoomWidth = 14;
		generator.maximumRoomHeight = 14;
		generator.initialRoomCreationCount = 10;
		generator.runCompleteGeneration ();

		Debug.Log ("Rooms: " + generator.dungeon.rooms.Count + "/Hallways: " + generator.dungeon.hallways.Count);

		var dungeonTransform = GetComponent<Transform> ();

		var roomIndex = 0;
		foreach (var room in generator.dungeon.rooms) {
			roomIndex++;
			var newRoom = GameObject.Instantiate (roomObject, containerTransform);
			newRoom.transform.localPosition = new Vector3 ((float) room.rect.center.x * 10, 0, (float) room.rect.center.y * 10);
			newRoom.transform.localScale = new Vector3 ((float) room.rect.size.width, 1, (float) room.rect.size.height);
			newRoom.GetComponentInChildren<TextMesh> ().text = "R " + roomIndex;
			// newRoom.GetComponent<Renderer>().material.color = Color.blue;
		}
		var hallIndex = 0;
		foreach (var hallway in generator.dungeon.hallways) {
			hallIndex++;
			var hallwaySegmentIndex = 0;
			foreach (var rect in hallway.rects) {
				hallwaySegmentIndex++;
				// Debug.Log("hallway" + rect.description);
				var newHallway = GameObject.Instantiate (hallwayObject, containerTransform);
				newHallway.transform.localPosition = new Vector3 ((float) rect.center.x * 10, 0, (float) rect.center.y * 10);
				newHallway.transform.localScale = new Vector3 ((float) rect.size.width, 1, (float) rect.size.height);
				newHallway.GetComponentInChildren<TextMesh> ().text = "H " + hallIndex + "-" + hallwaySegmentIndex;
				// newHallway.GetComponent<Renderer>().material.color = Color.cyan;
			}
		}
	}

}