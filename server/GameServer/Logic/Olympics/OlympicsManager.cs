﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace GameServer.Logic.Olympics
{
	
	public class OlympicsManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		
		public static OlympicsManager getInstance()
		{
			return OlympicsManager.instance;
		}

		
		public bool initialize()
		{
			this.InitOlympics();
			return true;
		}

		
		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1050, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1051, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1059, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1052, 2, 2, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1053, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1054, 2, 2, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1055, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1056, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1057, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1058, 2, 2, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1061, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1060, 1, 1, OlympicsManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			return true;
		}

		
		public bool showdown()
		{
			return true;
		}

		
		public bool destroy()
		{
			return true;
		}

		
		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		
		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1050:
				return this.ProcessOlympicsGradeCmd(client, nID, bytes, cmdParams);
			case 1051:
				return this.ProcessOlympicsGameCountCmd(client, nID, bytes, cmdParams);
			case 1052:
				return this.ProcessOlympicsGameOperateCmd(client, nID, bytes, cmdParams);
			case 1054:
				return this.ProcessOlympicsGuessSubCmd(client, nID, bytes, cmdParams);
			case 1055:
				return this.ProcessOlympicsGuessListCmd(client, nID, bytes, cmdParams);
			case 1056:
				return this.ProcessOlympicsRankCmd(client, nID, bytes, cmdParams);
			case 1057:
				return this.ProcessOlympicsShopListCmd(client, nID, bytes, cmdParams);
			case 1058:
				return this.ProcessOlympicsShopBuyCmd(client, nID, bytes, cmdParams);
			case 1059:
				return this.ProcessOlympicsGameBeginCmd(client, nID, bytes, cmdParams);
			case 1060:
				return this.ProcessOlympicsAwardStateCmd(client, nID, bytes, cmdParams);
			case 1061:
				return this.ProcessOlympicsAwardCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		
		public void processEvent(EventObjectEx eventObject)
		{
			throw new NotImplementedException();
		}

		
		private bool ProcessOlympicsGradeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, "0:0", false);
					return true;
				}
				string result = this.OlympicsGradeGet(client);
				client.sendCmd(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsGameCountCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, "0:0", false);
					return true;
				}
				lock (this._mutex)
				{
					this.JudgeClearOlympicsActivityData(client);
					int dayID = this.GetOlympicsDay();
					int[] shootArr = this.OlympicsGameCountGet(client, EGameType.Shoot);
					if (shootArr[0] != dayID)
					{
						int[] array = new int[5];
						array[0] = dayID;
						shootArr = array;
						this.OlympicsGameCountSet(client, EGameType.Shoot, string.Join<int>(",", shootArr));
					}
					int[] ballArr = this.OlympicsGameCountGet(client, EGameType.Football);
					if (ballArr[0] != dayID)
					{
						int[] array = new int[5];
						array[0] = dayID;
						ballArr = array;
						this.OlympicsGameCountSet(client, EGameType.Football, string.Join<int>(",", ballArr));
					}
					int shootCount = shootArr[1];
					if (shootArr[4] > 0)
					{
						shootCount--;
					}
					int ballCount = ballArr[1];
					if (ballArr[4] > 0)
					{
						ballCount--;
					}
					client.sendCmd(1051, string.Format("{0}:{1}", shootCount, ballCount), false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsGameBeginCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				lock (this._mutex)
				{
					int gameType = int.Parse(cmdParams[0]);
					string result = this.OlympicsGameBegin(client, (EGameType)gameType);
					client.sendCmd(nID, result, false);
				}
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private string OlympicsGameBegin(GameClient client, EGameType gameType)
		{
			string result = "{0}:{1}:{2}:{3}:{4}";
			string result2;
			if (!this._olympicsIsOpen)
			{
				result2 = string.Format(result, new object[]
				{
					-1,
					(int)gameType,
					0,
					0,
					0
				});
			}
			else
			{
				int[] oldArr = this.OlympicsGameCountGet(client, gameType);
				OlympicsGameInfo gameInfo = null;
				this._gameDic.TryGetValue((int)gameType, out gameInfo);
				if (gameInfo == null)
				{
					result2 = string.Format(result, new object[]
					{
						-6,
						(int)gameType,
						0,
						0,
						0
					});
				}
				else if (oldArr[4] > 0 && oldArr[2] >= 0 && oldArr[2] < gameInfo.CountGame)
				{
					result2 = string.Format(result, new object[]
					{
						1,
						(int)gameType,
						oldArr[1],
						oldArr[2],
						oldArr[3]
					});
				}
				else if (oldArr[1] >= gameInfo.CountFree + gameInfo.CountDiamond)
				{
					result2 = string.Format(result, new object[]
					{
						-7,
						(int)gameType,
						0,
						0,
						0
					});
				}
				else
				{
					if (oldArr[1] >= gameInfo.CountFree)
					{
						int priceNeed = gameInfo.DiamondList[oldArr[1] - gameInfo.CountFree];
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, priceNeed, "奥运次数", true, false, false, DaiBiSySType.None))
						{
							return string.Format(result, new object[]
							{
								-8,
								(int)gameType,
								0,
								0,
								0
							});
						}
					}
					oldArr[1]++;
					oldArr[2] = 0;
					oldArr[3] = 0;
					oldArr[4] = 1;
					this.OlympicsGameCountSet(client, gameType, string.Join<int>(",", oldArr));
					result2 = string.Format(result, new object[]
					{
						1,
						(int)gameType,
						oldArr[1],
						oldArr[2],
						oldArr[3]
					});
				}
			}
			return result2;
		}

		
		private bool ProcessOlympicsGameOperateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			string result = "{0}:{1}:{2}";
			try
			{
				lock (this._mutex)
				{
					if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
					{
						return false;
					}
					int gameType = int.Parse(cmdParams[0]);
					int value = int.Parse(cmdParams[1]);
					string beginStr = this.OlympicsGameBegin(client, (EGameType)gameType);
					int[] beginArr = StringUtil.StringToIntArr(beginStr, ':');
					if (beginArr[0] != 1)
					{
						client.sendCmd(nID, string.Format(result, beginArr[0], gameType, 0), false);
						return true;
					}
					int[] oldArr = this.OlympicsGameCountGet(client, (EGameType)gameType);
					if (oldArr[4] == 0)
					{
						client.sendCmd(nID, string.Format(result, 0, gameType, 0), false);
						return true;
					}
					int dayID = this.GetOlympicsDay();
					if (dayID != oldArr[0])
					{
						oldArr[0] = dayID;
						oldArr[1] = 1;
					}
					oldArr[2]++;
					int resultValue;
					if (gameType == 1)
					{
						if (value < 0 || value > 10)
						{
							client.sendCmd(nID, string.Format(result, 0, gameType, 0), false);
							return true;
						}
						oldArr[3] += value;
						resultValue = oldArr[3];
					}
					else
					{
						int resultBall = 0;
						int random = RandomHelper.GetRandomNumber(0, 100);
						int rate = (int)(this.GetFootballRate() * 100.0);
						if (random < rate)
						{
							resultBall = 1;
						}
						oldArr[3] += resultBall;
						resultValue = resultBall;
					}
					OlympicsGameInfo info = this._gameDic[gameType];
					if (oldArr[2] >= info.CountGame)
					{
						int winState = (oldArr[3] >= info.CountWin) ? 1 : 0;
						int score = oldArr[3];
						int grade = (winState > 0) ? info.GradeWin : info.GradeLost;
						oldArr[2] = 0;
						oldArr[3] = 0;
						oldArr[4] = 0;
						this.OlympicsGradeAdd(client, grade);
						client.sendCmd(1053, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							gameType,
							winState,
							score,
							grade
						}), false);
						this.CheckTip(client);
					}
					this.OlympicsGameCountSet(client, (EGameType)gameType, string.Join<int>(",", oldArr));
					int[] shootArr = this.OlympicsGameCountGet(client, EGameType.Shoot);
					int[] ballArr = this.OlympicsGameCountGet(client, EGameType.Football);
					int shootCount = shootArr[1];
					if (shootArr[4] > 0)
					{
						shootCount--;
					}
					int ballCount = ballArr[1];
					if (ballArr[4] > 0)
					{
						ballCount--;
					}
					client.sendCmd(1051, string.Format("{0}:{1}", shootCount, ballCount), false);
					client.sendCmd(nID, string.Format(result, 1, gameType, resultValue), false);
					return true;
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsGuessSubCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<int>(nID, -1, false);
					return true;
				}
				string todayAnswer = cmdParams[0];
				int day = int.Parse(cmdParams[1]);
				int[] arr = StringUtil.StringToIntArr(todayAnswer, ',');
				if (arr.Length != 3)
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				if (day != this.GetOlympicsDayFromBegin())
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				int dayID = this.GetOlympicsDay();
				OlympicsGuessDataDB oldData = this.DBOlympicsGuess(client, dayID);
				if (oldData.A1 != -1 || oldData.A2 != -1 || oldData.A3 != -1)
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				oldData.A1 = arr[0];
				oldData.A2 = arr[1];
				oldData.A3 = arr[2];
				if (!this.DBOlympicsGuessUpdate(client, oldData))
				{
					client.sendCmd<int>(nID, 0, false);
					return true;
				}
				this.CheckTip(client);
				client.sendCmd<int>(nID, 1, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsGuessListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<OlympicsGuessDataResult>(nID, new OlympicsGuessDataResult(), false);
					return true;
				}
				int type = int.Parse(cmdParams[0]);
				int day = this.GetOlympicsDay();
				if (type == 2)
				{
					day--;
				}
				if (day <= 0)
				{
					client.sendCmd<OlympicsGuessDataResult>(nID, null, false);
					return true;
				}
				OlympicsGuessDataDB answerData = this.DBOlympicsGuess(client, day);
				List<OlympicsGuessData> list = this.GetOlympicsGuessList(answerData);
				client.sendCmd<OlympicsGuessDataResult>(nID, new OlympicsGuessDataResult
				{
					Type = type,
					List = list,
					DayID = this.TransformOffsetDayToFromBegin(day)
				}, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsRankCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<List<KFRankData>>(nID, new List<KFRankData>(), false);
					return true;
				}
				List<KFRankData> listCmd = null;
				KFRankData myData = null;
				List<KFRankData> list = AllyClient.getInstance().HRankTopList(1);
				if (list != null && list.Count > 0)
				{
					myData = list.Find((KFRankData _x) => _x != null && _x.RoleID == client.ClientData.RoleID);
				}
				if (myData == null)
				{
					myData = AllyClient.getInstance().HRankData(1, client.ClientData.RoleID);
					if (myData != null)
					{
						listCmd = new List<KFRankData>(list);
						listCmd.Add(myData);
					}
				}
				if (null != listCmd)
				{
					client.sendCmd<List<KFRankData>>(nID, listCmd, false);
					return true;
				}
				client.sendCmd<List<KFRankData>>(nID, list, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsShopListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				if (!this._olympicsIsOpen)
				{
					client.sendCmd<List<OlympicsShopData>>(nID, new List<OlympicsShopData>(), false);
					return true;
				}
				int dayID = this.GetOlympicsDay();
				Dictionary<int, int> myCountDic = this.OlympicsShopCountGet(client, dayID);
				List<OlympicsShopData> list = this.GetOlympicsShopList(dayID, myCountDic);
				client.sendCmd<List<OlympicsShopData>>(nID, list, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsShopBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result2;
			lock (this._mutex)
			{
				string result = "{0}:{1}:{2}:{3}";
				try
				{
					if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
					{
						return false;
					}
					int id = int.Parse(cmdParams[0]);
					int count = int.Parse(cmdParams[1]);
					if (!this._olympicsIsOpen)
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							0,
							id,
							0,
							0
						}), false);
						return true;
					}
					OlympicsShopData shopData = this.GetOlympicsShopData(id);
					if (shopData == null)
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							0,
							id,
							0,
							0
						}), false);
						return true;
					}
					int dayID = this.GetOlympicsDay();
					if (shopData.DayID != dayID)
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							0,
							id,
							0,
							0
						}), false);
						return true;
					}
					int myCount = 0;
					Dictionary<int, int> myCountDic = this.OlympicsShopCountGet(client, dayID);
					myCountDic.TryGetValue(id, out myCount);
					if (myCount + count > shopData.NumSingle)
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							-2,
							id,
							0,
							0
						}), false);
						return true;
					}
					int allCount = 0;
					this._shopCountDic.TryGetValue(id, out allCount);
					if (allCount + count > shopData.NumFull)
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							-3,
							id,
							0,
							0
						}), false);
						return true;
					}
					string gradeStr = this.OlympicsGradeGet(client);
					int[] gradeArr = StringUtil.StringToIntArr(gradeStr, ':');
					int needGrade = shopData.Price * count;
					if (gradeArr[1] < needGrade)
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							-4,
							id,
							0,
							0
						}), false);
						return true;
					}
					if (!Global.CanAddGoodsNum(client, count))
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							-5,
							id,
							0,
							0
						}), false);
						return true;
					}
					if (!this._shopCountDic.ContainsKey(id))
					{
						this._shopCountDic.Add(id, count);
					}
					else
					{
						Dictionary<int, int> dictionary;
						int key;
						(dictionary = this._shopCountDic)[key = id] = dictionary[key] + count;
					}
					if (!this.DBOlympicsShopUpdate(dayID, id, this._shopCountDic[id]))
					{
						client.sendCmd(nID, string.Format(result, new object[]
						{
							0,
							id,
							0,
							0
						}), false);
						return true;
					}
					if (!myCountDic.ContainsKey(id))
					{
						myCountDic.Add(id, count);
					}
					else
					{
						Dictionary<int, int> dictionary;
						int key;
						(dictionary = myCountDic)[key = id] = dictionary[key] + count;
					}
					this.OlympicsShopCountSet(client, dayID, myCountDic);
					this.OlympicsGradeSet(client, gradeArr[0], gradeArr[1] - needGrade);
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, shopData.Goods.GoodsID, shopData.Goods.GCount, shopData.Goods.Quality, "", shopData.Goods.Forge_level, shopData.Goods.Binding, 0, "", true, count, "奥运积分商店", "1900-01-01 12:00:00", shopData.Goods.AddPropIndex, shopData.Goods.BornIndex, shopData.Goods.Lucky, shopData.Goods.Strong, shopData.Goods.ExcellenceInfo, shopData.Goods.AppendPropLev, 0, null, null, 0, true);
					client.sendCmd(nID, string.Format(result, new object[]
					{
						1,
						id,
						myCountDic[id],
						this._shopCountDic[id]
					}), false);
					return true;
				}
				catch (Exception ex)
				{
					DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result2 = false;
			}
			return result2;
		}

		
		private bool ProcessOlympicsAwardStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string result = "{0}:{1}";
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int awardState = 1;
				int awardType = Convert.ToInt32(cmdParams[0]);
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, string.Format(result, awardType, awardState), false);
					return true;
				}
				switch (awardType)
				{
				case 1:
				{
					int dayID = this.GetOlympicsDay();
					List<OlympicsGuessDataDB> list = this.DBOlympicsGuessList(client);
					if (list != null && list.Count > 0)
					{
						foreach (OlympicsGuessDataDB item in list)
						{
							if (item.DayID < dayID && (item.Award1 == 0 || item.Award2 == 0 || item.Award3 == 0))
							{
								awardState = 0;
								break;
							}
						}
					}
					break;
				}
				case 2:
				{
					int[] gradeArr = StringUtil.StringToIntArr(this.OlympicsGradeGet(client), ':');
					int myGradeAll = gradeArr[0];
					if (myGradeAll <= 0)
					{
						awardState = -4;
					}
					else if (!this.IsRankAwardTime())
					{
						awardState = -10;
					}
					else
					{
						awardState = Global.GetRoleParamsInt32FromDB(client, "20015");
					}
					break;
				}
				}
				client.sendCmd(nID, string.Format(result, awardType, awardState), false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private bool ProcessOlympicsAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				string result = "{0}:{1}:{2}:{3}";
				int awardType = Convert.ToInt32(cmdParams[0]);
				EOperateType awardState = EOperateType.Fail;
				int awardID = 0;
				int rank = 0;
				if (!this._olympicsIsOpen)
				{
					client.sendCmd(nID, string.Format(result, new object[]
					{
						awardType,
						awardState,
						awardID,
						rank
					}), false);
					return true;
				}
				switch (awardType)
				{
				case 1:
					awardState = this.OlympicAwardGuess(client, out awardID);
					break;
				case 2:
					awardState = this.OlympicAwardRank(client, out awardID, out rank);
					break;
				}
				result = string.Format(result, new object[]
				{
					awardType,
					(int)awardState,
					awardID,
					rank
				});
				client.sendCmd(nID, result, false);
				return true;
			}
			catch (Exception ex)
			{
				DataHelper.WriteFormatExceptionLog(ex, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		
		private List<OlympicsGuessData> GetOlympicsGuessList(OlympicsGuessDataDB answerData)
		{
			IOrderedEnumerable<OlympicsGuessData> temp = from info in this._guessDic.Values
			where info.DayID == answerData.DayID
			orderby info.ID
			select info;
			List<OlympicsGuessData> result;
			if (!temp.Any<OlympicsGuessData>())
			{
				result = null;
			}
			else
			{
				int index = 0;
				int[] answerArr = new int[]
				{
					answerData.A1,
					answerData.A2,
					answerData.A3
				};
				List<OlympicsGuessData> list = new List<OlympicsGuessData>();
				foreach (OlympicsGuessData info2 in temp)
				{
					OlympicsGuessData d = new OlympicsGuessData();
					d.Clone(info2);
					d.DayID = this.TransformOffsetDayToFromBegin(d.DayID);
					d.Select = answerArr[index++];
					list.Add(d);
				}
				result = list;
			}
			return result;
		}

		
		private List<OlympicsShopData> GetOlympicsShopList(int day, Dictionary<int, int> myCountDic)
		{
			IOrderedEnumerable<OlympicsShopData> temp = from info in this._shopDic.Values
			where info.DayID == day
			orderby info.ID
			select info;
			List<OlympicsShopData> result;
			if (!temp.Any<OlympicsShopData>())
			{
				result = null;
			}
			else
			{
				List<OlympicsShopData> list = new List<OlympicsShopData>();
				foreach (OlympicsShopData info2 in temp)
				{
					OlympicsShopData d = new OlympicsShopData();
					d.Clone(info2);
					d.DayID = this.TransformOffsetDayToFromBegin(d.DayID);
					this._shopCountDic.TryGetValue(info2.ID, out d.NumFullBuy);
					myCountDic.TryGetValue(info2.ID, out d.NumSingleBuy);
					list.Add(d);
				}
				result = list;
			}
			return result;
		}

		
		private EOperateType OlympicAwardGuess(GameClient client, out int awardID)
		{
			awardID = 0;
			List<OlympicsGuessDataDB> dbList = new List<OlympicsGuessDataDB>();
			int dayID = this.GetOlympicsDay();
			List<OlympicsGuessDataDB> answerList = this.DBOlympicsGuessList(client);
			foreach (OlympicsGuessDataDB answerItem in answerList)
			{
				if (answerItem.DayID < dayID && (answerItem.Award1 == 0 || answerItem.Award2 == 0 || answerItem.Award3 == 0))
				{
					List<OlympicsGuessData> guesslist = this.GetOlympicsGuessList(answerItem);
					if (guesslist != null && guesslist.Count > 0)
					{
						if (answerItem.Award1 == 0 && guesslist[0].Select > 0 && guesslist[0].Answer > 0)
						{
							answerItem.Award1 = 1;
							if (guesslist[0].Select == guesslist[0].Answer)
							{
								awardID += guesslist[0].Grade;
							}
						}
						if (answerItem.Award2 == 0 && guesslist[1].Select > 0 && guesslist[1].Answer > 0)
						{
							answerItem.Award2 = 1;
							if (guesslist[1].Select == guesslist[1].Answer)
							{
								awardID += guesslist[1].Grade;
							}
						}
						if (answerItem.Award3 == 0 && guesslist[2].Select > 0 && guesslist[2].Answer > 0)
						{
							answerItem.Award3 = 1;
							if (guesslist[2].Select == guesslist[2].Answer)
							{
								awardID += guesslist[2].Grade;
							}
						}
						dbList.Add(answerItem);
					}
				}
			}
			foreach (OlympicsGuessDataDB item in dbList)
			{
				this.DBOlympicsGuessUpdate(client, item);
			}
			if (awardID > 0)
			{
				this.OlympicsGradeAdd(client, awardID);
			}
			return EOperateType.Succ;
		}

		
		private EOperateType OlympicAwardRank(GameClient client, out int awardID, out int rankOut)
		{
			awardID = 0;
			rankOut = 0;
			int rank = 50001;
			EOperateType result;
			if (!this.IsRankAwardTime())
			{
				result = EOperateType.EAwardTime;
			}
			else
			{
				int[] gradeArr = StringUtil.StringToIntArr(this.OlympicsGradeGet(client), ':');
				int myGradeAll = gradeArr[0];
				if (myGradeAll <= 0)
				{
					result = EOperateType.ENoGrade;
				}
				else
				{
					KFRankData myData = AllyClient.getInstance().HRankData(1, client.ClientData.RoleID);
					if (myData != null)
					{
						rank = myData.Rank;
					}
					OlympicsRankAwardInfo awardInfo = this._rankAwardList.Find((OlympicsRankAwardInfo _x) => rank >= _x.RankBegin && (_x.RankEnd == -1 || rank <= _x.RankEnd));
					if (awardInfo == null)
					{
						result = EOperateType.Fail;
					}
					else
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "20015", 1, true);
						List<GoodsData> awardList = awardInfo.DefaultGoodsList;
						if (Global.CanAddGoodsDataList(client, awardInfo.DefaultGoodsList))
						{
							for (int i = 0; i < awardList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardList[i].GoodsID, awardList[i].GCount, awardList[i].Quality, "", awardList[i].Forge_level, awardList[i].Binding, 0, "", true, 1, "奥运奖励", "1900-01-01 12:00:00", awardList[i].AddPropIndex, awardList[i].BornIndex, awardList[i].Lucky, awardList[i].Strong, awardList[i].ExcellenceInfo, awardList[i].AppendPropLev, 0, null, null, 0, true);
							}
						}
						else
						{
							int mailID = Global.UseMailGivePlayerAward2(client, awardList, GLang.GetLang(505, new object[0]), GLang.GetLang(505, new object[0]), 0, 0, 0);
						}
						awardID = awardInfo.ID;
						rankOut = ((rank >= 50001) ? -1 : rank);
						result = EOperateType.Succ;
					}
				}
			}
			return result;
		}

		
		private void JudgeClearOlympicsActivityData(GameClient client)
		{
			if (this._olympicsIsOpen)
			{
				string KeyString = string.Format("{0}", this.timeBegin.ToString("yyyy-MM-dd HH:mm:ss")).Replace(':', '$');
				string OlympicsDate = Global.GetRoleParamByName(client, "20014");
				if (string.IsNullOrEmpty(OlympicsDate))
				{
					OlympicsDate = KeyString;
					Global.SaveRoleParamsStringToDB(client, "20014", OlympicsDate, true);
				}
				else if (string.Compare(KeyString, OlympicsDate) != 0)
				{
					OlympicsDate = KeyString;
					this.OlympicsGradeSet(client, 0, 0);
					Global.SaveRoleParamsInt32ValueToDB(client, "20015", 0, true);
					Global.SaveRoleParamsStringToDB(client, "20014", OlympicsDate, true);
				}
			}
		}

		
		private string OlympicsGradeGet(GameClient client)
		{
			string grade = Global.GetRoleParamByName(client, "20010");
			string result;
			if (string.IsNullOrEmpty(grade))
			{
				result = "0:0";
			}
			else
			{
				grade = grade.Replace(",", ":");
				result = grade;
			}
			return result;
		}

		
		private void OlympicsGradeSet(GameClient client, int gradeAll, int gradeLeft)
		{
			string value = string.Format("{0},{1}", gradeAll, gradeLeft);
			Global.SaveRoleParamsStringToDB(client, "20010", value, true);
			client.sendCmd(1050, string.Format("{0}:{1}", gradeAll, gradeLeft), false);
		}

		
		public void OlympicsGradeAdd(GameClient client, int addGrade)
		{
			if (this._olympicsIsOpen && !this.IsRankAwardTime())
			{
				string gradeStr = this.OlympicsGradeGet(client);
				int[] gradeArr = StringUtil.StringToIntArr(gradeStr, ':');
				int all = gradeArr[0] + addGrade;
				int left = gradeArr[1] + addGrade;
				this.OlympicsGradeSet(client, all, left);
				RoleData4Selector rs = Global.RoleDataEx2RoleData4Selector(client.ClientData.GetRoleDataEx());
				byte[] roleData = DataHelper.ObjectToBytes<RoleData4Selector>(rs);
				AllyClient.getInstance().HRankUpdate(1, gradeArr[0] + addGrade, client.ClientData.RoleID, client.ClientData.ZoneID, client.ClientData.RoleName, roleData);
			}
		}

		
		private int[] OlympicsGameCountGet(GameClient client, EGameType gameType)
		{
			string oldValue = "";
			switch (gameType)
			{
			case EGameType.Shoot:
				oldValue = Global.GetRoleParamByName(client, "20011");
				break;
			case EGameType.Football:
				oldValue = Global.GetRoleParamByName(client, "20012");
				break;
			}
			int dayID = this.GetOlympicsDay();
			int[] result;
			if (string.IsNullOrEmpty(oldValue))
			{
				int[] array = new int[5];
				array[0] = dayID;
				result = array;
			}
			else
			{
				int[] arr = StringUtil.StringToIntArr(oldValue, ',');
				result = arr;
			}
			return result;
		}

		
		private void OlympicsGameCountSet(GameClient client, EGameType gameType, string value)
		{
			switch (gameType)
			{
			case EGameType.Shoot:
				Global.SaveRoleParamsStringToDB(client, "20011", value, true);
				break;
			case EGameType.Football:
				Global.SaveRoleParamsStringToDB(client, "20012", value, true);
				break;
			}
		}

		
		private Dictionary<int, int> OlympicsShopCountGet(GameClient client, int dayID)
		{
			Dictionary<int, int> result = new Dictionary<int, int>();
			string str = Global.GetRoleParamByName(client, "20013");
			Dictionary<int, int> result2;
			if (string.IsNullOrEmpty(str))
			{
				result2 = result;
			}
			else
			{
				string[] arr = str.Split(new char[]
				{
					'*'
				});
				int oldDayID = Global.SafeConvertToInt32(arr[0]);
				if (oldDayID != dayID)
				{
					this.OlympicsShopCountSet(client, dayID, null);
					result2 = result;
				}
				else
				{
					if (arr.Length > 1 && !string.IsNullOrEmpty(arr[1]))
					{
						int[] countArr = StringUtil.StringToIntArr(arr[1], ',');
						for (int i = 0; i < countArr.Length; i++)
						{
							result.Add(countArr[i++], countArr[i]);
						}
					}
					result2 = result;
				}
			}
			return result2;
		}

		
		private void OlympicsShopCountSet(GameClient client, int dayID, Dictionary<int, int> dic)
		{
			StringBuilder sb = new StringBuilder();
			if (dic != null)
			{
				foreach (KeyValuePair<int, int> d in dic)
				{
					if (sb.Length > 0)
					{
						sb.Append(",");
					}
					sb.Append(string.Format("{0},{1}", d.Key, d.Value));
				}
			}
			Global.SaveRoleParamsStringToDB(client, "20013", string.Format("{0}*{1}", dayID, sb.ToString()), true);
		}

		
		private Dictionary<int, int> DBOlympicsShopList(int dayID)
		{
			return Global.sendToDB<Dictionary<int, int>, int>(13124, dayID, GameManager.ServerId);
		}

		
		private bool DBOlympicsShopUpdate(int dayID, int shopID, int count)
		{
			string cmd2db = string.Format("{0}:{1}:{2}", dayID, shopID, count);
			return Global.sendToDB<bool, string>(13125, cmd2db, GameManager.ServerId);
		}

		
		private OlympicsGuessDataDB DBOlympicsGuess(GameClient client, int dayID)
		{
			string cmd2db = string.Format("{0}:{1}", client.ClientData.RoleID, dayID);
			return Global.sendToDB<OlympicsGuessDataDB, string>(13126, cmd2db, GameManager.ServerId);
		}

		
		private List<OlympicsGuessDataDB> DBOlympicsGuessList(GameClient client)
		{
			return Global.sendToDB<List<OlympicsGuessDataDB>, int>(13127, client.ClientData.RoleID, GameManager.ServerId);
		}

		
		private bool DBOlympicsGuessUpdate(GameClient client, OlympicsGuessDataDB data)
		{
			return Global.sendToDB<bool, OlympicsGuessDataDB>(13128, data, GameManager.ServerId);
		}

		
		public void CheckOlympicsOpenState(long ticks, bool changeDay = false)
		{
			if (changeDay || ticks - this._lastTicks >= 10000L)
			{
				this._lastTicks = ticks;
				DateTime timeNow = TimeUtil.NowDateTime();
				if (!this._olympicsIsOpen && timeNow >= this.timeBegin && timeNow < this.timeAwardEnd)
				{
					this._olympicsIsOpen = true;
					this._olympicsOpenTime = this.timeBegin;
					int dayId = this.GetOlympicsDay();
					this._shopCountDic = this.DBOlympicsShopList(dayId);
					ActivityData activityData = new ActivityData();
					activityData.ActivityType = 4;
					activityData.ActivityIsOpen = true;
					activityData.TimeBegin = this.timeBegin;
					activityData.TimeEnd = this.timeEnd;
					activityData.TimeAwardBegin = this.timeAwardBegin;
					activityData.TimeAwardEnd = this.timeAwardEnd;
					SingletonTemplate<ActivityManager>.Instance().ActivityAdd(activityData);
					AllyClient.getInstance().HRankTopList(1);
				}
				if (this._olympicsIsOpen && (timeNow > this.timeAwardEnd || timeNow < this.timeBegin))
				{
					this._olympicsIsOpen = false;
					this._olympicsOpenTime = DateTime.MinValue;
					this._shopCountDic.Clear();
					SingletonTemplate<ActivityManager>.Instance().ActivityDel(4);
				}
			}
		}

		
		private int TransformOffsetDayToFromBegin(int OffsetDay)
		{
			return OffsetDay - Global.GetOffsetDay(this.timeBegin);
		}

		
		private int GetOlympicsDay()
		{
			int day = 0;
			if (this._olympicsIsOpen && this._olympicsOpenTime > DateTime.MinValue)
			{
				day = Global.GetOffsetDay(TimeUtil.NowDateTime()) + 1;
			}
			return day;
		}

		
		private int GetOlympicsDayFromBegin()
		{
			int day = 0;
			if (this._olympicsIsOpen && this._olympicsOpenTime > DateTime.MinValue)
			{
				day = Global.GetOffsetDay(TimeUtil.NowDateTime()) - Global.GetOffsetDay(this._olympicsOpenTime) + 1;
			}
			return day;
		}

		
		public void InitOlympics()
		{
			lock (this._mutex)
			{
				string str = GameManager.systemParamsList.GetParamValueByName("AoYunTime");
				if (!string.IsNullOrEmpty(str))
				{
					string[] arr = str.Split(new char[]
					{
						','
					});
					if (arr != null && arr.Length == 2)
					{
						DateTime.TryParse(arr[0], out this.timeBegin);
						DateTime.TryParse(arr[1], out this.timeEnd);
					}
				}
				str = GameManager.systemParamsList.GetParamValueByName("AoYunAwardTime");
				if (!string.IsNullOrEmpty(str))
				{
					string[] arr = str.Split(new char[]
					{
						','
					});
					DateTime.TryParse(arr[0], out this.timeAwardBegin);
					DateTime.TryParse(arr[1], out this.timeAwardEnd);
				}
				ActivityData activityData = new ActivityData();
				activityData.ActivityType = 4;
				activityData.ActivityIsOpen = this._olympicsIsOpen;
				activityData.TimeBegin = this.timeBegin;
				activityData.TimeEnd = this.timeEnd;
				activityData.TimeAwardBegin = this.timeAwardBegin;
				activityData.TimeAwardEnd = this.timeAwardEnd;
				SingletonTemplate<ActivityManager>.Instance().UpdateActivityData(activityData);
				this.InitOlympicsGame();
				this.InitOlympicsGuess();
				this.InitOlympicsRankAward();
				this.InitOlympicsShop();
			}
		}

		
		private bool IsRankAwardTime()
		{
			return TimeUtil.NowDateTime() >= this.timeAwardBegin && TimeUtil.NowDateTime() < this.timeAwardEnd;
		}

		
		private double GetFootballRate()
		{
			return GameManager.systemParamsList.GetParamValueDoubleByName("AoYunGoalOdds", 0.0);
		}

		
		private void InitOlympicsGame()
		{
			lock (this._mutex)
			{
				string fileName = Global.GameResPath("Config/AoYunMatch.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					try
					{
						this._gameDic.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (xmlItem != null)
							{
								OlympicsGameInfo info = new OlympicsGameInfo();
								info.GameID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								info.CountFree = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "FreeNum", "0"));
								string diamondStr = Global.GetDefAttributeStr(xmlItem, "NeedZhuanShi", "0");
								string[] diamondArr = diamondStr.Split(new char[]
								{
									','
								});
								List<int> list = new List<int>();
								foreach (string d in diamondArr)
								{
									list.Add(int.Parse(d));
								}
								info.DiamondList = list;
								info.CountDiamond = list.Count;
								info.CountGame = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "GameNum", "0"));
								info.CountWin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "WinNum", "0"));
								info.GradeWin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "WinJiFen", "0"));
								info.GradeLost = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "LoseJiFen", "0"));
								this._gameDic.Add(info.GameID, info);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
					}
				}
			}
		}

		
		private void InitOlympicsGuess()
		{
			lock (this._mutex)
			{
				string fileName = Global.GameResPath("Config/AoYunQuestion.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					try
					{
						this._guessDic.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (xmlItem != null)
							{
								OlympicsGuessData info = new OlympicsGuessData();
								info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								info.DayID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Day", "0"));
								info.DayID += Global.GetOffsetDay(this.timeBegin);
								info.Content = Global.GetDefAttributeStr(xmlItem, "Question", "");
								info.A = Global.GetDefAttributeStr(xmlItem, "A", "0");
								info.B = Global.GetDefAttributeStr(xmlItem, "B", "0");
								info.C = Global.GetDefAttributeStr(xmlItem, "C", "0");
								info.D = Global.GetDefAttributeStr(xmlItem, "D", "0");
								info.Answer = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Answer", "0"));
								info.Grade = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "JiFen", "0"));
								this._guessDic.Add(info.ID, info);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
					}
				}
			}
		}

		
		private void InitOlympicsRankAward()
		{
			lock (this._mutex)
			{
				string fileName = Global.GameResPath("Config/AoYunAward.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					try
					{
						this._rankAwardList.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (xmlItem != null)
							{
								OlympicsRankAwardInfo info = new OlympicsRankAwardInfo();
								info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								info.Content = Global.GetDefAttributeStr(xmlItem, "Name", "");
								info.RankBegin = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "BeginNum", "0"));
								info.RankEnd = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "EngNum", "0"));
								if (info.RankEnd == -1)
								{
									info.RankEnd = int.MaxValue;
								}
								string goods = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
								if (!string.IsNullOrEmpty(goods))
								{
									string[] fields = goods.Split(new char[]
									{
										'|'
									});
									if (fields.Length > 0)
									{
										info.DefaultGoodsList = GoodsHelper.ParseGoodsDataList(fields, fileName);
									}
								}
								this._rankAwardList.Add(info);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
					}
				}
			}
		}

		
		private void InitOlympicsShop()
		{
			lock (this._mutex)
			{
				string fileName = Global.GameResPath("Config/AoYunQiangGou.xml");
				XElement xml = CheckHelper.LoadXml(fileName, true);
				if (null != xml)
				{
					try
					{
						this._shopDic.Clear();
						IEnumerable<XElement> xmlItems = xml.Elements();
						foreach (XElement xmlItem in xmlItems)
						{
							if (xmlItem != null)
							{
								OlympicsShopData info = new OlympicsShopData();
								info.ID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "ID", "0"));
								info.DayID = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "Day", "0"));
								info.DayID += Global.GetOffsetDay(this.timeBegin);
								string goodsStr = Global.GetDefAttributeStr(xmlItem, "GoodsOne", "");
								if (!string.IsNullOrEmpty(goodsStr))
								{
									info.Goods = GoodsHelper.ParseGoodsData(goodsStr, fileName);
								}
								info.Price = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "JiFen", "0"));
								info.NumSingle = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "SinglePurchase", "0"));
								info.NumFull = Convert.ToInt32(Global.GetDefAttributeStr(xmlItem, "FullPurchase", "0"));
								this._shopDic.Add(info.ID, info);
							}
						}
					}
					catch (Exception ex)
					{
						LogManager.WriteLog(LogTypes.Fatal, string.Format("加载[{0}]时出错!!!", fileName), null, true);
					}
				}
			}
		}

		
		private OlympicsShopData GetOlympicsShopData(int id)
		{
			OlympicsShopData result;
			if (this._shopDic.ContainsKey(id))
			{
				result = this._shopDic[id];
			}
			else
			{
				result = null;
			}
			return result;
		}

		
		public void CheckTip(GameClient client)
		{
			if (!this._olympicsIsOpen || this.IsRankAwardTime())
			{
				client._IconStateMgr.AddFlushIconState(20001, false);
				client._IconStateMgr.AddFlushIconState(20002, false);
				client._IconStateMgr.AddFlushIconState(20000, false);
				client._IconStateMgr.SendIconStateToClient(client);
			}
			else
			{
				bool isTipMatch = false;
				bool isTipGuess = false;
				int dayID = this.GetOlympicsDay();
				foreach (OlympicsGameInfo info in this._gameDic.Values)
				{
					int[] countArr = this.OlympicsGameCountGet(client, (EGameType)info.GameID);
					if (countArr[1] < info.CountFree || countArr[0] != dayID)
					{
						isTipMatch = true;
					}
				}
				OlympicsGuessDataDB answer = this.DBOlympicsGuess(client, dayID);
				List<OlympicsGuessData> list = this.GetOlympicsGuessList(answer);
				if (list != null && (answer.A1 == -1 || answer.A2 == -1 || answer.A3 == -1))
				{
					isTipGuess = true;
				}
				bool bAnyTipChanged = false;
				bAnyTipChanged |= client._IconStateMgr.AddFlushIconState(20001, isTipMatch);
				bAnyTipChanged |= client._IconStateMgr.AddFlushIconState(20002, isTipGuess);
				bAnyTipChanged |= client._IconStateMgr.AddFlushIconState(20000, isTipMatch || isTipGuess);
				if (bAnyTipChanged)
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		
		private const int OLYMPICS_GUESS_COUNT = 3;

		
		private const int OLYMPICS_RANK_MAX = 50001;

		
		public const int _sceneType = 10006;

		
		public object _mutex = new object();

		
		private DateTime timeBegin = DateTime.MinValue;

		
		private DateTime timeEnd = DateTime.MinValue;

		
		private DateTime timeAwardBegin = DateTime.MinValue;

		
		private DateTime timeAwardEnd = DateTime.MinValue;

		
		private static OlympicsManager instance = new OlympicsManager();

		
		private long _lastTicks = 0L;

		
		private bool _olympicsIsOpen = false;

		
		private DateTime _olympicsOpenTime = DateTime.MinValue;

		
		private Dictionary<int, int> _shopCountDic = new Dictionary<int, int>();

		
		private Dictionary<int, OlympicsGameInfo> _gameDic = new Dictionary<int, OlympicsGameInfo>();

		
		private Dictionary<int, OlympicsGuessData> _guessDic = new Dictionary<int, OlympicsGuessData>();

		
		private List<OlympicsRankAwardInfo> _rankAwardList = new List<OlympicsRankAwardInfo>();

		
		private Dictionary<int, OlympicsShopData> _shopDic = new Dictionary<int, OlympicsShopData>();
	}
}
