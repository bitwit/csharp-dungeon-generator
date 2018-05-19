using System;

public struct Size {

    public double width;
    public double height;
    
    public Size(double width, double height) {
        this.width = width;
        this.height = height;
    }
    
    public string description {
        get {
            return "W:" + width + "H:" + height;
        }
    }

    public bool isEqualTo(Size otherSize) {
        return width == otherSize.width && height == otherSize.height;
    }
}