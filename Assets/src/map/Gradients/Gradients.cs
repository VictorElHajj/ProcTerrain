
public static class Gradients {
    //TODO replace double[,] with pointMap
    public static PointMap getRectangleGradient (int width, int height, int distanceFromEdge) {
            PointMap squareGradient = new PointMap(height, width);
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    double distanceFromTop = j < (width/2.0) ? j : width-j;
                    double distanceFromSide = i < (height/2.0) ? i : height-i;
                    double closestDistance = System.Math.Min(distanceFromTop, distanceFromSide);
                    if (closestDistance > distanceFromEdge)
                        squareGradient.Points[i,j] = new Point(i, 0, j);
                    else {
                        squareGradient.Points[i,j] = new Point(i, (1.0-closestDistance/(double)distanceFromEdge), j);
                    }
                }
            }
            return squareGradient;
        }
    public static PointMap getElipseGradient (int width, int height) {
        PointMap elipseGradient = new PointMap(height, width);
        for (int i = 0; i < height; i++) {
            for (int j = 0; j < width; j++) {
                double dX = (height/2.0)-i;
                double dY = (width/2.0)-j;
                double distanceFromCenter = System.Math.Sqrt(dX*dX + dY*dY);
                double radius = height/2;
                double normalDistance = distanceFromCenter/radius;
                elipseGradient.Points[i,j] = new Point(i, normalDistance, j);
            }
        }
        return elipseGradient;
    }
}