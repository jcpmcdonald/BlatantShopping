using System;
using System.Text;

namespace BlatantShopping
{
	class Receipt
	{
		private StringBuilder receiptBuilder = new StringBuilder();


		public void AddProduct(int quantity, String product, decimal pricePaid)
		{
			receiptBuilder.AppendLine(String.Format("{0,4}{1,-20}{2,7}", quantity + "x ", product, pricePaid));
		}


		public void AddSavings(decimal savings)
		{
			receiptBuilder.AppendLine(String.Format("{0,8}{1,-16}{2,7}", "", "You saved", "(-" + savings + ")"));
		}


		public void AddTotalSaved(decimal totalSavings)
		{
			receiptBuilder.AppendLine();
			receiptBuilder.AppendLine(String.Format("{0,8}{1,-16}{2,7}", "", "Total saved", "(-" + totalSavings + ")"));
		}


		public override string ToString()
		{
			return receiptBuilder.ToString();
		}
	}
}
