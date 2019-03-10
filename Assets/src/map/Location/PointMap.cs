using System.Linq;

public class PointMap {
    public Point[,] Points {get; set;}
    public int Width {get;}
    public int Height {get;}
    public PointMap (int width, int height) {
        this.Points = new Point[height, width];
        this.Width = width;
        this.Height = height;

        //Initializes as a flat map
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++)
            {
                this.Points[i,j] = new Point(i,0,j);
            }
        }
    }

    public Point getLowestNeighboor (Point p) {
        p = this.Points[p.Pos.x, p.Pos.z];
        Point[] neighbors = new Point[8];
        int[] dX = new int[] { 1, 1, 1,  0, 0, -1, -1, -1};
        int[] dz = new int[] {-1, 0, 1, -1, 1, -1,  0,  1};

        for (int i = 0; i < 8; i++)
        {
            neighbors[i] = Points[p.Pos.x+dX[i], p.Pos.z+dz[i]];
        }

        //finds smallest value
        double smallest =  neighbors.Min((Point n) => n.Pos.y);
        //returns the object with that value
        return neighbors.First((Point n) => n.Pos.y==smallest);
    }
}