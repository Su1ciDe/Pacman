using CMP.Scripts.Helper;

namespace CMP.Scripts.AiStates
{
	public class ChaseState : GhostState
	{
		public ChaseState(GhostBlackboard blackboard) : base(blackboard)
		{
		}

		public override void OnEnter()
		{
		}

		public override void Update()
		{
			if (!UpdateMovement()) return;

			var direction = GhostBlackboard.CurrentDirection;
			if (direction == Direction.None || IsCorner())
			{
				var gridData = GhostBlackboard.GridData;
				var pacmanCell = GhostBlackboard.Pacman.CurrentCell;
				direction = GridPathfinder.GetFirstStep(gridData, GhostBlackboard.CurrentCell, cell => cell == pacmanCell, cellType => cellType.GetIsMovable());
			}

			if (direction != Direction.None && GhostBlackboard.GridData.CanMove(GhostBlackboard.CurrentCell + direction.ToVector2Int()))
			{
				StartMove(direction);
			}
		}

		private bool IsCorner()
		{
			var current = GhostBlackboard.CurrentDirection;
			if (!GhostBlackboard.GridData.CanMove(GhostBlackboard.CurrentCell + current.ToVector2Int()))
				return true;

			var reverse = current.Reverse();
			foreach (var direction in GameSettings.DirectionsToCheck)
			{
				if (direction != current && direction != reverse && GhostBlackboard.GridData.CanMove(GhostBlackboard.CurrentCell + direction.ToVector2Int()))
					return true;
			}

			return false;
		}
	}
}