using System;

namespace MandelbrotApp;

class CanvasCoordinatesGiver
{
    public double Offset { get; set; }
    public double Precision { get; set; }
    public int MiddleCoordinate { get; set; }
    public (int, int)[,] Coordinates { get; }
    public CanvasCoordinatesGiver(int size)
    {
        Coordinates = new (int, int)[size, size];
        Offset = 8; // The range of the Mandelbrot set to consider
        Precision = Offset / size;
        MiddleCoordinate = size / 2;
    }

    public bool ShouldColorPixel(int x, int y)
    {
        double MandelXEquivalent = (x - MiddleCoordinate) * Precision;
        double MandelYEquivalent = (y - MiddleCoordinate) * Precision;
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