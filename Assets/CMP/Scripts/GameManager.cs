using System.Collections.Generic;
using CMP.Scripts.Helper;
using UnityEngine;

namespace CMP.Scripts
{
	public enum GameMode
	{
		Scatter,
		Chase,
		GameOver,
	}

	public class GameManager : MonoBehaviour
	{
		private Pacman _pacman;
		private InputManager _inputManager;
		private GameMode _gameMode = GameMode.Scatter;
		private readonly List<Ghost> _ghosts = new();

		public GameMode GameMode => _gameMode;

		private void Start()
		{
			var gridData = AssetDatabase.Instance.GridData;
			_pacman = Instantiate(AssetDatabase.Instance.PacmanPrefab);
			_inputManager = Instantiate(AssetDatabase.Instance.InputManagerPrefab);
			_pacman.Init(gridData, _inputManager);
			CreateGhosts(gridData);
			CreateBackground(gridData);
			AdjustCamera(gridData);
		}

		private void Update()
		{
			if (_gameMode == GameMode.GameOver) return;

			foreach (var ghost in _ghosts)
			{
				if (Vector3.Distance(ghost.transform.position, _pacman.transform.position) < GameSettings.CatchDistance)
				{
					GameOver();
					return;
				}
			}
		}

		public void StartChase()
		{
			if (_gameMode != GameMode.Scatter) return;

			_gameMode = GameMode.Chase;
			foreach (var ghost in _ghosts)
			{
				if (ghost.CurrentStateType == GhostStateType.Scatter)
				{
					ghost.ChangeState(GhostStateType.Chase);
				}
			}
		}

		private void GameOver()
		{
			_gameMode = GameMode.GameOver;
			foreach (var ghost in _ghosts)
			{
				ghost.enabled = false;
			}

			_pacman.Fail();
		}

		private void CreateBackground(GridData gridData)
		{
			var targetTexture = MapTextureGenerator.Generate(gridData, AssetDatabase.Instance.MapVisualSettings);
			var textureObject = new GameObject("MapTexture");
			textureObject.transform.position = new Vector3(-0.5f, -0.5f, 0f);
			var targetSprite = Sprite.Create(targetTexture, new Rect(0f, 0f, targetTexture.width, targetTexture.height), Vector2.zero, AssetDatabase.Instance.MapVisualSettings.pixelsPerCell);
			var spriteRenderer = textureObject.AddComponent<SpriteRenderer>();
			spriteRenderer.sprite = targetSprite;
			spriteRenderer.sortingOrder = -1;
		}

		private void AdjustCamera(GridData gridData)
		{
			var mainCamera = Camera.main;
			mainCamera.orthographicSize = gridData.Height + GameSettings.CameraPadding;
			mainCamera.transform.position = new Vector3(gridData.Width / 2f - 0.5f, 0f, -10f);
		}

		private void CreateGhosts(GridData gridData)
		{
			var spawnCells = gridData.GetCoordsOfCellType(CellType.AiSpawnZone);
			for (var i = 0; i < GameSettings.AiCharacterCount; i++)
			{
				if (i + 1 > spawnCells.Count)
					break;

				var ghost = Instantiate(AssetDatabase.Instance.Ghost);
				ghost.Init(gridData, _pacman, this, spawnCells[i], GameSettings.AiJoinDelays[i]);
				_ghosts.Add(ghost);
			}
		}
	}
}