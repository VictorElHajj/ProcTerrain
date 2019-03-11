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

    //Spreads out sediment up-picking between all points within the radius
    void erode (particle p, double pick) {
        int width = map.GetLength(0);
        int heigth = map.GetLength(1);

        Dictionary<(int, int), double> weights = new Dictionary<(int, int), double>();
        double totalWeight = 0;

        List<(int i, int j)> pointsWithinRadius = new List<(int, int)>();
        for(int x = -radius; x <= radius; ++x) {
            for(int y = -radius; y <= radius; ++y) {
                if(x*x + y*y <= radius * radius)   {
                    if (x+p.x < width && y+p.y < heigth && x+p.x >= 0 && y + p.y >= 0)
                        pointsWithinRadius.Add((x + p.x, y + p.y));
                }
            }
        }
        foreach ((int i, int j) point in pointsWithinRadius) {
            int i = point.i;
            int j = point.j;
            double dist = Math.Sqrt((i-p.x)*(i-p.x) + (j-p.y)*(j-p.y));
            double weight = Math.Max(0, radius - dist);
            weights.Add((i, j), weight);
            totalWeight += weight;
        }
        foreach ((int i, int j) point in pointsWithinRadius) {
            int i = point.i;
            int j = point.j;
            double w = weights[(i,j)]/totalWeight;
            map[i,j][1] -= w*pick;
            p.sediment += w*pick;
        }
    }

    void depose (particle p, double drop) {
        int width = map.GetLength(0);
        int heigth = map.GetLength(1);

        Dictionary<(int, int), double> weights = new Dictionary<(int, int), double>();
        double totalWeight = 0;

        List<(int i, int j)> pointsWithinRadius = new List<(int, int)>();
        for(int x = -radius; x <= radius; ++x) {
            for(int y = -radius; y <= radius; ++y) {
                if(x*x + y*y <= radius * radius)   {
                    if (x+p.x < width && y+p.y < heigth && x+p.x >= 0 && y + p.y >= 0)
                        pointsWithinRadius.Add((x + p.x, y + p.y));
                }
            }
        }
        foreach ((int i, int j) point in pointsWithinRadius) {
            int i = point.i;
            int j = point.j;
            double dist = Math.Sqrt((i-p.x)*(i-p.x) + (j-p.y)*(j-p.y));
            double weight = Math.Max(0, radius - dist);
            weights.Add((i, j), weight);
            totalWeight += weight;
        }
        foreach ((int i, int j) point in pointsWithinRadius) {
            int i = point.i;
            int j = point.j;
            double w = weights[(i,j)]/totalWeight;
            map[i,j][1] += w*drop;
            p.sediment -= w*drop;
        }

    }
}