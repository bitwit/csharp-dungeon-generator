using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(DungeonCreator))]
public class DungeonInspector : Editor {
	
	public override void OnInspectorGUI() {
		//base.OnInspectorGUI();
		DrawDefaultInspector();
		
		if(GUILayout.Button("Regenerate")) {
			DungeonCreator dungeonCreator = (DungeonCreator)target;
			dungeonCreator.CreateNewDungeon();
		}
	}
}
