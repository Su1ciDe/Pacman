using UnityEngine;

namespace CMP.Scripts.AiStates
{
	public class GhostBlackboard
	{
		public Ghost Ghost;
		public GridData GridData;
		public Pacman Pacman;
		public GameManager GameManager;

		public Vector2Int SpawnCell;
		public float JoinDelay;

		public Vector2Int CurrentCell;
		public Vector2Int TargetCell;
		public Direction CurrentDirection = Direction.None;
		public float MoveTimer;
		public bool IsMoving;
	}
}
