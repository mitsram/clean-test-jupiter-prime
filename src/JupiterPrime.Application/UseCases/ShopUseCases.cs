using JupiterPrime.Application.Interfaces;
using JupiterPrime.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JupiterPrime.Application.UseCases;

public class ShopUseCases : IDisposable
{   
    private readonly IShopService _shopService;
    private bool _disposed = false;

    public ShopUseCases(IWebDriverAdapter webDriver)
    {
        if (webDriver == null)
            throw new ArgumentNullException(nameof(webDriver));

        _shopService = new ShopService(webDriver);
    }

    public void NavigateToShopPage()
    {
        _shopService.NavigateToShopPage();
    }

    public void AddItemToCart(string itemName)
    {
        Console.WriteLine("---> Use Cases");
        _shopService.AddItemToCart(itemName);
        
    }

    public void RemoveItemFromCart(string itemName)
    {
        _shopService.RemoveItemFromCart(itemName);
    }

    public int GetCartItemCount()
    {
        return _shopService.GetCartItemCount();
    }

    public bool IsItemInCart(string itemName)
    {
        return _shopService.IsItemInCart(itemName);
    }

    public void NavigateToCartPage()
    {
        _shopService.NavigateToCartPage();
    }

    public List<string> GetAvailableItems()
    {
        return _shopService.GetAvailableItems();
    }

    public decimal GetItemPrice(string itemName)
    {
        return _shopService.GetItemPrice(itemName);
    }

    public void SortItemsBy(string sortOption)
    {
        _shopService.SortItemsBy(sortOption);
    }

    public bool IsItemAvailable(string itemName)
    {
        return _shopService.IsItemAvailable(itemName);
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
                (_shopService as IDisposable)?.Dispose();
            }

            _disposed = true;
        }
    }
}
