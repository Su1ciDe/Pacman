using CMP.Scripts.Helper;

namespace CMP.Scripts.AiStates
{
	public class JoiningGameState : GhostState
	{
		public JoiningGameState(GhostBlackboard blackboard) : base(blackboard)
		{
		}

		public override void OnEnter()
		{
		}

		public override void Update()
		{
			if (!UpdateMovement()) return;

			var gridData = GhostBlackboard.GridData;
			if (gridData.GetCellAt(GhostBlackboard.CurrentCell) == CellType.JoinGameCell)
			{
				// start to chase when joining the game if other ghosts seen the pacman
				var nextState = GhostBlackboard.GameManager.GameMode == GameMode.Chase ? GhostStateType.Chase : GhostStateType.Scatter;
				GhostBlackboard.Ghost.ChangeState(nextState);
				return;
			}

			var direction = GridPathfinder.GetFirstStep(gridData, GhostBlackboard.CurrentCell, cell => gridData.GetCellAt(cell) == CellType.JoinGameCell,
				cellType => cellType is not (CellType.Wall or CellType.Invalid));

			if (direction != Direction.None)
			{
				StartMove(direction);
			}
		}
	}
}