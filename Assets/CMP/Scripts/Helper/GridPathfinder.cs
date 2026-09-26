using System;
using System.Collections.Generic;
using UnityEngine;

namespace CMP.Scripts.Helper
{
	public static class GridPathfinder
	{
		// breadth-first search on the grid. Returns the first direction of the shortest path to the closest target
		public static Direction GetFirstStep(GridData gridData, Vector2Int start, Func<Vector2Int, bool> isTarget, Func<CellType, bool> isWalkable)
		{
			if (isTarget(start)) 
				return Direction.None;

			var firstSteps = new Dictionary<Vector2Int, Direction> { { start, Direction.None } };
			var queue = new Queue<Vector2Int>();
			queue.Enqueue(start);

			while (queue.Count > 0)
			{
				var cell = queue.Dequeue();
				foreach (var direction in GameSettings.DirectionsToCheck)
				{
					var neighbour = cell + direction.ToVector2Int();
					if (firstSteps.ContainsKey(neighbour)) 
						continue;
					if (!isWalkable(gridData.GetCellAtOrDefault(neighbour, CellType.Invalid))) 
						continue;

					var firstStep = cell == start ? direction : firstSteps[cell];
					if (isTarget(neighbour)) 
						return firstStep;

					firstSteps.Add(neighbour, firstStep);
					queue.Enqueue(neighbour);
				}
			}

			return Direction.None;
		}
	}
}