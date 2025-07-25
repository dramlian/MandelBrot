using System;

namespace MandelbrotApp;

class CanvasCoordinatesGiver
{
    public double Offset { get; set; }
    public double Precision { get; set; }
    public int MiddleCoordinateX { get; set; }
    public int MiddleCoordinateY { get; set; }
    public CanvasCoordinatesGiver(int size, double offset, double precision, int middleCoordinateX, int middleCoordinateY)
    {
        Offset = offset;
        Precision = precision;
        MiddleCoordinateX = middleCoordinateX;
        MiddleCoordinateY = middleCoordinateY;
    }

    public bool ShouldColorPixel(int x, int y)
    {
        double MandelXEquivalent = (x - MiddleCoordinateX) * Precision;
        double MandelYEquivalent = (y - MiddleCoordinateY) * Precision;
        return IsInMandelbrotSet(MandelXEquivalent, MandelYEquivalent);
    }
    
    bool IsInMandelbrotSet(double x, double y, int maxIterations = 1000)
    {
        double zx = 0;
        double zy = 0;

        for (int i = 0; i < maxIterations; i++)
        {
            double zx2 = zx * zx;
            double zy2 = zy * zy;

            if (zx2 + zy2 > 4.0)
                return false;

            double temp = zx2 - zy2 + x;
            zy = 2 * zx * zy + y;
            zx = temp;
        }

        return true;
    }

}