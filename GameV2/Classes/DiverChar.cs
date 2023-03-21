using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls; // for Canvas
using Windows.UI.Xaml.Media.Imaging; //for BitMapImage
using GameV2.Classes;


namespace GameV2.Classes
{
    public class DiverChar : Characters
    {
        const int height = 100,
                  width = 100;

        int _startTop = 500,
            _startLeft = 300,
            _size = 100;
        private int _score;
        
        private int _lifes;

        public int Score 
        { 
            get { return _score; }
            set { _score = value; } 
        }
        public int LivesLeft
        {
            get { return _lifes; }
            set { _lifes = value; }
        }
        public DiverChar(Canvas _playground, int lifes, int score) : base("ms-appx:///Assets/PicsOfProject/Diver.png")
        {
            _BaseCanvasplayground = _playground;
            _Baseimg = new Image();
            _Baseimg.Source = new BitmapImage(new Uri(_BaseSrc));
            //apply ther size to our player
            _Baseimg.Height = _size;
            _Baseimg.Width = _size;
            this._score = score;
            this._lifes = lifes;
            //set the x,y position of the player
            Canvas.SetLeft(_Baseimg, _startLeft);
            Canvas.SetTop(_Baseimg, _startTop);
            //Adding Player to the canvas
            _BaseCanvasplayground.Children.Add(_Baseimg);

        }



        //constructor2 for the load game option

        public DiverChar(Canvas _playground, int startTop, int startLeft, int lifes,int score) : base("ms-appx:///Assets/PicsOfProject/Diver.png")
        {
            _BaseCanvasplayground = _playground;
            _Baseimg = new Image();
            _Baseimg.Source = new BitmapImage(new Uri(_BaseSrc));


            //Apply the size to the player
            _Baseimg.Height = _size;
            _Baseimg.Width = _size;

            //Set the x,y position of the player 
            Canvas.SetLeft(_Baseimg, _startLeft);
            Canvas.SetTop(_Baseimg, _startTop);
            this._lifes = lifes;
            this._score = score;
            //adding player to the canvas
            _BaseCanvasplayground.Children.Add(_Baseimg);

        }
    }
}
