using JupiterPrime.Application.Interfaces;
using JupiterPrime.Application.Services;
using JupiterPrime.Application.UseCases;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JupiterPrime.Tests
{
    [TestFixture]
    public class ShopTests : BaseTest
    {
        private IWebDriverAdapter driver;
        private ShopUseCases shop;

        [SetUp]
        public override async Task SetupAsync()
        {
            await base.SetupAsync();
            driver = await webDriverFactory.CreateWebDriver();
            shop = new ShopUseCases(driver);
        }

        [TearDown]
        public override async Task TearDown()
        {
            shop.Dispose();
            await base.TearDown();
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void NavigateToShopPage_ShouldDisplayCorrectItems()
        {
            shop.NavigateToShopPage();
            var availableItems = shop.GetAvailableItems();

            Assert.That(availableItems, Has.Count.EqualTo(6), "There should be 6 items available");
            Assert.That(availableItems, Contains.Item("Jupiter Pilot Jacket"), "Jupiter Pilot Jacket should be available");
            Assert.That(availableItems, Contains.Item("Jupiter Adventure Pants"), "Jupiter Adventure Pants should be available");
            Assert.That(availableItems, Contains.Item("Jupiter T-Shirt"), "Jupiter T-Shirt should be available");
            Assert.That(availableItems, Contains.Item("Jupiter Boots"), "Jupiter Boots should be available");
            Assert.That(availableItems, Contains.Item("Jupiter Socks"), "Jupiter Socks should be available");
            Assert.That(availableItems, Contains.Item("Jupiter Backpack"), "Jupiter Backpack should be available");
        }

        [Test]
        public void AddItemToCart_ShouldIncreaseCartCount()
        {
            shop.NavigateToShopPage();
            int initialCount = shop.GetCartItemCount();

            shop.AddItemToCart("Jupiter Pilot Jacket");

            int newCount = shop.GetCartItemCount();
            Assert.That(newCount, Is.EqualTo(initialCount + 1), "Cart count should increase by 1");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void RemoveItemFromCart_ShouldDecreaseCartCount()
        {
            shop.NavigateToShopPage();
            shop.AddItemToCart("Jupiter Pilot Jacket");
            int initialCount = shop.GetCartItemCount();

            shop.RemoveItemFromCart("Jupiter Pilot Jacket");

            int newCount = shop.GetCartItemCount();
            Assert.That(newCount, Is.EqualTo(initialCount - 1), "Cart count should decrease by 1");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void IsItemInCart_ShouldReturnTrueForAddedItem()
        {
            shop.NavigateToShopPage();
            shop.AddItemToCart("Jupiter Pilot Jacket");

            bool isInCart = shop.IsItemInCart("Jupiter Pilot Jacket");

            Assert.That(isInCart, Is.True, "Added item should be in the cart");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void GetItemPrice_ShouldReturnCorrectPrice()
        {
            shop.NavigateToShopPage();

            decimal price = shop.GetItemPrice("Jupiter Pilot Jacket");

            Assert.That(price, Is.EqualTo(499.99m), "Jupiter Pilot Jacket price should be $499.99");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void SortItemsByPriceHighToLow_ShouldSortCorrectly()
        {
            shop.NavigateToShopPage();

            shop.SortItemsBy("Price (high to low)");
            var sortedItems = shop.GetAvailableItems();

            Assert.That(sortedItems.First(), Is.EqualTo("Jupiter Pilot Jacket"), "Jupiter Pilot Jacket should be the most expensive item");
            Assert.That(sortedItems.Last(), Is.EqualTo("Jupiter Socks"), "Jupiter Socks should be the least expensive item");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void SortItemsByPriceLowToHigh_ShouldSortCorrectly()
        {
            shop.NavigateToShopPage();

            shop.SortItemsBy("Price (low to high)");
            var sortedItems = shop.GetAvailableItems();

            Assert.That(sortedItems.First(), Is.EqualTo("Jupiter Socks"), "Jupiter Socks should be the least expensive item");
            Assert.That(sortedItems.Last(), Is.EqualTo("Jupiter Pilot Jacket"), "Jupiter Pilot Jacket should be the most expensive item");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void SortItemsByNameAToZ_ShouldSortCorrectly()
        {
            shop.NavigateToShopPage();

            shop.SortItemsBy("Name (A to Z)");
            var sortedItems = shop.GetAvailableItems();

            Assert.That(sortedItems.First(), Is.EqualTo("Jupiter Adventure Pants"), "Jupiter Adventure Pants should be first alphabetically");
            Assert.That(sortedItems.Last(), Is.EqualTo("Jupiter T-Shirt"), "Jupiter T-Shirt should be last alphabetically");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void SortItemsByNameZToA_ShouldSortCorrectly()
        {
            shop.NavigateToShopPage();

            shop.SortItemsBy("Name (Z to A)");
            var sortedItems = shop.GetAvailableItems();

            Assert.That(sortedItems.First(), Is.EqualTo("Jupiter T-Shirt"), "Jupiter T-Shirt should be first in reverse alphabetical order");
            Assert.That(sortedItems.Last(), Is.EqualTo("Jupiter Adventure Pants"), "Jupiter Adventure Pants should be last in reverse alphabetical order");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void AddMultipleItemsToCart_ShouldUpdateCartCorrectly()
        {
            shop.NavigateToShopPage();

            shop.AddItemToCart("Jupiter Pilot Jacket");
            shop.AddItemToCart("Jupiter Adventure Pants");
            shop.AddItemToCart("Jupiter T-Shirt");

            Assert.That(shop.GetCartItemCount(), Is.EqualTo(3), "Cart should contain 3 items");
            Assert.That(shop.IsItemInCart("Jupiter Pilot Jacket"), Is.True, "Jupiter Pilot Jacket should be in the cart");
            Assert.That(shop.IsItemInCart("Jupiter Adventure Pants"), Is.True, "Jupiter Adventure Pants should be in the cart");
            Assert.That(shop.IsItemInCart("Jupiter T-Shirt"), Is.True, "Jupiter T-Shirt should be in the cart");
        }

        [Test]
        [Ignore("This is how you skip a test")]
        public void NavigateToCartPage_ShouldDisplayCorrectItems()
        {
            shop.NavigateToShopPage();
            shop.AddItemToCart("Jupiter Pilot Jacket");
            shop.AddItemToCart("Jupiter Adventure Pants");

            shop.NavigateToCartPage();

            Assert.That(driver.GetCurrentUrl(), Does.Contain("/cart"), "URL should contain '/cart'");
            Assert.That(shop.IsItemInCart("Jupiter Pilot Jacket"), Is.True, "Jupiter Pilot Jacket should be in the cart");
            Assert.That(shop.IsItemInCart("Jupiter Adventure Pants"), Is.True, "Jupiter Adventure Pants should be in the cart");
        }
    }
}
