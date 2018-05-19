using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class DungeonGenerator<RoomType, HallwayType>
    where RoomType : DungeonRoom, new ()
    where HallwayType : DungeonHallway, new () {

    public Size dungeonSize = new Size (width: 64, height: 64);
    public Size creationBounds = new Size (width: 64, height: 64);
    public double minimumRoomWidth = 5;
    public double maximumRoomWidth = 14;
    public double minimumRoomHeight = 5;
    public double maximumRoomHeight = 14;
    public double minimumRoomSpacing = 2;
    public double maxRoomSpacing = 8;
    public double hallwayWidth = 4.0;

    public int initialRoomCreationCount = 30;
    public int maximumStepsBeforeRetry = 50;

    public Dungeon<RoomType, HallwayType> dungeon;

    public List<RoomType> layoutRooms = new List<RoomType> ();
    public int[,] grid;

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

        for (int i = 0; i < initialRoomCreationCount; i++) {

            var offsetX = (dungeonSize.width - creationBounds.width) / 2;
            var offsetY = (dungeonSize.height - creationBounds.height) / 2;

            Random rnd = new Random ();
            var x = offsetX + Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (creationBounds.width)));
            var y = offsetY + Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (creationBounds.height)));
            var width = Math.Floor (minimumRoomWidth + Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (maximumRoomWidth - minimumRoomWidth))));
            var height = Math.Floor (minimumRoomHeight + Convert.ToDouble (rnd.Next (0, Convert.ToInt32 (maximumRoomHeight - minimumRoomHeight))));

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

            double velocityX = 0;
            double velocityY = 0;
            int neighborCount = 0;

            foreach (var otherRoom in layoutRooms) {

                //TODO: equatability
                if (currentRoom == otherRoom) {
                    continue;
                }

                var paddedRect = currentRoom.rect.insetBy (-minimumRoomSpacing);
                if (false == paddedRect.intersects (otherRoom.rect)) {
                    continue;
                }

                var diffPos = paddedRect.origin.diffOf (otherRoom.rect.origin);

                velocityX += diffPos.x;
                velocityY += diffPos.y;
                neighborCount += 1;
            }

            if (neighborCount == 0) {
                continue;
            }

            velocityX /= Convert.ToDouble (neighborCount);
            velocityY /= Convert.ToDouble (neighborCount);

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
            var newX = Math.Ceiling (room.rect.origin.x);
            var newY = Math.Ceiling (room.rect.origin.y);
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

                var paddedRect = currentRoom.rect.insetBy (-minimumRoomSpacing);
                if (paddedRect.intersects (otherRoom.rect)) {
                    return false;
                }
            }
        }
        return true;
    }

    public void removeRoomsOutOfBounds () {

        //inset dungeon rect to prevent rooms on edges
        var dungeonRect = new Rect (origin: new Point (x : 0, y : 0), size : dungeonSize).insetBy (1);
        layoutRooms = layoutRooms.Where (room => dungeonRect.contains (room.rect)).ToList ();
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
            foreach(var otherRoom in connections) {
                var otherVertex = graph.createVertex (otherRoom);
                var hallway = new HallwayType();
                graph.addEdge (currentVertex, otherVertex, hallway, currentRoom.rect.center.distanceFrom (otherRoom.rect.center));
            }
        }

        layoutRooms = finalRooms;

        return graph;
    }

    public void generateDungeonGraph() {
        if (dungeon != null) {
            return;
        }
        
        var tree = GraphHelpers.minimumSpanningTreeKruskal(graph: generateGraph());
        this.dungeon = new Dungeon<RoomType, HallwayType>(tree);
    }
    
    public void generateHallways() {
        
        generateDungeonGraph();
        generateLineHallways();
        foreach(var hallway in dungeon.hallways) {
            
            var lineSet = hallway.points;
           
            if (lineSet.Count < 2) { continue; }
            
            var firstLineStart = lineSet[0].roundedUp();
            var firstLineEnd = lineSet[1].roundedUp();

            var verticalDiff = firstLineStart.diffOf(firstLineEnd);
            var verticalDirection = verticalDiff.ToDirection();
            var roundedHalfWidth = Math.Ceiling(hallwayWidth / 2);
            
            //vertical hallways are first
            if (verticalDirection == Direction.Down) {
                var origin = firstLineStart.offsetBy(x: -roundedHalfWidth, y: 0);
                var rect = new Rect(origin: origin, size: new Size(width: hallwayWidth, height: firstLineStart.distanceFrom(firstLineEnd)));
                hallway.rects.Add(rect);
            } else {
                var origin = firstLineEnd.offsetBy(x: -roundedHalfWidth, y: 0);
                var rect = new Rect(origin: origin, size: new Size(width: hallwayWidth, height: firstLineStart.distanceFrom(firstLineEnd)));
                hallway.rects.Add(rect);
            }
            
            if (lineSet.Count < 3) { continue; }
            
            var secondLineStart = lineSet[0].roundedUp();
            var secondLineEnd = lineSet[1].roundedUp();

            var horizontalDiff = secondLineStart.diffOf(secondLineEnd);
            var horizontalDirection = horizontalDiff.ToDirection();
            
            //horizontal comes second
            if (horizontalDirection == Direction.Left) {
                var origin = secondLineStart.offsetBy(x: 0, y: -roundedHalfWidth);
                var rect = new Rect(origin: origin, size: new Size(width: secondLineStart.distanceFrom(secondLineEnd), height: hallwayWidth));
                hallway.rects.Add(rect);
            } else {
                var origin = secondLineEnd.offsetBy(x: 0, y: -roundedHalfWidth);
                var rect = new Rect(origin: origin, size: new Size(width: secondLineStart.distanceFrom(secondLineEnd), height: hallwayWidth));
                hallway.rects.Add(rect);
            }
        }
    }
    
    public void generateLineHallways() {

        foreach(var edge in dungeon.edges) {
            
            var fromRoom = edge.from.data;
            var toRoom = edge.to.data;
            
            var lineOrigin = fromRoom.rect.center;
            edge.data.points.Add(lineOrigin);
            
            var positionDiff = toRoom.rect.center.diffOf(lineOrigin);
            var verticalLinePoint = lineOrigin.offsetBy(new Point(0, positionDiff.y));
            edge.data.points.Add(verticalLinePoint);
            
            if (toRoom.rect.intersects(lineOrigin, verticalLinePoint)) {
                continue;
            }
            
            var horizontalLinePoint = verticalLinePoint.offsetBy(new Point(positionDiff.x, 0));
            edge.data.points.Add(horizontalLinePoint);
        }
    }
    
    public int[,] to2DGrid() {
        
        if (grid != null || grid.Length != 0) {
            return grid;
        }

        int[,] newGrid = new int[Convert.ToInt32(dungeonSize.height),Convert.ToInt32(dungeonSize.width)];
        
        List<Rect> rects = new List<Rect>();

        foreach(var room in dungeon.rooms) {
            rects.Add(room.rect);
        }
        foreach(var hallway in dungeon.hallways) {
            rects.AddRange(hallway.rects);
        }
        
        foreach(var rect in rects) {
            var initialX = Convert.ToInt32(rect.origin.x);
            var maxX = Convert.ToInt32(rect.origin.x + rect.size.width);
            var initialY = Convert.ToInt32(rect.origin.y);
            var maxY = Convert.ToInt32(rect.origin.y + rect.size.height);
            
            for (var x = initialX; x < maxX; x++) {
                for(var y = initialY; y < maxY; y++) {
                    newGrid[y, x] = 1;
                }
            }
        }
        
        this.grid = newGrid;
        
        return grid;
    }

}