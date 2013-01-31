using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quoridor.GameComponents;

namespace Quoridor.Utilities
{
	public static class ShortestPathUtility
	{
		public static List<BacktrackHelper> BuildShortestPath(this Player player, Board board)
		{
			Dictionary<Square, BacktrackHelper> visited = new Dictionary<Square, BacktrackHelper>();
			List<BacktrackHelper> frontier = new List<BacktrackHelper>();

			// in this case we don't care which link did we use to reach the position.
			frontier.Add(new BacktrackHelper(player.Position, null));

			BacktrackHelper desitnation = null;
			// Do this in BFS way. Maybe use A* in future.
			while (frontier.Count > 0)
			{
				var current = frontier[0];
				frontier.RemoveAt(0);

				if (player.IsAGoal(current.ToSquare))
				{
					desitnation = current;
					break;
				}

				visited[current.ToSquare] = current;

				var reachableSquares = board.GetLinksFromSquare(current.ToSquare).Select(l => new BacktrackHelper(l.LinkedSquare(current.ToSquare), l));

				// filter ones that are visited or already in frontier.
				reachableSquares = reachableSquares.Where(rs => !visited.ContainsKey(rs.ToSquare) && !frontier.Contains(rs, rs));

				// add the rest of them to frontier.				
				frontier.AddRange(reachableSquares);
			}

			if (desitnation == null)
			{
				throw new InvalidOperationException("Path to the goal is blocked");
			}

			// rewind the shortest path into a list of motions.
			var shortestPath = new List<BacktrackHelper>();
			while (desitnation.ToSquare != player.Position)
			{
				shortestPath.Insert(0, desitnation);
				desitnation = visited[desitnation.ByLink.LinkedSquare(desitnation.ToSquare)];
			}

			return shortestPath;
		}

		public static bool ValidatePath(this Player player, Board board, List<BacktrackHelper> path)
		{
			if (path.Count == 0)
			{
				return false;
			}

			// starting from player current position follow directions and validate:
			// every move is valid
			// we reach destination 

			BacktrackHelper position = new BacktrackHelper(player.Position, null);
			for (int i = 0; i < path.Count; i++)
			{
				var nextPosition = path[i];
				MoveResult moveResult = board.GetMoveResult(position.ToSquare, nextPosition.ByLink);

				if (moveResult != MoveResult.Succesfull)
				{
					return false;
				}

				position = nextPosition;
			}

			return player.IsAGoal(position.ToSquare);
		}
	}

	public class BacktrackHelper : IEqualityComparer<BacktrackHelper>
	{
		private Square _toSquare;
		private Link _byLink;

		public BacktrackHelper(Square toSquare, Link byLink)
		{
			_toSquare = toSquare;
			_byLink = byLink;
		}

		public Link ByLink { get { return _byLink; } }
		public Square ToSquare { get { return _toSquare; } }

		#region IEqualityComparer
		public bool Equals(BacktrackHelper x, BacktrackHelper y)
		{
			// we don't care how did we get to particular square.
			// we only care that the spot is already in frontier
			return x._toSquare == y._toSquare;
		}

		public int GetHashCode(BacktrackHelper obj)
		{
			// It's reasonable to assume that X & Y << than 2 bytes.
			return obj._toSquare.X << 16 + obj._toSquare.Y;
		}
		#endregion
	}
}
