using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BlatantShopping.Sales
{
	/// <summary>
	/// Buy 3 apples for $2
	/// </summary>
	class GroupSale : ISale
	{
		public int groupSize;
		public decimal pricePerGroup;


		public GroupSale(int groupSize, decimal pricePerGroup)
		{
			this.groupSize = groupSize;
			this.pricePerGroup = pricePerGroup;
		}


		public decimal GetSalePrice(int quantity)
		{
			return (int)(quantity / groupSize) * pricePerGroup;
		}

		public int QuantityAppliedTo(int quantity)
		{
			// Only applies to multiples of the group size
			return (quantity / groupSize) * groupSize;
		}
	}
}
