using JupiterPrime.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var addToCartButton = _driver.FindElementByXPath($"//div[contains(@class, 'inventory_item_name') and text()='{itemName}']/ancestor::div[contains(@class, 'inventory_item')]//button[text()='Add to cart']");
            addToCartButton.Click();
        }

        public void RemoveItemFromCart(string itemName)
        {
            var removeButton = _driver.FindElementByXPath($"//div[contains(@class, 'inventory_item_name') and text()='{itemName}']/ancestor::div[contains(@class, 'inventory_item')]//button[text()='Remove']");
            removeButton.Click();
        }

        public int GetCartItemCount()
        {
            var cartBadge = _driver.FindElementById("cart-count");
            var badgeText = cartBadge.Text;
            return int.Parse(badgeText);
        }

        public bool IsItemInCart(string itemName)
        {
            NavigateToCartPage();
            var cartItems = _driver.FindElementsByClassName("inventory_item_name");
            return cartItems.Any(item => item.Text == itemName);
        }

        public void NavigateToCartPage()
        {
            var cartLink = _driver.FindElementByClassName("shopping_cart_link");
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
