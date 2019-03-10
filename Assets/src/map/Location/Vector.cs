using System;

public class Vector {
    public (double x, double y) Dir {get; set;}

    public Vector (double x, double y) {
        this.Dir = (x, y);
    }

    public Vector ((double, double) dir) {
        this.Dir = dir;
    }

    public Vector (Point p1, Point p2) {
        (int x, double y, int z) = p1.Pos;
        (int x2, double y2, int z2) = p2.Pos;
        this.Dir = (x2-x, z2-z);
    }

    //Static
    public static Vector Random(int seed) {
        Random rand = new Random(seed);
        return new Vector(rand.Next(0, 2), rand.Next(0, 2));
    }

    public static Vector Zero() {
        return new Vector (0,0);
    }

    //local
    public Vector Scale(double scalar) {
        (double x, double y) = this.Dir;
        return new Vector(x*scalar, y*scalar);
    }

    public Vector Add(Vector other) {
        (double x1, double y1) = this.Dir;
        (double x2, double y2) = other.Dir;
        return new Vector(x1+x2, y1+y2);
    }
}