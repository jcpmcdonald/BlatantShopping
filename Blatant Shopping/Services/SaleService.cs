using System;
using System.Collections.Generic;
using System.IO;
using BlatantShopping.Sales;
using Newtonsoft.Json;

namespace BlatantShopping.Services
{
	class SaleService
	{
		public Dictionary<String, List<ISale>> GetSalesFromFile(String filename)
		{
			return GetSalesFromJson(File.ReadAllText(filename));
		}

		public Dictionary<String, List<ISale>> GetSalesFromStream(Stream stream)
		{
			StreamReader sr = new StreamReader(stream);
			return GetSalesFromJson(sr.ReadToEnd());
		}

		public Dictionary<String, List<ISale>> GetSalesFromJson(String json)
		{
			var settings = new JsonSerializerSettings();
			settings.TypeNameHandling = TypeNameHandling.Auto;
			var sales = JsonConvert.DeserializeObject<Dictionary<String, List<ISale>>>(json, settings);
			return sales;
		}
	}
}
