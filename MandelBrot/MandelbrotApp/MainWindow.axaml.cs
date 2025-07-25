using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia;

namespace MandelbrotApp;

public partial class MainWindow : Window
{
    private Image imageControl = null!;
    private WriteableBitmap bitmap = null!;
    private const int CanvasSize = 1000;

    private CanvasCoordinatesGiver coordinatesGiver;

    public MainWindow()
    {
        coordinatesGiver = new CanvasCoordinatesGiver(CanvasSize, 8, 8.0 / CanvasSize, CanvasSize / 2, CanvasSize / 2);
        InitializeComponent();
        SetupCanvas();
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
    }
}              

