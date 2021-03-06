﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using BlatantShopping.Services;

namespace BlatantShopping
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 1)
			{
				Console.WriteLine("Expected 1 argument, found {0}\n\nUsage: {1} shoppingListFile\nWhere the shoppingListFile is a flat text file containing one product per line", args.Length, AppDomain.CurrentDomain.FriendlyName);
				Console.WriteLine();
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey();
				return;
			}

			var cartService = new CartService();
			var cart = cartService.GetCartFromFile(args[0]);

			Directory.SetCurrentDirectory(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "../../../../Input Files/");
			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalogFromFile("priceCatalog.json");

			var saleService = new SaleService();
			var sales = saleService.GetSalesFromFile("saleCatalog.json");

			Receipt receipt;
			decimal total = priceService.GetPrice(cart, priceCatalog, sales, out receipt);

			Console.WriteLine(receipt);
			Console.ReadKey();
		}
	}
}
