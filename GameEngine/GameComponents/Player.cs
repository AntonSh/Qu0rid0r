using System.Collections.Generic;
using System.Linq;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// Represents read only game player but not immutable.
	/// </summary>
	public class Player
	{
		private readonly PlayerId _playerId;
		private readonly Square[] _goalPositions;
		private int _fencesAvailable;
		private Square _position;

		/// <summary>
		/// Initializes a player for the game start.
		/// </summary>
		/// <param name="id">Player ID.</param>
		/// <param name="fencesAvailable">How many fences available for the player.</param>
		/// <param name="startPosition">Player start position.</param>
		/// <param name="boardSize">Board size.</param>
		public Player(PlayerId id, int fencesAvailable, Square startPosition, IEnumerable<Square> goalPositions)
		{
			_playerId = id;
			_fencesAvailable = fencesAvailable;
			_position = startPosition;

			_goalPositions = goalPositions.ToArray(); 
		}

		/// <summary>
		/// Player position on the board.
		/// </summary>
		public Square Position { get { return _position; } }

		/// <summary>
		/// Player ID
		/// </summary>
		public PlayerId PlayerId { get { return _playerId; } }

		// check for null
		public bool IsAGoal(Square square)
		{
			return _goalPositions.Contains(square);
		}

		/// <summary>
		/// Checks if player reached winning position.
		/// </summary>
		/// <returns></returns>
		internal bool IsInWinningPosition()
		{
			return IsAGoal(_position);
		}

		/// <summary>
		/// Moves player to the new position 
		/// </summary>
		/// <param name="position">New position</param>
		protected void MoveTo(Square position)
		{
			_position = position;
		}
	}
}
