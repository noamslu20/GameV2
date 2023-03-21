using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using GameV2.Classes;


    

namespace GameV2.Classes
{
    public class Characters //Base class
    {
        //local members
        protected string _BaseSrc;
        protected Canvas _BaseCanvasplayground;
        protected Image _Baseimg;
        private int _Basestep = 10;
        Random _rnd = new Random();
        //prop
        public Image BaseImage
        {
            get { return _Baseimg; }
            set { _Baseimg = value; }
        }
        public Characters(string src)
        {
            if (src == "ms-appx:///Assets/PicsOfProject/Diver.png")
            {
                _BaseSrc = src;
            }
            else
            {
                //otherwise use random enemy pic
                //download random enemy pics
                string path = _rnd.Next(1, 5).ToString();
                string EnemyRandomPath = "ms-appx:///Assets/PicsOfProject/Enemies/" + path + ".png";
                _BaseSrc = EnemyRandomPath;
            }
        }
        public double GetTop()
        {
            return Canvas.GetTop(_Baseimg);
        }
        public double GetLeft()
        {
            return Canvas.GetLeft(_Baseimg);
        }
        public double GetHeight()
        {
            return _Baseimg.Height;
        }
        public double GetWidth()
        {
            return _Baseimg.Width;
        }
        //removes them from the list
        public void Destroy()
        {
            _BaseCanvasplayground.Children.Remove(_Baseimg);
        }
        //implementing the basic movement of the player on the Canvas element
        //Each Creature can move to 4 sides
        //
        public void MoveUp()
        {
            //If imape Position from The Top of the main window(example 500px from the top of the window)
            //If image is on 500px from the top, and 500 is less than the basestep(10px)
            //Simple decrease the LENGTH between Top of the window and player
            //this will create of a moovement
            if (Canvas.GetTop(_Baseimg) <= _Basestep)
            {
                return;
            }
            Canvas.SetLeft(_Baseimg, Canvas.GetLeft(_Baseimg));
            Canvas.SetTop(_Baseimg, Canvas.GetTop(_Baseimg) - _Basestep);
        }
        public void MoveDown()
        {
            //if BaseImage TOP Coordinates + Basestep(10) + baseImg.height, is greater than canvas Height
            // Do nothing
            //Else Increase the step to the Topcoordinates which will moove the goodie down
            if (Canvas.GetTop(_Baseimg) + _Basestep + _Baseimg.Height > _BaseCanvasplayground.ActualHeight)
            {
                return;
            }
            Canvas.SetLeft(_Baseimg, Canvas.GetLeft(_Baseimg));
            Canvas.SetTop(_Baseimg, Canvas.GetTop(_Baseimg) + _Basestep);
        }

        public void MoveLeft()
        {
            if (Canvas.GetLeft(_Baseimg) <= _Basestep)
            {
                return;
            }
            Canvas.SetLeft(_Baseimg, Canvas.GetLeft(_Baseimg) - _Basestep);
            Canvas.SetTop(_Baseimg, Canvas.GetTop(_Baseimg));
        }
        public void MoveRight()
        {
            if (Canvas.GetLeft(_Baseimg) + _Basestep + _Baseimg.Width > +_BaseCanvasplayground.ActualWidth)
            {
                return;
            }
            Canvas.SetLeft(_Baseimg, Canvas.GetLeft(_Baseimg) + _Basestep);
            Canvas.SetTop(_Baseimg, Canvas.GetTop(_Baseimg));
        }

        public void MoveDiagonalUp()
        {
            Canvas.SetLeft(_Baseimg, Canvas.GetLeft(_Baseimg) - _Basestep);
            Canvas.SetTop(_Baseimg, Canvas.GetTop(_Baseimg) - _Basestep);
        }




    }
}
