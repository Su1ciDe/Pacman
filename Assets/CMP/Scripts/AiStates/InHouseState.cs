using UnityEngine;

namespace CMP.Scripts.AiStates
{
	public class InHouseState : GhostState
	{
		private float timer;

		public InHouseState(GhostBlackboard blackboard) : base(blackboard)
		{
		}

		public override void OnEnter()
		{
			timer = 0f;
			GhostBlackboard.CurrentDirection = Direction.Up;
		}

		public override void Update()
		{
			timer += Time.deltaTime;
			if (timer >= GhostBlackboard.JoinDelay)
			{
				GhostBlackboard.Ghost.ChangeState(GhostStateType.JoiningGame);
				return;
			}

			if (!UpdateMovement()) return;

			var direction = GhostBlackboard.CurrentDirection;
			if (!CanBob(direction))
			{
				direction = direction.Reverse();
			}

			if (CanBob(direction))
			{
				StartMove(direction);
			}
		}

		private bool CanBob(Direction direction)
		{
			var spawnCell = GhostBlackboard.SpawnCell;
			var nextCell = GhostBlackboard.CurrentCell + direction.ToVector2Int();
			if (nextCell.x != spawnCell.x || Mathf.Abs(nextCell.y - spawnCell.y) > 1) 
				return false;

			var cellType = GhostBlackboard.GridData.GetCellAtOrDefault(nextCell, CellType.Invalid);
			return cellType is CellType.AiSpawnZone or CellType.Empty;
		}
	}
}
