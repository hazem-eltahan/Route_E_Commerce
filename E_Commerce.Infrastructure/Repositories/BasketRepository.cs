using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Baskets;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class BasketRepository : IBasketRepository
    {
        private readonly IDatabase database;
        public BasketRepository(IConnectionMultiplexer connection)
        {
            database = connection.GetDatabase();
        }

        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? timeToLive = null, CancellationToken ct = default)
        {
            var value = JsonSerializer.Serialize(basket);
            var result = await database.StringSetAsync(basket.Id, value, timeToLive ?? TimeSpan.FromDays(7));

            return result ? basket : null;
        }

        public async Task<bool> DeleteBasketAsync(string basketId, CancellationToken ct = default)
        {
            return await database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string basketId, CancellationToken ct = default)
        {
            var basket = await database.StringGetAsync(basketId);

            return basket.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(basket!);
        }
    }
}
