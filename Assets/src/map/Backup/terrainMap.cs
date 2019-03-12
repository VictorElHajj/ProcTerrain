using System;
using System.Collections;
using System.Collections.Generic;
//Using https://github.com/Auburns/FastNoise_CSharp, don't forget to credit.

public class terrainMap
{
    public double[,][] map; //A 2D array of third dimensiononal vectors
    public FastNoise simplex1;
    private int seed;
    Random rand;
    //Erosion settings
    double erosions = 20000;
    double inertia = 0.3; //Set value
    double minSlope = 0.01;
    double capacity = 4;
    double deposition = 0.2;
    double erosion = 0.7; //Need to try 0.1 and 0.9;
    int radius = 6;
    double gravity = 10; //Try low and high values
    double evaporation = 0.02; //Also important.
    int maxSteps = 64; //try lowering
    //TODOO
    /* 
    Implement proper deposition!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        */
    public terrainMap (int width, int heigth, int seed) {
        this.map = new double[width+1,heigth+1][]; //10 by 10 example
        this.seed = seed;
        this.simplex1 = new FastNoise();
        this.simplex1.SetNoiseType(FastNoise.NoiseType.Simplex);

        //For a 10 by 10 map we need 11 by 11 vertices.
        width+=1;
        heigth+=1;

        //Seeds
        this.rand = new Random(seed);
        int seed1 = rand.Next();
        int seed2 = rand.Next();
        int seed3 = rand.Next();
        int rainSeed = rand.Next();

        double [,] elipseGradient = getElipseGradient(width, heigth);
        double [,] rainMap = new double[width, heigth];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < heigth; j++) {
                //Initial Simplex Heigtht for rain density and initial terrain heigth
                double h = simplex1.GetNoise(seed1+i/2, seed1+j/2)*0.7+simplex1.GetNoise(seed2+i*1, seed2+j*1)*0.2+simplex1.GetNoise(seed3+i*1, seed3+j*1)*0.1;
                double r = simplex1.GetNoise(rainSeed+i, rainSeed+j);
                //Normalizing from [-1,1] to [0,1]
                h += 1.0; // [0,2]
                h /= 2.0; // [0,1]
                r += 1.0; // [0,2]
                r /= 2.0; // [0,1]

                //Cutting of edges with a circular mask. Uncomment two lines below to turn into island
                //h -= elipseGradient[i,j];
                //h = Math.Max(h, 0);

                //If initial terrain is zero then the rain should be zero, used to not waste computing in the erosion method.
                r = h == 0 ? 0 : r;

                //Increasing amplitude.
                h *= 80;

                map[i,j] = new double[] {i, h, j};
                rainMap[i,j] = r;
            }
        }

        for (int e = 0; e < erosions; e++) {
            int x = rand.Next(0, width);
            int y = rand.Next(0, heigth);
            if (rainMap[x,y] == 0) //Todo replace
                continue;
            var (xDir, yDir) = randomLocationDelta();
            particle newInput = particleErosion(new particle(x, y, xDir, yDir, 0.0, 10, 0.0)); //Maybe replace rainmap by constant 10
            if (newInput == null)
                continue;

            for (int i = 0; i < maxSteps; i++) {
                newInput = particleErosion(newInput);
                if (newInput == null)
                    break;
            }
        }
    }

    class particle {
        public int x, y, xDir, yDir;
        public double speed, water, sediment;
        public particle(int x, int y, int xDir, int yDir, double speed, double water, double sediment) {
            this.x = x;
            this.y = y;
            this.xDir = xDir;
            this.yDir = yDir;
            this.speed = speed;
            this.water = water;
            this.sediment = sediment;
        }
    }
    //Particle based erosion. 
    particle particleErosion(particle p) {
            //Die if neighboring end of map
            int i, j;
            var values = getLowestNeighbor(p.x, p.y);
            if (values.HasValue)
                (i, j) = values.Value;
            else
                return null;
            int x2, y2, xDir2, yDir2;
            (x2, y2, xDir2, yDir2) = getNextNeighbor(p.x, p.y, i, j, p.xDir, p.yDir); //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //(x2, y2, xDir2, yDir2) = (p.x+i, p.y+j, i, j);

            //If we are in a dip
            double hdelta = map[x2,y2][1] - map[p.x, p.y][1]; //Something wrong here - apparantly not
            if (map[x2,y2][1] > map[p.x, p.y][1]) {
                //If theres enough sediment, fill the dip and reset speed then keep going
                if (p.sediment >= hdelta) {
                    depose(p, hdelta);
                    //map[p.x, p.y][1] += hdelta; //Should this be reversed??
                    p.sediment -= hdelta;
                    p.speed = 0; //Maybe remove this entirely
                    (xDir2, yDir2) = randomLocationDelta();
                    return new particle(p.x, p.y, xDir2, yDir2, p.speed, p.water, p.sediment);
                }
                //Otherwise, die trying
                else {
                    depose(p, p.sediment);
                    //map[p.x, p.y][1] += p.sediment;
                    return null;
                }
            }
            else {
                //If we are not in a dip
                double carryCapacity = Math.Max(-hdelta, minSlope) * p.speed * p.water * capacity;
                if (p.sediment > carryCapacity) {
                    double drop = (p.sediment-carryCapacity) * deposition;
                    depose(p, drop);
                }
                else {
                    double pick = Math.Min( (carryCapacity-p.sediment)*erosion, -hdelta);
                    erode(p, pick);
                }
                p.speed = Math.Sqrt(p.speed*p.speed - hdelta*gravity);
                p.water = p.water*(1-evaporation);
                return new particle(x2, y2, xDir2, yDir2, p.speed, p.water, p.sediment);
            }
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
    //Checks all neighbors and returns the relative index of the lowest. Returns null if location borders edge of map.
    public (int i, int j)? getLowestNeighbor (int x, int y) {
        int[] dX = {-1,-1,-1, 0, 0, 1, 1, 1};
        int[] dY = {-1, 0, 1,-1, 1,-1, 0, 1};
        int smallestI = 10; //Default value is pointless
        double smallestH = 10000000000; //Todo, make it = amplitude
        for (int i = 0; i < 8; i++) {
            int X = x + dX[i];
            int Y = y + dY[i];
            if (X >= 0 && X < map.GetLength(0) && Y >= 0 && Y < map.GetLength(1)) {
                double h = map[X,Y][1];
                if (h < smallestH) {
                    smallestH = h;
                    smallestI = i;
                }
                if (smallestH == 0 && map[x, y][1] == 0) //Left the island, no point to continue
                    return null;
            }
            else {
                return null; //Borders edge of map
            }
        }
        return (dX[smallestI], dY[smallestI]);
    }

    //Implements inertia, instead of chosing straigth path it combines past direction with steepest.
    (int x2, int y2, int xDir2, int yDir2) getNextNeighbor (int x, int y, int lowX, int lowY, int xDir, int yDir) {
        int k = Convert.ToInt32(xDir*inertia + lowX*(1-inertia));
        int l = Convert.ToInt32(yDir*inertia + lowY*(1-inertia));
        /*k = Math.Max(-1, k);
        k = Math.Min(1, k);

        l = Math.Max(-1, l);
        l = Math.Min(1, l);*/
  
        if (k == 0 && l == 0) //To make sure it always moves. 
            (k, l) = randomLocationDelta();
        return ((x+k), (y+l), k, l);
    }

    (int k, int l) randomLocationDelta() {
        int[] dX = {-1, 0, 1,-1, 1,-1, 0, 1};
        int[] dY = {-1,-1,-1, 0, 0, 1, 1, 1};
        return (dX[rand.Next(0,7)], dY[rand.Next(0,7)]);
    }

    //Creates a recntangular gradient with an inner flat rectangle.
    private double[,] getRectangleGradient (int width, int heigth, int distanceFromEdge) {
        double[,] squareGradient = new double[width+1, heigth+1];
        for (int i = 0; i < width; i++) {
            for (int j = 0; j < heigth; j++) {
                double distanceFromTop = j < (heigth/2.0) ? j : heigth-j;
                double distanceFromSide = i < (width/2.0) ? i : width-i;
                double closestDistance = Math.Min(distanceFromTop, distanceFromSide);
                if (closestDistance > distanceFromEdge)
                    squareGradient[i,j] = 0;
                else {
                    squareGradient[i,j] = 1-closestDistance/distanceFromEdge;   
                }
            }
        }
        return squareGradient;
    }
    //TODO: make it actually work for rectangles. Only works for squares atm. Need to be eclipse not circle.
    private double[,] getElipseGradient (int width, int heigth) {
        double[,] elipseGradient = new double[width, heigth];
         for (int i = 0; i < width; i++) {
            for (int j = 0; j < heigth; j++) {
                double dX = (width/2.0)-i;
                double dY = (heigth/2.0)-j;
                double distanceFromCenter = Math.Sqrt(dX*dX + dY*dY);
                double radius = width/2;
                double normalDistance = distanceFromCenter/radius;
                elipseGradient[i,j] = normalDistance;
            }
         }
        return elipseGradient;
    }

    /*
    IDEAS
        1. Noise map to decide rain amount. Use for erosion and dryness
        2. Noise map for rock strata. Different toughness
        3. A function that does the erosion producedure without eroding, to calculate how much water flows trough each point.
     */
}
