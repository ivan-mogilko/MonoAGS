﻿using System;
using System.Linq;
using AGS.API;
using System.Collections.Generic;

namespace AGS.Engine
{
	public class EPPathFinder : IPathFinder
	{
		private bool[][] _mask;

		public EPPathFinder()
		{
			AllowEndNodeUnwalkable = false;
			CrossCorner = true;
			CrossAdjacentPoint = false;
			SmoothPath = true;
			Heuristics = HeuristicMode.EUCLIDEAN;
		}

		public bool AllowEndNodeUnwalkable { get; set; }
		public bool CrossCorner { get; set; }
		public bool CrossAdjacentPoint { get; set; }
		public bool SmoothPath { get; set; }
		public HeuristicMode Heuristics { get; set; }
		public bool UseRecursive { get; set; }

		public void Init(bool[][] mask)
		{
			this._mask = mask;
		}

		public IEnumerable<ILocation> GetWalkPoints(ILocation from, ILocation to)
		{
			if (_mask == null || _mask [0] == null)
				return new List<ILocation> ();
			var grid = new StaticGrid (_mask.Length, _mask[0].Length, _mask);
			return getWalkPoints(grid, from, to);
		}

		private IEnumerable<ILocation> getWalkPoints(StaticGrid grid, ILocation from, ILocation to)
		{
			JumpPointParam input = new JumpPointParam (grid, getPos(from), getPos(to), AllowEndNodeUnwalkable, CrossCorner, CrossAdjacentPoint, Heuristics) { UseRecursive = UseRecursive };
			var cells = JumpPointFinder.FindPath (input);
			if (!SmoothPath) cells = JumpPointFinder.GetFullPath(cells);
			return cells.Select (c => getLocation (c, to.Z));
		}

		private GridPos getPos(ILocation location)
		{
			return new GridPos ((int)location.X, (int)location.Y);
		}

		private ILocation getLocation(GridPos pos, float z)
		{
			return new AGSLocation (pos.x, pos.y, z);
		}
	}
}

