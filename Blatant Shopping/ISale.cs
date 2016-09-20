using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlatantShopping
{
	interface ISale
	{
		decimal GetSalePrice(int quantity);
		int QuantityAppliedTo(int quantity);
	}
}
