using System;
using System.Collections.Generic;

public class DungeonRoom {
    public Rect rect;
    public DungeonRoom() {
        this.rect = Rect.zero;
    }
    public String description {
        get {
            return rect.center.description;
        }
    }

    public override int GetHashCode() {
        return rect.center.description.GetHashCode();
    }
}

public class DungeonHallway {
    public List<Point> points = new List<Point>();
    public List<Rect> rects = new List<Rect>();
    public DungeonHallway() {
        
    }
}