﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BlatantShopping
{
	class PriceService
	{

		public Dictionary<String, decimal> GetPriceListFromFile(String filename)
		{
			return GetPriceList(File.ReadAllText(filename));
		}

		public Dictionary<String, decimal> GetPriceList(String json)
		{
			if (String.IsNullOrEmpty(json))
			{
				throw new ArgumentException("The parameter 'json' can not be null or empty");
			}

			// Lowercase all the JSON. The only string to be affected is the product name, which we want to be lower case
			var priceList = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(json.ToLowerInvariant());
			return priceList;
		}


		public decimal GetPrice(Dictionary<String, int> shoppingList, Dictionary<String, decimal> priceList, Dictionary<String, List<ISale>> saleList)
		{
			decimal total = 0;

			// Go through each item in the shopping list and get the best price
			foreach (var item in shoppingList)
			{
				total += GetPrice(item.Key, item.Value, priceList, saleList);
			}

			return total;
		}


		public decimal GetPrice(String product, int quantity, Dictionary<String, decimal> priceList, Dictionary<String, List<ISale>> saleList)
		{
			// Look for any sales
			decimal bestSalePrice = decimal.MaxValue;
			var sales = saleList[product.ToLower()];
			foreach (var sale in sales)
			{
				
			}

			// If any sales matched the product, but there are items that didn't fit in the sale, recursively find 


			// Look up the regular price
			decimal regularPrice = priceList[product] * quantity;

			// Return the cheapest price
			return Math.Min(bestSalePrice, regularPrice);
		}
	}
}
