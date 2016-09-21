# Blatant Shopping
A console-based kiosk checkout system that calculates the best price for your shopping list based on the current prices and sales, then provides an itemized list and a total.

# Notes
- JSON is used for the price and sale catalogs
- NUnit is used for unit testing
- Exceptions are thrown to the user when there is an unrecoverable error. I try to [Fail Fast](https://en.wikipedia.org/wiki/Fail-fast), instead of progressing with flaws.

# Assumptions
- Product names are case-insensitive. Apples are aPpLes
- Sales and prices will be provided in valid JSON, preferably by a developer or a 3rd party tool (not included)
- The sale and price catalogs are available under the /Input Files/ directory of the repository, and are named saleCatalog.json and priceCatalog.json respectively.
- Sales will be manually added/removed when they become valid/invalid
- Products in the shopping cart must exist in price list. A store clerk will need to add the product to the price list, or remove it from the shopping cart.
- Sale prices will only be applied if they are cheaper than the regular price. Sales that increase the price will be ignored.
- Multiple sales can apply to a product purchase. For example:
  - Bananas are $0.75 regularly.
  - SALE 1: Buy 3 bananas for $1.25
  - SALE 2: Bananas are on sale for $0.50
  - If 4 bananas are purchased, 3 will sell for $1.25, and the last one will sell for $0.50, for a total of $1.75
- The "advanced requirement" sales are identical: The "Additional product discount" is a re-wording of the "Group promotional price". In both cases, you're selling bundles of a product for a reduced price (2 for 1, 3 for $1). I have implemented the "Group promotional price", which can be used as an "Additional product discount". In addition, I have added a "Custom Reason" that can read whatever the store would like (2 for 1, buy 2 get 1 50% off, etc)
- This application will only be used as an interactive console application. The application outputs the receipt, and there is no easy way to get the total price without parsing the output. The application also waits for a keypress when it is done printing the receipt.
