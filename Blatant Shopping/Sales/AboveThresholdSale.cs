﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlatantShopping.Sales
{
	class AboveThresholdSale : ISale
	{
		public int threshold;
		public decimal priceEachAfterThreshold;
		public String customReason = null;

		/// <summary>
		/// If threshold or more items are purchased, get a deal
		/// </summary>
		/// <param name="threshold">Deal is applied if the quantity purchased is greater than or equal to the threshold</param>
		/// <param name="priceEachAfterThreshold">Price each after threshold is met or exceeded</param>
		public AboveThresholdSale(int threshold, decimal priceEachAfterThreshold)
		{
			this.threshold = threshold;
			this.priceEachAfterThreshold = priceEachAfterThreshold;
		}


		public decimal GetSalePrice(int quantity)
		{
			return QuantityAppliedTo(quantity) * priceEachAfterThreshold;
		}


		public int QuantityAppliedTo(int quantity)
		{
			return (quantity >= threshold) ? quantity : 0;
		}


		public string GetReasoning(int quantity)
		{
			if (String.IsNullOrEmpty(customReason))
			{
				return String.Format("{0}@ Buy min {1} for {2:C}ea = {3:C}", QuantityAppliedTo(quantity), threshold, priceEachAfterThreshold, GetSalePrice(quantity));
			}
			else
			{
				return String.Format("{0}@ {1} = {2:C}", QuantityAppliedTo(quantity), customReason, GetSalePrice(quantity));
			}
		}
	}
}
