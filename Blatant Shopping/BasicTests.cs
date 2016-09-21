using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlatantShopping.Sales;
using BlatantShopping.Services;
using Newtonsoft.Json;
using NUnit.Framework;

namespace BlatantShopping
{
	[TestFixture]
	internal class BasicTests
	{
		[Test]
		public void SimplePriceLoad()
		{
			var priceService = new PriceService();

			// Load a simple list
			var simplePriceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75' }");
			Assert.That(simplePriceCatalog, Is.Not.Null);
			Assert.That(simplePriceCatalog["banana"], Is.EqualTo(0.75m));
			Assert.That(simplePriceCatalog["apple"], Is.EqualTo(1.75m));
		}

		[Test]
		public void InvalidPriceLoads()
		{
			// All forms of empty price lists are invalid and should fail as soon as they are noticed

			var priceService = new PriceService();

			// Load an empty list
			Assert.Throws<ArgumentException>(() => priceService.GetPriceCatalog("{ }"));

			// Load a blank string
			Assert.Throws<ArgumentException>(() => priceService.GetPriceCatalog(""));

			// Load a null
			Assert.Throws<ArgumentException>(() => priceService.GetPriceCatalog(null));
		}

		[Test]
		public void DuplicatePriceLoad()
		{
			var priceService = new PriceService();

			// Load a list with duplicates, even though duplicate keys aren't technically allowed in JSON, and are strictly forbidden in a Dictionary<>
			var duplicatePriceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75' , 'banana': '0.60'}");
			Assert.That(duplicatePriceCatalog, Is.Not.Null);

			// The last value for banana should take effect
			Assert.That(duplicatePriceCatalog["banana"], Is.EqualTo(0.60m));
		}

		[Test]
		public void MixedCasePriceLoad()
		{
			var priceService = new PriceService();

			// Mixed case
			var mixedCasePriceCatalog = priceService.GetPriceCatalog("{ 'bAnAnA': '0.75', 'aPPle': '1.75' }");
			Assert.That(mixedCasePriceCatalog, Is.Not.Null);

			// ToLower() should always be used when looking up prices
			Assert.That(mixedCasePriceCatalog["baNaNa".ToLower()], Is.EqualTo(0.75m));
			Assert.That(mixedCasePriceCatalog["AppLe".ToLower()], Is.EqualTo(1.75m));
		}


		// TODO: Save some prices to a file, then load prices from the file


		[Test]
		public void RegularPriceItems()
		{
			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			Assert.That(priceService.GetRegularPrice("Apple", 1, priceCatalog), Is.EqualTo(1.75m));
			Assert.That(priceService.GetRegularPrice("Grapefruit", 5, priceCatalog), Is.EqualTo(5m * 5m));
		}

		[Test]
		public void SimpleCartLoad()
		{
			var cartService = new CartService();
			var cart = cartService.GetCartFromString(new[]{ "aPPle", "baNana" });
			Assert.That(cart, Is.Not.Null);
			Assert.That(cart["apple"], Is.EqualTo(1));
			Assert.That(cart["banana"], Is.EqualTo(1));
		}

		[Test]
		public void DuplicateCartLoad()
		{
			// Test loading duplicate, and out of order items
			var cartService = new CartService();
			var cart = cartService.GetCartFromString(new[] { "aPPle", "baNana", "appLe", "Banana", "bAnana" });
			Assert.That(cart, Is.Not.Null);
			Assert.That(cart["apple"], Is.EqualTo(2));
			Assert.That(cart["banana"], Is.EqualTo(3));
		}


		[Test]
		public void RegularPriceCart()
		{
			var cartService = new CartService();
			var cart1 = cartService.GetCartFromString(new[] { "apple", "apple" });
			var cart2 = cartService.GetCartFromString(new[] { "apple", "grapefruit", "grapefruit", "grapefruit", "grapefruit", "grapefruit" });

			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");
			

			Assert.That(priceService.GetPrice(cart1, priceCatalog, null),
			            Is.EqualTo(1.75m * 2));
			Assert.That(priceService.GetPrice(cart2, priceCatalog, null),
			            Is.EqualTo((1.75m * 1) + (5m * 5)));
		}


		[Test]
		public void BasicSalePrice()
		{
			var cartService = new CartService();
			var cart1 = cartService.GetCartFromString(new[] { "apple", "apple" });
			var cart2 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "grapefruit" });

			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			// Apples on sale for $1 each
			var sales = new Dictionary<String, List<ISale>>{ { "apple", new List<ISale>{ new SalePrice(1.00m) } } };

			Assert.That(priceService.GetPrice(cart1, priceCatalog, sales),
			            Is.EqualTo(1.00m * 2));

			Assert.That(priceService.GetPrice(cart2, priceCatalog, sales),
			            Is.EqualTo((1.00m * 3) + 5m));
		}


		[Test]
		public void SimpleSaleLoadFromJson()
		{
			var saleService = new SaleService();
			// Bring in the assembly name dynamically instead of hard-coding to allow for easier refactoring
			var sales = saleService.GetSalesFromJson(String.Format("{{'apple':[{{'$type':'{0}', 'salePrice':0.50}}]}}", typeof(SalePrice).AssemblyQualifiedName));
			
			Assert.That(sales["apple"][0].GetSalePrice(1), Is.EqualTo(0.50));
		}


		[Test]
		public void GroupSale()
		{
			var cartService = new CartService();
			var cart1 = cartService.GetCartFromString(new[] { "apple", "apple" });
			var cart2 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "grapefruit" });

			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			// Buy 2 apples for $2
			var sales = new Dictionary<String, List<ISale>> { { "apple", new List<ISale> { new GroupSale(2, 2.00m) } } };

			Assert.That(priceService.GetPrice(cart1, priceCatalog, sales),
						Is.EqualTo(2.00m));

			// First 2 apples are $2, 3rd apple is 1.75 (and grapefruit is $5)
			Assert.That(priceService.GetPrice(cart2, priceCatalog, sales),
						Is.EqualTo(2.00m + 1.75m + 5m));
		}


		[Test]
		public void GroupAndFixedSale()
		{
			var cartService = new CartService();
			var cart1 = cartService.GetCartFromString(new[] { "apple", "apple", "apple" });
			var cart2 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "apple" });

			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			// Buy 1 apple for $1.25
			// OR
			// Buy 2 apples for $2.00
			var sales = new Dictionary<String, List<ISale>> { { "apple", new List<ISale> { new GroupSale(2, 2.00m), new SalePrice(1.25m) } } };

			// Best combination of 3 apples is $3.25  (2 for $2 + 1 for $1.25)
			Assert.That(priceService.GetPrice(cart1, priceCatalog, sales),
						Is.EqualTo(3.25m));

			// Best combination of 4 apples is $4 (2 for $2, twice)
			Assert.That(priceService.GetPrice(cart2, priceCatalog, sales),
						Is.EqualTo(4.00m));
		}


		[Test]
		public void AboveThresholdSale()
		{
			var cartService = new CartService();
			var cart1 = cartService.GetCartFromString(new[] { "apple", "apple", "apple" }); // 3
			var cart2 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "apple" }); // 4
			var cart3 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "apple", "apple", "apple" }); // 6

			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			// Buy 4 or more apples for $0.50 each
			var sales = new Dictionary<String, List<ISale>> { { "apple", new List<ISale> { new AboveThresholdSale(4, 0.50m) } } };

			// This is under the threshold, no deal
			Assert.That(priceService.GetPrice(cart1, priceCatalog, sales),
						Is.EqualTo(1.75m * 3));

			// At the threshold
			Assert.That(priceService.GetPrice(cart2, priceCatalog, sales),
						Is.EqualTo(0.50m * 4));

			// Above the threshold
			Assert.That(priceService.GetPrice(cart3, priceCatalog, sales),
						Is.EqualTo(0.50m * 6));
		}


		[Test]
		public void BelowThresholdSale()
		{
			var cartService = new CartService();
			var cart1 = cartService.GetCartFromString(new[] { "apple", "apple", "apple" }); // 3
			var cart2 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "apple" }); // 4
			var cart3 = cartService.GetCartFromString(new[] { "apple", "apple", "apple", "apple", "apple", "apple" }); // 6

			var priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog("{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			// Buy 4 or fewer apples for $0.50 each
			var sales = new Dictionary<String, List<ISale>> { { "apple", new List<ISale> { new BelowThresholdSale(4, 0.50m) } } };

			// This is under the threshold
			Assert.That(priceService.GetPrice(cart1, priceCatalog, sales),
						Is.EqualTo(0.50m * 3));

			// At the threshold
			Assert.That(priceService.GetPrice(cart2, priceCatalog, sales),
						Is.EqualTo(0.50m * 4));

			// Above the threshold, no deal
			Assert.That(priceService.GetPrice(cart3, priceCatalog, sales),
						Is.EqualTo(1.75m * 6));
		}

	}
}
