public class Particle {
    //Location data
    public Point Pos {get; set;}
    public Vector Dir {get; set;}

    //Simulation data
    public double Speed {get; set;}
    public double Water {get; set;}
    public double Sediment {get; set;}

    public Particle (Point pos, Vector dir, double speed, double water, double sediment) {
        this.Pos = pos;
        this.Dir = dir;
        this.Speed = speed;
        this.Water = water;
        this.Sediment = sediment;
    }
}