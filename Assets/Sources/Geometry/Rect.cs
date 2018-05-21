using System;

public struct Rect {

    public static Rect zero = Rect.RectFromXY(0,0,0,0);

    public Point origin;
    public Size size;

    public double minX {
        get {
            return origin.x;
        }
    }

    public double minY {
        get {
            return origin.y;
        }
    }

    public double maxX {
        get {
            return origin.x + size.width;
        }
    }

    public double maxY {
        get {
            return origin.y + size.height;
        }
    }

    public Point center {
        get {
            var centerX = origin.x + (size.width / 2);
            var centerY = origin.y + (size.height / 2);
            return new Point (x: centerX, y: centerY);
        }
        set {
            var originX = value.x - (size.width / 2);
            var originY = value.y - (size.height / 2);
            this.origin = new Point (x: originX, y: originY);
        }
    }

    public Point end {
        get {
            return origin.OffsetBy (new Point (size.width, size.height));
        }
    }

    public Line[] lines {
        get {
            var lines = new Line[4];
            lines[0] = new Line(new Point(minX, minY), new Point(maxX, minY));
            lines[1] = new Line(new Point(minX, minY), new Point(minX, maxY));
            lines[2] = new Line(new Point(minX, maxY), new Point(maxX, maxY));
            lines[3] = new Line(new Point(maxX, minY), new Point(maxX, maxY));
            return lines;
        }
    }

    public double diagonalLength {
        get {
            return origin.DistanceFrom (end);
        }
    }

    public Rect (Point origin, Size size) {
        this.origin = origin;
        this.size = size;
        AdjustForNegativeSizes ();
    }

    public static Rect RectFromCenter (Point center, Size size) {
        var originX = center.x - (size.width / 2);
        var originY = center.y - (size.height / 2);
        var origin = new Point (x: originX, y: originY);
        var rect = new Rect (origin, size);
        rect.AdjustForNegativeSizes ();
        return rect;
    }

    public static Rect RectFromXY (double x, double y, double width, double height) {
        var rect = new Rect (new Point (x: x, y: y), new Size (width: width, height: height));
        rect.AdjustForNegativeSizes ();
        return rect;
    }

    public void AdjustForNegativeSizes () {
        if (size.width < 0) {
            origin.x -= size.width;
            size.width = Math.Abs (size.width);
        }
        if (size.height < 0) {
            origin.y -= size.height;
            size.height = Math.Abs (size.height);
        }
    }

    public bool Contains (Point point) {
        return point.x >= minX && point.x < maxX &&
            point.y >= minY && point.y < maxY;
    }
    public bool Contains (Line line) {
        return Contains(line.start) && Contains(line.end);
    }

    public bool Contains (Rect rect2) {
        return
        this.Contains (rect2.origin) &&
            this.Contains (new Point (x: rect2.maxX, y: rect2.minY)) &&
            this.Contains (new Point (x: rect2.maxX, y: rect2.maxY)) &&
            this.Contains (new Point (x: rect2.minX, y: rect2.maxY));
    }

    public bool Intersects (Point lineOrigin, Point lineEnd) {
        return (lineOrigin.x > minX && lineOrigin.x < maxX && lineOrigin.y > minY && lineOrigin.y < maxY) ||
        (lineEnd.x > minX && lineEnd.x < maxX && lineEnd.y > minY && lineEnd.y < maxY);
    }

    public bool Intersects (Line line) {
        return Intersects(line.start, line.end);
    }

    public bool Intersects (Rect rect2) {
        return minX < rect2.maxX && rect2.minX < maxX &&
            minY < rect2.maxY && rect2.minY < maxY;
    }

    public Rect InsetBy (double inset) {
        var newSize = new Size (width: size.width - (inset * 2), height: size.height - (inset * 2));
        return Rect.RectFromCenter (center, newSize);
    }

    public string description {
        get {
            return origin.description + "/" + size.description;
        }
    }

    //TODO: confirm usage
    public bool isEqualTo (Rect lhs, Rect rhs) {
        return size.isEqualTo(rhs.size) && lhs.center.isEqualTo(rhs.center);
    }

}