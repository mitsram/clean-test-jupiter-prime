using JupiterPrime.Application.Interfaces;

namespace JupiterPrime.Application.Services
{
    public class ShopService : IShopService, IDisposable
    {
        private readonly IWebDriverAdapter _driver;
        private const string ShopUrl = "http://jupiterprime-react-prod.s3-website.us-east-2.amazonaws.com/shop";
        private bool _disposed = false;

        public ShopService(IWebDriverAdapter driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public void NavigateToShopPage()
        {
            _driver.NavigateToUrl(ShopUrl);
        }

        public void AddItemToCart(string itemName)
        {            
            var productElements = _driver.FindElementsByTestId("product");            

            foreach (var productElement in productElements)
            {
                var nameElement = _driver.FindElementByDataLocator(productElement, "product-title");                
                if (nameElement.Text == itemName.Trim())
                {
                    var addToCartButton = _driver.FindElementByDataLocator(productElement, "add-to-cart-button");
                    addToCartButton.Click();
                    return;
                }
            }
            throw new Exception($"Product '{itemName}' not found.");
        }

        public void RemoveItemFromCart(string itemName, int quantityToRemove = 1)
        {
            // TODO: Improvement for multiple items dispose
            int remainingToRemove = quantityToRemove;
            int maxAttempts = 3;
            int attempt = 0;

            while (remainingToRemove > 0 && attempt < maxAttempts)
            {
                try
                {
                    _driver.WaitForElementToBeVisible("#cart-items");
                    var cartTable = _driver.FindElementById("cart-items");
                    var cartRows = _driver.FindChildElements(cartTable, "tbody tr");
                    Console.WriteLine($"Found {cartRows.Count} rows in the cart.");

                    bool itemFound = false;

                    foreach (var row in cartRows)
                    {
                        var cells = _driver.FindChildElements(row, "td");
                        if (cells.Count < 3)
                        {
                            Console.WriteLine($"Row does not have enough cells. Found {cells.Count} cells.");
                            continue;
                        }
                        var titleCell = cells.ElementAt(1);
                        var quantityCell = cells.ElementAt(0);
                        
                        if (titleCell.Text.Trim() == itemName.Trim())
                        {
                            itemFound = true;
                            int currentQuantity = int.Parse(quantityCell.Text);

                            var removeButton = _driver.FindElementByDataLocator(row, "remove-from-cart-button");
                            if (removeButton == null)
                            {
                                throw new Exception($"Remove button not found for {itemName}");
                            }                        
                            removeButton.Click();                            
                            _driver.WaitForElementToDisappear(removeButton);

                            // Wait for the cart count to update
                            var cartCountElement = _driver.FindElementById("cart-count");
                            _driver.WaitForTextToChange(cartCountElement, currentQuantity.ToString());

                            remainingToRemove--;

                            if (currentQuantity == 1 || remainingToRemove == 0)
                            {
                                Console.WriteLine($"Removed {quantityToRemove - remainingToRemove} {itemName} from cart.");
                                return;
                            }
                            // Break the loop to re-fetch the updated cart rows
                            break;
                        }
                    }
                    if (!itemFound)
                    {
                        throw new Exception($"Product '{itemName}' not found in the cart.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Attempt {attempt + 1} failed: {ex.Message}");
                    attempt++;
                    if (attempt >= maxAttempts)
                    {
                        throw;
                    }
                    Task.Delay(2000).Wait(); // Wait for 1 second before retrying
                }
            }
        }

        public int GetCartItemCount()
        {
            var cartBadge = _driver.FindElementById("cart-count");
            var badgeText = cartBadge.Text;
            return int.Parse(badgeText);
        }

        public bool IsItemInCart(string itemName)
        {
            try
            {
                _driver.WaitForElementToBeVisible("#cart-items");
                var cartTable = _driver.FindElementById("cart-items");
                var cartRows = _driver.FindChildElements(cartTable, "tbody tr");
                
                foreach (var row in cartRows)
                {
                    var cells = _driver.FindChildElements(row, "td");
                    if (cells.Count < 2) continue;
                    
                    var titleCell = cells.ElementAt(1);
                    
                    if (titleCell.Text.Trim().Equals(itemName.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking if item is in cart: {ex.Message}");
                // Optionally, you can rethrow the exception if you want the test to fail
                // throw;
            }
            
            return false;
        }

        public void NavigateToCartPage()
        {
            var cartLink = _driver.FindElementById("menu-cart");
            cartLink.Click();
        }

        public List<string> GetAvailableItems()
        {
            var itemElements = _driver.FindElementsByClassName("inventory_item_name");
            return itemElements.Select(e => e.Text).ToList();
        }

        public decimal GetItemPrice(string itemName)
        {
            var priceElement = _driver.FindElementByXPath($"//div[contains(@class, 'inventory_item_name') and text()='{itemName}']/ancestor::div[contains(@class, 'inventory_item')]//div[contains(@class, 'inventory_item_price')]");
            var priceText = priceElement.Text;
            return decimal.Parse(priceText.Replace("$", ""));
        }

        public void SortItemsBy(string sortOption)
        {
            var sortDropdown = _driver.FindElementByClassName("product_sort_container");
            sortDropdown.Click();
            var option = _driver.FindElementByXPath($"//option[text()='{sortOption}']");
            option.Click();
        }

        public bool IsItemAvailable(string itemName)
        {
            var items = GetAvailableItems();
            return items.Contains(itemName);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _driver.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
