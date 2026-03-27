// ============================================================
// FILE: tests/InventoryManagement.IntegrationTests/Controllers/ProductsControllerTests.cs
// ============================================================
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using InventoryManagement.Application.DTOs;
using InventoryManagement.IntegrationTests.Infrastructure;
using Xunit;

namespace InventoryManagement.IntegrationTests.Controllers
{
    public class ProductsControllerTests : IClassFixture<InventoryApiFactory>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(InventoryApiFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> GetAdminTokenAsync()
        {
            var loginDto = new LoginDto { Username = "admin", Password = "Admin123!" };
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginDto);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed with status {response.StatusCode}. Response: {errorContent}");
            }
            
            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            return result!.AccessToken;
        }

        [Fact]
        public async Task GetAll_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/products");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetAll_WithAdminToken_ReturnsPaginatedProducts()
        {
            // Arrange
            string token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/products");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("totalCount");
            content.Should().Contain("Laptop");
            content.Should().Contain("Mouse");
        }

        [Fact]
        public async Task CreateProduct_ValidData_ReturnsCreatedProduct()
        {
            // Arrange
            string token = await GetAdminTokenAsync();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var newProduct = new CreateProductDto
            {
                SKU = "SKU-003",
                ProductName = "Keyboard",
                CategoryId = "C1"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/products", newProduct);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var result = await response.Content.ReadFromJsonAsync<ProductResponseDto>();
            result.Should().NotBeNull();
            result!.SKU.Should().Be("SKU-003");
            result.ProductName.Should().Be("Keyboard");
        }
    }
}
