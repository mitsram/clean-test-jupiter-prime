using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JupiterPrime.Application.Interfaces
{
    public interface IShopService : IDisposable
    {
        void NavigateToShopPage();
        void AddItemToCart(string itemName);
        void RemoveItemFromCart(string itemName, int quantityToRemove = 1);
        int GetCartItemCount();
        bool IsItemInCart(string itemName);
        void NavigateToCartPage();
        List<string> GetAvailableItems();
        decimal GetItemPrice(string itemName);
        void SortItemsBy(string sortOption);
        bool IsItemAvailable(string itemName);
    }
}

