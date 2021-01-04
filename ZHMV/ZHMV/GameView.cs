using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZHMV.Model;

namespace ZHMV
{
    public partial class GameView : Form
    {
        private ModelMV _model; 
        private Button[,] _buttonGrid; 
        private Timer _timer; 
        public GameView()
        {
            InitializeComponent();
        }
        private void GameView_Load(object sender, EventArgs e)
        {
            // modell létrehozása és az eseménykezelők társítása
            _model = new ModelMV();
            _model.GameAdvanced += new EventHandler<ModelEventArgs>(Game_GameAdvanced);
            _model.GameOver += new EventHandler<ModelEventArgs>(Game_GameOver);

            // időzítő létrehozása
            _timer = new Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(Timer_Tick);

            // játéktábla és menük inicializálása
            _model.NewGame();
            GenerateTable();
            SetupTable();
            refreshTable();


        }

        private void Game_GameAdvanced(Object sender, ModelEventArgs e)
        {
                _LabelTime.Text = TimeSpan.FromSeconds(e.GameTime).ToString("g");
                _LabelFoxLife.Text = e.FoxLife.ToString();
            
        }

        private void Game_GameOver(Object sender, ModelEventArgs e)
        {
            _timer.Stop();

            foreach (Button button in _buttonGrid) // kikapcsoljuk a gombokat
                button.Enabled = false;

            if (e.IsWon) // győzelemtől függő üzenet megjelenítése
            {
                MessageBox.Show("Gratulálok, győztél!" + Environment.NewLine +
                                "Összesen " + e.FoxLife + " lépést tettél meg és " +
                                TimeSpan.FromSeconds(e.GameTime).ToString("g") + " ideig játszottál.",
                                "Game",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Sajnálom, vesztettél, lejárt az idő!",
                                "Game",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Asterisk);
            }
        }

        private void ButtonGrid_MouseClick(Object sender, MouseEventArgs e)
        {
            // a TabIndex-ből megkapjuk a sort és oszlopot
            Int32 x = ((sender as Button).TabIndex - 100) / _model.Table.Size;
            Int32 y = ((sender as Button).TabIndex - 100) % _model.Table.Size;
            Debug.Write(_model.Table.GetValue(x, y) + "\n");

            //_model.Step(x, y); // lépés a játékban

            // mező frissítése
            if (_model.Table.IsEmpty(x, y))
                _buttonGrid[x, y].Text = String.Empty;
            else
                _buttonGrid[x, y].Text = _model.Table[x, y].ToString();
        }
        private void refreshTable()
        {
            for (Int32 i = 0; i < _model.Table.Size; i++)
            {
                for (Int32 j = 0; j < _model.Table.Size; j++)
                {
                    _buttonGrid[i, j].Text = _model.Table.IsEmpty(i, j)
                        ? String.Empty
                        : _model.Table[i, j].ToString();

                    switch (_model.Table.GetValue(i, j))
                    {
                        case 0:
                            _buttonGrid[i, j].BackColor = Color.White;
                            break;
                        case 1:
                            _buttonGrid[i, j].BackColor = Color.Red;
                            break;
                        case 2:
                            _buttonGrid[i, j].BackColor = Color.Orange;
                            break;
                        case 3:
                            _buttonGrid[i, j].BackColor = Color.Green;
                            break;
                    }
                }
            }

        }
        private void DeleteTable()
        {
            for (Int32 i = 0; i < _model.Table.Size; i++) 
            { 
                for (Int32 j = 0; j < _model.Table.Size; j++)
                {
                    Controls.Remove(_buttonGrid[i, j]);
                }
            }
        }
        private void MenuFileNewGame_Click(Object sender, EventArgs e)
        {
            DeleteTable();
            _model.NewGame(_model.CurrentSize);
            GenerateTable();
            SetupTable();

            _timer.Start();
        }


        private void MenuFileNewGame6_Click(Object sender, EventArgs e)
        {
            DeleteTable();
            _model.NewGame(6);
            GenerateTable();
            SetupTable();

            _timer.Start();
        }

        private void MenuFileNewGame8_Click(Object sender, EventArgs e)
        {
            DeleteTable();
            _model.NewGame(8);
            GenerateTable();
            SetupTable();

            _timer.Start();
        }

        private void MenuFileNewGame10_Click(Object sender, EventArgs e)
        {
            DeleteTable();
            _model.NewGame(10);
            GenerateTable();
            SetupTable();

            _timer.Start();
        }

        private void Timer_Tick(Object sender, EventArgs e)
        {
            _model.AdvanceTime(); // játék léptetése
            refreshTable();
        }


        private void GenerateTable()
        {
            _buttonGrid = new Button[_model.Table.Size, _model.Table.Size];
            for (Int32 i = 0; i < _model.Table.Size; i++)
                for (Int32 j = 0; j < _model.Table.Size; j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Location = new Point(5 + 50 * j, 35 + 50 * i); // elhelyezkedés
                    _buttonGrid[i, j].Size = new Size(50, 50); // méret
                    _buttonGrid[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGrid[i, j].TabIndex = 100 + i * _model.Table.Size + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGrid[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    _buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezelő hozzárendelése minden gombhoz

                    Controls.Add(_buttonGrid[i, j]);
                    // felvesszük az ablakra a gombot
                }

        }


        private void SetupTable()
        {
            for (Int32 i = 0; i < _buttonGrid.GetLength(0); i++)
            {
                for (Int32 j = 0; j < _buttonGrid.GetLength(1); j++)
                {
                    _buttonGrid[i, j].Text = _model.Table.IsEmpty(i, j)
                        ? String.Empty
                        : _model.Table[i, j].ToString();
                    _buttonGrid[i, j].Enabled = true;
                    _buttonGrid[i, j].BackColor = Color.White;

                }
            }

            _LabelTime.Text = TimeSpan.FromSeconds(_model.GameTime).ToString("g");
            _LabelFoxLife.Text = _model.FoxLife.ToString();
            refreshTable();
        }

        private void GameView_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_timer.Enabled) return;
            //Debug.Write(e.KeyCode);
            if(e.KeyCode == Keys.A)
            {
                _model.MoveFox(0, -1);
                //Debug.Write("bal");
            }
            else if(e.KeyCode == Keys.D)
            {
                _model.MoveFox(0, 1);
               // Debug.Write("jobb");
            }
            else if (e.KeyCode == Keys.W)
            {
                //Debug.Write("fel");
                _model.MoveFox(-1, 0);
            }
            else if (e.KeyCode == Keys.S)
            {
                _model.MoveFox(1, 0);
                //Debug.Write("le");
            }
            _LabelTime.Text = TimeSpan.FromSeconds(_model.GameTime).ToString("g");
            _LabelFoxLife.Text = _model.FoxLife.ToString();
            refreshTable();
            
        }

        private void GameView_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void GameView_KeyUp(object sender, KeyEventArgs e)
        {
            

        }
    }
}
