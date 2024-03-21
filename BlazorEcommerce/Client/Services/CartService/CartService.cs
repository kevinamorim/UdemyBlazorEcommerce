
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
            cart.Add(cartItem);

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
    }
}
