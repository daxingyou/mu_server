﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Logic.ActivityNew.SevenDay
{
	
	public class SevenDayLoginAct
	{
		
		public void LoadConfig()
		{
			Dictionary<int, SevenDayLoginAct._DayAward> tmpDict = new Dictionary<int, SevenDayLoginAct._DayAward>();
			try
			{
				XElement xml = XElement.Load(Global.GameResPath("Config/SevenDay/SevenDayLogin.xml")).Element("GiftList");
				foreach (XElement xmlItem in xml.Elements())
				{
					int day = (int)Global.GetSafeAttributeLong(xmlItem, "ID");
					SevenDayLoginAct._DayAward award = new SevenDayLoginAct._DayAward();
					string goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsOne");
					if (!string.IsNullOrEmpty(goodsIDs))
					{
						string[] fields = goodsIDs.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, string.Format("读取{0}活动配置文件中的物品配置项失败", "Config/SevenDay/SevenDayLogin.xml"), null, true);
						}
						else
						{
							award.AllAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "Config/SevenDay/SevenDayLogin.xml");
						}
					}
					goodsIDs = Global.GetSafeAttributeStr(xmlItem, "GoodsTwo");
					if (!string.IsNullOrEmpty(goodsIDs))
					{
						string[] fields = goodsIDs.Split(new char[]
						{
							'|'
						});
						if (fields.Length <= 0)
						{
							LogManager.WriteLog(LogTypes.Warning, "Config/SevenDay/SevenDayLogin.xml", null, true);
						}
						else
						{
							award.OccAward.GoodsDataList = HuodongCachingMgr.ParseGoodsDataList(fields, "Config/SevenDay/SevenDayLogin.xml");
						}
					}
					string timeGoods = Global.GetSafeAttributeStr(xmlItem, "GoodsThr");
					string timeList = Global.GetSafeAttributeStr(xmlItem, "EffectiveTime");
					award.TimeAward.Init(timeGoods, timeList, "Config/SevenDay/SevenDayLogin.xml 时效性物品");
					tmpDict[day] = award;
				}
				lock (this.ConfigMutex)
				{
					this.DayAwardDict = tmpDict;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(LogTypes.Error, string.Format("七日登录活动加载配置失败{0}", "Config/SevenDay/SevenDayLogin.xml"), ex, true);
			}
		}

		
		public ESevenDayActErrorCode HandleGetAward(GameClient client, int day)
		{
			int currDay;
			ESevenDayActErrorCode result;
			if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
			{
				result = ESevenDayActErrorCode.NotInActivityTime;
			}
			else
			{
				Dictionary<int, SevenDayItemData> actData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Login);
				if (actData == null)
				{
					result = ESevenDayActErrorCode.NotInActivityTime;
				}
				else if (day <= 0 || day > currDay)
				{
					result = ESevenDayActErrorCode.NotReachCondition;
				}
				else
				{
					Dictionary<int, SevenDayLoginAct._DayAward> tmpDayAwardDict = null;
					lock (this.ConfigMutex)
					{
						tmpDayAwardDict = this.DayAwardDict;
					}
					SevenDayLoginAct._DayAward dayAward = null;
					if (tmpDayAwardDict == null || !tmpDayAwardDict.TryGetValue(day, out dayAward))
					{
						result = ESevenDayActErrorCode.ServerConfigError;
					}
					else
					{
						lock (actData)
						{
							SevenDayItemData data = null;
							if (!actData.TryGetValue(day, out data))
							{
								result = ESevenDayActErrorCode.NotReachCondition;
							}
							else if (data.Params1 != 1 || data.AwardFlag == 1)
							{
								result = ESevenDayActErrorCode.NotReachCondition;
							}
							else
							{
								int awardGoodsCnt = 0;
								if (dayAward.AllAward != null && dayAward.AllAward.GoodsDataList != null)
								{
									awardGoodsCnt += dayAward.AllAward.GoodsDataList.Count;
								}
								if (dayAward.OccAward != null && dayAward.OccAward.GoodsDataList != null)
								{
									awardGoodsCnt += dayAward.OccAward.GoodsDataList.Count((GoodsData _goods) => Global.IsRoleOccupationMatchGoods(client, _goods.GoodsID));
								}
								if (dayAward.TimeAward != null)
								{
									awardGoodsCnt += dayAward.TimeAward.GoodsCnt();
								}
								if (awardGoodsCnt <= 0 || !Global.CanAddGoodsNum(client, awardGoodsCnt))
								{
									result = ESevenDayActErrorCode.NoBagSpace;
								}
								else
								{
									data.AwardFlag = 1;
									if (!SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Login, day, data, client.ServerId))
									{
										data.AwardFlag = 0;
										result = ESevenDayActErrorCode.DBFailed;
									}
									else
									{
										if (!SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.AllAward, ESevenDayActType.Login) || !SingletonTemplate<SevenDayActivityMgr>.Instance().GiveAward(client, dayAward.OccAward, ESevenDayActType.Login) || !SingletonTemplate<SevenDayActivityMgr>.Instance().GiveEffectiveTimeAward(client, dayAward.TimeAward.ToAwardItem(), ESevenDayActType.Login))
										{
											LogManager.WriteLog(LogTypes.Error, string.Format("玩家领取七日活动奖励，设置领奖了但是发奖失败 roleid={0}, day={1}", client.ClientData.RoleID, day), null, true);
										}
										result = ESevenDayActErrorCode.Success;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		
		public bool HasAnyAwardCanGet(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client))
			{
				result = false;
			}
			else
			{
				Dictionary<int, SevenDayItemData> actData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Login);
				if (actData == null)
				{
					result = false;
				}
				else
				{
					lock (actData)
					{
						foreach (KeyValuePair<int, SevenDayItemData> kvp in actData)
						{
							SevenDayItemData data = kvp.Value;
							if (data.Params1 == 1 && data.AwardFlag != 1)
							{
								return true;
							}
						}
					}
					result = false;
				}
			}
			return result;
		}

		
		public void Update(GameClient client)
		{
			if (client != null)
			{
				int currDay;
				if (SingletonTemplate<SevenDayActivityMgr>.Instance().IsInActivityTime(client, out currDay))
				{
					Dictionary<int, SevenDayItemData> actData = SingletonTemplate<SevenDayActivityMgr>.Instance().GetActivityData(client, ESevenDayActType.Login);
					lock (actData)
					{
						if (!actData.ContainsKey(currDay))
						{
							SevenDayItemData itemData = new SevenDayItemData();
							itemData.AwardFlag = 0;
							itemData.Params1 = 1;
							if (SingletonTemplate<SevenDayActivityMgr>.Instance().UpdateDb(client.ClientData.RoleID, ESevenDayActType.Login, currDay, itemData, client.ServerId))
							{
								actData[currDay] = itemData;
							}
						}
					}
				}
			}
		}

		
		private Dictionary<int, SevenDayLoginAct._DayAward> DayAwardDict = null;

		
		private object ConfigMutex = new object();

		
		private class _DayAward
		{
			
			public AwardItem AllAward = new AwardItem();

			
			public AwardItem OccAward = new AwardItem();

			
			public AwardEffectTimeItem TimeAward = new AwardEffectTimeItem();
		}
	}
}
