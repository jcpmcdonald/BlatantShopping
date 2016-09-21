using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
			// Throw away the receipt
			Receipt receipt;
			return GetPrice(shoppingList, priceCatalog, saleCatalog, out receipt);
		}



		/// <summary>
		/// Returns the best price for the provided shopping list
		/// </summary>
		/// <param name="shoppingList">The shopping list, provided in a dictionary&gt;&lt; of products and quantities</param>
		/// <param name="priceCatalog">A price catalog</param>
		/// <param name="saleCatalog">A catalog of all the current sales</param>
		/// <returns>The best price for the provided shopping list</returns>
		public decimal GetPrice(Dictionary<String, int> shoppingList, Dictionary<String, decimal> priceCatalog, Dictionary<String, List<ISale>> saleCatalog, out Receipt receipt)
		{
			decimal total = 0;
			decimal totalSavings = 0;
			receipt = new Receipt();

			// Go through each item in the shopping list and get the best price
			foreach (var item in shoppingList)
			{
				String product = item.Key;
				int quantity = item.Value;
				String saleReasoning = "";

				decimal regularPrice = GetRegularPrice(product, quantity, priceCatalog);
				decimal salePrice = GetPrice(product, quantity, priceCatalog, saleCatalog, ref saleReasoning);

				decimal pricePaid = Math.Min(salePrice, regularPrice);
				receipt.AddProduct(quantity, product, pricePaid);

				receipt.AddSaleReasoning(saleReasoning);

				if (salePrice < regularPrice)
				{
					decimal savings = regularPrice - salePrice;
					totalSavings += savings;

					receipt.AddSavings(savings);
				}

				total += pricePaid;
			}

			if (totalSavings > 0)
			{
				receipt.AddTotalSaved(totalSavings);
			}

			receipt.AddTotal(total);

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
			// Throw away the sale reasoning
			String saleReasoning = "";
			return GetPrice(product, quantity, priceCatalog, saleCatalog, ref saleReasoning);
		}


		/// <summary>
		/// Returns the best price for the provided product and quantity of product
		/// </summary>
		/// <param name="product">The product's name</param>
		/// <param name="quantity">The product's quantity</param>
		/// <param name="priceCatalog">A price catalog</param>
		/// <param name="saleCatalog">A catalog of all the current sales</param>
		/// <returns>The best price for the product and quantity</returns>
		private decimal GetPrice(String product, int quantity, Dictionary<String, decimal> priceCatalog, Dictionary<String, List<ISale>> saleCatalog, ref String saleReasoning)
		{
			// Base case for recursion
			if (quantity == 0)
			{
				return 0;
			}

			// Look for any sales, and recursively look for sales on the remaining items
			decimal bestSalePrice = decimal.MaxValue;
			String bestSaleReason = "";
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
						String leftoverSaleReason = "";

						decimal priceForLeftovers = GetPrice(product, quantityLeftOut, priceCatalog, saleCatalog, ref leftoverSaleReason);

						// If we have found a cheaper price, update the best sale price found
						if ((salePrice + priceForLeftovers) < bestSalePrice)
						{
							bestSalePrice = (salePrice + priceForLeftovers);
							if (String.IsNullOrEmpty(leftoverSaleReason))
							{
								bestSaleReason = sale.GetReasoning(quantity);
							}
							else
							{
								bestSaleReason = sale.GetReasoning(quantity) + "\n" + leftoverSaleReason;
							}
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
			if (bestSalePrice < regularPrice)
			{
				saleReasoning = bestSaleReason;
				return bestSalePrice;
			}
			else
			{
				saleReasoning = String.Format("{0}@ Regular {1}ea = {2}", quantity, priceCatalog[product.ToLower()], regularPrice);
				return regularPrice;
			}
		}
	}
}
