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
    private int seed;

    public Terrain (int width, int height, int seed) {
        width++; //10 by 10 vertices is only 9 by 9 tiles
        height++;
        this.Map = new PointMap(width, height);
        this.Particles = new List<Particle>();
        this.seed = seed;
    }

}