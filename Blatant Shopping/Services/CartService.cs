using System;
using System.Collections.Generic;
using System.IO;

namespace BlatantShopping.Services
{
	class CartService
	{
		public Dictionary<String, int> GetCartFromFile(String filename)
		{
			return GetCartFromString(File.ReadAllLines(filename));
		}


		public Dictionary<String, int> GetCartFromString(String[] products)
		{
			Dictionary<String, int> shoppingCart = new Dictionary<String, int>();

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
