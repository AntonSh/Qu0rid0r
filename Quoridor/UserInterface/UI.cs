using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms;
using Quoridor.AI;
using Quoridor.GameComponents;
using System;
using System.Drawing;

namespace Quoridor.UserInterface
{
	public partial class UI : Form
	{
		#region Enums
		private enum Direction { Up = 0, Down, Left, Right }
		#endregion

		#region Fields
		private BoardDrawer _boardDrawer = new BoardDrawer();
		private Dictionary<Button, Direction> _controlButtons = new Dictionary<Button, Direction>();
		private Dictionary<Keys, Direction> _controlKeys = new Dictionary<Keys, Direction>();
		private AIPlayer _aiPlayer;

		private bool _fenceSettingMode;
		private Point _fenceTopLeft = new Point(0,0);
		private Point _fenceBottomRight = new Point(1,1);
		private bool _verticalFence;
		#endregion

		#region Constructors
		public UI()
		{
			InitializeComponent();

			// Up and down deliberately switched to be correct in the game.
			_controlButtons[UpButton] = Direction.Down;
			_controlButtons[DownButton] = Direction.Up;
			_controlButtons[LeftButton] = Direction.Left;
			_controlButtons[RightButton] = Direction.Right;


			// Up and down deliberately switched to be correct in the game.
			_controlKeys[Keys.Up] = Direction.Down;
			_controlKeys[Keys.Down] = Direction.Up;
			_controlKeys[Keys.Left] = Direction.Left;
			_controlKeys[Keys.Right] = Direction.Right;

			Player player = GameEngine.Instance.Board.Players.Single(p => p.PlayerId == PlayerId.Second);
			_aiPlayer = new ShortestPathRunner(player, GameEngine.Instance.Board);
		}
		#endregion

		#region Control handlers

		private void UI_MouseUp(object sender, MouseEventArgs e)
		{
			var point = Placeholder.PointToClient(PointToScreen(e.Location));
			
			Square newPosition = _boardDrawer.GetSquareForCoordinates(point.X , point.Y, GameEngine.Instance.Board, Placeholder.Bounds);
			GameEngine.Instance.MoveCurrentPlayer(newPosition);
			Invalidate();
			InvokeAI();
		}

		private void UI_Paint(object sender, PaintEventArgs e)
		{
			_boardDrawer.DrawBoard(GameEngine.Instance.Board, e.Graphics, Placeholder.Bounds);
			_boardDrawer.MarkCurrentPlayer(GameEngine.Instance.CurrentPlayer, GameEngine.Instance.Board.Size, e.Graphics, Placeholder.Bounds);

			if (GameEngine.Instance.GameFinished)
			{
				_boardDrawer.DrawGameOver(GameEngine.Instance.CurrentPlayer, e.Graphics, Placeholder.Bounds);
				return;
			}		

			if (_fenceSettingMode) 
			{
				_boardDrawer.DrawTempFence(_fenceTopLeft.X, _fenceTopLeft.Y, _fenceBottomRight.X, _fenceBottomRight.Y, _verticalFence, GameEngine.Instance.Board.Size, e.Graphics, Placeholder.Bounds);
			}
		}

		private void CursorButtonClick(object sender, EventArgs e)
		{
			if (_fenceSettingMode)
			{
				MoveTemporaryFence(_controlButtons[(Button)sender]);
			}
		}

		private void UI_KeyUp(object sender, KeyEventArgs e)
		{
			if (_controlKeys.ContainsKey(e.KeyCode))
			{
				if (_fenceSettingMode)
				{
					MoveTemporaryFence(_controlKeys[e.KeyCode]);
				}
			}
		}

		private void SetFenceButton_Click(object sender, EventArgs e)
		{
			_fenceSettingMode = !_fenceSettingMode;
			EnableButtons(_fenceSettingMode);

			if (_fenceSettingMode)
			{
				SetFenceButton.Text = "Commit Fence";
			}
			else 
			{
				CommitTemporaryFence();
				SetFenceButton.Text = "Set Fence";

				InvokeAI();
			}

			Invalidate();
		}

		private void EnableButtons(bool state)
		{
			RotateFenceButton.Enabled = state;
			UpButton.Enabled = state;
			DownButton.Enabled = state;
			LeftButton.Enabled = state;
			RightButton.Enabled = state;
		}
		
		private void RotateFenceButton_Click(object sender, EventArgs e)
		{
			_verticalFence = !_verticalFence;

			Invalidate();
		}
		
		#endregion

		#region Private methods (candidates to move to ctrlr class)

		[Obsolete]
		private void MakeMove(Direction direction)
		{
			Square currentPosition = GameEngine.Instance.CurrentPlayer.Position;
			int x = currentPosition.X;
			int y = currentPosition.Y;

			switch (direction)
			{
				case Direction.Up:
					y++;
					break;
				case Direction.Down:
					y--;
					break;
				case Direction.Left:
					x--;
					break;
				case Direction.Right:
					x++;
					break;
				default:
					throw new InvalidOperationException("");
			}

			Square newPosition = currentPosition.Neighbours.Single(n => (n.X == x) && (n.Y == y));

			GameEngine.Instance.MoveCurrentPlayer(newPosition);
			Invalidate();
			InvokeAI();
		}

		private void MoveTemporaryFence(Direction direction)
		{
			switch (direction)
			{
				case Direction.Up:
					_fenceTopLeft.Y++;
					_fenceBottomRight.Y++;
					break;
				case Direction.Down:
					_fenceTopLeft.Y--;
					_fenceBottomRight.Y--;
					break;
				case Direction.Left:
					_fenceTopLeft.X--;
					_fenceBottomRight.X--;
					break;
				case Direction.Right:
					_fenceTopLeft.X++;
					_fenceBottomRight.X++;
					break;
				default:
					break;
			}
		
			Invalidate();
		}

		private void CommitTemporaryFence()
		{
			GameEngine.Instance.SetFence(_fenceTopLeft.X, _fenceTopLeft.Y, _fenceBottomRight.X, _fenceBottomRight.Y, _verticalFence);
		}

		private void InvokeAI() 
		{
			if (!GameEngine.Instance.GameFinished)
			{
				_aiPlayer.DoNextStep();
				Invalidate();
			}
		}

		#endregion

	}
}
