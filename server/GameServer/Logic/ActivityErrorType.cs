﻿using System;

namespace GameServer.Logic
{
	// Token: 0x020006AA RID: 1706
	internal enum ActivityErrorType
	{
		// Token: 0x040035EF RID: 13807
		HEFULOGIN_NOTVIP = -100,
		// Token: 0x040035F0 RID: 13808
		FATALERROR = -60,
		// Token: 0x040035F1 RID: 13809
		AWARDCFG_ERROR = -50,
		// Token: 0x040035F2 RID: 13810
		AWARDTIME_OUT = -40,
		// Token: 0x040035F3 RID: 13811
		NOTCONDITION = -30,
		// Token: 0x040035F4 RID: 13812
		BAG_NOTENOUGH = -20,
		// Token: 0x040035F5 RID: 13813
		ALREADY_GETED = -10,
		// Token: 0x040035F6 RID: 13814
		MINAWARDCONDIONVALUE = -5,
		// Token: 0x040035F7 RID: 13815
		ACTIVITY_NOTEXIST = -1,
		// Token: 0x040035F8 RID: 13816
		RECEIVE_SUCCEED
	}
}