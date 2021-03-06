﻿using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	
	public class SaleGoodsMoneyPerItemCompare : IComparer<SaleGoodsData>
	{
		
		public SaleGoodsMoneyPerItemCompare(int desc)
		{
			this.Desc = (desc != 0);
		}

		
		public int Compare(SaleGoodsData x, SaleGoodsData y)
		{
			int ret = 0;
			if (x.SalingGoodsData.GCount <= 0)
			{
				if (y.SalingGoodsData.GCount > 0)
				{
					ret = -1;
				}
			}
			else
			{
				if (y.SalingGoodsData.GCount <= 0)
				{
					return 1;
				}
				ret = x.SalingGoodsData.SaleYuanBao / x.SalingGoodsData.GCount - y.SalingGoodsData.SaleYuanBao / y.SalingGoodsData.GCount;
				if (ret == 0)
				{
					ret = x.SalingGoodsData.SaleMoney1 / x.SalingGoodsData.GCount - y.SalingGoodsData.SaleMoney1 / y.SalingGoodsData.GCount;
				}
			}
			if (this.Desc)
			{
				ret = -ret;
			}
			return ret;
		}

		
		public static readonly SaleGoodsMoneyPerItemCompare DescInstance = new SaleGoodsMoneyPerItemCompare(1);

		
		public static readonly SaleGoodsMoneyPerItemCompare AscInstance = new SaleGoodsMoneyPerItemCompare(0);

		
		private bool Desc = true;
	}
}
