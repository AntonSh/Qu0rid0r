using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quoridor.GameComponents;
using Quoridor.Utilities;

namespace Quoridor.AI
{
	/// <summary>
	/// Simple AI which will just move the token towards the goal by the shortest path.
	/// </summary>
	public class ShortestPathRunner : AIPlayer
	{
		private readonly Board _board;
		private List<BacktrackHelper> _cachedShortestPath = new List<BacktrackHelper>();
		
		public ShortestPathRunner(Player player, Board board) : base (player) 
		{
			_board = board;
		}

		protected override Link GetNextPosition()
		{
			if(! Player.ValidatePath(_board, _cachedShortestPath))
			{
			 	_cachedShortestPath = Player.BuildShortestPath(_board);
			}

			Link nextStep = _cachedShortestPath[0].ByLink;
			_cachedShortestPath.RemoveAt(0);

			return nextStep;
		}
	}
}
