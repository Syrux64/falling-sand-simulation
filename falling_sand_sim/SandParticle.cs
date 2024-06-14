using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace falling_sand_sim
{
    internal class SandParticle
    {
        public Rectangle sand;
        public bool state;
        public int size;

        public SandParticle() {
            size = 5;
            sand = new Rectangle
            {
                Width = size,
                Height = size,
                Stroke = Brushes.Black,
                StrokeThickness = 0,
                Fill = Brushes.SkyBlue,
            };
            state = false;
        }

        public void SetPos(int posX, int posY) {
            Canvas.SetLeft(sand, posX);
            Canvas.SetTop(sand, posY);
        }

        public Rectangle GetShape()
        {
            return sand;
        }

        public int GetSize()
        {
            return size;
        }
    }
}
