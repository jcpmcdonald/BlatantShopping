using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

			// Apples for sale: $0.50 each
			var sales = new Dictionary<String, List<ISale>>{ { "apple", new List<ISale>{ new SalePrice(0.50m) } } };

			Assert.That(priceService.GetPrice(cart1, priceCatalog, sales),
			            Is.EqualTo(0.50m * 2));

			Assert.That(priceService.GetPrice(cart2, priceCatalog, sales),
			            Is.EqualTo((0.50m * 3) + 5m));
		}


		[Test]
		public void SimpleSaleLoadFromJson()
		{
			var saleService = new SaleService();
			var sales = saleService.GetSalesFromJson("{'apple':[{'$type':'BlatantShopping.SalePrice, BlatantShopping', 'salePrice':0.50}]}");
			
			Assert.That(sales["apple"][0].GetSalePrice(1), Is.EqualTo(0.50));
		}
	}
}
