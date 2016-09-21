using System;
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
	}
}
