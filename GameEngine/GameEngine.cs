using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// Game logic class.
	/// </summary>
	public class GameEngine
	{
		#region Fields
		private static GameEngine _instance = new GameEngine();
		private PlayableBoard _board;
		private PlayerId _currentPlayer;
		#endregion

		#region Constructor
		private GameEngine()
		{
			NewGame(5, 5);

			AddFence(0, 0, 1, 0,
					  0, 1, 1, 1);

			AddFence(2, 0, 2, 1,
					  3, 0, 3, 1);

			AddFence(2, 2, 3, 2,
					  2, 3, 3, 3);

			AddFence(1, 2, 1, 3,
					  2, 2, 2, 3);
		}
		#endregion

		#region Properties
		public static GameEngine Instance
		{
			get { return _instance; }
		}

		public Board Board
		{
			get { return _board; }
		}

		public Player CurrentPlayer
		{
			get { return _board.Players.Single(p => p.PlayerId == _currentPlayer); }
		}

		public bool GameFinished { get; private set; }
		#endregion

		#region Public methods
		/// <summary>
		/// 
		/// </summary>
		/// <param name="newPosition"></param>
		public void MoveCurrentPlayer(Square newPosition)
		{
			if (!GameFinished)
			{
				Link link = _board.GetLinksFromSquare(CurrentPlayer.Position).Single(l => l.Squares.Contains(newPosition));
				_board.MovePlayer(CurrentPlayer, link);

				GameFinished = CheckWinningConditions();
				if (!GameFinished)
				{
					_currentPlayer = NextPlayer();
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="link"></param>
		public void MoveCurrentPlayer(Link link) 
		{
			// check link is not null

			if (!GameFinished)
			{
				if (!link.Squares.Contains(CurrentPlayer.Position)) 
				{
					throw new InvalidOperationException("Invalid link provided.");
				}

				Square target = link.LinkedSquare(CurrentPlayer.Position);
				_board.MovePlayer(CurrentPlayer, link);

				GameFinished = CheckWinningConditions();
				if (!GameFinished)
				{
					_currentPlayer = NextPlayer();
				}
			}
		}

		public void SetFence(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY, bool vertical)
		{
			Square topLeft = _board.Squares.Single(s => s.X == topLeftX && s.Y == topLeftY);
			Square bottomRight = _board.Squares.Single(s => s.X == bottomRightX && s.Y == bottomRightY);

			Link link1;
			Link link2;
			if (vertical)
			{
				link1 = topLeft.Links.Single(l => l.LinkedSquare(topLeft).X == bottomRightX);
				link2 = bottomRight.Links.Single(l => l.LinkedSquare(bottomRight).X == topLeftX);
			}
			else
			{
				link1 = topLeft.Links.Single(l => l.LinkedSquare(topLeft).Y == bottomRightY);
				link2 = bottomRight.Links.Single(l => l.LinkedSquare(bottomRight).Y == topLeftY);
			}

			ValidateFence(link1, link2);
			_board.SetFence(link1, link2);

			_currentPlayer = NextPlayer();
		}
		#endregion

		#region Private methods

		private void ValidateFence(Link link1, Link link2)
		{
			switch (_board.GetFenceSetResult(link1, link2))
			{
				case FenceSetResult.Succesfull:
					break;
				case FenceSetResult.OverlappedWithOtherFence:
					throw new InvalidOperationException("Can not set a fence where another is set already.");
				case FenceSetResult.CrossedAnotherFence:
					throw new InvalidOperationException("Fences couldn't cross each other.");
				case FenceSetResult.BlockedPlayerGoal:
					throw new InvalidOperationException("Players should always be able to reach their destination.");
				default:
					throw new InvalidOperationException("Invalid links provided");
			}
		}

		/// <summary>
		/// Starts a new game.
		/// </summary>
		/// <param name="boardSize"></param>
		/// <param name="fences"></param>
		private void NewGame(int boardSize, int fences)
		{
			_board = new PlayableBoard(boardSize, fences);
			_currentPlayer = PlayerId.First;
			GameFinished = false;
		}

		private PlayerId NextPlayer()
		{
			// Cyclic mechanism to select a player will be needed if more than 2 players will be playing
			return Board.Players.Where(p => p.PlayerId != _currentPlayer).Single().PlayerId;
		}

		[Obsolete("Use only for UI testing")]
		private void AddFence(int sq1x, int sq1y, int sq2x, int sq2y, int sq3x, int sq3y, int sq4x, int sq4y)
		{
			// SQ1 and SQ2 are on the same side of the fence; sq3 & sq4 are on the other
			Square square1 = _board.Squares.Single(s => s.X == sq1x && s.Y == sq1y);
			Square square2 = square1.Neighbours.Single(s => s.X == sq2x && s.Y == sq2y);

			Link link1 = square1.Links.Single(l => l.LinkedSquare(square1).X == sq3x && l.LinkedSquare(square1).Y == sq3y);
			Link link2 = square2.Links.Single(l => l.LinkedSquare(square2).X == sq4x && l.LinkedSquare(square2).Y == sq4y);

			_board.SetFence(link1, link2);
		}

		private bool CheckWinningConditions()
		{
			return Board.Players.Any(p => p.IsInWinningPosition());
		}
		#endregion

		#region Private classes
		/// <summary>
		/// Playable board.
		/// This class shouldn't be passed as it allows modification.
		/// </summary>
		private class PlayableBoard : Board
		{
			/// <summary>
			/// Initializes the empty square board.
			/// </summary>
			/// <param name="size">Length of the board (board will be size*size).</param>
			/// <param name="maxFences">Total number of fences every single player can set.</param>
			internal PlayableBoard(int size, int maxFences)
				: base(size, maxFences)
			{ }

			/// <summary>
			/// Copy constructor.
			/// </summary>
			/// <param name="other">Protorype board to copy from.</param>
			internal PlayableBoard(PlayableBoard other) : base(other) { }

			/// <summary>
			/// Moves player by the link.
			/// </summary>
			internal new void MovePlayer(Player player, Link link)
			{
				base.MovePlayer(player, link);
			}
			
			/// <summary>
			/// Sets fence to separate 2 pairs of squares.
			/// </summary>
			/// <param name="firstSplit">Fist split link.</param>
			/// <param name="secondSplit">Second split link.</param>
			internal new void SetFence(Link firstLink, Link secondLink)
			{
				base.SetFence(firstLink, secondLink);
			}
		}
		#endregion
	}
}