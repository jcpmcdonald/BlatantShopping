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
		public void LoadPrices()
		{
			PriceService priceService = new PriceService();

			// Load a simple list
			var simplePriceList = priceService.GetPriceList(@"{ 'bananas': '0.75', 'apples': '1.75' }");
			Assert.That(simplePriceList, Is.Not.Null);
			Assert.That(simplePriceList["bananas"], Is.EqualTo(0.75m));
			Assert.That(simplePriceList["apples"], Is.EqualTo(1.75m));

			// Load an empty list
			var emptyPriceList = priceService.GetPriceList(@"{ }");
			Assert.That(emptyPriceList, Is.Not.Null);
			Assert.That(emptyPriceList.Count, Is.EqualTo(0));

			// Load a blank string. Fail Fast
			Assert.Throws<ArgumentException>(() => priceService.GetPriceList(@""));

			// Load a null. Fail Fast
			Assert.Throws<ArgumentException>(() => priceService.GetPriceList(null));

			// Load a list with duplicates, even though duplicate keys aren't technically allowed in JSON, and are strictly forbidden in a Dictionary<>
			var duplicatePriceList = priceService.GetPriceList(@"{ 'bananas': '0.75', 'apples': '1.75' , 'bananas': '0.60'}");
			Assert.That(duplicatePriceList, Is.Not.Null);
			// The last value for bananas should take effect
			Assert.That(duplicatePriceList["bananas"], Is.EqualTo(0.60m));

		}


		[Test]
		public void LoadSales()
		{

		}
	}
}
