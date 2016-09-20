namespace BlatantShopping.Sales
{
	interface ISale
	{
		decimal GetSalePrice(int quantity);
		int QuantityAppliedTo(int quantity);
	}
}
