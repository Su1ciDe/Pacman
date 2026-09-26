using UnityEngine;

namespace CMP.Scripts.AiStates
{
	public abstract class GhostState
	{
		protected GhostBlackboard GhostBlackboard;

		public GhostState(GhostBlackboard blackboard)
		{
			GhostBlackboard = blackboard;
		}

		public abstract void OnEnter();
		public abstract void Update();

		protected virtual bool UpdateMovement()
		{
			var bb = GhostBlackboard;
			if (!bb.IsMoving)
			{
				bb.MoveTimer = 0f;
				return true;
			}

			bb.MoveTimer += Time.deltaTime;
			var t = Mathf.Clamp01(bb.MoveTimer / GameSettings.AiMovementDuration);
			bb.Ghost.transform.position = Vector3.Lerp(GridData.GetCellPositionAt(bb.CurrentCell.x, bb.CurrentCell.y), GridData.GetCellPositionAt(bb.TargetCell.x, bb.TargetCell.y), t);
			if (t < 1f)
			{
				return false;
			}

			bb.CurrentCell = bb.TargetCell;
			bb.IsMoving = false;
			bb.MoveTimer -= GameSettings.AiMovementDuration;
			return true;
		}

		protected void StartMove(Direction direction)
		{
			var bb = GhostBlackboard;
			bb.CurrentDirection = direction;
			bb.TargetCell = bb.CurrentCell + direction.ToVector2Int();
			bb.IsMoving = true;
			bb.Ghost.LookAt(direction);
		}
	}
}