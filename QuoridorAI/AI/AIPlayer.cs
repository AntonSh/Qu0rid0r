using System;
using Quoridor.GameComponents;

namespace Quoridor.AI
{
	/// <summary>
	/// Abstraction for AI player.
	/// </summary>
	public abstract class AIPlayer
	{
		private readonly Player _player;

		public AIPlayer(Player player)
		{
			_player = player;
		}

		public Player Player
		{
			get { return _player; }
		}

		// Does a step for computer player.
		public void DoNextStep()
		{
			if (GameEngine.Instance.CurrentPlayer != _player)
			{
				throw new InvalidOperationException("AI called for the wrong player.");
			}

			GameEngine.Instance.MoveCurrentPlayer(GetNextPosition());
		}

		protected abstract Link GetNextPosition();
	}
}
