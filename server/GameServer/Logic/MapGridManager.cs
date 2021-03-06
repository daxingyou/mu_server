﻿using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Logic
{
	
	public class MapGridManager
	{
		
		
		public Dictionary<int, MapGrid> DictGrids
		{
			get
			{
				return this._DictGrids;
			}
		}

		
		public void InitAddMapGrid(int mapCode, int mapWidth, int mapHeight, int gridWidth, int gridHeight, GameMap gameMap)
		{
			MapGrid mapGrid = new MapGrid(mapCode, mapWidth, mapHeight, gridWidth, gridHeight, gameMap);
			lock (this._DictGrids)
			{
				this._DictGrids.Add(mapCode, mapGrid);
			}
		}

		
		public MapGrid GetMapGrid(int mapCode)
		{
			MapGrid result;
			lock (this._DictGrids)
			{
				MapGrid mapGrid;
				if (this._DictGrids.TryGetValue(mapCode, out mapGrid))
				{
					result = mapGrid;
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		
		public string GetAllMapClientCountForConsole()
		{
			StringBuilder sb = new StringBuilder();
			foreach (KeyValuePair<int, MapGrid> kv in this._DictGrids)
			{
				if (null != kv.Value)
				{
					int count = kv.Value.GetGridClientCountForConsole();
					if (count > 0)
					{
						sb.AppendFormat("{0}:{1}\n", kv.Key, count);
					}
				}
			}
			return sb.ToString();
		}

		
		private Dictionary<int, MapGrid> _DictGrids = new Dictionary<int, MapGrid>(100);
	}
}
