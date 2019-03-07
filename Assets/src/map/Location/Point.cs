public class Point {
    public (int x, int y, int z) Pos {get; set;}

    public Point (int x, int y, int z) {
        this.Pos = (x, y, z);
    }
    public Point ((int, int, int) pos) {
        this.Pos = pos;
    }
}