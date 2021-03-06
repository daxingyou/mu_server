﻿using System;

namespace GameServer.Logic.Name
{
	
	public enum ChangeNameError
	{
		
		Success,
		
		InvalidName,
		
		DBFailed,
		
		NoChangeNameTimes,
		
		SelfIsBusy,
		
		NameAlreayUsed,
		
		NameLengthError,
		
		NotContainRole,
		
		NeedVerifySecPwd,
		
		ZuanShiNotEnough,
		
		ServerDenied,
		
		BackToSelectRole
	}
}
