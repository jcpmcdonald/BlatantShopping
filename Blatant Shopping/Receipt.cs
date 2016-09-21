using System;
using System.Collections.Generic;
using System.Text;

namespace BlatantShopping
{
	class Receipt
	{
		private StringBuilder receiptBuilder = new StringBuilder();


		public void AddProduct(int quantity, String product, decimal pricePaid)
		{
			receiptBuilder.AppendLine();
			receiptBuilder.AppendLine(String.Format("{0,4}{1,-30}{2,7:C}", quantity + "x ", product, pricePaid));
		}


		public void AddSavings(decimal savings)
		{
			receiptBuilder.AppendLine(String.Format("{0,8}{1,26}{2,8}", "", "You saved ", "($" + savings + ")"));
		}


		public void AddTotalSaved(decimal totalSavings)
		{
			receiptBuilder.AppendLine("".PadRight(42, '_'));
			receiptBuilder.AppendLine(String.Format("{0,8}{1,-26}{2,8}", "", "Total saved ", "($" + totalSavings + ")"));
		}

		public void AddTotal(decimal total)
		{
			receiptBuilder.AppendLine();
			receiptBuilder.AppendLine(String.Format("{0,-34}{1,8:C}", "Total ", total));
		}


		public void AddSaleReasoning(List<String> saleReasons)
		{
			foreach (var saleReason in saleReasons)
			{
				receiptBuilder.AppendLine(String.Format("{0,8}{1,-16}", "", saleReason));
			}
		}

		public override string ToString()
		{
			return receiptBuilder.ToString();
		}
	}
}
