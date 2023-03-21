using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace GameV2.Classes
{
    class SaveDataSchema   //model to create json file
    {
        public int _EnemieStartTop { get; set; }
        public int _EnemieStartLeft { get; set; }
        public int _IdEnemie { get; set; }
        public int _PlayerStartTop { get; set; }
        public int _PlayerStartLeft { get; set; }
        public int _LifeIsLeft { get; set; }
        public int Score { get; set; }
        public SaveDataSchema(int EnemieStartTop, int EnemieStartLeft, int idEnemy, int PlayerStartTop, int PlayerStartLeft, int LifeIsLeft, int score)
        {
            //enemy
            this._EnemieStartTop = EnemieStartTop;
            this._EnemieStartLeft = EnemieStartLeft;
            this._IdEnemie = idEnemy;
            //player
            this._PlayerStartTop = PlayerStartTop;
            this._PlayerStartLeft = PlayerStartLeft;
            this._LifeIsLeft = LifeIsLeft;
            this.Score = score;
        }
    }
}



