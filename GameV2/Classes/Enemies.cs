using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GameV2.Classes;

namespace GameV2.Classes
{
    internal class Enemies : Characters
    {

        int StartTop = 300;
        int StartLeft = 200;
        int Size = 70;
        int Id = 0;

        public double Height
        {
            get { return _Baseimg.ActualHeight; }
            set { _Baseimg.Height = value; }
        }

        public Enemies(Canvas playground, int startleft, int starttop, int id) : base("ms-appx:///Assets/PicsOfProject/Enemies/2.png")
        {

            // Local numbers
            StartLeft = startleft;
            StartTop = starttop;
            Id = id;

            //Base members
            _BaseCanvasplayground = playground;
            _Baseimg = new Image();
            _Baseimg.Source = new BitmapImage(new Uri(_BaseSrc));
            _Baseimg.Height = Size;
            _Baseimg.Width = Size;

            //set the enemy coordinates left-top
            Canvas.SetLeft(_Baseimg, StartLeft);
            Canvas.SetTop(_Baseimg, StartTop);

            //put the enemy into the canvas as a child 
            _BaseCanvasplayground.Children.Add(_Baseimg);

        }

        public int GetId()
        {
            return this.Id;
        }
        public void KillEnemies()
        {
            _BaseCanvasplayground.Children.Remove(_Baseimg); // the object will be removed in gamedriver class
        }


    }
}
