using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DungeonGenerator {
    public class DungeonGenerator<RoomType, HallwayType>
        where RoomType : DungeonRoom, new ()
    where HallwayType : DungeonHallway, new () {

        public Size dungeonSize = new Size (width: 64, height: 64);
        public Size creationBounds = new Size (width: 64, height: 64);
        public float minimumRoomWidth = 5;
        public float maximumRoomWidth = 14;
        public float minimumRoomHeight = 5;
        public float maximumRoomHeight = 14;
        public float minimumRoomSpacing = 2;
        public float maxRoomSpacing = 8;
        public float hallwayWidth = 4.0f;

        public int initialRoomCreationCount = 30;
        public int maximumStepsBeforeRetry = 50;

        public Dungeon<RoomType, HallwayType> dungeon;

        public List<RoomType> layoutRooms = new List<RoomType> ();
        public int[, ] grid;

        private int numberOfStepsTaken = 0;

        public DungeonGenerator () { }

        public void runCompleteGeneration () {
            var stopwatch = new Stopwatch ();
            stopwatch.Start ();
            generateRooms ();

            while (false == containsNoIntersectingRooms ()) {
                applyFittingStep ();
            }

            roundRoomPositions ();

            while (false == containsNoIntersectingRooms ()) {
                applyFittingStep ();
                roundRoomPositions ();
            }

            generateHallways ();

            stopwatch.Stop ();

            Console.WriteLine ("Dungeon generated in " + stopwatch.ElapsedMilliseconds + " milliseconds");
        }

        public void generateRooms () {

            numberOfStepsTaken = 0;

            Random rnd = new Random ();
            for (int i = 0; i < initialRoomCreationCount; i++) {

                var offsetX = (dungeonSize.width - creationBounds.width) / 2;
                var offsetY = (dungeonSize.height - creationBounds.height) / 2;

                float x = offsetX + (float)Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (creationBounds.width)));
                float y = offsetY + (float)Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (creationBounds.height)));
                float width = (float)Math.Floor (minimumRoomWidth + Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (maximumRoomWidth - minimumRoomWidth))));
                float height = (float)Math.Floor (minimumRoomHeight + Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (maximumRoomHeight - minimumRoomHeight))));

                var position = new Point (x: x, y: y);
                var size = new Size (width: width, height: height);
                var rect = new Rect (origin: position, size: size);
                var room = new RoomType ();
                room.rect = rect;
                layoutRooms.Add (room);
            }

            dungeon = null;
        }

        public void applyFittingStep () {

            if (numberOfStepsTaken > maximumStepsBeforeRetry) {
                generateRooms ();
            }

            numberOfStepsTaken += 1;
            removeRoomsOutOfBounds ();

            foreach (var currentRoom in layoutRooms) {

                float velocityX = 0;
                float velocityY = 0;
                int neighborCount = 0;

                foreach (var otherRoom in layoutRooms) {

                    //TODO: equatability
                    if (currentRoom == otherRoom) {
                        continue;
                    }

                    var paddedRect = currentRoom.rect.InsetBy (-minimumRoomSpacing);
                    if (false == paddedRect.Intersects (otherRoom.rect)) {
                        continue;
                    }

                    var diffPos = paddedRect.origin.DiffOf (otherRoom.rect.origin);

                    velocityX += diffPos.x;
                    velocityY += diffPos.y;
                    neighborCount += 1;
                }

                if (neighborCount == 0) {
                    continue;
                }

                velocityX /= (float)Convert.ToDouble (neighborCount);
                velocityY /= (float)Convert.ToDouble (neighborCount);

                velocityX = velocityX / currentRoom.rect.diagonalLength;
                velocityY = velocityY / currentRoom.rect.diagonalLength;

                var newX = currentRoom.rect.origin.x + velocityX;
                var newY = currentRoom.rect.origin.y + velocityY;
                var newPosition = new Point (x: newX, y: newY);
                var newRect = new Rect (origin: newPosition, size: currentRoom.rect.size);
                currentRoom.rect = newRect;
            }

        }

        public void roundRoomPositions () {
            foreach (var room in layoutRooms) {
                var newX = (float)Math.Ceiling (room.rect.origin.x);
                var newY = (float)Math.Ceiling (room.rect.origin.y);
                room.rect.origin = new Point (x: newX, y: newY);
            }
        }

        public bool containsNoIntersectingRooms () {
            foreach (var currentRoom in layoutRooms) {
                foreach (var otherRoom in layoutRooms) {

                    //TODO: equatability
                    if (currentRoom == otherRoom) {
                        continue;
                    }

                    var paddedRect = currentRoom.rect.InsetBy (-minimumRoomSpacing);
                    if (paddedRect.Intersects (otherRoom.rect)) {
                        return false;
                    }
                }
            }
            return true;
        }

        public void removeRoomsOutOfBounds () {

            //inset dungeon rect to prevent rooms on edges
            var dungeonRect = new Rect (origin: new Point (x : 0, y : 0), size : dungeonSize).InsetBy (1);
            layoutRooms = layoutRooms.Where (room => dungeonRect.Contains (room.rect)).ToList ();
        }

        public Dungeon<RoomType, HallwayType> generateGraph () {

            var graph = new Dungeon<RoomType, HallwayType> ();
            var connectableRoomRadius = (maxRoomSpacing / 2);
            var connectedRooms = new Dictionary<RoomType, List<RoomType>> ();

            foreach (var currentRoom in layoutRooms) {

                var currentRoomReach = new Circle (currentRoom.rect);
                currentRoomReach.radius += connectableRoomRadius;

                var pairings = new List<RoomType> ();
                foreach (var otherRoom in layoutRooms) {

                    //TODO: equatability
                    if (currentRoom == otherRoom) {
                        continue;
                    }

                    var otherRoomReach = new Circle (otherRoom.rect);
                    otherRoomReach.radius += connectableRoomRadius;

                    if (currentRoomReach.intersects (otherRoomReach)) {
                        pairings.Add (otherRoom);
                    }
                }

                connectedRooms.Add (currentRoom, pairings);
            }

            var finalRooms = new List<RoomType> (connectedRooms.Count);
            foreach (KeyValuePair<RoomType, List<RoomType>> entry in connectedRooms) {
                var currentRoom = entry.Key;
                var connections = entry.Value;

                finalRooms.Add (currentRoom);
                var currentVertex = graph.createVertex (currentRoom);
                foreach (var otherRoom in connections) {
                    var otherVertex = graph.createVertex (otherRoom);
                    var hallway = new HallwayType ();
                    graph.addEdge (currentVertex, otherVertex, hallway, currentRoom.rect.center.DistanceFrom (otherRoom.rect.center));
                }
            }

            layoutRooms = finalRooms;

            return graph;
        }

        public void generateDungeonGraph () {
            if (dungeon != null) {
                return;
            }

            var initialGraph = generateGraph ();
            var tree = GraphHelpers.minimumSpanningTreeKruskal (graph: initialGraph);
            this.dungeon = new Dungeon<RoomType, HallwayType> (tree);
        }

        public void generateHallways () {

            generateDungeonGraph ();
            generateLineHallways ();
            foreach (var hallway in dungeon.hallways) {

                var lines = hallway.lines;
                var roundedHalfWidth = (float)Math.Ceiling (hallwayWidth / 2);
                foreach (var initialLine in lines) {
                    var line = initialLine.RoundedUp ();
                    var diff = line.start.DiffOf (line.end);
                    var direction = diff.ToDirection ();

                    if (direction == Direction.Down) {
                        var origin = line.start.OffsetBy (x: -roundedHalfWidth, y : 0);
                        var rect = new Rect (origin: origin, size: new Size (width : hallwayWidth, height : line.start.DistanceFrom (line.end)));
                        hallway.rects.Add (rect);
                    } else if (direction == Direction.Up) {
                        var origin = line.end.OffsetBy (x: -roundedHalfWidth, y : 0);
                        var rect = new Rect (origin: origin, size: new Size (width : hallwayWidth, height : line.start.DistanceFrom (line.end)));
                        hallway.rects.Add (rect);
                    } else if (direction == Direction.Left) {
                        var origin = line.start.OffsetBy (x: 0, y: -roundedHalfWidth);
                        var rect = new Rect (origin: origin, size: new Size (width : line.start.DistanceFrom (line.end), height : hallwayWidth));
                        hallway.rects.Add (rect);
                    } else {
                        var origin = line.end.OffsetBy (x: 0, y: -roundedHalfWidth);
                        var rect = new Rect (origin: origin, size: new Size (width : line.start.DistanceFrom (line.end), height : hallwayWidth));
                        hallway.rects.Add (rect);
                    }
                }
            }
        }

        public void generateLineHallways () {

            foreach (var edge in dungeon.edges) {
                var fromRoom = edge.from.data;
                var toRoom = edge.to.data;

                var lineOrigin = fromRoom.rect.center;
                var positionDiff = toRoom.rect.center.DiffOf (lineOrigin);
                var verticalLinePoint = lineOrigin.OffsetBy (new Point (0, positionDiff.y));
                var horizontalLinePoint = verticalLinePoint.OffsetBy (new Point (positionDiff.x, 0));
                Line firstLine = new Line (lineOrigin, verticalLinePoint);
                Line secondLine = new Line (verticalLinePoint, horizontalLinePoint);

                if (false == fromRoom.rect.Contains (firstLine) /*lineOrigin.isEqualTo(verticalLinePoint)*/ ) {
                    var fromRoomFirstLineIntersectionPoint = firstLine.ClosestPointOfIntersection (fromRoom.rect, firstLine.end);
                    firstLine.start = fromRoomFirstLineIntersectionPoint.Value;

                    if (toRoom.rect.Intersects (firstLine)) {
                        var toRoomFirstLineIntersectionPoint = firstLine.ClosestPointOfIntersection (toRoom.rect, firstLine.start);
                        firstLine.end = toRoomFirstLineIntersectionPoint.Value;
                        edge.data.lines.Add (firstLine);
                        continue;
                    } else {
                        edge.data.lines.Add (firstLine);
                    }
                } else {
                    var fromRoomSecondLineIntersectionPoint = secondLine.ClosestPointOfIntersection (fromRoom.rect, secondLine.end);
                    secondLine.start = fromRoomSecondLineIntersectionPoint.Value;
                }

                var toRoomSecondLineIntersectionPoint = secondLine.ClosestPointOfIntersection (toRoom.rect, secondLine.start);
                secondLine.end = toRoomSecondLineIntersectionPoint.Value;
                edge.data.lines.Add (secondLine);
            }
        }

        public int[, ] to2DGrid () {

            if (grid != null) {
                return grid;
            }

            int[, ] newGrid = new int[Convert.ToInt32 (dungeonSize.height), Convert.ToInt32 (dungeonSize.width)];

            List<Rect> rects = new List<Rect> ();

            foreach (var room in dungeon.rooms) {
                rects.Add (room.rect);
            }
            foreach (var hallway in dungeon.hallways) {
                rects.AddRange (hallway.rects);
            }

            foreach (var rect in rects) {
                var initialX = Convert.ToInt32 (rect.origin.x);
                var maxX = Convert.ToInt32 (rect.origin.x + rect.size.width) - 1;
                var initialY = Convert.ToInt32 (rect.origin.y);
                var maxY = Convert.ToInt32 (rect.origin.y + rect.size.height) - 1;

                for (var x = initialX; x < maxX; x++) {
                    for (var y = initialY; y < maxY; y++) {
                        newGrid[y, x] = 1;
                    }
                }
            }

            this.grid = newGrid;

            return grid;
        }

    }
}