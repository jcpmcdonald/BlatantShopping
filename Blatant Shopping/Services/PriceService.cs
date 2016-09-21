﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlatantShopping.Sales;
using Newtonsoft.Json;

namespace BlatantShopping.Services
{
	class PriceService
	{

		public Dictionary<String, decimal> GetPriceCatalogFromFile(String filename)
		{
			try
			{
				return GetPriceCatalog(File.ReadAllText(filename));
			}
			catch (ArgumentException ae)
			{
				// Fail closer to the real issue
				throw new InvalidDataException(String.Format("File '{0}' is null or empty", filename), ae);
			}
		}

		public Dictionary<String, decimal> GetPriceCatalog(String json)
		{
			if (String.IsNullOrEmpty(json))
			{
				throw new ArgumentException("The parameter 'json' can not be null or empty");
			}

			// Lowercase all the JSON. The only string to be affected is the product name, which we want to be lower case
			var priceList = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(json.ToLowerInvariant());

			if (!priceList.Any())
			{
				throw new ArgumentException("The price list is empty");
			}

			return priceList;
		}

		



		/// <summary>
		/// Returns the best price for the provided shopping list
		/// </summary>
		/// <param name="shoppingList">The shopping list, provided in a dictionary&gt;&lt; of products and quantities</param>
		/// <param name="priceCatalog">A price catalog</param>
		/// <param name="saleCatalog">A catalog of all the current sales</param>
		/// <returns>The best price for the provided shopping list</returns>
		public decimal GetPrice(Dictionary<String, int> shoppingList, Dictionary<String, decimal> priceCatalog, Dictionary<String, List<ISale>> saleCatalog)
		{
			decimal total = 0;

			// Go through each item in the shopping list and get the best price
			foreach (var item in shoppingList)
			{
				decimal regularPrice = GetRegularPrice(item.Key, item.Value, priceCatalog);
				decimal salePrice = GetPrice(item.Key, item.Value, priceCatalog, saleCatalog);

				// TODO: Add a line item for the product, with the amount saved (if any)

				total += Math.Min(salePrice, regularPrice);
			}

			return total;
		}


		public decimal GetRegularPrice(String product, int quantity, Dictionary<String, decimal> priceCatalog)
		{
			// Return the price without any sales
			return GetPrice(product, quantity, priceCatalog, null);
		}


		/// <summary>
		/// Returns the best price for the provided product and quantity of product
		/// </summary>
		/// <param name="product">The product's name</param>
		/// <param name="quantity">The product's quantity</param>
		/// <param name="priceCatalog">A price catalog</param>
		/// <param name="saleCatalog">A catalog of all the current sales</param>
		/// <returns>The best price for the product and quantity</returns>
		public decimal GetPrice(String product, int quantity, Dictionary<String, decimal> priceCatalog, Dictionary<String, List<ISale>> saleCatalog)
		{
			if (quantity == 0)
			{
				return 0;
			}

			// Look for any sales, and recursively look for sales on the remaining items
			decimal bestSalePrice = decimal.MaxValue;
			if (saleCatalog != null && saleCatalog.ContainsKey(product.ToLower()))
			{
				var sales = saleCatalog[product.ToLower()];
				foreach (var sale in sales)
				{
					// Check if this sale applies to the desired quantity
					int quantityAppliedTo = sale.QuantityAppliedTo(quantity);
					if (quantityAppliedTo > 0)
					{
						// See if any items were left out of the sale
						int quantityLeftOut = quantity - quantityAppliedTo;
						decimal salePrice = sale.GetSalePrice(quantity);

						// Get the best price for the leftovers
						decimal priceForLeftovers = GetPrice(product, quantityLeftOut, priceCatalog, saleCatalog);

						// If we have found a cheaper price, update the best sale price found
						if ((salePrice + priceForLeftovers) < bestSalePrice)
						{
							bestSalePrice = (salePrice + priceForLeftovers);
						}
					}
				}
			}


			// Look up the regular price
			decimal regularPrice;
			if (priceCatalog.ContainsKey(product.ToLower()))
			{
				regularPrice = priceCatalog[product.ToLower()] * quantity;
			}
			else
			{
				// Fail fast
				throw new Exception(String.Format("Product '{0}' is missing a regular price", product));
			}

			// Return the cheapest price.
			return Math.Min(bestSalePrice, regularPrice);
		}
	}
}
