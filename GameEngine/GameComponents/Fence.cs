using System;
using System.Collections.Generic;
using System.Linq;

namespace Quoridor.GameComponents
{
	/// <summary>
	/// Represents a fence that splits 2 pair of squares.
	/// </summary>
	public class Fence
	{
		private readonly Link[] _splittedLinks = new Link[2];

		/// <summary>
		/// Initializes the Fence that splits 4 adjacent squares that from a square into 2 pairs.
		/// </summary>
		/// <param name="link1"></param>
		/// <param name="link2"></param>
		public Fence(Link link1, Link link2)
		{
			// Check if they are null
			if (link1.Squares.Any(s1 => link2.Squares.All(s2 => !s1.IsAdjacentTo(s2))))
			{
				throw new InvalidOperationException("Squares should be adjacent");
			}

			_splittedLinks[0] = link1;
			_splittedLinks[1] = link2;
		}

		/// <summary>
		/// 
		/// </summary>
		public IEnumerable<Link> SplittedLinks
		{
			get { return _splittedLinks; }
		}
		
		/// <summary>
		/// Returns string representation for the fence.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("-{0}-{1}-", _splittedLinks[0], _splittedLinks[1]);
		}
	}
}
