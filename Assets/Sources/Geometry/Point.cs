using System;

public struct Point {
    public double x;
    public double y;
    public static Point zero = new Point(0, 0);
    
    public Point(double x, double y) {
        this.x = x;
        this.y = y;
    }
    
    public Point offsetBy(Point offset) {
        return new Point(x + offset.x, y + offset.y);
    }
    
    public Point offsetBy(double x, double y) {
        return offsetBy(new Point(x: x, y: y));
    }
    
    public Point diffOf(Point point) {
        return new Point(x: x - point.x, y: y - point.y);
    }
    
    public double distanceFrom(Point otherPoint) {
        double xDist = x - otherPoint.x;
        double yDist = y - otherPoint.y;

        return Math.Sqrt( (xDist * xDist) + (yDist * yDist) );
    }
    
    public Point roundedUp() {
        return new Point(Math.Ceiling(x), Math.Ceiling(y));
    }
    
    public Direction ToDirection()
    {
        var z = Math.Atan2(this.x, this.y);
        if (z == 0 || z == Math.Abs(Math.PI/4)) {
            return Direction.Up;
        }
        else if (z == Math.PI) {
            return Direction.Down;
        }
        else if (z == (Math.PI / 2)) {
            return Direction.Right;
        }
        else if (z == -(Math.PI / 2)) {
            return Direction.Left;
        }
        else {
            return Direction.Down;
        }
    }
    public String description {
        get {
            return "x:" + x + "y:" + y;
        }
    }

    public bool isEqualTo(Point otherPoint) {
        return this.x == otherPoint.x && this.y == otherPoint.y;
    }
}