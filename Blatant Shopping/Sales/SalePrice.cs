using System;

namespace BlatantShopping.Sales
{
	class SalePrice : ISale
	{
		public decimal salePrice;
		public String customReason = null;


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


		public string GetReasoning(int quantity)
		{
			if (String.IsNullOrEmpty(customReason))
			{
				return String.Format("{0}@ {1:C}ea = {2:C}", QuantityAppliedTo(quantity), salePrice, GetSalePrice(quantity));
			}
			else
			{
				return String.Format("{0}@ {1} = {2:C}", QuantityAppliedTo(quantity), customReason, GetSalePrice(quantity));
			}
		}
	}
}
