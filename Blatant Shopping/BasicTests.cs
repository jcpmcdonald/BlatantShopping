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
			PriceService priceService = new PriceService();

			// Load a simple list
			var simplePriceCatalog = priceService.GetPriceCatalog(@"{ 'banana': '0.75', 'apple': '1.75' }");
			Assert.That(simplePriceCatalog, Is.Not.Null);
			Assert.That(simplePriceCatalog["banana"], Is.EqualTo(0.75m));
			Assert.That(simplePriceCatalog["apple"], Is.EqualTo(1.75m));
		}


		[Test]
		public void InvalidPriceLoads()
		{
			PriceService priceService = new PriceService();

			// Load an empty list
			Assert.Throws<ArgumentException>(() => priceService.GetPriceCatalog(@"{ }"));

			// Load a blank string. Fail Fast
			Assert.Throws<ArgumentException>(() => priceService.GetPriceCatalog(@""));

			// Load a null. Fail Fast
			Assert.Throws<ArgumentException>(() => priceService.GetPriceCatalog(null));
		}


		[Test]
		public void DuplicatePriceLoad()
		{
			PriceService priceService = new PriceService();

			// Load a list with duplicates, even though duplicate keys aren't technically allowed in JSON, and are strictly forbidden in a Dictionary<>
			var duplicatePriceCatalog = priceService.GetPriceCatalog(@"{ 'banana': '0.75', 'apple': '1.75' , 'banana': '0.60'}");
			Assert.That(duplicatePriceCatalog, Is.Not.Null);
			// The last value for bananas should take effect
			Assert.That(duplicatePriceCatalog["banana"], Is.EqualTo(0.60m));
		}

		[Test]
		public void MixedCasePriceLoad()
		{
			PriceService priceService = new PriceService();

			// Mixed case
			var mixedCasePriceCatalog = priceService.GetPriceCatalog(@"{ 'bAnAnA': '0.75', 'aPPle': '1.75' }");
			Assert.That(mixedCasePriceCatalog, Is.Not.Null);

			// ToLower() should always be used when looking up prices
			Assert.That(mixedCasePriceCatalog["baNaNa".ToLower()], Is.EqualTo(0.75m));
			Assert.That(mixedCasePriceCatalog["AppLe".ToLower()], Is.EqualTo(1.75m));
		}


		// TODO: Save some prices to a file, then load prices from the file


		[Test]
		public void RegularProductPriceTests()
		{
			PriceService priceService = new PriceService();
			var priceCatalog = priceService.GetPriceCatalog(@"{ 'banana': '0.75', 'apple': '1.75', 'orange': '2.50', 'grapefruit': '5.00'}");

			// No sales
			var saleCatalog = new Dictionary<String, List<ISale>>();

			Assert.That(priceService.GetRegularPrice("Apple", 1, priceCatalog), Is.EqualTo(1.75m));
			Assert.That(priceService.GetRegularPrice("Grapefruit", 5, priceCatalog), Is.EqualTo(5m * 5m));
		}
	}
}
