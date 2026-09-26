using UnityEngine;

namespace CMP.Scripts
{
	public class Pacman : MonoBehaviour
	{
		public Animator Animator;
		private const string FailAnimationName = "FailAnimation";

		private GridData gridData;
		private InputManager inputManager;

		private Vector2Int currentCell;
		private Vector2Int targetCell;
		private Direction currentDirection = Direction.None;
		private Direction queuedDirection = Direction.None;
		private float moveTimer;
		private bool isMoving;

		public Vector2Int CurrentCell => currentCell;
		public Direction CurrentDirection => currentDirection;

		public void Init(GridData _gridData, InputManager _inputManager)
		{
			gridData = _gridData;
			inputManager = _inputManager;
			var startCell = gridData.GetCoordsOfCellType(CellType.Pacman)[0];
			currentCell = startCell;
			targetCell = startCell;
			transform.position = GridData.GetCellPositionAt(startCell.x, startCell.y);
		}

		private void Update()
		{
			if (!gridData) return;

			var input = inputManager.ConsumeInput();
			if (input != Direction.None)
			{
				queuedDirection = input;
			}

			if (isMoving)
			{
				// if player inputs the opposite direction, action is instantaneous
				if (queuedDirection == currentDirection.Reverse())
				{
					(currentCell, targetCell) = (targetCell, currentCell);
					currentDirection = queuedDirection;
					queuedDirection = Direction.None;
					moveTimer = GameSettings.PacmanMovementDuration - moveTimer;
					transform.rotation = currentDirection.ToQuaternion();
				}

				moveTimer += Time.deltaTime;
				var t = Mathf.Clamp01(moveTimer / GameSettings.PacmanMovementDuration);
				transform.position = Vector3.Lerp(GridData.GetCellPositionAt(currentCell.x, currentCell.y), GridData.GetCellPositionAt(targetCell.x, targetCell.y), t);
				if (t < 1f)
				{
					return;
				}

				currentCell = targetCell;
				isMoving = false;
				moveTimer -= GameSettings.PacmanMovementDuration;
			}

			TryStartNextMove();
		}

		private void TryStartNextMove()
		{
			if (queuedDirection != Direction.None && CanMove(queuedDirection))
			{
				currentDirection = queuedDirection;
				queuedDirection = Direction.None;
			}

			if (currentDirection == Direction.None || !CanMove(currentDirection))
			{
				moveTimer = 0f;
				return;
			}

			targetCell = currentCell + currentDirection.ToVector2Int();
			isMoving = true;
			transform.rotation = currentDirection.ToQuaternion();
		}

		public void Fail()
		{
			enabled = false;
			Animator.Play(FailAnimationName);
		}

		private bool CanMove(Direction direction)
		{
			var nextCell = currentCell + direction.ToVector2Int();
			return gridData.GetCellAtOrDefault(nextCell, CellType.Invalid).GetIsMovable();
		}
	}
}