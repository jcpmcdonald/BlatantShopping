using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlatantShopping
{
	class SalePrice : ISale
	{
		public decimal salePrice;


		public SalePrice(decimal salePrice)
		{
			this.salePrice = salePrice;
		}


		public decimal GetSalePrice(int quantity)
		{
			return salePrice * quantity;
		}

		public int QuantityAppliedTo(int quantity)
		{
			// Sale prices apply to any quantity of items
			return quantity;
		}
	}
}
