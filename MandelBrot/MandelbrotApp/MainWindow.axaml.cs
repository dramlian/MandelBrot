using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Input;

namespace MandelbrotApp;

public partial class MainWindow : Window
{
    private Image imageControl = null!;
    private WriteableBitmap bitmap = null!;
    private readonly int CanvasSize;
    private double Offset;
    private double Precision;
    private int MiddleCoordinateX;
    private int MiddleCoordinateY;
    private CanvasCoordinatesGiver coordinatesGiver;

    public MainWindow()
    {
        CanvasSize = 1000;
        Offset = 8;
        Precision = Offset / CanvasSize;
        MiddleCoordinateX = CanvasSize / 2;
        MiddleCoordinateY = CanvasSize / 2;
        coordinatesGiver = new CanvasCoordinatesGiver(CanvasSize, Offset, Precision, MiddleCoordinateX, MiddleCoordinateY);
        InitializeComponent();
        SetupCanvas();
        
        this.KeyDown += OnKeyDown;
        this.Focusable = true;
        this.Focus();
    }

    private void SetupCanvas()
    {
        bitmap = new WriteableBitmap(new PixelSize(CanvasSize, CanvasSize), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        
        imageControl = new Image
        {
            Width = CanvasSize,
            Height = CanvasSize,
            Source = bitmap
        };
        Content = imageControl;

        DrawPixels();
    }

    private void DrawPixels()
    {
        using (var lockedBitmap = bitmap.Lock())
        {
            var buffer = new byte[CanvasSize * CanvasSize * 4];
            
            for (int y = 0; y < CanvasSize; y++)
            {
                for (int x = 0; x < CanvasSize; x++)
                {
                    int index = (y * CanvasSize + x) * 4;

                    if (coordinatesGiver.ShouldColorPixel(x, y))
                    {
                        buffer[index] = 0;   
                        buffer[index + 1] = 0; 
                        buffer[index + 2] = 255;
                        buffer[index + 3] = 255; 
                    }
                }
            }
            
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, lockedBitmap.Address, buffer.Length);
        }
        
        imageControl.Source = null;
        imageControl.Source = bitmap;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        bool shouldUpdate = false;

        switch (e.Key)
        {
            case Key.Up:
                MiddleCoordinateY -= 150;
                shouldUpdate = true;
                break;
            case Key.Down:
                MiddleCoordinateY += 150;
                shouldUpdate = true;
                break;
            case Key.Left:
                MiddleCoordinateX -= 150;
                shouldUpdate = true;
                break;
            case Key.Right:
                MiddleCoordinateX += 150;
                shouldUpdate = true;
                break;
            case Key.Space:
                Offset -= 0.5;
                if (Offset <= 0) Offset = 0.1;
                Precision = Offset / CanvasSize;
                shouldUpdate = true;
                break;
        }

        if (shouldUpdate)
        {
            UpdateCoordinatesGiver();
            DrawPixels();
            // Force the image to refresh
            imageControl.InvalidateVisual();
        }
    }

    private void UpdateCoordinatesGiver()
    {
        coordinatesGiver.MiddleCoordinateX = MiddleCoordinateX;
        coordinatesGiver.MiddleCoordinateY = MiddleCoordinateY;
        coordinatesGiver.Offset = Offset;
        coordinatesGiver.Precision = Precision;
    }
}              

