using System;
using System.Collections;
using System.Collections.Generic;
//Using https://github.com/Auburns/FastNoise_CSharp, don't forget to credit.

public class Terrain {
    //Erosion properties
    private double erosions = 0;
    private double inertia = 0.3; //Set value
    private double minSlope = 0.01;
    private double capacity = 8;
    private double deposition = 0.2;
    private double erosion = 0.7; //Need to try 0.1 and 0.9
    private double gravity = 10; //Try low and high values
    private double evaporation = 0.02; //Also important.
    private int radius = 1;
    private int maxSteps = 64; //try lowering

    public PointMap Map {get;}
    private List<Particle> Particles;
    Random rand;

    private int seed;
    public FastNoise Simplex;

    public Terrain (int width, int height, int seed) {
        width++; //10 by 10 vertices is only 9 by 9 tiles
        height++;

        this.Map = new PointMap(width, height);
        this.Particles = new List<Particle>();
        this.Simplex = new FastNoise();
        this.Simplex.SetNoiseType(FastNoise.NoiseType.Simplex);
        this.seed = seed;
        this.rand = new Random(seed);

        int seed1 = rand.Next();
        int seed2 = rand.Next();
        int seed3 = rand.Next();
        int rainSeed = rand.Next();

        PointMap elipseGradient = Gradients.getElipseGradient(width, height);

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                //Initial Simplex Heigtht for rain density and initial terrain heigth
                double h = Simplex.GetNoise(seed1+i*1, seed1+j*1)*0.7+Simplex.GetNoise(seed2+i*2, seed2+j*2)*0.2+Simplex.GetNoise(seed3+i*4, seed3+j*4)*0.1;
                //Normalizing from [-1,1] to [0,1]
                h += 1.0; // [0,2]
                h /= 2.0; // [0,1]

                //Cutting of edges with a circular mask. Uncomment two lines below to turn into island
                //h -= elipseGradient.Points[i,j].Pos.y;
                //h = Math.Max(h, 0);

                //Increasing amplitude.
                h *= 50;

                Map.Points[i,j] = new Point(i, h, j);
            }
        }
        for (int i = 0; i < erosions; i++)
        {
            Point initPos = Point.Random(width, height, seed);
            Particle p = new Particle(initPos, Vector.Zero(), 0, 1, 0);
            for (int steps = 0; steps < maxSteps; steps++)
            {
                Vector LowestNeighbor = new Vector(p.Pos, Map.getLowestNeighboor(p.Pos));
                Vector DirNew = (p.Dir.Scale(inertia)).Add(LowestNeighbor.Scale(-(1-inertia)));
                //Round to [-1,1] and randomize if rounded to 0,0
            }
        }
    }

}