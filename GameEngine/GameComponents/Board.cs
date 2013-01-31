using System;
using System.Collections.Generic;
using System.Linq;
using Quoridor.Utilities;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// Represents a playing board for 1-1 game.
	/// Base class which is read only but not immutable.
	/// </summary>
	public abstract class Board
	{
		#region Fields
		private readonly int _size;
		private readonly int _maxFences;
		private MovablePlayer[] _players = new MovablePlayer[3];

		private Square[] _squares;
		private IReadOnlyCollection<Link> _links;
		private List<Fence> _fences = new List<Fence>();
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes the empty square board.
		/// Original game uses board of 9*9 so odd size is prefered.
		/// </summary>
		/// <param name="size">Length of the board (board will be size*size).</param>
		/// <param name="maxFences">Total number of fences single player can set.</param>
		protected Board(int size, int maxFences)
		{
			_size = size;

			InitializeField(size);
			InitializePlayers(maxFences);

			_maxFences = 2 * maxFences;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other">Board to copy from</param>
		protected Board(Board other)
		{
			throw new NotImplementedException("NYI");
		}
		#endregion

		#region Properties
		/// <summary>
		/// Collection of fences already set in the game.
		/// </summary>
		public IEnumerable<Fence> Fences
		{
			get { return _fences; }
		}

		/// <summary>
		/// Collection of players with their positions.
		/// </summary>
		public IEnumerable<Player> Players
		{
			get { return _players.Skip(1); }
		}

		/// <summary>
		/// Size of the board.
		/// </summary>
		public int Size
		{
			get { return _size; }
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<Square> Squares
		{
			get { return _squares; }
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get all links available from particular square including jump links.
		/// </summary>
		/// <param name="square"></param>
		/// <returns></returns>
		public IEnumerable<Link> GetLinksFromSquare(Square square)
		{
			foreach (Link link in square.Links)
			{
				MoveResult linkMoveResult = GetMoveResult(square, link);

				if (linkMoveResult == MoveResult.Succesfull)
				{
					yield return link;
				}

				if (linkMoveResult == MoveResult.BlockedByPlayer)
				{
					foreach (var jumpLink in GetJumpLinks(square, link))
					{
						yield return jumpLink;
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="from"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public MoveResult GetMoveResult(Square from, Link link)
		{
			JumpLink jumpLink = link as JumpLink;

			if (jumpLink != null) 
			{
				if (GetMoveResult(from, jumpLink.Via) != MoveResult.BlockedByPlayer) 
				{
					return MoveResult.Invalid;
				}

				return GetMoveResult(jumpLink.Via, jumpLink.LinkedSquare(from));
			}

			// Blocked by fence has higher priority than blocked by player so it goes first.
			if (Fences.Any(f => f.SplittedLinks.Contains(link)))
			{
				return MoveResult.BlockedByFence;
			}

			if (IsSquareOccupied(link.LinkedSquare(from)))
			{
				return MoveResult.BlockedByPlayer;
			}

			return MoveResult.Succesfull;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="link1"></param>
		/// <param name="link2"></param>
		/// <returns></returns>
		public FenceSetResult GetFenceSetResult(Link link1, Link link2) 
		{
			// validate if squares are adjacent and no fence available for jump links
			if ((link1 is JumpLink) || (link2 is JumpLink) || !link1.Squares.All(s1 => link2.Squares.Any(s2 => s1.IsAdjacentTo(s2)))) 
			{
				return FenceSetResult.Invalid;
			}

			var allFencesLinks = _fences.SelectMany(f => f.SplittedLinks);
			if (allFencesLinks.Any(l => l == link1 || l == link2))
			{
				return FenceSetResult.OverlappedWithOtherFence;
			}

			// validate that fence is not crossing another fence (situation when all 4 squares match)
			if (_fences.Any(f => FencesCross(f, link1, link2)))
			{
				return FenceSetResult.CrossedAnotherFence;
			}

			// validate that the path to goal exists. For that move shortest path logic to the board.
			if (!Players.All(p => CanReachDestination(p)))
			{
				return FenceSetResult.BlockedPlayerGoal;
			}

			return FenceSetResult.Succesfull;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="square"></param>
		/// <returns></returns>
		public bool IsSquareOccupied(Square square)
		{
			return Players.Any(p => p.Position == square);
		}
		#endregion

		#region Protected Methods
		
		/// <summary>
		/// Moves player in by the link. 
		/// </summary>
		/// <param name="player"></param>
		/// <param name="link"></param>
		protected void MovePlayer(Player player, Link link) 
		{
			Square currentPosition = player.Position;

			// Validate current -> desired is not bloced by fence and isn't occupied
			if (GetMoveResult(currentPosition, link) != MoveResult.Succesfull)
			{
				throw new InvalidOperationException("Can not jump over the fence or step on the other player.");
			}

			Square newPosition = link.LinkedSquare(currentPosition);

			((MovablePlayer)player).MoveTo(newPosition);
		}

		/// <summary>
		/// Sets fence to separate 2 pairs of squares.
		/// </summary>
		/// <param name="firstSplit">Fist pair of split squares.</param>
		/// <param name="secondSplit">Second pair of split squares.</param>
		protected void SetFence(Link first, Link second)
		{
			if (_fences.Count == _maxFences)
			{
				throw new InvalidOperationException("All fences already used.");
			}

			// Check for NULL

			//TODO: Make sure that the fence is not splitting existing fence into 2 pieces
			// for this check if the square is not crossed already by existing fences

			_fences.Add(new Fence(first, second));
		}
		#endregion

		#region Private methods

		private bool CanReachDestination(Player p)
		{
			try
			{
				p.BuildShortestPath(this);
				return true;
			}
			catch (InvalidOperationException e)
			{
				if (e.Message == "Path to the goal is blocked")
				{
					return false;
				}

				throw;
			}
		}

		private bool FencesCross(Fence f, Link link1, Link link2)
		{
			HashSet<Square> squares = new HashSet<Square>(f.SplittedLinks.SelectMany(link => link.Squares).Concat(link1.Squares).Concat(link2.Squares));
			return squares.Count == 4;
		}





		// Assumes the linked square is ocupied by player
		private IEnumerable<Link> GetJumpLinks(Square square, Link link)
		{
			// Jump links are used in the case one player stands in front of the other.
			// Jump links aren't presisted as they become stale quickly
			// Jumps can be made over a single player.
			// In case strait jump not possible because of the fences diagonal jumps can be made if aren't blocked by fences.

			Square target = link.LinkedSquare(square);
			Link straitJump = GetStraitJumpLink(square, link);

			if (straitJump != null)
			{
				yield return straitJump;
				yield break;
			}

			// handle diagonal jumps
			foreach (var l in GetDiagonalJumpLinks(square, link))
			{
				yield return l;
			}
		}

		private IEnumerable<Link> GetDiagonalJumpLinks(Square square, Link link)
		{
			Square target = link.LinkedSquare(square);
		
			int deltaX = target.X - square.X;
			int deltaY = target.Y - square.Y;
			int[] jumpDirection = { -1, 1 };

			foreach (int direction in jumpDirection)
			{
				int jumpX = deltaX == 0 ? target.X + direction : target.X;
				int jumpY = deltaY == 0 ? target.Y + direction : target.Y;
				Square diagonalJump = target.Neighbours.FirstOrDefault(s => s.X == jumpX && s.Y == jumpY);

				if (diagonalJump == null)
				{
					continue;
				}

				MoveResult fromTarget = GetMoveResult(target, diagonalJump);

				switch (fromTarget)
				{
					case MoveResult.Succesfull:
						yield return new JumpLink(square, target, diagonalJump);
						break;
					case MoveResult.BlockedByFence:
					case MoveResult.BlockedByPlayer:
						break;
					default:
						throw new InvalidOperationException("Unexpected move result.");
				}
			}
		}

		private Link GetStraitJumpLink(Square square, Link link)
		{
			Square target = link.LinkedSquare(square);

			// Check strait jump first			
			int jumpX = target.X + (target.X - square.X);
			int jumpY = target.Y + (target.Y - square.Y);
			Square straitJump = target.Neighbours.FirstOrDefault(s => s.X == jumpX && s.Y == jumpY);

			if (straitJump == null)
			{
				return null;
			}

			MoveResult fromTarget = GetMoveResult(target, straitJump);

			switch (fromTarget)
			{
				case MoveResult.Succesfull:
					return new JumpLink(square, target, straitJump);
				case MoveResult.BlockedByFence:
				case MoveResult.BlockedByPlayer:
					return null;				
				default:
					throw new InvalidOperationException("Unexpected move result.");
			}
		}

		private MoveResult GetMoveResult(Square from, Square to)
		{
			if (from.IsAdjacentTo(to))
			{
				return GetMoveResult(from, from.Links.Single(l => l.LinkedSquare(from) == to));
			}
			
			throw new InvalidOperationException("Jump cases should go via links.");
		}

		private void InitializePlayers(int maxFences)
		{
			int middle = _size / 2;

			var firstPosition = _squares[GetIndexFromPosition(middle, _size - 1)];
			var firstGoals = Enumerable.Range(0, _size).Select(i => _squares[GetIndexFromPosition(i, 0)]);

			_players[(int)PlayerId.First] = new MovablePlayer(PlayerId.First, maxFences, firstPosition, firstGoals);

			var secondPosition = _squares[GetIndexFromPosition(middle, 0)];
			var secondGoals = Enumerable.Range(0, _size).Select(i => _squares[GetIndexFromPosition(i, _size - 1)]);

			_players[(int)PlayerId.Second] = new MovablePlayer(PlayerId.Second, maxFences, secondPosition, secondGoals);
		}

		private void InitializeField(int size)
		{
			_squares = new Square[size * size];

			var links = new List<Link>(2 * size * (size - 1));

			for (int y = 0; y < _size; y++)
			{
				for (int x = 0; x < _size; x++)
				{
					int position = GetIndexFromPosition(x, y);
					_squares[position] = new Square(x, y);

					if (x > 0)
					{
						// Add horizontal link
						int neibourPosition = GetIndexFromPosition((x - 1), y);
						Link link = new Link(_squares[position], _squares[neibourPosition]);
						links.Add(link);
						_squares[position].AddLink(link);
						_squares[neibourPosition].AddLink(link);
					}

					if (y > 0)
					{
						// Add vertical link
						int neibourPosition = GetIndexFromPosition(x, y - 1);
						Link link = new Link(_squares[position], _squares[neibourPosition]);
						links.Add(link);
						_squares[position].AddLink(link);
						_squares[neibourPosition].AddLink(link);
					}
				}
			}

			_links = links;
		}

		private int GetIndexFromPosition(int x, int y)
		{
			return x * _size + y;
		}
		#endregion

		#region Private Classes
		/// <summary>
		/// Represents mutable game player.
		/// </summary>
		private class MovablePlayer : Player
		{
			/// <summary>
			/// Initializes a player for the game start.
			/// </summary>
			/// <param name="id">Player ID.</param>
			/// <param name="fencesAvailable">How many fences available for the player.</param>
			/// <param name="startPosition">Player start position.</param>
			/// <param name="boardSize">Board size.</param>
			public MovablePlayer(PlayerId id, int fencesAvailable, Square startPosition, IEnumerable<Square> goalPositions)
				: base(id, fencesAvailable, startPosition, goalPositions)
			{ }

			/// <summary>
			/// Moves player to the new position 
			/// </summary>
			/// <param name="position">New position</param>
			public new void MoveTo(Square position)
			{
				base.MoveTo(position);
			}
		}
		#endregion
	}
}
