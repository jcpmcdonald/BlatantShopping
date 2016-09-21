using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlatantShopping.Sales
{
	class BelowThresholdSale : ISale
	{
		public int threshold;
		public decimal priceEachBeforeThreshold;

		/// <summary>
		/// If threshold or fewer items are purchased, get a deal
		/// </summary>
		/// <param name="threshold">Deal is applied if the quantity purchased is less than or equal to the threshold</param>
		/// <param name="priceEachBeforeThreshold">Price each before threshold is exceeded</param>
		public BelowThresholdSale(int threshold, decimal priceEachBeforeThreshold)
		{
			this.threshold = threshold;
			this.priceEachBeforeThreshold = priceEachBeforeThreshold;
		}


		public decimal GetSalePrice(int quantity)
		{
			return QuantityAppliedTo(quantity) * priceEachBeforeThreshold;
		}


		public int QuantityAppliedTo(int quantity)
		{
			return (quantity <= threshold) ? quantity : 0;
		}
	}
}
