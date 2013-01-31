using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Quoridor.GameComponents;

namespace Quoridor.UserInterface
{
	public class BoardDrawer
	{
		private Pen _gridPen = new Pen(Color.DarkSlateGray, 2F);
		private Pen _fencePen = new Pen(Color.Tomato, 8F);
		private Pen _tempFencePen = new Pen(Color.YellowGreen, 8F);

		private Pen _currentPlayerPen = new Pen(Color.Black,4F);
		private Brush _gameOverBrush = Brushes.White;
		private Pen _gameOverPen = new Pen(Color.Tomato, 8F);
		private Dictionary<PlayerId, Brush> _playerColors = new Dictionary<PlayerId, Brush>();

		internal BoardDrawer() 
		{
			_playerColors[PlayerId.First] = Brushes.Blue;
			_playerColors[PlayerId.Second] = Brushes.Green;
		}

		internal void DrawBoard(Board board, Graphics graphics, Rectangle bounds)
		{
			RectangleF rect = new RectangleF(bounds.X, bounds.Y, bounds.Width, bounds.Height);

			DrawGrid(graphics, board, rect);
			DrawFences(board, graphics, rect);
			DrawPlayers(board, graphics, rect);			
		}

		internal void DrawGameOver(Player player, Graphics graphics, Rectangle bounds)
		{
			RectangleF newRect = new RectangleF(0.25F * bounds.Width + bounds.X, 0.25F * bounds.Height + bounds.Y, 0.5F * bounds.Width, 0.2F * bounds.Height);

			graphics.FillRectangle(_gameOverBrush, newRect);

			string message = String.Format("Game Over!\r\n{0} is a Winner!", player.PlayerId);
			Font drawFont = new Font("Courier New", 20);
			graphics.DrawString(message, drawFont, Brushes.Black, newRect, new StringFormat() { Alignment = StringAlignment.Center });
		}

		internal void DrawTempFence(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, bool vertical, int boardSize, Graphics graphics, Rectangle canvasRectangle)
		{
			if (vertical)
			{
				DrawVerticalFence(topLeftY, topLeftX, graphics, canvasRectangle, boardSize, _tempFencePen);
			}
			else
			{
				DrawHorizontalFence(topLeftY, topLeftX, graphics, canvasRectangle, boardSize, _tempFencePen);
			}
		}

		internal Square GetSquareForCoordinates(int screenX, int screenY, Board board, Rectangle canvasRectangle)
		{
			int pixPerSquareH = canvasRectangle.Width / board.Size;
			int pixPerSquareV = canvasRectangle.Height / board.Size;

			int x = screenX / pixPerSquareH;
			int y = screenY / pixPerSquareV;
			return board.Squares.FirstOrDefault(s => s.X == x && s.Y == y);
		}

		internal void MarkCurrentPlayer(Player player, int boardSize, Graphics graphics, Rectangle canvasRectangle)
		{
			Brush brush = _playerColors[player.PlayerId];
			Square position = player.Position;

			RectangleF squareRectangle = GetSquareRectangleF(canvasRectangle, boardSize, position);
			graphics.DrawEllipse(_currentPlayerPen, squareRectangle);

			var links = GameEngine.Instance.Board.GetLinksFromSquare(position).ToList();

			foreach (var link in links)
			{
				Rectangle tgtRect = GetSquareRectangle(canvasRectangle, boardSize, link.LinkedSquare(position));
				graphics.FillRectangle(Brushes.SeaGreen, tgtRect);
			}
		}



		#region private logic
		private void DrawPlayers(Board board, Graphics graphics, RectangleF canvasRectangle)
		{
			foreach (Player player in board.Players)
			{
				Brush brush = _playerColors[player.PlayerId];
				RectangleF squareRectangle = GetSquareRectangleF(canvasRectangle, board.Size, player.Position);

				graphics.FillEllipse(brush, squareRectangle);
			}
		}

		private void DrawFences(Board board, Graphics graphics, RectangleF canvasRectangle)
		{
			foreach (Fence fence in board.Fences)
			{
				Link link = fence.SplittedLinks.First();
				Square[] squares = fence.SplittedLinks.SelectMany(l => l.Squares).ToArray();
				int leftColumn = squares.Min(s => s.X);
				int topRow = squares.Min(s => s.Y);

				if (link.Squares.First().X != link.Squares.Last().X)
				{
					DrawVerticalFence(topRow, leftColumn, graphics, canvasRectangle, board.Size, _fencePen);
				}
				else
				{
					DrawHorizontalFence(topRow, leftColumn, graphics, canvasRectangle, board.Size, _fencePen);
				}
			}
		}

		private void DrawHorizontalFence(int topRow, int leftColumn, Graphics graphics, RectangleF canvasRectangle, int boardSize, Pen pen)
		{
			float y = GetVerticalPositionForLowerEdge(canvasRectangle, boardSize, topRow);
			float left = GetHorizontalPositionForLeftEdge(canvasRectangle, boardSize, leftColumn);
			float right = GetHorizontalPositionForRightEdge(canvasRectangle, boardSize, leftColumn +1);

			graphics.DrawLine(pen, left, y, right, y);
		}

		private void DrawVerticalFence(int topRow, int leftColumn, Graphics graphics, RectangleF canvasRectangle, int boardSize, Pen pen)
		{
			float x = GetHorizontalPositionForRightEdge(canvasRectangle, boardSize, leftColumn);
			float top = GetVerticalPositionForUpperEdge(canvasRectangle, boardSize, topRow);
			float bottom = GetVerticalPositionForLowerEdge(canvasRectangle, boardSize, topRow + 1);

			graphics.DrawLine(pen, x, top, x, bottom);
		}

		private void DrawGrid(Graphics graphics, Board board, RectangleF canvasRectangle)
		{
			foreach (var square in board.Squares)
			{
				Rectangle rect = GetSquareRectangle(canvasRectangle, board.Size, square);
				
				graphics.DrawRectangle(_gridPen, rect);
			}
		}

		private Rectangle GetSquareRectangle(RectangleF canvasRectangle, int boardSize, Square square) 
		{
			RectangleF rect = GetSquareRectangleF(canvasRectangle, boardSize, square);
			return new Rectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height);
		}

		private RectangleF GetSquareRectangleF(RectangleF canvasRectangle, int boardSize, Square square)
		{
			int row = square.Y;
			int column = square.X;

			float left = GetHorizontalPositionForLeftEdge(canvasRectangle, boardSize, column);
			float width = GetHorizontalPositionForRightEdge(canvasRectangle, boardSize, column) - left;
			float top = GetVerticalPositionForUpperEdge(canvasRectangle, boardSize, row);
			float height = GetVerticalPositionForLowerEdge(canvasRectangle, boardSize, row) - top;

			return new RectangleF(left, top, width, height);
		}

		private float GetHorizontalPositionForLeftEdge(RectangleF canvasRectangle, int boardSize, int columnNumber)
		{
			float left = canvasRectangle.Left;
			float right = canvasRectangle.Right;
			float top = canvasRectangle.Top;
			float bottom = canvasRectangle.Bottom;

			float horisontalStep = canvasRectangle.Width / boardSize;
			float vericalStep = canvasRectangle.Height / boardSize;

			return left + horisontalStep * columnNumber;
		}

		private float GetHorizontalPositionForRightEdge(RectangleF canvasRectangle, int boardSize, int columnNumber) 
		{
			return GetHorizontalPositionForLeftEdge(canvasRectangle, boardSize, columnNumber + 1);
		}

		private float GetVerticalPositionForLowerEdge(RectangleF canvasRectangle, int boardSize, int rowNumber)
		{
			return GetVerticalPositionForUpperEdge(canvasRectangle, boardSize, rowNumber + 1);
		}

		private float GetVerticalPositionForUpperEdge(RectangleF canvasRectangle, int boardSize, int rowNumber)
		{
			float left = canvasRectangle.Left;
			float right = canvasRectangle.Right;
			float top = canvasRectangle.Top;
			float bottom = canvasRectangle.Bottom;

			float horisontalStep = canvasRectangle.Width / boardSize;
			float vericalStep = canvasRectangle.Height / boardSize;

			return top + vericalStep * rowNumber;
		}
		#endregion
	}
}
