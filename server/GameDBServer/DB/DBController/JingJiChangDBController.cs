﻿using System;
using System.Collections.Generic;
using System.Data;
using GameDBServer.Logic;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	
	public class JingJiChangDBController : DBController<PlayerJingJiData>
	{
		
		private JingJiChangDBController()
		{
		}

		
		public static JingJiChangDBController getInstance()
		{
			return JingJiChangDBController.instance;
		}

		
		public PlayerJingJiData getPlayerJingJiDataById(int Id)
		{
			string sql = string.Format("select * from t_jingjichang where roleId = {0};", Id);
			return base.queryForObject(sql);
		}

		
		public List<PlayerJingJiData> getPlayerJingJiDataList()
		{
			string sql = string.Format("select * from t_jingjichang where ranking != -1 order by ranking limit {0};", JingJiChangConstants.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		
		public bool updateNextRewardTime(int roleId, long nextRewardTime)
		{
			string sql = string.Format("update t_jingjichang set nextRewardTime={0} where roleId={1};", nextRewardTime, roleId);
			return base.update(sql) > 0;
		}

		
		public bool updateJingJiDataForFailed(int roleId, long nextChallengeTime)
		{
			string sql = string.Format("update t_jingjichang set winCount=0,nextChallengeTime={0} where roleId={1};", nextChallengeTime, roleId);
			return base.update(sql) > 0;
		}

		
		public bool updateJingJiWinCount(int roleId, int winCount)
		{
			string sql = string.Format("update t_jingjichang set winCount={0} where roleId={1};", winCount, roleId);
			return base.update(sql) > 0;
		}

		
		public bool updateJingJiMaxWinCount(int roleId, int maxWinCount)
		{
			string sql = string.Format("update t_jingjichang set maxwincnt={0} where roleId={1};", maxWinCount, roleId);
			return base.update(sql) > 0;
		}

		
		public bool updateJingJiRanking(int roleId, int ranking)
		{
			string sql = string.Format("update t_jingjichang set ranking={0} where roleId={1};", ranking, roleId);
			return base.update(sql) > 0;
		}

		
		public bool updateNextChallengeTime(int roleId, long nextChallengeTime)
		{
			string sql = string.Format("update t_jingjichang set nextChallengeTime={0} where roleId={1};", nextChallengeTime, roleId);
			return base.update(sql) > 0;
		}

		
		public bool updateJingJiDataForWin(PlayerJingJiData data)
		{
			data.convertString();
			string sql = "update t_jingjichang set level=@level,occupationid=@occupationid,changeLiveCount=@changeLiveCount,winCount=@winCount,nextChallengeTime=@nextChallengeTime,baseProps=@baseProps,extProps=@extProps,equipDatas=@equipDatas,skillDatas=@skillDatas,CombatForce=@CombatForce,wingData=@wingData,settingFlags=@settingFlags,shenshiequip=@shenShiEquipData,passiveEffect=@passiveEffectData,suboccupation=@suboccupation where roleId=@roleId;";
			MySQLConnection conn = null;
			int resultCount = -1;
			try
			{
				conn = this.dbMgr.DBConns.PopDBConnection();
				GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", sql), EventLevels.Important);
				MySQLCommand cmd = new MySQLCommand();
				cmd.Connection = conn;
				cmd.CommandType = CommandType.Text;
				cmd.CommandText = sql;
				cmd.Parameters.Add("@roleId", DbType.Int32);
				cmd.Parameters.Add("@level", DbType.Int32);
				cmd.Parameters.Add("@changeLiveCount", DbType.Int32);
				cmd.Parameters.Add("@winCount", DbType.Int32);
				cmd.Parameters.Add("@nextChallengeTime", DbType.Int64);
				cmd.Parameters.Add("@baseProps", DbType.String);
				cmd.Parameters.Add("@extProps", DbType.String);
				cmd.Parameters.Add("@equipDatas", DbType.String);
				cmd.Parameters.Add("@skillDatas", DbType.String);
				cmd.Parameters.Add("@CombatForce", DbType.Int32);
				cmd.Parameters.Add("@wingData", DbType.String);
				cmd.Parameters.Add("@settingFlags", DbType.Int64);
				cmd.Parameters.Add("@occupationid", DbType.Int32);
				cmd.Parameters.Add("@shenShiEquipData", DbType.String);
				cmd.Parameters.Add("@passiveEffectData", DbType.String);
				cmd.Parameters.Add("@suboccupation", DbType.Int32);
				cmd.Parameters["@roleId"].Value = data.roleId;
				cmd.Parameters["@level"].Value = data.level;
				cmd.Parameters["@changeLiveCount"].Value = data.changeLiveCount;
				cmd.Parameters["@winCount"].Value = data.winCount;
				cmd.Parameters["@nextChallengeTime"].Value = data.nextChallengeTime;
				cmd.Parameters["@baseProps"].Value = data.stringBaseProps;
				cmd.Parameters["@extProps"].Value = data.stringExtProps;
				cmd.Parameters["@equipDatas"].Value = data.stringEquipDatas;
				cmd.Parameters["@skillDatas"].Value = data.stringSkillDatas;
				cmd.Parameters["@CombatForce"].Value = data.combatForce;
				cmd.Parameters["@wingData"].Value = data.stringWingData;
				cmd.Parameters["@settingFlags"].Value = data.settingFlags;
				cmd.Parameters["@occupationid"].Value = data.occupationId;
				cmd.Parameters["@shenShiEquipData"].Value = data.stringShenShiEuipSkill;
				cmd.Parameters["@passiveEffectData"].Value = data.stringPassiveEffect;
				cmd.Parameters["@suboccupation"].Value = data.SubOccupation;
				try
				{
					resultCount = cmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("向数据库更新竞技场数据失败: {0},{1}", sql, ex), null, true);
				}
				cmd.Dispose();
				cmd = null;
			}
			finally
			{
				if (null != conn)
				{
					this.dbMgr.DBConns.PushDBConnection(conn);
				}
			}
			return resultCount > 0;
		}

		
		public bool insertJingJiData(PlayerJingJiData data)
		{
			data.convertString();
			int resultCount = -1;
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string cmdText = string.Format("insert into t_jingjichang \r\n                    (roleId,roleName,name,zoneId,level,changeLiveCount,occupationId,winCount,ranking,nextRewardTime,nextChallengeTime,\r\n                    baseProps,extProps,equipDatas,skillDatas,CombatForce,sex,wingData,settingFlags, shenshiequip, passiveEffect,suboccupation) \r\n                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8}, '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', {18}, '{19}', '{20}','{21}')", new object[]
				{
					data.roleId,
					data.roleName,
					data.name,
					data.zoneId,
					data.level,
					data.changeLiveCount,
					data.occupationId,
					data.winCount,
					data.ranking,
					data.nextRewardTime,
					data.nextChallengeTime,
					data.stringBaseProps,
					data.stringExtProps,
					data.stringEquipDatas,
					data.stringSkillDatas,
					data.combatForce,
					data.sex,
					data.stringWingData,
					data.settingFlags,
					data.stringShenShiEuipSkill,
					data.stringPassiveEffect,
					data.SubOccupation
				});
				resultCount = conn.ExecuteNonQuery(cmdText, 0);
			}
			return resultCount >= 0;
		}

		
		internal void OnChangeName(int roleId, string oldName, string newName)
		{
			using (MyDbConnection3 conn = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_jingjichang SET roleName='{0}',name='{1}' WHERE roleId={2}", newName, newName, roleId);
				conn.ExecuteNonQuery(sql, 0);
			}
		}

		
		private static JingJiChangDBController instance = new JingJiChangDBController();
	}
}
