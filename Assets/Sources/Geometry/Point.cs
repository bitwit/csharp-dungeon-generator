using System;

namespace DungeonGenerator {
    public struct Point {
        public float x;
        public float y;
        public static Point zero = new Point (0, 0);

        public Point (float x, float y) {
            this.x = x;
            this.y = y;
        }

        public Point OffsetBy (Point offset) {
            return new Point (x + offset.x, y + offset.y);
        }

        public Point OffsetBy (float x, float y) {
            return OffsetBy (new Point (x: x, y: y));
        }

        public Point DiffOf (Point point) {
            return new Point (x: x - point.x, y: y - point.y);
        }

        public float DistanceFrom (Point otherPoint) {
            float xDist = x - otherPoint.x;
            float yDist = y - otherPoint.y;

            return (float)Math.Sqrt (Convert.ToDouble(xDist * xDist) + Convert.ToDouble(yDist * yDist));
        }

        public Point RoundedUp () {
            return new Point ((float)Math.Ceiling (x), (float)Math.Ceiling (y));
        }

        public Direction ToDirection () {
            var z = Math.Atan2 (this.x, this.y);
            if (z == 0 || z == Math.Abs (Math.PI / 4)) {
                return Direction.Up;
            } else if (z == Math.PI) {
                return Direction.Down;
            } else if (z == (Math.PI / 2)) {
                return Direction.Right;
            } else if (z == -(Math.PI / 2)) {
                return Direction.Left;
            } else {
                return Direction.Down;
            }
        }

        public String description {
            get {
                return "x:" + x + "y:" + y;
            }
        }

        public bool isEqualTo (Point otherPoint) {
            return this.x == otherPoint.x && this.y == otherPoint.y;
        }
    }
}