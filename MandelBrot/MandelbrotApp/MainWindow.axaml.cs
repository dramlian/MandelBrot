using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;
using Avalonia.Input;

namespace MandelbrotApp;

public partial class MainWindow : Window
{
    private Image imageControl = null!;
    private WriteableBitmap bitmap = null!;
    private const int CanvasSize = 1000;

    private double Offset;
    private double Precision;
    private int MiddleCoordinateX;
    private int MiddleCoordinateY;

    private CanvasCoordinatesGiver coordinatesGiver;

    public MainWindow()
    {
        Offset = 8;
        Precision = Offset / CanvasSize;
        MiddleCoordinateX = CanvasSize / 2;
        MiddleCoordinateY = CanvasSize / 2;
        coordinatesGiver = new CanvasCoordinatesGiver(CanvasSize, Offset, Precision, MiddleCoordinateX, MiddleCoordinateY);
        InitializeComponent();
        SetupCanvas();
        
        // Enable keyboard events
        this.KeyDown += OnKeyDown;
        this.Focusable = true;
        this.Focus();
    }

    private void SetupCanvas()
    {
        // Create a WriteableBitmap for efficient pixel manipulation
        bitmap = new WriteableBitmap(new PixelSize(CanvasSize, CanvasSize), new Vector(96, 96), PixelFormat.Bgra8888, AlphaFormat.Premul);
        
        // Create an Image control to display the bitmap
        imageControl = new Image
        {
            Width = CanvasSize,
            Height = CanvasSize,
            Source = bitmap
        };

        // Set the image as the content of the window
        Content = imageControl;

        // Draw pixels efficiently
        DrawPixels();
    }

    private void DrawPixels()
    {
        using (var lockedBitmap = bitmap.Lock())
        {
            var buffer = new byte[CanvasSize * CanvasSize * 4]; // 4 bytes per pixel (BGRA)
            
            for (int y = 0; y < CanvasSize; y++)
            {
                for (int x = 0; x < CanvasSize; x++)
                {
                    int index = (y * CanvasSize + x) * 4;

                    if (coordinatesGiver.ShouldColorPixel(x, y))
                    {
                        // Set pixel to red
                        buffer[index] = 0;     // Blue
                        buffer[index + 1] = 0; // Green
                        buffer[index + 2] = 255; // Red
                        buffer[index + 3] = 255; // Alpha
                    }
                    // If not coloring, leave as default (0,0,0,0) which is transparent/black
                }
            }
            
            // Copy the buffer to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, lockedBitmap.Address, buffer.Length);
        }
        
        // Invalidate the bitmap to force a redraw
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
                if (Offset <= 0) Offset = 0.1; // Prevent offset from becoming zero or negative
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

