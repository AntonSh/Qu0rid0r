using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// represents a link from one square to another.
	/// </summary>
	public class Link
	{
		private readonly Square[] _squares = new Square[2];

		private Link() { }

		internal Link(Square square1, Square square2)
		{	
			_squares[0] = square1;
			_squares[1] = square2;
		}

		public IEnumerable<Square> Squares { get { return _squares; } 
		}

		public Square LinkedSquare(Square square) 
		{
			return _squares.Single(s => s != square);
		}

		public override string ToString()
		{
			return String.Format("[{0}<->{1}]", _squares[0], _squares[1]);
		}
	}
}
