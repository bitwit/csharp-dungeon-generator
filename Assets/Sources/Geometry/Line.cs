using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonGenerator {
    public struct Line {

        public Point start;
        public Point end;
        public Line (Point start, Point end) {
            this.start = start;
            this.end = end;
        }

        public Line RoundedUp () {
            return new Line (start.RoundedUp (), end.RoundedUp ());
        }

        /* NOTE:
            Calculation treats line as indefinite, not just as a segment
        */
        public Point? PointOfIntersection (Line otherLine) {
            var A1 = end.y - start.y;
            var B1 = start.x - end.x;
            var C1 = A1 * start.x + B1 * start.y;

            var A2 = otherLine.end.y - otherLine.start.y;
            var B2 = otherLine.start.x - otherLine.end.x;
            var C2 = A2 * otherLine.start.x + B2 * otherLine.start.y;

            var delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                return null;

            var x = (B2 * C1 - B1 * C2) / delta;
            var y = (A1 * C2 - A2 * C1) / delta;

            return new Point (x, y);
        }

        public Point? ClosestPointOfIntersection (Rect rect, Point goal) {
            var intersections = new List<Point> ();
            foreach (var line in rect.lines) {
                var pointOfIntersection = PointOfIntersection (line);
                if (pointOfIntersection.HasValue) {
                    intersections.Add (pointOfIntersection.Value);
                }
            }
            if (intersections.Count == 0) {
                return null;
            }
            return intersections.OrderBy (x => x.DistanceFrom (goal)).ToArray () [0];
        }
        public String description {
            get {
                var p1 = "x1:" + start.x + "y1:" + start.y;
                var p2 = "x2:" + start.x + "y2:" + start.y;
                return p1 + "," + p2;
            }
        }

    }
}