using GameV2.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace GameV2
{
    public sealed partial class MainPage : Page
    {
        GameDriver _GameDriver;
        private bool _IsGameLoaded { get; }

        public MainPage()
        {
            this.InitializeComponent();
            _GameDriver = new GameDriver(CanvasPlayingArea, btnPauseGame, btnResumeGame, btnStartNewgame, btnSaveFile, btnLoadFile, _IsGameLoaded, GameOverDialog, YouWinDialog, PauseDialog, txtLifesLeft,Scoretxt);
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown; //Register the KeyDown Event With a CoreWindow_KeyDown Method
            SetButtonsToNewGameState(); //start button state
        }

        //what key was pressed
        //This Method Fires EveryTime you press a button

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {

                case VirtualKey.Up:
                    if (_GameDriver.IsGameRunning)
                    {
                        _GameDriver.MovePlayerUp();
                    }
                    break;
                case VirtualKey.Down:
                    if (_GameDriver.IsGameRunning)
                    {
                        _GameDriver.MovePlayerDown();
                    }
                    break;
                case VirtualKey.Left:
                    if (_GameDriver.IsGameRunning)
                    {
                        _GameDriver.MovePlayerLeft();
                    }
                    break;
                case VirtualKey.Right:
                    if (_GameDriver.IsGameRunning)
                    {
                        _GameDriver.MovePlayerRight();
                    }
                    break;
                case VirtualKey.P:
                    if (_GameDriver.IsGameRunning)
                    {
                        _GameDriver.PauseGame();
                    }
                    break;
                case VirtualKey.O:
                    if (_GameDriver.IsPaused)
                    {
                        _GameDriver.ResumeGame();
                    }
                    break;
            }
        }
        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            //FreezePlayer();
            // UnFreezePlayer();
            _GameDriver.NewGame();

        }
        private void PauseGameButton_Click(object sender, RoutedEventArgs e)
        {
            //freezing the game here so the player will be able to freeze the game and save it or plan strategy on how to win
            //if he wants to pause he can click p
            _GameDriver.IsPaused = true;
            _GameDriver.StopTimer();
            SetButtonsToPauseState();
        }
        private void ResumeGame(object sender, RoutedEventArgs e)
        {         
            _GameDriver.ResumeGame();
            btnResumeGame.Visibility = Visibility.Collapsed;
            btnPauseGame.Visibility = Visibility.Visible;
        }
        private void SetButtonsToNewGameState()
        {
            btnResumeGame.Visibility = Visibility.Collapsed;
            btnPauseGame.Visibility = Visibility.Collapsed;
            btnLoadFile.Visibility = Visibility.Visible;
            btnSaveFile.Visibility = Visibility.Collapsed;
        }
        private void SetButtonsToPauseState()
        {
            btnStartNewgame.Visibility = Visibility.Visible;
            btnResumeGame.Visibility = Visibility.Visible;
            btnPauseGame.Visibility = Visibility.Collapsed;
            btnSaveFile.Visibility = Visibility.Visible;
            btnLoadFile.Visibility = Visibility.Visible;
        }
        //Save + Load methods
        private void btnSaveFile_Click(object sender, RoutedEventArgs e)
        {
            _GameDriver.SaveFile();
        }

        private void btnLoadFile_Click(object sender, RoutedEventArgs e)
        {
            _GameDriver.LoadFile();
        }

        private async void ShowIntro()
        {
            await NewGameDialogWindow.ShowAsync();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShowIntro();
        }
    }
}
