﻿using System;

namespace GameServer.Logic
{
	
	public enum MagicActionIDs
	{
		
		FOREVER_ADDHIT,
		
		RANDOM_ADDATTACK1,
		
		RANDOM_ADDATTACK2,
		
		ATTACK_STRAIGHT,
		
		ATTACK_FRONT,
		
		PUSH_STRAIGHT,
		
		PUSH_CIRCLE,
		
		MAGIC_ATTACK,
		
		DS_ATTACK,
		
		RANDOM_MOVE,
		
		FIRE_WALL,
		
		FIRE_CIRCLE,
		
		NEW_MAGIC_SUBINJURE,
		
		DS_ADDLIFE,
		
		DS_CALL_GUARD,
		
		DS_HIDE_ROLE,
		
		TIME_DS_ADD_DEFENSE,
		
		TIME_DS_ADD_MDEFENSE,
		
		TIME_DS_SUB_DEFENSE,
		
		TIME_DS_INJURE,
		
		PHY_ATTACK,
		
		INSTANT_ATTACK,
		
		INSTANT_MAGIC,
		
		INSTANT_ATTACK1,
		
		INSTANT_MAGIC1,
		
		INSTANT_ATTACK2LIFE,
		
		INSTANT_MAGIC2LIFE,
		
		TIME_ATTACK,
		
		TIME_MAGIC,
		
		FOREVER_ADDDEFENSE,
		
		FOREVER_ADDATTACK,
		
		FOREVER_ADDMAGICDEFENSE,
		
		FOREVER_ADDMAGICATTACK,
		
		TIME_ADDDEFENSE,
		
		TIME_SUBDEFENSE,
		
		TIME_ADDATTACK,
		
		TIME_SUBATTACK,
		
		TIME_ADDMAGICDEFENSE,
		
		TIME_SUBMAGICDEFENSE,
		
		TIME_ADDMAGIC,
		
		TIME_SUBMAGIC,
		
		INSTANT_ADDLIFE1,
		
		INSTANT_ADDMAGIC1,
		
		INSTANT_ADDLIFE2,
		
		INSTANT_ADDMAGIC2,
		
		INSTANT_ADDLIFE3,
		
		INSTANT_ADDLIFE4,
		
		INSTANT_COOLDOWN,
		
		TIME_SUBLIFE,
		
		TIME_ADDLIFE,
		
		TIME_SLOW,
		
		TIME_ADDDODGE,
		
		TIME_FREEZE,
		
		TIME_INJUE2LIFE,
		
		INSTANT_BURSTATTACK,
		
		FOREVER_ADDDRUGEFFECT,
		
		INSTANT_REMOVESLOW,
		
		TIME_SUBINJUE,
		
		TIME_ADDINJUE,
		
		TIME_SUBINJUE1,
		
		TIME_ADDINJUE1,
		
		TIME_DELAYATTACK,
		
		TIME_DELAYMAGIC,
		
		FOREVER_ADDDODGE,
		
		TIME_INJUE2MAGIC,
		
		FOREVER_ADDMAGICV,
		
		FOREVER_ADDMAGICRECOVER,
		
		FOREVER_ADDLIFE,
		
		INSTANT_MOVE,
		
		INSTANT_STOP,
		
		TIME_ADDMAGIC1,
		
		GOTO_MAP,
		
		INSTANT_MAP_POS,
		
		GOTO_LAST_MAP,
		
		ADD_HORSE,
		
		ADD_PET,
		
		ADD_HORSE_EXT,
		
		ADD_PET_GRID,
		
		ADD_SKILL,
		
		NEW_INSTANT_ATTACK,
		
		NEW_INSTANT_MAGIC,
		
		NEW_FOREVER_ADDDEFENSE,
		
		NEW_FOREVER_ADDATTACK,
		
		NEW_FOREVER_ADDMAGICDEFENSE,
		
		NEW_FOREVER_ADDMAGICATTACK,
		
		NEW_FOREVER_ADDHIT,
		
		NEW_FOREVER_ADDDODGE,
		
		NEW_FOREVER_ADDBURST,
		
		NEW_FOREVER_ADDMAGICV,
		
		NEW_FOREVER_ADDLIFE,
		
		NEW_TIME_INJUE2MAGIC,
		
		NEW_TIME_ATTACK,
		
		NEW_TIME_MAGIC,
		
		NEW_INSTANT_ADDLIFE,
		
		DB_ADD_DBL_EXP,
		
		DB_ADD_DBL_MONEY,
		
		DB_ADD_DBL_LINGLI,
		
		DB_ADD_LIFERESERVE,
		
		DB_ADD_MAGICRESERVE,
		
		DB_ADD_LINGLIRESERVE,
		
		DB_ADD_TEMPATTACK,
		
		DB_ADD_TEMPDEFENSE,
		
		DB_ADD_UPLIEFLIMIT,
		
		DB_ADD_UPMAGICLIMIT,
		
		NEW_ADD_LINGLI,
		
		NEW_ADD_EXP,
		
		NEW_ADD_DAILYCXNUM,
		
		GOTO_NEXTMAP,
		
		GET_AWARD,
		
		NEW_INSTANT_ADDLIFE2,
		
		NEW_INSTANT_ATTACK3,
		
		NEW_INSTANT_MAGIC3,
		
		NEW_TIME_ATTACK3,
		
		NEW_TIME_MAGIC3,
		
		NEW_INSTANT_ADDLIFE3,
		
		NEW_TIME_INJUE2MAGIC3,
		
		GOTO_WUXING_MAP,
		
		GET_WUXING_AWARD,
		
		LEAVE_LAOFANG,
		
		GOTO_CAISHENMIAO,
		
		DB_ADD_ANTIBOSS,
		
		RELOAD_COPYMONSTERS,
		
		DB_ADD_MONTHVIP,
		
		INSTALL_JUNQI,
		
		TAKE_SHELIZHIYUAN,
		
		DB_ADD_DBLSKILLUP,
		
		NEW_JIUHUA_ADDLIFE,
		
		NEW_LIANZHAN_DELAY,
		
		DB_ADD_THREE_EXP,
		
		DB_ADD_THREE_MONEY,
		
		DB_ADD_AF_PROTECT,
		
		NEW_INSTANT_ATTACK4,
		
		NEW_INSTANT_MAGIC4,
		
		NEW_TIME_MAGIC4,
		
		NEW_YINLIANG_RNDBAO,
		
		GOTO_LEAVELAOFANG,
		
		GOTO_MAPBYGOODS,
		
		SUB_ZUIEZHI,
		
		UN_PACK,
		
		GOTO_MAPBYVIP,
		
		GOTO_BATTLEMAP,
		
		FALL_BAOXIANG2,
		
		GOTO_SHILIANTA,
		
		NEW_ADD_GOLD,
		
		GOTO_SHENGXIAOGUESSMAP,
		
		GOTO_ARENABATTLEMAP,
		
		USE_GOODSFORDLG,
		
		DB_ADD_YINYONG,
		
		SUB_PKZHI,
		
		CALL_MONSTER,
		
		NEW_ADD_JIFEN,
		
		NEW_ADD_LIESHA,
		
		NEW_ADD_WUXING,
		
		NEW_ADD_ZHENQI,
		
		DB_ADD_TIANSHENG,
		
		ADD_XINGYUN,
		
		FALL_XINGYUN,
		
		NEW_PACK_SHILIAN,
		
		DB_NEW_ADD_ZHUFUTIME,
		
		NEW_ADD_MAPTIME,
		
		DB_ADD_WAWA_EXP,
		
		DB_TIME_LIFE_MAGIC,
		
		DB_INSTANT_LIFE_MAGIC,
		
		DB_ADD_MAXATTACKV,
		
		DB_ADD_MAXMATTACKV,
		
		DB_ADD_MAXDSATTACKV,
		
		DB_ADD_MAXDEFENSEV,
		
		DB_ADD_MAXMDEFENSEV,
		
		OPEN_QIAN_KUN_DAI,
		
		RUN_LUA_SCRIPT,
		
		DB_ADD_EXP,
		
		DB_ADD_SEASONVIP,
		
		DB_ADD_HALFYEARVIP,
		
		GOTO_MINGJIEMAP,
		
		ADD_GUMUMAPTIME,
		
		ADD_BOSSCOPYENTERNUM,
		
		GOTO_BOSSCOPYMAP,
		
		DB_ADD_FIVE_EXP,
		
		DB_ADD_RANDOM_EXP,
		
		GOTO_MAPBYYUANBAO,
		
		ADD_DAILY_NUM,
		
		DB_TIME_LIFE_NOSHOW,
		
		DB_TIME_MAGIC_NOSHOW,
		
		GOTO_GUMUMAP,
		
		ADD_PKKING_BUFF,
		
		DB_ADD_MULTIEXP,
		
		RANDOM_SHENQIZHIHUN,
		
		ADD_JIERI_BUFF,
		
		DB_ADD_ERGUOTOU,
		
		NEW_ADD_ZHANHUN,
		
		NEW_ADD_RONGYU,
		
		EXT_ATTACK_MABI,
		
		EXT_RESTORE_BLOOD,
		
		EXT_SUB_INJURE,
		
		DB_ADD_TEMPSTRENGTH,
		
		DB_ADD_TEMPINTELLIGENCE,
		
		DB_ADD_TEMPDEXTERITY,
		
		DB_ADD_TEMPCONSTITUTION,
		
		DB_ADD_TEMPATTACKSPEED,
		
		DB_ADD_LUCKYATTACK,
		
		DB_ADD_FATALATTACK,
		
		DB_ADD_DOUBLEATTACK,
		
		DB_ADD_LUCKYATTACKPERCENTTIMER,
		
		DB_ADD_FATALATTACKPERCENTTIMER,
		
		DB_ADD_DOUBLETACKPERCENTTIMER,
		
		DB_ADD_MAXHPVALUE,
		
		DB_ADD_MAXMPVALUE,
		
		DB_ADD_LIFERECOVERPERCENT,
		
		MU_ADD_PHYSICAL_ATTACK,
		
		MU_ADD_MAGIC_ATTACK,
		
		MU_SUB_DAMAGE_PERCENT_TIMER,
		
		MU_ADD_HP_PERCENT_TIMER,
		
		MU_ADD_DEFENSE_TIMER,
		
		MU_ADD_ATTACK_TIMER,
		
		MU_ADD_HP,
		
		MU_BLINK_MOVE,
		
		MU_SUB_DAMAGE_PERCENT_TIMER1,
		
		MU_RANDOM_SHUXING,
		
		MU_RANDOM_STRENGTH,
		
		MU_RANDOM_INTELLIGENCE,
		
		MU_RANDOM_DEXTERITY,
		
		MU_RANDOM_CONSTITUTION,
		
		MU_ADD_PHYSICAL_ATTACK1,
		
		MU_ADD_PHYSICAL_ATTACK2,
		
		MU_ADD_ATTACK_DOWN,
		
		MU_ADD_HUNMI,
		
		MU_ADD_MOVESPEED_DOWN,
		
		MU_ADD_LIFE,
		
		MU_ADD_MAGIC_ATTACK1,
		
		MU_ADD_MAGIC_ATTACK2,
		
		MU_ADD_HIT_DOWN,
		
		MU_SUB_DAMAGE_PERCENT,
		
		MU_SUB_DAMAGE_VALUE,
		
		MU_ADD_DEFENSE_DOWN,
		
		MU_ADD_DEFENSE_ATTACK,
		
		MU_ADD_JITUI,
		
		MU_ADD_DINGSHENG,
		
		MU_ADD_HIT_DODGE,
		
		MU_ADD_CHENMO,
		
		HUIFU,
		
		MU_ADD_YISHANG,
		
		NODIE,
		
		GOTO_BLOODCASTLE,
		
		GET_AWARD_BLOODCASTLE,
		
		GOTO_DAIMONSQUARE,
		
		GOTO_ANGELTEMPLE,
		
		SCAN_SQUARE,
		
		FRONT_SECTOR,
		
		ROUNDSCAN,
		
		OPEN_TREASURE_BOX,
		
		GOTO_BOOSZHIJIA,
		
		GOTO_HUANGJINSHENGDIAN,
		
		ADD_VIPEXP,
		
		GET_AWARD_BLOODCASTLECOPYSCENE,
		
		ADDMONSTERSKILL,
		
		REMOVEMONSTERSKILL,
		
		BOSS_CALLMONSTERONE,
		
		BOSS_CALLMONSTERTWO,
		
		CLEAR_MONSTER_BUFFERID,
		
		UP_LEVEL,
		
		ADD_GUANGMUI,
		
		CLEAR_GUANGMUI,
		
		FEIXUE,
		
		ZHONGDU,
		
		LINGHUN,
		
		RANSHAO,
		
		HUZHAO,
		
		WUDIHUZHAO,
		
		MU_FIRE_WALL1,
		
		MU_FIRE_WALL9,
		
		MU_FIRE_WALL25,
		
		MU_FIRE_WALL_X,
		
		MU_FIRE_WALL_Y,
		
		MU_FIRE_SECTOR,
		
		MU_FIRE_STRAIGHT,
		
		MU_FIRE_WALL_ACTION,
		
		BOSS_ADDANIMATION,
		
		MU_ADD_MOVE_SPEED_DOWN,
		
		MU_ADD_PALSY,
		
		MU_ADD_FROZEN,
		
		MU_GETSHIZHUANG,
		
		MU_ADD_BATI,
		
		POTION,
		
		HOLYWATER,
		
		RECOVERLIFEV,
		
		RECOVERMAGICV,
		
		LIFESTEAL,
		
		LIFESTEALPERCENT,
		
		FATALHURT,
		
		ADDATTACK,
		
		ADDATTACKINJURE,
		
		HITV,
		
		ADDDEFENSE,
		
		COUNTERACTINJUREVALUE,
		
		DAMAGETHORN,
		
		DODGE,
		
		MAXLIFEPERCENT,
		
		STRENGTH,
		
		CONSTITUTION,
		
		DEXTERITY,
		
		INTELLIGENCE,
		
		ADD_SHENGWU,
		
		ADD_SHENGBEI,
		
		ADD_SHENGJIAN,
		
		ADD_SHENGGUAN,
		
		ADD_SHENGDIAN,
		
		AddAttackPercent,
		
		AddDefensePercent,
		
		HitPercent,
		
		WOLF_SEARCH_ROAD,
		
		WOLF_ATTACK_ROLE,
		
		SELF_BURST,
		
		MU_ADD_PROPERTY,
		
		ADD_GAIMING,
		
		MU_ADD_DAMAGETHORN,
		
		MU_ADD_VAMPIRE,
		
		BUY_YUEKA,
		
		ActionSeveralTimesBegin,
		
		NEW_ADD_CHENGJIU,
		
		ADD_SHENGWANG,
		
		ADD_XINGHUN,
		
		NEW_PACK_JINGYUAN,
		
		ADDYSFM,
		
		ADD_LINGJING,
		
		ADD_ZAIZAO,
		
		ADD_RONGYAO,
		
		ADD_GUARDPOINT,
		
		NEW_ADD_YINGGUANG,
		
		ADD_BANGGONG,
		
		ADD_ZHANMENGGAIMING,
		
		ADD_LANGHUN,
		
		ADD_ZHENGBADIANSHU,
		
		ADD_WANGZHEDIANSHU,
		
		ADD_MEILIDIANSHU,
		
		ADD_SHENLIJINGHUA,
		
		NEW_ADD_MONEY,
		
		NEW_ADD_YINLIANG,
		
		ADD_DJ,
		
		ADD_BINDYUANBAO,
		
		ADD_GOODWILL,
		
		FALL_BAOXIANG,
		
		MU_RANDOMSHIZHUANG,
		
		ADD_LINGDICAIJI_COUNT,
		
		ADD_NENGLIANG,
		
		ADD_JINGLINGSHENJI,
		
		ADD_FUWENZHICHEN,
		
		ADD_JUEXINGZHICHEN,
		
		FALL_BAOXIANG_2,
		
		ADD_JUEXING,
		
		ADD_HUNJING,
		
		ADD_MOBI,
		
		ADD_JINGLINGJUEXINGSHI,
		
		ADD_FUMOLINGSHI,
		
		ADD_FENYINJINGSHI,
		
		ADD_CHONGSHENGJINGSHI,
		
		ADD_XUANCAIJINGSHI,
		
		ADD_REBORNEQUIP1,
		
		ADD_REBORNEQUIP2,
		
		ADD_REBORNEQUIP3,
		
		ADD_ZHANDUIRONGYAO,
		
		ADD_TEAMPOINT,
		
		ADD_LUCKSTAR_MOJING,
		
		ADD_LUCKSTAR,
		
		ActionSeveralTimesEnd,
		
		MU_XINGHONG,
		
		MU_LUHUO,
		
		MU_JUXIONG,
		
		MU_SUOMING,
		
		MU_ZHANZHENG,
		
		MU_MENGJING,
		
		MU_SUILU,
		
		MU_JIELV,
		
		MU_XUELANG,
		
		MU_BAIZHAN,
		
		MU_XIAOYONG,
		
		MU_MAOYOU,
		
		MU_ADD_OWN_ELEMENT_DAMAGE,
		
		MU_ADD_OWN_ELEMENT_REDUCTION,
		
		MU_REDUCE_TARGET_ELEMENT_REDUCTION,
		
		MU_ADD_OWN_DAMAGE_REBOUND,
		
		SET_WING,
		
		SET_MAX_WING,
		
		SET_ALLGOODS_LV,
		
		SET_GOODS_LV,
		
		SET_MAX_GUOSHI,
		
		SET_MAX_XINGHUN,
		
		SET_MEILIN,
		
		SET_MAINTASK,
		
		SET_LEVEL,
		
		DB_ADD_REBORNEXP,
		
		DB_ADD_REBORNEXP_MONSTERS_MAX,
		
		DB_ADD_REBORNEXP_GOODS_MAX,
		
		DB_ADD_MULTI_REBORNEXP,
		
		MAX
	}
}
