using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls; //for Canvas object
using Windows.UI.Xaml.Media.Imaging;

namespace GameV2.Classes
{
    public class GameDriver 
    {

        //class members
        DiverChar _diverchar;
        List<Enemies> _enemies = new List<Enemies>(); //Holds the amount of Enemies
        Canvas _playgroundCanvas; //hold the actual canvas with the player and enemies
        private DispatcherTimer _tmr = new DispatcherTimer();  //the timer that will move the enemy
        private DispatcherTimer _tmrExplosion = new DispatcherTimer(); //Timer That removes Explosion Images after 800 milliseconds
        private DispatcherTimer _tmrImageBlink = new DispatcherTimer(); //timer for blinking Player
        Random _rnd = new Random();  //randomise the enemies on screen
        private AppBarButton _btnStartNewGame,    //Buttons that will be binded to MainPage's buttons through GameDrive constructor
                             _btnResumeGame,
                             _btnPause,
                             _btnSaveFile,
                             _btnLoadFile;
        private List<SaveDataSchema> _SavedDataList;  //hold the saved data 
        public bool IsGameRunning;
        public bool IsGameIsLoaded { get; set; }
        public bool IsLoadedMoreThenOnce;
        private bool IsGameWasPlayedPreviously = false;
        public bool IsPaused;
        private int PausedTimes = 1;
        private bool _IsPlayerHit;
        private bool _IsLoopOn; //loop of blinking kobe
        private int LoadedTimes = 0;
        private int _diverTop,
                    _diverLeft;
        private int _enemyTopExplosionCoordinates;
        private int _enemyLeftExplosionCoordinates;
        private int CollisionTimes = 0;
        private int _Counter_HowLong_ToBlink = 15; //number of blinks like loop
        private TextBlock _textBlockPlayerLifeLeft = new TextBlock();
        private TextBlock _txtBlockScore = new TextBlock();
        List<Image> _ExplosionImgList = new List<Image>();
        Image _explosionImage = new Image();

        ContentDialog _DialogLose = new ContentDialog();
        ContentDialog _DialogWin = new ContentDialog();
        ContentDialog _DialogPause = new ContentDialog();
        

        //constructor

        public GameDriver(Canvas playground, AppBarButton pauseGameBtn, AppBarButton resumeGameBtn, AppBarButton startNewGame, AppBarButton saveFileBtn, AppBarButton loadFileBtn, bool isGameIsLoaded,ContentDialog DialogLose, ContentDialog DialogWin, ContentDialog DialogPause, TextBlock textBlockLifesLeft,TextBlock score)
        {
            
            //game timer
            _tmr.Interval = new TimeSpan(0, 0, 0, 0, 150); //set 150 miliseconds control the enemies speed
            _tmr.Tick += OnTickHandler;
            //Explosion Timer
            _tmrExplosion.Interval = new TimeSpan(0, 0, 0, 0, 1000);  //tmr of the image explosion 
            _tmrExplosion.Tick += OnTickHandlerExplosion;
            //blink
            _tmrImageBlink.Interval = new TimeSpan(0, 0, 0, 0, 50);   //timr of the blinking effect tmr
            _tmrImageBlink.Tick += OnTickPlayerBlinking;

            
            this._txtBlockScore = score;
            this._playgroundCanvas = playground;
            this._btnPause = pauseGameBtn;
            this._btnResumeGame = resumeGameBtn;
            this._btnStartNewGame = startNewGame;
            this._btnSaveFile = saveFileBtn;
            this._btnLoadFile = loadFileBtn;
            this.IsGameIsLoaded = isGameIsLoaded;
            this._DialogLose = DialogLose;
            this._DialogWin = DialogWin;
            this._DialogPause = DialogPause;
            this._textBlockPlayerLifeLeft = textBlockLifesLeft;
            
        }
        //create a new game 
        public void NewGame()
        {
            ClearBoard();
            CreateBoard();
            
        }
        private void CreateBoard()    //create the board
        {
            //Fresh Start
            SetPlayButtonState();
            _diverchar = new DiverChar(_playgroundCanvas, 3, 0);
            CreateEnemies();
            this._textBlockPlayerLifeLeft.Text = _diverchar.LivesLeft.ToString();
            this._txtBlockScore.Text = _diverchar.Score.ToString();
            _tmr.Start();
            _tmrImageBlink.Start();
            IsGameWasPlayedPreviously = true;
            IsGameRunning = true;
            _tmrExplosion.Start();          
        }
        private void UpdateLoads()
        {
                CreateEnemies2();
                if (_diverchar!=null) //to prevent crashing when trying to load before started playing
                {
                 _diverchar.Destroy();
                 _diverchar = new DiverChar(_playgroundCanvas, _diverTop, _diverLeft, _diverchar.LivesLeft, _diverchar.Score);
                 _tmr.Start();
                 _tmrImageBlink.Start();
                 IsGameRunning = true;
                 IsGameWasPlayedPreviously = true;
                 _tmrExplosion.Start();
                 this._textBlockPlayerLifeLeft.Text = _diverchar.LivesLeft.ToString();
                 this._txtBlockScore.Text = _diverchar.Score.ToString();
                }
        }

        public void CreateEnemies()    //create the enemies
        {
            Random rnd = new Random();
            int _Startleftrandom = 0;
            int _Starttoprandom = 0;
            int _enemyID = 0;
            int CheckDistanceLeft;
            int CheckDistanceTop;
            //creating 9 enemies
            for (int i = 0; i < 9; i++)
            {
                _Startleftrandom = rnd.Next(0, (int)_playgroundCanvas.ActualWidth - 100);
                _Starttoprandom = rnd.Next(0, (int)_playgroundCanvas.ActualHeight - 100);
                CheckDistanceLeft = Math.Abs(_Startleftrandom - (int)_diverchar.GetLeft());
                CheckDistanceTop = Math.Abs(_Starttoprandom - (int)_diverchar.GetTop());
                if (CheckDistanceLeft>=200 && CheckDistanceTop>=200)
                {
                _enemies.Add(new Enemies(_playgroundCanvas, _Startleftrandom, _Starttoprandom, _enemyID));
                _enemyID++;
                }
                else
                {
                    i--;
                }
            }
           
        }

        public void CreateEnemies2()   //create the enemies from the saves
        {

            int PlayerTop = 0;
            int PlayerLeft = 0;
            if (_diverchar!=null) 
            {
                for (int i = 0; i < _SavedDataList.Count; i++)
                {
                    _enemies.Add(new Enemies(_playgroundCanvas, _SavedDataList[i]._EnemieStartLeft, _SavedDataList[i]._EnemieStartTop, _SavedDataList[i]._IdEnemie));
                    PlayerTop = _SavedDataList[i]._PlayerStartTop;
                    PlayerLeft = _SavedDataList[i]._PlayerStartLeft;
                    _diverchar.LivesLeft = _SavedDataList[i]._LifeIsLeft;
                    _diverchar.Score = _SavedDataList[i].Score;
                }
            }
            else
            {
                FileNotExistMessage();
            }
            _diverTop = PlayerTop;
            _diverLeft = PlayerLeft;
        }
       private void OnTickHandlerExplosion(object sender, object e)   //deletes the xplosion image  
        {
            foreach (var item in _ExplosionImgList)
            {
                Canvas.SetLeft(item, _enemyLeftExplosionCoordinates);
                Canvas.SetTop(item, _enemyTopExplosionCoordinates);
                _playgroundCanvas.Children.Remove(item);
            }
        }
        private void ChaseAfterKobe(List<Enemies> enemies)  //enemies chase after kobe
        {
            
            for (int i = 0; i < enemies.Count; i++)
            {
                int randEnemy = _rnd.Next(25);
                if (_diverchar.GetTop() < _enemies[i].GetTop()) //check top
                {
                    if (randEnemy != 0)
                    {
                        _enemies[i].MoveUp();
                    }
                }
                else if (_diverchar.GetTop() > _enemies[i].GetTop())
                {
                    if (randEnemy != 0)
                    {
                        _enemies[i].MoveDown();
                    }
                }
                if (_diverchar.GetLeft() < _enemies[i].GetLeft())
                {
                    if (randEnemy != 0)
                    {
                        _enemies[i].MoveLeft();
                    }
                }
                else if (_diverchar.GetLeft() > _enemies[i].GetLeft())
                {
                    if (randEnemy != 0)
                    {
                        _enemies[i].MoveRight();
                    }
                }
            }
        }

        private void OnTickPlayerBlinking(object sender, object e)  //calls the action of blinking
        {
            StartBlinking();
        }
        private void StartBlinking()  //blink the player when gets hit (in total only changes the pic of the char to skull and back)
        {
            if (_IsPlayerHit && _Counter_HowLong_ToBlink != 0 && _IsLoopOn) //makes a loop that makes kobe change pic to skull and blink meanwhile
            {
                _Counter_HowLong_ToBlink--;
                BlinkPlayer();
            }
            else if (_Counter_HowLong_ToBlink == 0) //stop blink when counter is zero
            {
                _Counter_HowLong_ToBlink = 15; //set counter to 15 blinks
                _IsLoopOn = false;
                _IsPlayerHit = false;
            }
            else
            {
                //otherwise set image to kobe
                _Counter_HowLong_ToBlink--;
                UnblinkPlayer();
            }
        }
        private void BlinkPlayer()   //blink kobe when he gets hit (change its pic to skull when enemy touches him)
        {
            string path = "ms-appx:///Assets/PicsOfProject/Skull.png";
            _diverchar.BaseImage.Source = new BitmapImage(new Uri(path));
            _IsPlayerHit = false;
        }
        private void UnblinkPlayer() //unblink kobe return to kobe when no one touches him
        {
            string path2 = "ms-appx:///Assets/PicsOfProject/Diver.png";
            _diverchar.BaseImage.Source = new BitmapImage(new Uri(path2));
            _IsPlayerHit = true;


        }
        private void ClearBoard()    //clear the board
        {
            SetPlayButtonState();
            if (_diverchar != null)
            {
                _diverchar.Destroy();
            }
            for (int i = 0; i < _enemies.Count; i++)
            {
                if (_enemies[i] != null)
                {
                    _enemies[i].Destroy();
                }
            }
            _tmr.Stop();
            _tmrImageBlink.Stop();
            _tmrExplosion.Stop();
            _enemies.Clear();
            //_LifesLeft = 0;
            IsGameRunning = false;
            
            
            LoadedTimes = 0;
            IsLoadedMoreThenOnce = false;
            _IsPlayerHit = false;
            _IsLoopOn = false;
        }
        private void EnemyCollision()   //enemy collision with each other*
        {
            const int _step = 10;
            for (int i = 0; i < _enemies.Count; i++)
            {
                for (int j = 0; j < _enemies.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    Enemies _tempEnemy1 = _enemies[i];
                    Enemies _tempEnemy2 = _enemies[j];
                    Point _centerOfImg = new Point(
                        (int)(_enemies[i].GetLeft() + _enemies[i].GetWidth() / 2),
                        (int)(_enemies[i].GetTop() + _enemies[i].GetHeight() / 2));
                    Point _centerOfImg2 = new Point(
                        (int)(_enemies[j].GetLeft() + _enemies[j].GetWidth() / 2),
                        (int)(_enemies[j].GetTop() + _enemies[j].GetHeight() / 2));
                    double _distance = Math.Sqrt(
                        Math.Pow(_centerOfImg.X - _centerOfImg2.X, 2) +
                        Math.Pow(_centerOfImg.Y - _centerOfImg2.Y, 2));
                    if (_tempEnemy1.Height + _step >= _distance)
                    {
                        _diverchar.Score += 20;
                        _txtBlockScore.Text = _diverchar.Score.ToString();
                        _tempEnemy1.Destroy();
                        _enemies.Remove(_tempEnemy1);
                        _tempEnemy2.Destroy();
                        _enemies.Remove(_tempEnemy2);
                        _enemyLeftExplosionCoordinates = (int)_tempEnemy1.GetLeft();
                        _enemyTopExplosionCoordinates = (int)_tempEnemy1.GetTop();
                        DisplayExplosionOnCollision();
                        _enemyLeftExplosionCoordinates = (int)_tempEnemy2.GetLeft();
                        _enemyTopExplosionCoordinates = (int)_tempEnemy2.GetTop();
                        DisplayExplosionOnCollision();
                        if (_enemies.Count<=1)
                        {
                            CreateEnemies();
                        }
                        break;
                    }
                }
            }
        }

        private void PlayerCollision()     //kobe collision with enemies
        {
            double Xtop = _diverchar.GetTop();
            double Yleft = _diverchar.GetLeft();
            double size = 70;

            for (int i = 0; i < _enemies.Count; i++)
            {
                if ((_diverchar.GetTop() > _enemies[i].GetTop() - size && _diverchar.GetTop() < _enemies[i].GetTop() + size)
                && _diverchar.GetLeft() > _enemies[i].GetLeft() - size && _diverchar.GetLeft() < _enemies[i].GetLeft() + size)
                {
                    //Collision happend
                    CollisionTimes++;
                    _IsPlayerHit = true;
                    _IsLoopOn = true;

                    //Life Reducing 
                    //Reduce life level only after 2 physical hits.
                    if (_diverchar.LivesLeft != 0 && CollisionTimes == 2)
                    {
                        ReduceLife();
                        CollisionTimes = 0;
                    }
                    if (_diverchar.LivesLeft == 0)
                    {
                        _tmr.Stop();
                        _tmrImageBlink.Stop();
                        GameOverLose();
                        CollisionTimes = 0;
                    }
                }
                else
                {
                    _IsPlayerHit = false;
                }
            }
        }
        private void DisplayExplosionOnCollision()  //Display the Explosion Image on Collision
        {
            Image ExplosionImage = new Image();
            Uri uri = new Uri("ms-appx:///Assets/PicsOfProject/Exlosion.gif");
            ExplosionImage.Source = new BitmapImage(uri);
            //apply the size of the explosion
            ExplosionImage.Height = 100;
            ExplosionImage.Width = 100;

            //set the x,y position of the new explosion
            Canvas.SetTop(ExplosionImage, _enemyTopExplosionCoordinates);
            Canvas.SetLeft(ExplosionImage, _enemyLeftExplosionCoordinates);

            //Adding Explosion to the Canvas
            _explosionImage = ExplosionImage;
            _playgroundCanvas.Children.Add(_explosionImage);
            _ExplosionImgList.Add(ExplosionImage); 
        }
        private void ReducePlayerLifeOnScreen() //display the life of kobe
        {
            _textBlockPlayerLifeLeft.Text = _diverchar.LivesLeft.ToString();
        }
        private void ReduceLife() //calculate the life of kobe
        {
            _diverchar.LivesLeft--;
            ReducePlayerLifeOnScreen();
        }

        private void OnTickHandler(object sender, object e)
        {
            ChaseAfterKobe(_enemies);
            EnemyCollision();
            PlayerCollision();
            CheckWin();

        }

        internal void ResumeGame()
        {
            //resume game

            _DialogPause.Hide();
            _tmr.Start();
            _tmrExplosion.Start();
            SetPlayButtonState();
            IsGameRunning = true;
            IsPaused = false;
            PausedTimes = 1;
        }
       
        internal async void PauseGame()  //pause the game
        {
            IsPaused = true;
            _tmr.Stop();
            _tmrExplosion.Stop();
            if (IsPaused == true && PausedTimes == 1 && IsGameRunning) //first time paused
            {
                IsGameRunning = false;
                PausedTimes++;
                await _DialogPause.ShowAsync();
            }
            else if (IsGameWasPlayedPreviously)
            {
                _tmr.Start();
                _tmrExplosion.Start();
                IsGameRunning = true;
                _DialogPause.Hide();
                IsPaused = false;  //reset
                PausedTimes = 1;  //reset
            }
        }
        internal void StopTimer() //for music future stop the timer of the game
        {
            _tmr.Stop();
            IsGameRunning = false;

        }
        private void SetButtonsOnWins()  //define the buttons when u win (what's visible and what's not)
        {
            _btnPause.Visibility = Visibility.Collapsed;
            _btnResumeGame.Visibility = Visibility.Collapsed;
            _btnStartNewGame.Visibility = Visibility.Visible;
            _btnSaveFile.Visibility = Visibility.Collapsed;
            _btnLoadFile.Visibility = Visibility.Visible;
        }
        private void SetButtonsOnLoseGame() //define the buttons when u lose (what's visible and what's not)
        {
            _btnPause.Visibility = Visibility.Collapsed;
            _btnResumeGame.Visibility = Visibility.Collapsed;
            _btnStartNewGame.Visibility = Visibility.Visible;
            _btnSaveFile.Visibility = Visibility.Collapsed;
            _btnLoadFile.Visibility = Visibility.Visible;
        }
        private void SetPlayButtonState()  //define the buttons when u are at the start
        {
            _btnPause.Visibility = Visibility.Visible;
            _btnResumeGame.Visibility = Visibility.Collapsed;
            _btnStartNewGame.Visibility = Visibility.Visible;
            _btnSaveFile.Visibility = Visibility.Visible;
            _btnLoadFile.Visibility = Visibility.Visible;
        }
        private void SetButtonsOnLoad()
        {
            _btnPause.Visibility = Visibility.Visible;
            _btnResumeGame.Visibility = Visibility.Collapsed;
            _btnStartNewGame.Visibility = Visibility.Visible;
            _btnSaveFile.Visibility = Visibility.Visible;
            _btnLoadFile.Visibility = Visibility.Visible;
        }

        internal async void LoadFile()  //load the file that u saved
        {
           
            try
            {
                List<SaveDataSchema> _data = new List<SaveDataSchema>();
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("myconfig.json");

                //TextReadingOrder Saved Data into string
                string json_str = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
                _data = JsonConvert.DeserializeObject<List<SaveDataSchema>>(json_str);
                _SavedDataList = _data;
                //clear board before proceeding
                ClearBoard();
                UpdateLoads();
                
                IsGameIsLoaded = true;
                LoadedTimes++;
                if (LoadedTimes > 1)
                {
                    IsLoadedMoreThenOnce = true;
                }
            }
            catch (Exception)
            {
                FileNotExistMessage();
            }
        }

        internal async void SaveFile()  //save the file so u can continue
        {
            List<SaveDataSchema> _simpleDataList = new List<SaveDataSchema>();
            foreach (var CurrentEnemy in _enemies)
            {
                _simpleDataList.Add(new SaveDataSchema((int)CurrentEnemy.GetTop(),
                                                       (int)CurrentEnemy.GetLeft(),
                                                            CurrentEnemy.GetId(),
                                                       (int)this._diverchar.GetTop(),
                                                       (int)this._diverchar.GetLeft(),
                                                            this._diverchar.LivesLeft,
                                                            this._diverchar.Score));
           }
            try
            {
                string json = JsonConvert.SerializeObject(_simpleDataList.ToArray());
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("myconfig.json");
                await FileIO.WriteTextAsync(file, json);
                
            }
            catch (Exception)
            {
                //if file exist simply delete the previous
                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("myconfig.json");
                string name = "myconfig.json";
                StorageFile manifestFile = await storageFolder.GetFileAsync(name);
                await manifestFile.DeleteAsync();
                
                //then save the new one 
                //serialize data to json string 
                string json = JsonConvert.SerializeObject(_simpleDataList.ToArray());
                //write string to a file 
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("myconfig.json");
                await FileIO.WriteTextAsync(file, json);
                
            }
        }

        private void CheckWin() //check if the player won the game
        {

            if (_diverchar.Score>=180 )
            {
               GameOverWin();
            }
           
        }
        private void GameOverLose()   //game over
        {
            IsGameWasPlayedPreviously = false;
            IsGameRunning = false;
            
            SetButtonsOnLoseGame();
            _tmrExplosion.Stop();
            _tmr.Stop();
            _diverchar.Score = 0;
            ClearBoard();
            GameOverMessage();
        }
        private void GameOverWin()  //game win
        {
            ClearBoard();
            
            IsGameWasPlayedPreviously = false;
            SetButtonsOnWins();
            _tmrExplosion.Stop();
            _tmr.Stop();
            IsGameRunning = false;
            _diverchar.Score = 0;
            YouWinMessage();
        }
        public void MovePlayerUp()  //up
        {
            _diverchar.MoveUp();
        }
        public void MovePlayerDown()  //down
        {
            _diverchar.MoveDown();
        }
        public void MovePlayerLeft()  //left
        {
            _diverchar.MoveLeft();
        }
        public void MovePlayerRight()  //right
        {
            _diverchar.MoveRight();
        }
        
        private async void GameOverMessage()  //msg of lose
        {
            await _DialogLose.ShowAsync();
        }

        private async void YouWinMessage()  //msg of win
        {
            await _DialogWin.ShowAsync();
        }
        
        private async void FileNotExistMessage()  //msg for not saving the file 
        {
            MessageDialog msg = new MessageDialog("File Not Exist, Please Save The File First");
            await msg.ShowAsync();
        }

    }
}
