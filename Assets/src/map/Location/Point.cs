using System;

public class Point {
    public (int x, double y, int z) Pos {get; set;}

    public Point (int x, double y, int z) {
        this.Pos = (x, y, z);
    }
    public Point ((int, double, int) pos) {
        this.Pos = pos;
    }

    public static Point Random(int width, int height, int seed) {
        Random rand = new Random(seed);
        return new Point(rand.Next(0, height), 0, rand.Next(0, width));
    }

    public static Point Zero() {
        return new Point (0,0,0);
    }
}