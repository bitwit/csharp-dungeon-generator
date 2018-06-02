using System;

namespace DungeonGenerator {
    public struct Size {

        public float width;
        public float height;

        public Size (float width, float height) {
            this.width = width;
            this.height = height;
        }

        public string description {
            get {
                return "W:" + width + "H:" + height;
            }
        }

        public bool isEqualTo (Size otherSize) {
            return width == otherSize.width && height == otherSize.height;
        }
    }
}