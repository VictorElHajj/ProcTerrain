public class Vector {
    public (int x, int y) Dir {get; set;}

    public Vector (int x, int y) {
        this.Dir = (x, y);
    }

    public Vector ((int, int) dir) {
        this.Dir = dir;
    }
    //TODO Write Vector functions
}