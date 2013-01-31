using System;
using System.Linq;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// represents a link with a jump. this one will make path validation much easier.
	/// </summary>
	public class JumpLink : Link
	{
		private readonly Square _via;

		internal JumpLink(Square from, Square via, Square to)
			: base(from, to)
		{
			if (!from.IsAdjacentTo(via) || !to.IsAdjacentTo(via))
			{
				throw new InvalidOperationException("Squares should be adjacent.");
			}

			_via = via;
		}

		public Square From
		{
			get { return Squares.First(); }
		}

		public Square Via
		{
			get { return _via; }
		}

		public Square To
		{
			get { return Squares.Last(); }
		}
	}
}
