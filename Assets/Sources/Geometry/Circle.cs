using System;

namespace DungeonGenerator {
    public struct Circle {

        public Point center;
        public float radius;

        public Circle (Rect rect) {
            radius = rect.diagonalLength / 2;
            center = rect.center;
        }

        public bool intersects (Circle circle2) {

            var radiusDiff = Math.Pow (radius - circle2.radius, 2);
            var centerXDiff = Math.Pow (center.x - circle2.center.x, 2);
            var centerYDiff = Math.Pow (center.y - circle2.center.y, 2);
            var radiusCombine = Math.Pow (radius + circle2.radius, 2);
            var centerDiffCombine = centerXDiff + centerYDiff;

            return radiusDiff <= centerDiffCombine &&
                centerDiffCombine <= radiusCombine;
        }
    }
}