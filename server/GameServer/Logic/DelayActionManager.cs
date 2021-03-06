﻿using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	
	internal class DelayActionManager
	{
		
		public static void AddDelayAction(DelayAction action)
		{
			lock (DelayActionManager.m_Actions)
			{
				DelayActionManager.m_Actions.Add(action);
			}
		}

		
		public static void RemoveDelayAction(DelayAction action)
		{
			lock (DelayActionManager.m_Actions)
			{
				DelayActionManager.m_Actions.Remove(action);
			}
		}

		
		public static void StartAction(DelayAction action)
		{
			DelayActionType nActionID = action.m_DelayActionType;
			DelayActionType delayActionType = nActionID;
			if (delayActionType == DelayActionType.DA_BLINK)
			{
				int nParams = action.m_Params[0];
				GameClient client = action.m_Client;
				int nRadius = nParams * 100;
				GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
				int nDirection = client.ClientData.RoleDirection;
				Point pClientGrid = client.CurrentGrid;
				int nGridNum = nRadius / gameMap.MapGridWidth;
				int nTmp = nGridNum;
				List<Point> lMovePointsList = Global.GetGridPointByDirection(nDirection, (int)pClientGrid.X, (int)pClientGrid.Y, nGridNum);
				byte holdBitSet = 0;
				holdBitSet |= 1;
				holdBitSet |= 2;
				for (int i = 0; i < lMovePointsList.Count; i++)
				{
					if (Global.InObsByGridXY(client.ObjectType, client.ClientData.MapCode, (int)lMovePointsList[i].X, (int)lMovePointsList[i].Y, 0, holdBitSet))
					{
						break;
					}
					nGridNum--;
				}
				if (nGridNum < nTmp)
				{
					pClientGrid = lMovePointsList[nTmp - nGridNum - 1];
				}
				Point canMovePoint = pClientGrid;
				if (!Global.CanQueueMoveObject(client, nDirection, (int)pClientGrid.X, (int)pClientGrid.Y, nGridNum, nGridNum, holdBitSet, out canMovePoint, false))
				{
					Point clientMoveTo = new Point(canMovePoint.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), canMovePoint.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
					GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (int)clientMoveTo.X, (int)clientMoveTo.Y, client.ClientData.RoleDirection, 159, 3);
				}
				else
				{
					Point clientMoveTo = new Point(lMovePointsList[lMovePointsList.Count - 1].X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), lMovePointsList[lMovePointsList.Count - 1].Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
					GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (int)clientMoveTo.X, (int)clientMoveTo.Y, client.ClientData.RoleDirection, 159, 3);
				}
				List<object> objsList = Global.GetAll9Clients(client);
				string strcmd = string.Format("{0}", client.ClientData.RoleID);
				GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, objsList, strcmd, 511);
				DelayActionManager.RemoveDelayAction(action);
			}
		}

		
		public static void HeartBeatDelayAction()
		{
			for (int i = 0; i < DelayActionManager.m_Actions.Count; i++)
			{
				long ticks = TimeUtil.NOW();
				DelayAction tmpInfo = DelayActionManager.m_Actions[i];
				long lStart = tmpInfo.m_StartTime;
				long lDelay = tmpInfo.m_DelayTime;
				if (ticks - lStart > lDelay)
				{
					DelayActionManager.StartAction(tmpInfo);
				}
			}
		}

		
		private static List<DelayAction> m_Actions = new List<DelayAction>();
	}
}
