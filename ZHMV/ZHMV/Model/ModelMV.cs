using System;
using System.Collections.Generic;
using System.Text;

namespace ZHMV.Model
{
    class ModelMV
    {
        private ModelTable _table;
        private Int32 _gameStepCount;
        private Int32 _gameTime;
        private Int32 _currentSize;
        private Int32 _foxLife;
        private KeyValuePair<int,int> _foxPosition;
        private KeyValuePair<int, int> _cheesPositions;
        public Int32 GameStepCount { get { return _gameStepCount; } }
        public Int32 GameTime { get { return _gameTime; } }
        public Int32 FoxLife { get { return _foxLife; } }
        public ModelTable Table { get { return _table; } }
        public Boolean IsGameOver { get { return false; } }
        public int CurrentSize { get => _currentSize;}

        public event EventHandler<ModelEventArgs> GameAdvanced;
        public event EventHandler<ModelEventArgs> GameOver;

        public ModelMV()
        {
            _currentSize = 6;
            _table = new ModelTable();
            _foxPosition = new KeyValuePair<int, int>(0, 0);
            _cheesPositions = new KeyValuePair<int, int>(-1,-1);
            
        }
        public void NewGame()
        {
            _table = new ModelTable();
            //_eatenChees = 0;
            _gameTime = 0;
            _foxLife = _table.Size * 3;
            GenerateFields();
            putFoxPosition();
        }

        void putFoxPosition()
        {
            _table.SetValue(0, 0, 1);
        }
        public void NewGame(int size)
        {
            _table = new ModelTable(size);
            _foxLife = size * 3;
            _currentSize = size;
            _gameStepCount = 0;
            _gameTime = 0;
            GenerateFields();
            putFoxPosition();
        }

        public void AdvanceTime()
        {
            if (IsGameOver)
                return;

            if (_gameTime % 2 == 0)
            {
                spoiledCheese();
                makeCheese();
            }
            _gameTime++;

            _foxLife--;
            OnGameAdvanced();

            if (_foxLife == 0) 
                OnGameOver(false);
        }

        private void spoiledCheese()
        {
            if (_cheesPositions.Key == -1) return;
            _table.SetValue(_cheesPositions.Key, _cheesPositions.Value, 3);
            
        }
        private void makeCheese()
        {
            Random rand = new Random();
            int i = rand.Next(0, _table.Size);
            int j = rand.Next(0, _table.Size);
            while (_table.GetValue(i,j) != 0)
            {
                i = rand.Next(0, _table.Size);
                j = rand.Next(0, _table.Size);
            }
            _table.SetValue(i, j, 2);
            _cheesPositions = new KeyValuePair<int, int>(i, j);
        }

        public void Step(Int32 x, Int32 y)
        {
            if (IsGameOver)
                return;

            _table.StepValue(x, y);

            _gameStepCount++;

            OnGameAdvanced();

             if (_foxLife == 0) 
             {
                 OnGameOver(true);
             }
        }

        private void GenerateFields()
        {
            for(Int32 i = 0; i < _table.Size; i++)
            {
                for(Int32 j = 0; j < _table.Size; j++)
                {
                    _table.SetValue(i, j, 0);
                }
            }
        }

        private void OnGameAdvanced()
        {
            if (GameAdvanced != null)
                GameAdvanced(this, new ModelEventArgs(false, _foxLife, _gameTime));
        }
        private void OnGameOver(Boolean isWon)
        {
            if (GameOver != null)
                GameOver(this, new ModelEventArgs(isWon, _foxLife, _gameTime));
        }

        public void MoveFox(int x, int y)
        {
            if (_foxPosition.Key + x < 0 || _foxPosition.Key + x >= _table.Size || _foxPosition.Value + y < 0 || _foxPosition.Value + y >= _table.Size) return;
            else _table.SetValue(_foxPosition.Key, _foxPosition.Value, 0); 
            
            _foxPosition = new KeyValuePair<int, int>(_foxPosition.Key + x, _foxPosition.Value + y);
            if (_table.GetValue(_foxPosition.Key, _foxPosition.Value) == 2)
            {
                _cheesPositions = new KeyValuePair<int, int>(-1, -1);
                _foxLife += 2;
            }
            else if (_table.GetValue(_foxPosition.Key, _foxPosition.Value) == 3) 
            {
                _foxLife -= 3;
                
            }

            _table.SetValue(_foxPosition.Key, _foxPosition.Value, 1);
        }
    }
}
