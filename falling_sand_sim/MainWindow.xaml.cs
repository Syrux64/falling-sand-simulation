using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace falling_sand_sim
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer updateTimer;
        Random random = new Random();

        private SandParticle[][] sandParticles;
        private SandParticle tempSand;
        private int size;
        private int n; // total cells
        private double r, g, b;
        private int brush;

        public MainWindow()
        {
            InitializeComponent();
            r = random.Next(0, 256);
            g = random.Next(0, 256); 
            b = random.Next(0, 256); 

            tempSand = new SandParticle();
            size = tempSand.GetSize();
            n = 500 / size; // --  window size / cell size

            sandParticles = new SandParticle[n][];
            
            // place cells on the window
            for (int i = 0; i < n; i++){
                sandParticles[i] = new SandParticle[n];

                for (int j = 0; j < n; j++){
                    sandParticles[i][j] = new SandParticle();
                    sandParticles[i][j].SetPos(i * size, j * size);
                    mainCanvas.Children.Add(sandParticles[i][j].GetShape());
                }
            }

            // update screen 
            updateTimer = new DispatcherTimer();
            updateTimer.Interval = TimeSpan.FromMilliseconds(10);
            updateTimer.Tick += UpdateTimer_Tick;
            updateTimer.Start();
            mainCanvas.MouseMove += MouseDrag;
        }
        
        public void Gravity()
        {
            for (int j = n - 2; j >= 0; j--)
            {
                for (int i = 0; i < n; i++)
                {
                    // fall down
                    if (sandParticles[i][j].state && !sandParticles[i][j + 1].state)
                    {
                        sandParticles[i][j].state = false;
                        sandParticles[i][j].sand.Fill = Brushes.SkyBlue;

                        sandParticles[i][j + 1].state = true;
                        sandParticles[i][j + 1].sand.Fill = RGB(r, g, b);
                    }
                    // fall left/right
                    if (sandParticles[i][j].state && sandParticles[i][j + 1].state)
                        {
                            
                            int fallOn = random.Next(0, 2);

                            if (fallOn == 1 && (i + 1 < n && j + 1 < n) && !sandParticles[i + 1][j + 1].state)
                            {
                                sandParticles[i][j].state = false;
                                sandParticles[i][j].sand.Fill = Brushes.SkyBlue;
                                sandParticles[i + 1][j + 1].state = true;
                                sandParticles[i + 1][j + 1].sand.Fill = RGB(r, g, b);

                            }
                            else if (fallOn == 0 && (i - 1 >= 0 && j + 1 < n) && !sandParticles[i - 1][j + 1].state)
                            {
                                sandParticles[i][j].state = false;
                                sandParticles[i][j].sand.Fill = Brushes.SkyBlue;
                                sandParticles[i - 1][j + 1].state = true;
                                sandParticles[i - 1][j + 1].sand.Fill = RGB(r, g, b);
                            }
                        }
                }
            }
        }

        public void MouseDrag(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
                Point position = e.GetPosition(mainCanvas);
                int i = (int)(position.X / size);
                int j = (int)(position.Y / size);

                // brush
                int extent = brush / 2;

                for (int x = -extent; x <= extent; x++)
                    for (int y = -extent; y <= extent; y++)
                    {
                        int col = i + x;
                        int row = j + y;

                        if (col >= 0 && row >= 0 && col < n && row < n)
                            if (!sandParticles[col][row].state)
                            {
                                sandParticles[col][row].state = true;
                                sandParticles[col][row].sand.Fill = RGB(r, g, b);
                                if (r < 255) r += 0.1;
                                else if (g < 255) g += 0.1;
                                else if (b < 255) b += 0.1;
                                else
                                {
                                    r = random.Next(0, 256);
                                    g = random.Next(0, 256);
                                    b = random.Next(0, 256);
                                }
                            }
                    }
            }
        }

        public void BrushSizeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            brush = (int)brushSize.Value;
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    sandParticles[i][j].state = false;
                    sandParticles[i][j].sand.Fill = Brushes.SkyBlue;
                }
            }
        }

        private SolidColorBrush RGB(double r, double g, double b)
        {
            return new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
        }

        private void Snap(object sender, RoutedEventArgs e)
        {

            int width = (int)mainCanvas.ActualWidth;
            int height = (int)mainCanvas.ActualHeight;
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(width, height , 96d, 96d, PixelFormats.Pbgra32);

            renderBitmap.Render(mainCanvas);

            PngBitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));

            if (!Directory.Exists("Snaps"))
            {
                Directory.CreateDirectory("Snaps");
            }

            using (var fs = System.IO.File.OpenWrite(("Snaps/Scene_") + DateTime.Now.ToString("yyyMMdd_HHmmss")))
                pngEncoder.Save(fs);

            MessageBox.Show("Image Saved");
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            Gravity();
        }


    }

}