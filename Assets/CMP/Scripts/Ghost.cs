using System.Collections.Generic;
using CMP.Scripts.AiStates;
using UnityEngine;

namespace CMP.Scripts
{
	public enum GhostStateType
	{
		InHouse,
		JoiningGame,
		Scatter,
		Chase,
	}

	public class Ghost : MonoBehaviour
	{
		public GameObject LeftEye;
		public GameObject RightEye;

		private GhostBlackboard blackboard;
		private Dictionary<GhostStateType, GhostState> states;
		private GhostState currentState;

		public GhostStateType CurrentStateType { get; private set; }

		public void Init(GridData gridData, Pacman pacman, GameManager gameManager, Vector2Int spawnCell, float joinDelay)
		{
			blackboard = new GhostBlackboard
			{
				Ghost = this,
				GridData = gridData,
				Pacman = pacman,
				GameManager = gameManager,
				SpawnCell = spawnCell,
				JoinDelay = joinDelay,
				CurrentCell = spawnCell,
				TargetCell = spawnCell,
			};

			states = new Dictionary<GhostStateType, GhostState>
			{
				{ GhostStateType.InHouse, new InHouseState(blackboard) },
				{ GhostStateType.JoiningGame, new JoiningGameState(blackboard) },
				{ GhostStateType.Scatter, new ScatterState(blackboard) },
				{ GhostStateType.Chase, new ChaseState(blackboard) },
			};

			transform.position = GridData.GetCellPositionAt(spawnCell.x, spawnCell.y);
			ChangeState(GhostStateType.InHouse);
		}

		public void ChangeState(GhostStateType stateType)
		{
			CurrentStateType = stateType;
			currentState = states[stateType];
			currentState.OnEnter();
		}

		public void LookAt(Direction direction)
		{
			var rotation = direction.ToQuaternion();
			LeftEye.transform.localRotation = rotation;
			RightEye.transform.localRotation = rotation;
		}

		private void Update()
		{
			currentState?.Update();
		}
	}
}