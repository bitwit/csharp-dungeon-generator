using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateDungeon : MonoBehaviour {

	struct Constants {
        public static Size DungeonSize = new Size(width: 48, height: 48);
    }


	// Use this for initialization
	void Start () {
		UnitySystemConsoleRedirector.Redirect();

		var generator = new DungeonGenerator<DungeonRoom, DungeonHallway>();
        generator.dungeonSize = Constants.DungeonSize;
        generator.creationBounds = Constants.DungeonSize;
        generator.minimumRoomWidth = 5;
        generator.minimumRoomHeight = 5;
        generator.maximumRoomWidth = 10;
        generator.maximumRoomHeight = 10;
        generator.initialRoomCreationCount = 30;
        generator.runCompleteGeneration();

		Debug.Log("Dungeon Generator Complete");
		// Debug.Log(generator.to2DGrid());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
