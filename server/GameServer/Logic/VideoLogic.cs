﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Video;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	
	public class VideoLogic : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		
		public static VideoLogic getInstance()
		{
			return VideoLogic.instance;
		}

		
		public bool initialize()
		{
			this.LoadVideoXml();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1402, 1, 1, VideoLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1403, 2, 2, VideoLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(28, VideoLogic.getInstance());
			GlobalEventSource.getInstance().registerListener(12, VideoLogic.getInstance());
			return true;
		}

		
		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(28, VideoLogic.getInstance());
			GlobalEventSource.getInstance().removeListener(12, VideoLogic.getInstance());
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 12)
			{
				PlayerLogoutEventObject e = eventObject as PlayerLogoutEventObject;
				if (null != e)
				{
					GameClient client = e.getPlayer();
					this.HandlePlayerLogout(client);
				}
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject e2 = eventObject as OnStartPlayGameEventObject;
				if (null != e2)
				{
					GameClient client = e2.Client;
					this.HandleStartPlayGame(client);
				}
			}
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1402:
				result = this.ProcessGuanZhanRoleMiniDataCmd(client, nID, bytes, cmdParams);
				break;
			case 1403:
				result = this.ProcessGuanZhanTrackingCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		
		public void LoadVideoXml()
		{
			lock (this.Mutex)
			{
				this.VideoList.Clear();
				string fileName = Global.GameResPath("Config/Viedo.xml");
				XElement xml = ConfigHelper.Load(fileName);
				if (null == xml)
				{
					LogManager.WriteLog(LogTypes.Fatal, "未找到配置文件:" + fileName, null, true);
				}
				else
				{
					IEnumerable<XElement> xmlItems = xml.Elements();
					foreach (XElement xmlItem in xmlItems)
					{
						VideoData data = new VideoData();
						data.TalkID = Convert.ToInt32(Global.GetSafeAttributeStr(xmlItem, "TalkID"));
						data.MinZhuanSheng = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "MinZhuanSheng"));
						data.MinLevel = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "MinLevel"));
						data.MaxZhuanSheng = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "MaxZhuanSheng"));
						data.MaxLevel = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "MaxLevel"));
						data.MinVip = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "MinVip"));
						data.MaxVip = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "MaxVip"));
						data.PassWord = Global.GetSafeAttributeStr(xmlItem, "PassWord");
						data.ZhuanshengSift = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "ZhuanshengSift"));
						data.LevelSift = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "LevelSift"));
						data.VIPSift = (int)Convert.ToByte(Global.GetSafeAttributeStr(xmlItem, "VIPSift"));
						this.VideoList.Add(data);
					}
					this.VideoList = (from x in this.VideoList
					orderby x.MinVip descending
					select x).ToList<VideoData>();
				}
				this.VideoGMDict.Clear();
				fileName = Global.IsolateResPath("Config/GuanZhanList.xml");
				xml = ConfigHelper.Load(fileName);
				if (null != xml)
				{
					foreach (XElement xmlItem in xml.Elements())
					{
						this.VideoGMDict[Global.GetSafeAttributeStr(xmlItem, "UID")] = (int)ConfigHelper.GetElementAttributeValueLong(xmlItem, "ZoneID", 0L);
					}
				}
				else
				{
					LogManager.WriteLog(LogTypes.Fatal, "未找到配置文件:" + fileName, null, true);
				}
				this.ChuanSongDict.Clear();
				fileName = Global.IsolateResPath("Config/GuanZhanTransfer.xml");
				xml = ConfigHelper.Load(fileName);
				if (null != xml)
				{
					foreach (XElement xmlItem in xml.Elements())
					{
						ChuanSongItem data2 = new ChuanSongItem();
						data2.ID = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
						data2.MapCode = (int)Global.GetSafeAttributeLong(xmlItem, "MapCode");
						List<int> arr = Global.StringToIntList(Global.GetSafeAttributeStr(xmlItem, "Site"), '|');
						data2.PosX = arr[0];
						data2.PosY = arr[1];
						this.ChuanSongDict.Add(data2.ID, data2);
					}
				}
				else
				{
					LogManager.WriteLog(LogTypes.Fatal, "未找到配置文件:" + fileName, null, true);
				}
			}
		}

		
		public TCPProcessCmdResults ProcessOpenVideoCmd(TMSKSocket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string cmdData = null;
			try
			{
				cmdData = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("解析指令字符串错误, CMD={0}", (TCPGameServerCmds)nID), null, true);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 1700);
				return TCPProcessCmdResults.RESULT_DATA;
			}
			try
			{
				string[] fields = cmdData.Split(new char[]
				{
					':'
				});
				if (fields.Length != 1)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("指令参数个数错误, CMD={0}, Recv={1}, CmdData={2}", (TCPGameServerCmds)nID, fields.Length, cmdData), null, true);
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "0", 1700);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int roleID = Convert.ToInt32(fields[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				VideoData roomData = this.GetVideoRoomData(client);
				if (roomData == null)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, "", nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int filterStatus = this.GetPlayerFilterStatus(client, roomData);
				string strcmd = string.Format("{0}:{1}:{2}", roomData.TalkID, roomData.PassWord, filterStatus);
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, nID);
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		
		private VideoData GetVideoRoomData(GameClient client)
		{
			foreach (VideoData videoData in this.VideoList)
			{
				if (client.ClientData.VipLevel >= videoData.MinVip && client.ClientData.VipLevel <= videoData.MaxVip && client.ClientData.Level + client.ClientData.ChangeLifeCount * 100 <= videoData.MaxLevel + videoData.MaxZhuanSheng * 100 && client.ClientData.Level + client.ClientData.ChangeLifeCount * 100 >= videoData.MinLevel + videoData.MinZhuanSheng * 100)
				{
					return videoData;
				}
			}
			return null;
		}

		
		private int GetPlayerFilterStatus(GameClient client, VideoData data)
		{
			return (client.ClientData.Level >= data.LevelSift || client.ClientData.VipLevel >= data.VIPSift || client.ClientData.ChangeLifeCount >= data.ZhuanshengSift) ? 1 : 0;
		}

		
		public int GetOrSendPlayerVideoStatus(GameClient client, List<int> RoleCommonUseIntPamams = null)
		{
			int status = (this.GetVideoRoomData(client) == null) ? 0 : 1;
			if (RoleCommonUseIntPamams != null && RoleCommonUseIntPamams.Count >= 36 && RoleCommonUseIntPamams[36] == 0 && status == 1)
			{
				client.ClientData.RoleCommonUseIntPamams[36] = 1;
				GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.VideoButton, status);
			}
			return status;
		}

		
		public bool IsGuanZhanGM(GameClient client)
		{
			lock (this.Mutex)
			{
				int zoneId;
				if (this.VideoGMDict.TryGetValue(client.strUserID, out zoneId) && (zoneId <= 0 || zoneId == client.ClientData.ZoneID))
				{
					return true;
				}
			}
			return false;
		}

		
		public bool GetGuanZhanPos(int mapCode, ref int posX, ref int posY)
		{
			ChuanSongItem item = null;
			lock (this.Mutex)
			{
				foreach (ChuanSongItem v in this.ChuanSongDict.Values)
				{
					if (v.MapCode == mapCode)
					{
						item = v;
						if (v.PosX == posX && v.PosY == posY)
						{
							return true;
						}
					}
				}
				if (null != item)
				{
					posX = item.PosX;
					posY = item.PosY;
				}
			}
			return item != null;
		}

		
		public TCPProcessCmdResults ProcessGuanZhanMoveToCmd(TMSKSocket socket, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			try
			{
				SpriteMoveData moveData = DataHelper.BytesToObject<SpriteMoveData>(data, 0, count);
				if (null == moveData)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("ProcessOpenVideoCmd解析客户端数据失败!", new object[0]), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int roleID = moveData.roleID;
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), roleID), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				if (client.ClientData.HideGM == 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("帐号{0}无观战权限,禁止传送!", client.strUserID), null, true);
					return TCPProcessCmdResults.RESULT_OK;
				}
				int mapCode = moveData.mapCode;
				int toX = moveData.toX;
				int toY = moveData.toY;
				if (Global.GetMapSceneType(mapCode) != Global.GetMapSceneType(client.ClientData.MapCode))
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("必须进入观战地图,才可传送在本活动关联地图范围内传送!", client.strUserID), null, true);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GameMap gameMap;
				if (GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
				{
					if (mapCode != client.ClientData.MapCode)
					{
						client.ClientData.WaitingChangeMapToMapCode = mapCode;
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, mapCode, toX, toY, -1, 0);
					}
					else
					{
						GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, toX, toY, -1);
					}
				}
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(socket), false, false);
			}
			return TCPProcessCmdResults.RESULT_FAILED;
		}

		
		public bool ProcessGuanZhanRoleMiniDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					return true;
				}
				if (client.ClientData.HideGM == 0)
				{
					return true;
				}
				int posX = 0;
				int posY = 0;
				if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref posX, ref posY))
				{
					return true;
				}
				SceneUIClasses sceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				GuanZhanData gzData = new GuanZhanData();
				if (SceneUIClasses.BangHuiMatch == sceneType)
				{
					BangHuiMatchManager.getInstance().FillGuanZhanData(client, gzData);
				}
				if (SceneUIClasses.LangHunLingYu == sceneType)
				{
					LangHunLingYuManager.getInstance().FillGuanZhanData(client, gzData);
				}
				if (SceneUIClasses.EscapeBattle == sceneType)
				{
					EscapeBattleManager.getInstance().FillGuanZhanData(client, gzData);
				}
				client.sendCmd<GuanZhanData>(nID, gzData, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		public bool ProcessGuanZhanTrackingCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = Convert.ToInt32(cmdParams[0]);
				int beTrackRID = Convert.ToInt32(cmdParams[1]);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref roleID))
				{
					return true;
				}
				lock (this.Mutex)
				{
					if (client.ClientData.HideGM == 0 || roleID == beTrackRID)
					{
						return true;
					}
					int posX = 0;
					int posY = 0;
					if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref posX, ref posY))
					{
						int result = -21;
						client.sendCmd(nID, string.Format("{0}:{1}", result, -1), false);
						return true;
					}
					if (-1 == beTrackRID)
					{
						this.CancleTracking(client, true);
						return true;
					}
					GameClient beTrackingClient = GameManager.ClientMgr.FindClient(beTrackRID);
					if (beTrackingClient == null || beTrackingClient.ClientData.HideGM > 0 || beTrackingClient.ClientData.WaitingNotifyChangeMap || beTrackingClient.ClientData.WaitingForChangeMap)
					{
						int result = -21;
						client.sendCmd(nID, string.Format("{0}:{1}", result, -1), false);
						return true;
					}
					SceneUIClasses srcSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					SceneUIClasses tarSceneType = Global.GetMapSceneType(beTrackingClient.ClientData.MapCode);
					if (srcSceneType != tarSceneType)
					{
						int result = -21;
						client.sendCmd(nID, string.Format("{0}:{1}", result, -1), false);
						return true;
					}
					this.Tracking(client, beTrackingClient);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private void Tracking(GameClient tClient, GameClient beTClient)
		{
			try
			{
				lock (this.Mutex)
				{
					this.CancleTracking(tClient, true);
					if (tClient.ClientData.MapCode != beTClient.ClientData.MapCode)
					{
						GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), tClient, -1, beTClient.ClientData.MapCode, beTClient.ClientData.PosX, beTClient.ClientData.PosY, beTClient.ClientData.RoleDirection, 123);
					}
					else
					{
						GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, tClient, beTClient.ClientData.PosX, beTClient.ClientData.PosY, -1);
					}
					tClient.ClientData.BeTrackingRoleID = beTClient.ClientData.RoleID;
					if (!beTClient.ClientData.TrackingRoleIDList.Exists((int x) => x == tClient.ClientData.RoleID))
					{
						beTClient.ClientData.TrackingRoleIDList.Add(tClient.ClientData.RoleID);
					}
					int result = 0;
					tClient.sendCmd(1403, string.Format("{0}:{1}", result, beTClient.ClientData.RoleID), false);
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		public void CancleTracking(GameClient client, bool notify = true)
		{
			try
			{
				if (null != client)
				{
					lock (this.Mutex)
					{
						if (0 != client.ClientData.BeTrackingRoleID)
						{
							GameClient beTClient = GameManager.ClientMgr.FindClient(client.ClientData.BeTrackingRoleID);
							if (null != beTClient)
							{
								beTClient.ClientData.TrackingRoleIDList.RemoveAll((int x) => x == client.ClientData.RoleID);
							}
							client.ClientData.BeTrackingRoleID = 0;
							if (notify)
							{
								int result = 0;
								client.sendCmd(1403, string.Format("{0}:{1}", result, -1), false);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		public void TryTrackingOther(GameClient tClient, GameClient lostClient)
		{
			try
			{
				SceneUIClasses sceneType = Global.GetMapSceneType(lostClient.ClientData.MapCode);
				GuanZhanData gzData = new GuanZhanData();
				if (SceneUIClasses.BangHuiMatch == sceneType)
				{
					BangHuiMatchManager.getInstance().FillGuanZhanData(lostClient, gzData);
				}
				if (SceneUIClasses.EscapeBattle == sceneType)
				{
					EscapeBattleManager.getInstance().FillGuanZhanData(lostClient, gzData);
				}
				List<GuanZhanRoleMiniData> roleList = null;
				if (gzData.RoleMiniDataDict.TryGetValue(lostClient.ClientData.BattleWhichSide, out roleList))
				{
					foreach (GuanZhanRoleMiniData r in roleList)
					{
						if (r.RoleID != lostClient.ClientData.RoleID)
						{
							GameClient beTrackingClient = GameManager.ClientMgr.FindClient(r.RoleID);
							if (null != beTrackingClient)
							{
								this.Tracking(tClient, beTrackingClient);
								return;
							}
						}
					}
				}
				gzData.RoleMiniDataDict.Remove(lostClient.ClientData.BattleWhichSide);
				foreach (List<GuanZhanRoleMiniData> rolelist in gzData.RoleMiniDataDict.Values)
				{
					foreach (GuanZhanRoleMiniData r in roleList)
					{
						if (r.RoleID != lostClient.ClientData.RoleID)
						{
							GameClient beTrackingClient = GameManager.ClientMgr.FindClient(r.RoleID);
							if (null != beTrackingClient)
							{
								this.Tracking(tClient, beTrackingClient);
								return;
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		public void ClientRelive(GameClient client)
		{
			try
			{
				lock (this.Mutex)
				{
					if (client.ClientData.TrackingRoleIDList.Count != 0)
					{
						bool toGuanZhanMap = true;
						int posX = 0;
						int posY = 0;
						if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref posX, ref posY))
						{
							toGuanZhanMap = false;
						}
						List<int> tempTrackingRoleIDList = new List<int>(client.ClientData.TrackingRoleIDList);
						foreach (int rid in tempTrackingRoleIDList)
						{
							GameClient tClient = GameManager.ClientMgr.FindClient(rid);
							if (null != tClient)
							{
								if (!toGuanZhanMap)
								{
									this.CancleTracking(client, true);
								}
								else
								{
									this.Tracking(tClient, client);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private void HandleStartPlayGame(GameClient client)
		{
			try
			{
				lock (this.Mutex)
				{
					if (client.ClientData.BeTrackingRoleID != 0 || client.ClientData.TrackingRoleIDList.Count != 0)
					{
						bool toGuanZhanMap = true;
						int posX = 0;
						int posY = 0;
						if (!this.GetGuanZhanPos(client.ClientData.MapCode, ref posX, ref posY))
						{
							toGuanZhanMap = false;
						}
						if (0 != client.ClientData.BeTrackingRoleID)
						{
							if (!toGuanZhanMap)
							{
								this.CancleTracking(client, true);
							}
							else
							{
								ClientManager.DoSpriteMapGridMove(client, 0);
								GameClient beTClient = GameManager.ClientMgr.FindClient(client.ClientData.BeTrackingRoleID);
								if (null != beTClient)
								{
									int result = 0;
									client.sendCmd(1403, string.Format("{0}:{1}", result, beTClient.ClientData.RoleID), false);
								}
							}
						}
						List<int> tempTrackingRoleIDList = new List<int>(client.ClientData.TrackingRoleIDList);
						foreach (int rid in tempTrackingRoleIDList)
						{
							GameClient tClient = GameManager.ClientMgr.FindClient(rid);
							if (null != tClient)
							{
								if (!toGuanZhanMap)
								{
									this.CancleTracking(tClient, true);
								}
								else if (tClient.ClientData.MapCode != client.ClientData.MapCode)
								{
									GameManager.ClientMgr.ChangeMap(TCPManager.getInstance().MySocketListener, TCPOutPacketPool.getInstance(), tClient, -1, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection, 123);
								}
								else
								{
									GameManager.ClientMgr.NotifyOthersGoBack(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, tClient, client.ClientData.PosX, client.ClientData.PosY, -1);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private void HandlePlayerLogout(GameClient client)
		{
			try
			{
				lock (this.Mutex)
				{
					if (client.ClientData.BeTrackingRoleID != 0 || client.ClientData.TrackingRoleIDList.Count != 0)
					{
						if (0 != client.ClientData.BeTrackingRoleID)
						{
							this.CancleTracking(client, true);
						}
						List<int> tempTrackingRoleIDList = new List<int>(client.ClientData.TrackingRoleIDList);
						foreach (int rid in tempTrackingRoleIDList)
						{
							GameClient tClient = GameManager.ClientMgr.FindClient(rid);
							if (null != tClient)
							{
								this.CancleTracking(tClient, true);
								this.TryTrackingOther(tClient, client);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, "", false, false);
			}
		}

		
		private static VideoLogic instance = new VideoLogic();

		
		public object Mutex = new object();

		
		private List<VideoData> VideoList = new List<VideoData>();

		
		private Dictionary<string, int> VideoGMDict = new Dictionary<string, int>();

		
		private Dictionary<int, ChuanSongItem> ChuanSongDict = new Dictionary<int, ChuanSongItem>();
	}
}
