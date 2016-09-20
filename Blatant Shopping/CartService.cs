using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlatantShopping
{
	class CartService
	{
		public Dictionary<String, decimal> GetCartFromFile(String filename)
		{
			return GetCartFromString(File.ReadAllLines(filename));
		}


		public Dictionary<String, decimal> GetCartFromString(String[] products)
		{
			Dictionary<String, decimal> shoppingCart = new Dictionary<String, decimal>();

			foreach (var product in products)
			{
				if (!shoppingCart.ContainsKey(product.ToLower()))
				{
					// Add a new product
					shoppingCart.Add(product.ToLower(), 1);
				}
				else
				{
					// Increment the quantity
					shoppingCart[product.ToLower()]++;
				}
			}

			return shoppingCart;
		}
	}
}
