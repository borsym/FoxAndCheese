using System;
using System.Collections.Generic;
using System.Text;

namespace ZHMV.Model
{
    class ModelEventArgs : EventArgs
    {
        private Int32 _gameTime;
        private Int32 _foxLife;
        private Boolean _isWon;

        public Int32 GameTime { get { return _gameTime; } }
        public Int32 FoxLife { get { return _foxLife; } }
        public Boolean IsWon { get { return _isWon; } }
        public ModelEventArgs(Boolean isWon, Int32 foxLife, Int32 gameTime)
        {
            _isWon = isWon;
            _foxLife = foxLife;
            _gameTime = gameTime;
        }
    }
}
