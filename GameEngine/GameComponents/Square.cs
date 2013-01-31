using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// Represents a square on the board.
	/// </summary>
	public class Square
	{
		private readonly int _x;
		private readonly int _y;
		private List<Link> _links = new List<Link>();
		/// <summary>
		/// Create a square with position X and Y.
		/// </summary>
		/// <param name="X">X position.</param>
		/// <param name="Y">Y position.</param>
		public Square(int x, int y)
		{
			_x = x;
			_y = y;
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other">Prototype to copy.</param>
		public Square(Square other)
		{
			_x = other._x;
			_y = other._y;

			throw new NotImplementedException("NYI. need to deep copy links");
		}

		/// <summary>
		/// Adds a link to other adjacent node.
		/// </summary>
		/// <param name="link">The link.</param>
		internal void AddLink(Link link)
		{
			// handle null
			if (!link.Squares.Contains(this)) 
			{
				throw new InvalidOperationException("Link doesn't connect to current square.");
			}

			_links.Add(link);
		}
		
		/// <summary>
		/// X position of the Square.
		/// </summary>
		public int X { get { return _x; } }

		/// <summary>
		/// Y position of the Square.
		/// </summary>
		public int Y { get { return _y; } }

		/// <summary>
		/// Linked squares to this one.
		/// </summary>
		public IEnumerable<Square> Neighbours
		{
			get { return _links.SelectMany(l => l.Squares).Where(s => s != this); }
		}

		/// <summary>
		/// Return links from this square to other squares.
		/// </summary>
		public IEnumerable<Link> Links 
		{
			get { return _links; }
		}
		
		/// <summary>
		/// Returns if two squares are Adjacent to each other.
		/// </summary>
		/// <param name="other">other square.</param>
		public bool IsAdjacentTo(Square other)
		{
			return _links.Any(l => l.Squares.Contains(other));
		}

		/// <summary>
		/// Returns string representation for the Square.
		/// </summary>
		public override string ToString()
		{
			return String.Format("({0},{1})", _x, _y);
		}
	}
}