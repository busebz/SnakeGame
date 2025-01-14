using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeGame
{
    public partial class SnakeGame : Form
    {
        
        private int _snakePieceSpace = 2;
        private int _snakePieceNumber;
        private int _snakeSize = 20;
        private int _baitSize = 20;
        private Random _random;
        private Label _snakeHead;
        private Label _bait;
        private moveDirect _direct;
        private int _elapsedSeconds = 0;
        private int _elapsedMinutes = 0;
        private Timer _gameTimer;
        private Timer _moveTimer;
        public SnakeGame()
        {
            InitializeComponent();
            _random = new Random();
            _gameTimer = new Timer(); 
            _gameTimer.Interval = 1000; 
            _gameTimer.Tick += GameTimer_Tick; 

            timerMove = new Timer(); //For game time
            timerMove.Interval = 1000; 
            timerMove.Tick += timerMove_Tick; 

            _moveTimer = new Timer(); //For snake speed
            _moveTimer.Interval = 100; 
            _moveTimer.Tick += timerMove_Tick; //
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _snakePieceNumber = 0;
            MakeBait();
            PlaceBait();
            PlaceSnake();
            _direct = moveDirect.Right;
            _gameTimer.Start(); // Start the game timer when the form loads
            _moveTimer.Start(); // Start the move timer when the form loads
        }

        private void StartAgain()
        {
            this.panel.Controls.Clear();
            _snakePieceNumber = 0;
            MakeBait();
            PlaceBait();
            PlaceSnake();
            lblScore.Text = "0";
            _direct = moveDirect.Right;
            _elapsedSeconds = 0; // Reset seconds
            _elapsedMinutes = 0; // Reset minutes
            lblTime.Text = "00:00"; // Set initial time text immediately
            _gameTimer.Start(); // Start the game timer
            _moveTimer.Start(); // Start the move timer again
        }

        private Label MakeSnakePiece(int x, int y)
        {
            Label newPiece = new Label
            {
                Size = new Size(_snakeSize, _snakeSize),
                Location = new Point(x, y),
                BackColor = Color.Red
            };
            panel.Controls.Add(newPiece);  
            return newPiece;  
        }


        private void Reset()
        {
            MakeBait();
            PlaceBait();
        }

        private void PlaceSnake()
        {
            _snakeHead = MakeSnakePiece(0, 0);
            var locationx = (panel.Width /2) - (_snakeHead.Width /2);
            var locationy = (panel.Height / 2) - (_snakeHead.Height / 2);
            _snakeHead.Location = new Point(locationx,locationy);
        }

        private void MakeBait()
        {
            Label lbl = new Label()
            {
                Name = "bait",
                BackColor = Color.Yellow,
                Width = _baitSize,
                Height = _baitSize,
                
            };
            _bait = lbl;
            this.panel.Controls.Add(lbl);
        }

        private void PlaceBait()
        {
            var locationx = 0;
            var locationy = 0;

            bool durum;

            do
            {
                durum = false;
                locationx = _random.Next(0, panel.Width - _baitSize);
                locationy = _random.Next(0, panel.Height - _baitSize);           
                var rect1 = new Rectangle(new Point(locationx, locationy), _bait.Size);

                foreach (Control control in panel.Controls)
                {
                    if (control is Label && control.Name.Contains("snakePiece"))
                    {
                        var rect2 = new Rectangle(control.Location, control.Size);

                        if (rect1.IntersectsWith(rect2))
                        {
                            durum = true;
                            break;
                        }
                    }

                }


            } while (durum);

            _bait.Location = new Point(locationx, locationy);

        }

        private enum moveDirect
        {
            Up,
            Down,
            Right,
            Left
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {

            // Update the time display
            _elapsedSeconds++;
            if (_elapsedSeconds == 60)
            {
                _elapsedSeconds = 0;
                _elapsedMinutes++;
            }

            string formattedTime = string.Format("Time: {0:D2}:{1:D2}", _elapsedMinutes, _elapsedSeconds);
            lblTime.Text = formattedTime; // Update the time label
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var keyCode = e.KeyCode;

            if (_direct == moveDirect.Up && keyCode == Keys.S ||
                _direct == moveDirect.Down && keyCode == Keys.W ||
                _direct == moveDirect.Left && keyCode == Keys.D ||
                _direct == moveDirect.Right && keyCode == Keys.A)
            {
                return;
            }

            switch (keyCode)
            {
                case Keys.W:
                    _direct = moveDirect.Up;
                    break;
                case Keys.D:
                    _direct = moveDirect.Right;
                    break;
                case Keys.A:
                    _direct = moveDirect.Left;
                    break;
                case Keys.S:
                    _direct = moveDirect.Down;
                    break;
                default:
                    break;
            };
        }

        private void timerMove_Tick(object sender, EventArgs e)
        {
            
            MoveSnake();
            FollowTheSnakePiece();
            IsSnakeEatTheBait();
            IsGameOver();
        }

        private void MoveSnake()
        {
            var locationx = _snakeHead.Location.X;
            var locationy = _snakeHead.Location.Y;

            // Snakes first movement
            switch (_direct)
            {
                case moveDirect.Up:
                    _snakeHead.Location = new Point(locationx, locationy - (_snakeHead.Width + _snakePieceSpace));
                    break;
                case moveDirect.Down:
                    _snakeHead.Location = new Point(locationx, locationy + (_snakeHead.Width + _snakePieceSpace));
                    break;
                case moveDirect.Left:
                    _snakeHead.Location = new Point(locationx - (_snakeHead.Width + _snakePieceSpace), locationy);
                    break;
                case moveDirect.Right:
                    _snakeHead.Location = new Point(locationx + (_snakeHead.Width + _snakePieceSpace), locationy);
                    break;
                default:
                    break;
            }
        }

        private void IsSnakeEatTheBait()
        {
            var rect1 = new Rectangle(_snakeHead.Location, _snakeHead.Size);
            var rect2 = new Rectangle(_bait.Location, _bait.Size);

            // Snake ate the bait
            if (rect1.IntersectsWith(rect2))
            {
                lblScore.Text = (Convert.ToInt32(lblScore.Text) + 10).ToString();  
                PlaceBait();  // Place the bait randomly

                if (_snakePieceNumber == 0)
                {
                    _snakePieceNumber = 1;
                }
                else
                {
                    // If snake eat the first bait, add
                    _snakePieceNumber++;
                }

                if (_snakePieceNumber == 1) return;  

                var lastSnakePiece = (Label)panel.Controls[panel.Controls.Count - 1];  // Last snake piece
                int newX = lastSnakePiece.Location.X;
                int newY = lastSnakePiece.Location.Y;

                // Son parçanın tam ters yönünde yeni bir parça ekleyelim
                switch (_direct)
                {
                    case moveDirect.Up:
                        newY += _snakeSize + _snakePieceSpace;
                        break;
                    case moveDirect.Down:
                        newY -= _snakeSize + _snakePieceSpace;
                        break;
                    case moveDirect.Left:
                        newX += _snakeSize + _snakePieceSpace;
                        break;
                    case moveDirect.Right:
                        newX -= _snakeSize + _snakePieceSpace;
                        break;
                }

                MakeSnakePiece(newX, newY);
            }
        }

        private void FollowTheSnakePiece()
        {
            // The snakes every piece follows the previous one
            if (_snakePieceNumber <= 1) return;  

            for (int i = _snakePieceNumber; i > 1; i--)
            {
                var nextPiece = (Label)panel.Controls[i];
                var previousPiece = (Label)panel.Controls[i - 1];

                nextPiece.Location = previousPiece.Location;
            }
        }

        private void IsGameOver()
        {
            var rect1 = new Rectangle(_snakeHead.Location, _snakeHead.Size);

            bool isWallCollision = _snakeHead.Location.X < 0 ||
                                   _snakeHead.Location.Y < 0 ||
                                   _snakeHead.Location.X + _snakeHead.Width > panel.Width ||
                                   _snakeHead.Location.Y + _snakeHead.Height > panel.Height;

            bool isSelfCollision = panel.Controls.OfType<Label>()
                                                  .Any(c => c.Name.Contains("snakePiece") && c != _snakeHead &&
                                                            rect1.IntersectsWith(new Rectangle(c.Location, c.Size)));

            if (isWallCollision || isSelfCollision)
            {
                _gameTimer.Stop();
                _moveTimer.Stop();

                var gameOverLabel = new Label
                {
                    Text = $"Game Over!\nScore: {lblScore.Text}\n{lblTime.Text}\nPress R to Restart",
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Black,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };

                panel.Controls.Add(gameOverLabel);

                // 'R' button starts the game again
                this.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.R)
                    {
                        panel.Controls.Clear();
                        StartAgain();
                    }
                };
            }
        }
    }
}
