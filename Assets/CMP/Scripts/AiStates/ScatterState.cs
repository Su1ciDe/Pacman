using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.AiStates
{
	public class ScatterState : GhostState
	{
		private readonly List<Direction> options = new List<Direction>();

		public ScatterState(GhostBlackboard blackboard) : base(blackboard)
		{
		}

		public override void OnEnter()
		{
		}

		public override void Update()
		{
			if (HasLineOfSightToPacman())
			{
				GhostBlackboard.GameManager.StartChase();
				return;
			}

			if (!UpdateMovement()) return;

			var reverse = GhostBlackboard.CurrentDirection.Reverse();
			options.Clear();
			foreach (var direction in GameSettings.DirectionsToCheck)
			{
				if (direction != reverse && GhostBlackboard.GridData.CanMove(GhostBlackboard.CurrentCell + direction.ToVector2Int()))
				{
					options.Add(direction);
				}
			}

			// if only possible way is reverse than move reverse 
			if (options.Count == 0 && reverse != Direction.None && GhostBlackboard.GridData.CanMove(GhostBlackboard.CurrentCell + reverse.ToVector2Int()))
			{
				options.Add(reverse);
			}

			if (options.Count > 0)
			{
				StartMove(options[Random.Range(0, options.Count)]);
			}
		}

		// Looks along the ghost's moving direction until a wall or the chase trigger distance
		private bool HasLineOfSightToPacman()
		{
			var direction = GhostBlackboard.CurrentDirection;
			if (direction == Direction.None)
				return false;

			var pacmanCell = GhostBlackboard.Pacman.CurrentCell;
			var cell = GhostBlackboard.CurrentCell;
			for (var i = 0; i <= GameSettings.ChaseTriggerDistance; i++)
			{
				if (cell == pacmanCell)
					return true;

				cell += direction.ToVector2Int();
				if (!GhostBlackboard.GridData.GetCellAtOrDefault(cell, CellType.Invalid).GetIsMovable())
					return false;
			}

			return false;
		}
	}
}