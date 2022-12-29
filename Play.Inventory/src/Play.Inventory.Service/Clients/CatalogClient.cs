using Play.Inventory.Service.DTO;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient httpClient;
        public CatalogClient(HttpClient httpClient)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemsAsync()
            => await httpClient?.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
    }

}
