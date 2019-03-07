
public class Gradients {
    //TODO replace double[,] with pointMap
    public double[,] getRectangleGradient (int width, int heigth, int distanceFromEdge) {
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
    public double[,] getElipseGradient (int width, int heigth) {
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
}