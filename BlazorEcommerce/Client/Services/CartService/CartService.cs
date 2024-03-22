
using BlazorEcommerce.Shared;
using BlazorEcommerce.Shared.DTOs;
using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public CartService(ILocalStorageService localStorage, 
            HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public event Action OnChange;

        public async Task AddToCartAsync(CartItem cartItem)
        {
            List<CartItem>? cart = await GetCartFromStorageAsync();

            var sameItem = cart.Find(i => i.ProductId == cartItem.ProductId 
                && i.ProductTypeId == cartItem.ProductTypeId);

            if (sameItem == null) 
            {
                cart.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _localStorage.SetItemAsync("cart", cart);

            OnChange.Invoke();
        }

        private async Task<List<CartItem>> GetCartFromStorageAsync()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            cart ??= new List<CartItem>();
            return cart;
        }

        public async Task<List<CartItem>> GetCartItemsAsync()
        {
            return await GetCartFromStorageAsync();
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);

            var cartProducts = await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();
            return cartProducts?.Data ?? new List<CartProductResponse>();
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null) return;

            var cartItem = cart.Find(i => i.ProductId == productId && i.ProductTypeId == productTypeId);
            if (cartItem == null) return;

            cart.Remove(cartItem);
            await _localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }

        public async Task UpdateQuantity(CartProductResponse product)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null) return;

            var cartItem = cart.Find(i => i.ProductId == product.ProductId && i.ProductTypeId == product.ProductTypeId);
            if (cartItem == null) return;

            cartItem.Quantity = product.Quantity;
            await _localStorage.SetItemAsync("cart", cart);
            OnChange.Invoke();
        }
    }
}
