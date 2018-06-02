using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DungeonGenerator;

[ExecuteInEditMode]
public class DungeonCreator : MonoBehaviour {

	struct Constants {
		public static Size DungeonSize = new Size (width: 64, height: 64);
	}

	public GameObject roomObject;
	public GameObject hallwayObject;
	public Transform playerTransform;

	void Start () {
		CreateNewDungeon ();
	}

	public void CreateNewDungeon () {
		UnitySystemConsoleRedirector.Redirect ();

		var transform = GetComponent<Transform> ();

		//clear out any previous dungeon
		Debug.LogWarning ("Destroying old objects");
		foreach (Transform child in transform) {
			GameObject.DestroyImmediate (child.gameObject);
		}

		var container = new GameObject("Dungeon Container");
		var containerTransform = container.GetComponent<Transform>();
		container.transform.parent = transform;

		var generator = new DungeonGenerator<DungeonRoom, DungeonHallway> ();
		generator.dungeonSize = Constants.DungeonSize;
		generator.creationBounds = Constants.DungeonSize;
		generator.minimumRoomWidth = 5;
		generator.minimumRoomHeight = 5;
		generator.maximumRoomWidth = 14;
		generator.maximumRoomHeight = 14;
		generator.initialRoomCreationCount = 30;
		generator.runCompleteGeneration ();

		Debug.Log ("Rooms: " + generator.dungeon.rooms.Count + "/Hallways: " + generator.dungeon.hallways.Count);

		var dungeonTransform = GetComponent<Transform> ();

		var roomIndex = 0;
		foreach (var room in generator.dungeon.rooms) {
			roomIndex++;
			var newRoom = GameObject.Instantiate (roomObject, containerTransform);
			newRoom.transform.localPosition = new Vector3 ((float) room.rect.center.x, 0, (float) room.rect.center.y);
			newRoom.GetComponent<ProceduralRoom>().floorTransform.localPosition = new Vector3(-room.rect.size.width / 2, 0, -room.rect.size.height / 2);
			newRoom.GetComponentInChildren<TextMesh> ().text = "R " + roomIndex;
			
			var tileFloor = newRoom.GetComponentInChildren<TileMap> ();
			tileFloor.size_x = Mathf.RoundToInt(room.rect.size.width);
			tileFloor.size_z = Mathf.RoundToInt(room.rect.size.height);
			tileFloor.BuildMesh();

			playerTransform.position = newRoom.transform.position;
		}
		var hallIndex = 0;
		foreach (var hallway in generator.dungeon.hallways) {
			hallIndex++;
			var hallwaySegmentIndex = 0;
			foreach (var rect in hallway.rects) {
				hallwaySegmentIndex++;
				var newHallway = GameObject.Instantiate (hallwayObject, containerTransform);
				newHallway.transform.localPosition = new Vector3 ((float) rect.center.x, 0, (float) rect.center.y);
				newHallway.GetComponent<ProceduralRoom>().floorTransform.localPosition = new Vector3(-rect.size.width / 2, 0, -rect.size.height / 2);
				newHallway.GetComponentInChildren<TextMesh> ().text = "H " + hallIndex + "-" + hallwaySegmentIndex;
				
				var tileFloor = newHallway.GetComponentInChildren<TileMap> ();
				tileFloor.size_x = Mathf.RoundToInt(rect.size.width);
				tileFloor.size_z = Mathf.RoundToInt(rect.size.height);
				tileFloor.BuildMesh();
			}
		}
	}

}