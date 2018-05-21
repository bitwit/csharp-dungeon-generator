using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VisualDungeonCreationExample : MonoBehaviour {

	public enum GenerationPhase {
		fittingRooms,
		roundedFittingRooms,
		initialGraph,
		minimumGraph,
		hallways,
		grid
	}
	struct Constants {
		public static Size DungeonSize = new Size (width: 64, height: 64);
	}

	public GameObject boundaryObject;
	public GameObject roomObject;
	public GameObject hallwayObject;
	public GameObject pixelObject;
	DungeonGenerator<DungeonRoom, DungeonHallway> dungeonGenerator = new DungeonGenerator<DungeonRoom, DungeonHallway> ();
	GenerationPhase phase = GenerationPhase.fittingRooms;
	float accumulatedDelta = 0f;
	float timeInPhase = 0f;
	float maxTimePerPhase = 1f;
	bool isGenerating = false;
	public Transform dungeonContainer;

	public void Start () {
		UnitySystemConsoleRedirector.Redirect ();
		isGenerating = true;
		dungeonGenerator = new DungeonGenerator<DungeonRoom, DungeonHallway> ();
		dungeonGenerator.dungeonSize = Constants.DungeonSize;
		dungeonGenerator.creationBounds = Constants.DungeonSize;
		// dungeonGenerator.minimumRoomWidth = 15;
		// dungeonGenerator.minimumRoomHeight = 15;
		// dungeonGenerator.maximumRoomWidth = 50;
		// dungeonGenerator.maximumRoomHeight = 50;
		// dungeonGenerator.hallwayWidth = 15;
		// dungeonGenerator.initialRoomCreationCount = 30;
		dungeonGenerator.generateRooms ();
		phase = GenerationPhase.fittingRooms;
	}

	public void Update () {

		if (false == isGenerating) {
			return;
		}

		accumulatedDelta += Time.deltaTime;

		if (accumulatedDelta < 0.05) {
			return;
		}

		timeInPhase += accumulatedDelta;
		accumulatedDelta = 0;

		foreach (Transform child in dungeonContainer) {
			GameObject.Destroy (child.gameObject);
		}

		var boundaryRect = new Rect (origin: new Point (x : 0.0, y : 0.0), size : dungeonGenerator.dungeonSize);
		var newRoom = GameObject.Instantiate (boundaryObject, dungeonContainer);
		var rectTransform = newRoom.GetComponent<RectTransform> ();
		rectTransform.sizeDelta = new Vector2 ((float) boundaryRect.size.width, (float) boundaryRect.size.height);
		rectTransform.localPosition = new Vector3 ((float) boundaryRect.center.x, (float) boundaryRect.center.y, 0);

		switch (phase) {

			case GenerationPhase.fittingRooms:
				// Debug.Log ("fitting rooms");
				addRooms ();
				dungeonGenerator.applyFittingStep ();
				if (dungeonGenerator.containsNoIntersectingRooms ()) {
					phase = GenerationPhase.roundedFittingRooms;
					timeInPhase = 0;
				}
				break;
			case GenerationPhase.roundedFittingRooms:
				// Debug.Log ("rounding room positions");
				addRooms ();
				dungeonGenerator.roundRoomPositions ();
				if (dungeonGenerator.containsNoIntersectingRooms ()) {
					phase = GenerationPhase.initialGraph;
					timeInPhase = 0;
				}
				break;
			case GenerationPhase.initialGraph:
				// Debug.Log ("initial graph");
				addRooms ();
				var graph = dungeonGenerator.generateGraph ();
				addCircles ();
				addGraph (graph);
				if (timeInPhase > maxTimePerPhase) {
					phase = GenerationPhase.minimumGraph;
					timeInPhase = 0;
				}
				break;
			case GenerationPhase.minimumGraph:
				// Debug.Log ("minimum spanning tree graph");
				addRooms ();
				dungeonGenerator.generateDungeonGraph ();
				addGraph (dungeonGenerator.dungeon);
				if (timeInPhase > maxTimePerPhase) {
					phase = GenerationPhase.hallways;
					timeInPhase = 0;
					dungeonGenerator.generateHallways ();
				}
				break;
			case GenerationPhase.hallways:
				// Debug.Log ("hallways");
				addRooms ();
				addHallways ();
				if (timeInPhase > 3) {
					phase = GenerationPhase.grid;
					timeInPhase = 0;
				}
				break;
			case GenerationPhase.grid:
				// Debug.Log ("grid");
				add2DGrid ();
				isGenerating = false;
				break;
		}
	}

	void addRooms () {
		foreach (var room in dungeonGenerator.layoutRooms) {
			var newRoom = GameObject.Instantiate (roomObject, dungeonContainer);
			var rectTransform = newRoom.GetComponent<RectTransform> ();
			rectTransform.sizeDelta = new Vector2 ((float) room.rect.size.width, (float) room.rect.size.height);
			rectTransform.localPosition = new Vector3 ((float) room.rect.center.x, (float) room.rect.center.y, 0);
		}
	}

	void addCircles () {
		foreach (var room in dungeonGenerator.layoutRooms) {

			// var containmentCircle = Circle (fittedTo: room.rect);
			// containmentCircle.radius += (dungeonGenerator.maxRoomSpacing / 2);

			// var containmentCircleShape = SKShapeNode (circleOfRadius: CGFloat (containmentCircle.radius));
			// containmentCircleShape.position = room.rect.center.cgPoint;
			// containmentCircleShape.fillColor = .clear;
			// containmentCircleShape.strokeColor = .purple;
			// containmentCircleShape.isAntialiased = false;
			// containmentCircleShape.zPosition = 0;
			// addChild (containmentCircleShape);
		}
	}

	void addGraph (Dungeon<DungeonRoom, DungeonHallway> graph) {
		foreach (var vertex in graph.vertices) {
			var edges = graph.edgesFrom (vertex);
			foreach (var edge in edges) {

				// var path = CGMutablePath ();
				// path.move (to: edge.from.data.rect.center.cgPoint);
				// path.addLine (to: edge.to.data.rect.center.cgPoint);

				// var visualShape = SKShapeNode (path: path);
				// visualShape.fillColor = .clear;
				// visualShape.strokeColor = .green;
				// visualShape.alpha = 0.5;
				// visualShape.isAntialiased = false;
				// visualShape.zPosition = 2;
				// addChild (visualShape);
			}
		}
	}

	void addHallways () {
		foreach (var hallway in dungeonGenerator.dungeon.hallways) {
			foreach (var rect in hallway.rects) {
				var newHallway = GameObject.Instantiate (hallwayObject, dungeonContainer);
				var rectTransform = newHallway.GetComponent<RectTransform> ();
				rectTransform.sizeDelta = new Vector2 ((float) rect.size.width, (float) rect.size.height);
				rectTransform.localPosition = new Vector3 ((float) rect.center.x, (float) rect.center.y, 0);
			}
		}
	}

	void add2DGrid () {
		var grid = dungeonGenerator.to2DGrid ();
		for (var y = 0; y < grid.GetLength(0); y++) {
			for(var x = 0; x < grid.GetLength(1); x++) {
				var value = grid[y,x];
				if (value == 0) {
					continue;
				}
				var newPixel = GameObject.Instantiate (pixelObject, dungeonContainer);
				var rectTransform = newPixel.GetComponent<RectTransform> ();
				rectTransform.localPosition = new Vector3 ((float)x, (float)y, 0);
			}
		}
	}

}