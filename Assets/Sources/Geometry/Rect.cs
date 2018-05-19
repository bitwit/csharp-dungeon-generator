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
            return origin.offsetBy (new Point (size.width, size.height));
        }
    }

    public double diagonalLength {
        get {
            return origin.distanceFrom (end);
        }
    }

    public Rect (Point origin, Size size) {
        this.origin = origin;
        this.size = size;
        adjustForNegativeSizes ();
    }

    public static Rect RectFromCenter (Point center, Size size) {
        var originX = center.x - (size.width / 2);
        var originY = center.y - (size.height / 2);
        var origin = new Point (x: originX, y: originY);
        var rect = new Rect (origin, size);
        rect.adjustForNegativeSizes ();
        return rect;
    }

    public static Rect RectFromXY (double x, double y, double width, double height) {
        var rect = new Rect (new Point (x: x, y: y), new Size (width: width, height: height));
        rect.adjustForNegativeSizes ();
        return rect;
    }

    public void adjustForNegativeSizes () {
        if (size.width < 0) {
            origin.x -= size.width;
            size.width = Math.Abs (size.width);
        }
        if (size.height < 0) {
            origin.y -= size.height;
            size.height = Math.Abs (size.height);
        }
    }

    public bool contains (Point point) {
        return point.x >= minX && point.x < maxX &&
            point.y >= minY && point.y < maxY;
    }

    public bool contains (Rect rect2) {
        return
        this.contains (rect2.origin) &&
            this.contains (new Point (x: rect2.maxX, y: rect2.minY)) &&
            this.contains (new Point (x: rect2.maxX, y: rect2.maxY)) &&
            this.contains (new Point (x: rect2.minX, y: rect2.maxY));
    }

    public bool intersects (Point lineOrigin, Point lineEnd) {
        return (lineOrigin.x > minX && lineOrigin.x < maxX && lineOrigin.y > minY && lineOrigin.y < maxY) ||
        (lineEnd.x > minX && lineEnd.x < maxX && lineEnd.y > minY && lineEnd.y < maxY);
    }

    public bool intersects (Rect rect2) {
        return minX < rect2.maxX && rect2.minX < maxX &&
            minY < rect2.maxY && rect2.minY < maxY;
    }

    public Rect insetBy (double inset) {
        var newSize = new Size (width: size.width - (inset * 2), height: size.height - (inset * 2));
        return Rect.RectFromCenter (center, newSize);
    }

    public string description {
        get {
            return origin.description + "/" + size.description;
        }
    }

    public bool isEqualTo (Rect lhs, Rect rhs) {
        return size.isEqualTo(rhs.size) && lhs.center.isEqualTo(rhs.center);
    }

}