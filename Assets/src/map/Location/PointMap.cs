public class PointMap {
    public Point[,] Points {get; set;}

    public PointMap (int width, int height) {
        this.Points = new Point[height, width];
        //TOODOO initialize all points
    }
}