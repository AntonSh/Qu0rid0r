namespace Quoridor.GameComponents
{
	public enum PlayerId : int
	{
		Uninitialized = 0,
		First,
		Second,
	}

	public enum MoveResult
	{
		Invalid = 0,
		Succesfull,
		BlockedByFence,
		BlockedByPlayer,
	}

	public enum FenceSetResult
	{
		Invalid = 0,
		Succesfull,
		OverlappedWithOtherFence,
		CrossedAnotherFence,
		BlockedPlayerGoal,
	}


}
