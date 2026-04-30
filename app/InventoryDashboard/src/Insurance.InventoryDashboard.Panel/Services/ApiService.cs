using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;

namespace Insurance.InventoryDashboard.Panel.Services;

internal sealed class CommandResultWrapper<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public List<string>? ErrorMessages { get; set; }
}

internal sealed class QueryResultWrapper<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class ApiService : IApiService
{
    private const string InventoryApiPrefix = "/api/InventoryService";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, IConfiguration configuration, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.BaseAddress = new Uri(configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7228");
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(string userName, string password)
    {
        var payload = new { UserName = userName, Password = password };
        return await PostCommandWithDataAsync<LoginResponse>(
            "/api/UserService/Auth/login/by-credential",
            payload,
            token: null,
            fallbackError: "Login request failed.");
    }

    public Task<ApiResponse<List<OrganizationViewModel>>> GetOrganizationsAsync(string token) =>
        GetQueryAsync<List<OrganizationViewModel>>("/api/UserService/Organization/get-all", token, "Loading organizations failed.");

    public Task<ApiResponse<List<UserSummaryModel>>> GetUsersAsync(string token) =>
        GetQueryAsync<List<UserSummaryModel>>("/api/UserService/User/get-all", token, "Loading users failed.");

    public Task<ApiResponse<List<PersonaSummaryModel>>> GetPersonasAsync(string token) =>
        GetQueryAsync<List<PersonaSummaryModel>>("/api/UserService/Persona/get-all", token, "Loading personas failed.");

    public Task<ApiResponse<List<PermissionSummaryModel>>> GetPermissionsAsync(string token) =>
        GetQueryAsync<List<PermissionSummaryModel>>("/api/UserService/Permission/get-all", token, "Loading permissions failed.");

    public Task<ApiResponse<bool>> AssignPersonaToUserAsync(string userId, string personaId, string token) =>
        PostCommandAsync("/api/UserService/UserPersona/assign", new { UserId = userId, PersonaId = personaId }, token, "Assigning persona to user failed.");

    public Task<ApiResponse<bool>> RemovePersonaFromUserAsync(string userId, string personaId, string token) =>
        PostCommandAsync("/api/UserService/UserPersona/remove", new { UserId = userId, PersonaId = personaId }, token, "Removing persona from user failed.");

    public Task<ApiResponse<bool>> AssignPermissionToPersonaAsync(string personaId, string permissionId, string token) =>
        PostCommandAsync("/api/UserService/PersonaPermission/assign", new { PersonaId = personaId, PermissionId = permissionId }, token, "Assigning permission to persona failed.");

    public Task<ApiResponse<bool>> RemovePermissionFromPersonaAsync(string personaId, string permissionId, string token) =>
        PostCommandAsync("/api/UserService/PersonaPermission/remove", new { PersonaId = personaId, PermissionId = permissionId }, token, "Removing permission from persona failed.");

    public Task<ApiResponse<bool>> CreateCategoryAsync(UpsertCategoryRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "CAT"),
            Name = request.Name.Trim(),
            DisplayOrder = request.DisplayOrder,
            ParentCategoryRef = ParseNullableGuid(request.ParentCategoryId)
        };

        return PostCommandAsync($"{InventoryApiPrefix}/Category", payload, token, "Creating category failed.");
    }

    public Task<ApiResponse<bool>> UpdateCategoryAsync(string categoryId, UpsertCategoryRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "CAT"),
            Name = request.Name.Trim(),
            DisplayOrder = request.DisplayOrder,
            ParentCategoryRef = ParseNullableGuid(request.ParentCategoryId),
            IsActive = true
        };

        return PutCommandAsync($"{InventoryApiPrefix}/Category/{categoryId}", payload, token, "Updating category failed.");
    }

    public Task<ApiResponse<bool>> MoveCategoryAsync(string categoryId, string? parentCategoryId, string token)
    {
        var payload = new
        {
            ParentCategoryRef = ParseNullableGuid(parentCategoryId)
        };

        return PostCommandAsync($"{InventoryApiPrefix}/Category/{categoryId}/move", payload, token, "Moving category failed.");
    }

    public Task<ApiResponse<bool>> ActivateCategoryAsync(string categoryId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Category/{categoryId}/activate", new { }, token, "Activating category failed.");

    public Task<ApiResponse<bool>> DeactivateCategoryAsync(string categoryId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Category/{categoryId}/deactivate", new { }, token, "Deactivating category failed.");

    public Task<ApiResponse<bool>> DeleteCategoryAsync(string categoryId, string token) =>
        DeleteCommandAsync($"{InventoryApiPrefix}/Category/{categoryId}", token, "Deleting category failed.");

    public async Task<ApiResponse<List<CategoryNodeModel>>> GetCategoryTreeAsync(string token)
    {
        var route = BuildRouteWithQuery($"{InventoryApiPrefix}/Category/tree", ("includeInactive", "true"));
        var result = await GetQueryAsync<CategoryTreeQueryResultDto>(route, token, "Loading category tree failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<CategoryNodeModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items
            .Select(MapCategoryTreeNode)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList()
            ?? new List<CategoryNodeModel>();

        return new ApiResponse<List<CategoryNodeModel>> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<List<CategoryAttributeRuleModel>>> GetCategoryAttributeRulesAsync(
        string categoryId,
        string token,
        bool includeInherited = true,
        bool includeInactive = true)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Category/{categoryId}/attribute-rules",
            ("includeInherited", includeInherited.ToString().ToLowerInvariant()),
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));

        var result = await GetQueryAsync<CategoryAttributeRulesQueryResultDto>(route, token, "Loading category rules failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<CategoryAttributeRuleModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(MapCategoryAttributeRule).ToList() ?? new List<CategoryAttributeRuleModel>();
        return new ApiResponse<List<CategoryAttributeRuleModel>> { IsSuccess = true, Data = mapped };
    }

    public Task<ApiResponse<bool>> UpdateCategoryAttributeRuleAsync(
        string categoryId,
        string attributeId,
        UpdateCategoryAttributeRuleRequest request,
        string token)
    {
        var payload = new
        {
            IsRequired = request.IsRequired,
            IsVariant = request.IsVariant,
            DisplayOrder = request.DisplayOrder,
            IsOverridden = request.IsOverridden,
            IsActive = request.IsActive
        };

        return PutCommandAsync(
            $"{InventoryApiPrefix}/Category/{categoryId}/attribute-rules/{attributeId}",
            payload,
            token,
            "Updating category attribute rule failed.");
    }

    public Task<ApiResponse<bool>> ActivateCategoryAttributeRuleAsync(string categoryId, string attributeId, string token) =>
        PostCommandAsync(
            $"{InventoryApiPrefix}/Category/{categoryId}/attribute-rules/{attributeId}/activate",
            new { },
            token,
            "Activating category attribute rule failed.");

    public Task<ApiResponse<bool>> DeactivateCategoryAttributeRuleAsync(string categoryId, string attributeId, string token) =>
        PostCommandAsync(
            $"{InventoryApiPrefix}/Category/{categoryId}/attribute-rules/{attributeId}/deactivate",
            new { },
            token,
            "Deactivating category attribute rule failed.");

    public Task<ApiResponse<bool>> RemoveCategoryAttributeRuleAsync(string categoryId, string attributeId, string token) =>
        DeleteCommandAsync(
            $"{InventoryApiPrefix}/Category/{categoryId}/attribute-rules/{attributeId}",
            token,
            "Removing category attribute rule failed.");

    public async Task<ApiResponse<bool>> CreateAttributeDefinitionAsync(string categoryId, CreateAttributeDefinitionRequest request, string token)
    {
        var categoryRef = ParseGuidOrEmpty(categoryId);
        if (categoryRef == Guid.Empty)
        {
            return new ApiResponse<bool> { IsSuccess = false, Data = false, ErrorMessage = "Invalid category id." };
        }

        var createPayload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "ATT"),
            Name = request.Name.Trim(),
            DataType = NormalizeAttributeDataType(request.DataType),
            Scope = NormalizeAttributeScope(request.Scope)
        };

        var createResult = await PostCommandWithDataAsync<CreateAttributeDefinitionCommandResultDto>(
            $"{InventoryApiPrefix}/AttributeDefinition",
            createPayload,
            token,
            "Creating attribute definition failed.");

        if (!createResult.IsSuccess || createResult.Data is null)
        {
            return new ApiResponse<bool>
            {
                IsSuccess = false,
                Data = false,
                ErrorMessage = createResult.ErrorMessage ?? "Creating attribute definition failed."
            };
        }

        var assignPayload = new
        {
            AttributeRef = createResult.Data.AttributeDefinitionBusinessKey,
            IsRequired = request.IsRequired,
            IsVariant = request.IsVariant,
            DisplayOrder = request.DisplayOrder,
            IsOverridden = false,
            IsActive = true
        };

        return await PostCommandAsync(
            $"{InventoryApiPrefix}/Category/{categoryRef:D}/attribute-rules",
            assignPayload,
            token,
            "Assigning attribute to category failed.");
    }

    public Task<ApiResponse<bool>> UpdateAttributeDefinitionAsync(string attributeId, UpdateAttributeDefinitionRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "ATT"),
            Name = request.Name.Trim(),
            DataType = NormalizeAttributeDataType(request.DataType),
            Scope = NormalizeAttributeScope(request.Scope),
            IsActive = request.IsActive,
            Options = Array.Empty<object>()
        };

        return PutCommandAsync(
            $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}",
            payload,
            token,
            "Updating attribute definition failed.");
    }

    public Task<ApiResponse<bool>> ActivateAttributeDefinitionAsync(string attributeId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/activate", new { }, token, "Activating attribute definition failed.");

    public Task<ApiResponse<bool>> DeactivateAttributeDefinitionAsync(string attributeId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/deactivate", new { }, token, "Deactivating attribute definition failed.");

    public Task<ApiResponse<bool>> DeleteAttributeDefinitionAsync(string attributeId, string token) =>
        DeleteCommandAsync($"{InventoryApiPrefix}/AttributeDefinition/{attributeId}", token, "Deleting attribute definition failed.");

    public Task<ApiResponse<bool>> AddAttributeOptionAsync(string attributeId, AddAttributeOptionRequest request, string token)
    {
        var payload = new
        {
            Name = request.OptionName.Trim(),
            Value = request.OptionValue.Trim(),
            DisplayOrder = request.DisplayOrder
        };

        return PostCommandAsync(
            $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options/add",
            payload,
            token,
            "Adding attribute option failed.");
    }

    public Task<ApiResponse<bool>> UpdateAttributeOptionAsync(string attributeId, string optionId, UpdateAttributeOptionRequest request, string token)
    {
        var payload = new
        {
            Name = request.OptionName.Trim(),
            Value = request.OptionValue.Trim(),
            DisplayOrder = request.DisplayOrder
        };

        return PutCommandAsync(
            $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options/{optionId}",
            payload,
            token,
            "Updating attribute option failed.");
    }

    public Task<ApiResponse<bool>> ActivateAttributeOptionAsync(string attributeId, string optionId, string token) =>
        PostCommandAsync(
            $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options/{optionId}/activate",
            new { },
            token,
            "Activating attribute option failed.");

    public Task<ApiResponse<bool>> DeactivateAttributeOptionAsync(string attributeId, string optionId, string token) =>
        PostCommandAsync(
            $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options/{optionId}/deactivate",
            new { },
            token,
            "Deactivating attribute option failed.");

    public Task<ApiResponse<bool>> DeleteAttributeOptionAsync(string attributeId, string optionId, string token) =>
        DeleteCommandAsync(
            $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options/{optionId}",
            token,
            "Deleting attribute option failed.");

    public Task<ApiResponse<bool>> AssignAttributeToCategoryAsync(string categoryId, string attributeId, string token) =>
        AddCategoryAttributeRuleAsync(
            categoryId,
            new AddCategoryAttributeRuleRequest
            {
                AttributeId = attributeId,
                IsRequired = false,
                IsVariant = false,
                DisplayOrder = 0,
                IsOverridden = false,
                IsActive = true
            },
            token);

    public async Task<ApiResponse<List<AttributeDefinitionModel>>> GetActiveAttributeDefinitionsAsync(string token)
    {
        var result = await GetQueryAsync<AttributeDefinitionsQueryResultDto>(
            $"{InventoryApiPrefix}/AttributeDefinition/active",
            token,
            "Loading active attribute definitions failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<AttributeDefinitionModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(MapAttributeDefinitionListItem).ToList() ?? new List<AttributeDefinitionModel>();
        return new ApiResponse<List<AttributeDefinitionModel>> { IsSuccess = true, Data = mapped };
    }

    public Task<ApiResponse<bool>> AddCategoryAttributeRuleAsync(string categoryId, AddCategoryAttributeRuleRequest request, string token)
    {
        var payload = new
        {
            AttributeRef = ParseGuidOrEmpty(request.AttributeId),
            IsRequired = request.IsRequired,
            IsVariant = request.IsVariant,
            DisplayOrder = request.DisplayOrder,
            IsOverridden = request.IsOverridden,
            IsActive = request.IsActive
        };

        return PostCommandAsync(
            $"{InventoryApiPrefix}/Category/{categoryId}/attribute-rules",
            payload,
            token,
            "Adding category attribute rule failed.");
    }

    public async Task<ApiResponse<List<AttributeDefinitionModel>>> GetCategoryAttributesAsync(
        string categoryId,
        string token,
        bool includeInherited = false,
        bool includeInactive = false)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Category/{categoryId}/attributes",
            ("includeInherited", includeInherited.ToString().ToLowerInvariant()),
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));

        var result = await GetQueryAsync<CategoryAttributesQueryResultDto>(route, token, "Loading category attributes failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<AttributeDefinitionModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(MapAttributeDefinition).ToList() ?? new List<AttributeDefinitionModel>();
        return new ApiResponse<List<AttributeDefinitionModel>> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<List<AttributeOptionModel>>> GetAttributeOptionsByAttributeIdAsync(string attributeId, string token, bool onlyActive = false)
    {
        var route = onlyActive
            ? $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options/active"
            : $"{InventoryApiPrefix}/AttributeDefinition/{attributeId}/options";

        var result = await GetQueryAsync<AttributeOptionsQueryResultDto>(route, token, "Loading attribute options failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<AttributeOptionModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(MapAttributeOption).ToList() ?? new List<AttributeOptionModel>();
        return new ApiResponse<List<AttributeOptionModel>> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<CreateProductResultModel>> CreateProductWithResultAsync(UpsertProductRequest request, string token)
    {
        var payload = new
        {
            CategoryRef = ParseGuidOrEmpty(request.CategoryId),
            BaseSku = request.BaseSku.Trim(),
            Name = request.Name.Trim(),
            DefaultUomRef = ParseGuidOrEmpty(request.DefaultUomRef),
            TaxCategoryRef = ParseNullableGuid(request.TaxCategoryRef),
            AttributeValues = MapAttributeValues(request.AttributeValues)
        };

        var result = await PostCommandWithDataAsync<CreateProductCommandResultDto>(
            $"{InventoryApiPrefix}/Product",
            payload,
            token,
            "Creating product failed.");

        if (!result.IsSuccess || result.Data is null)
        {
            return new ApiResponse<CreateProductResultModel>
            {
                IsSuccess = false,
                ErrorMessage = result.ErrorMessage ?? "Creating product failed."
            };
        }

        return new ApiResponse<CreateProductResultModel>
        {
            IsSuccess = true,
            Data = new CreateProductResultModel
            {
                ProductId = result.Data.ProductBusinessKey.ToString("D"),
                BaseSku = result.Data.BaseSku,
                Name = result.Data.Name,
                IsActive = result.Data.IsActive
            }
        };
    }

    public async Task<ApiResponse<bool>> CreateProductAsync(UpsertProductRequest request, string token)
    {
        var result = await CreateProductWithResultAsync(request, token);
        return new ApiResponse<bool>
        {
            IsSuccess = result.IsSuccess,
            Data = result.IsSuccess,
            ErrorMessage = result.ErrorMessage
        };
    }

    public Task<ApiResponse<bool>> UpdateProductAsync(string productId, UpsertProductRequest request, string token)
    {
        var payload = new
        {
            CategoryRef = ParseGuidOrEmpty(request.CategoryId),
            Name = request.Name.Trim(),
            DefaultUomRef = ParseGuidOrEmpty(request.DefaultUomRef),
            TaxCategoryRef = ParseNullableGuid(request.TaxCategoryRef),
            IsActive = request.IsActive
        };

        return PutCommandAsync($"{InventoryApiPrefix}/Product/{productId}", payload, token, "Updating product failed.");
    }

    public Task<ApiResponse<bool>> ActivateProductAsync(string productId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Product/{productId}/activate", new { }, token, "Activating product failed.");

    public Task<ApiResponse<bool>> DeactivateProductAsync(string productId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Product/{productId}/deactivate", new { }, token, "Deactivating product failed.");

    public Task<ApiResponse<bool>> DeleteProductAsync(string productId, string token) =>
        DeleteCommandAsync($"{InventoryApiPrefix}/Product/{productId}", token, "Deleting product failed.");

    public Task<ApiResponse<bool>> ChangeProductCategoryAsync(string productId, ChangeProductCategoryRequest request, string token)
    {
        var payload = new
        {
            CategoryRef = ParseGuidOrEmpty(request.CategoryId)
        };

        return PostCommandAsync($"{InventoryApiPrefix}/Product/{productId}/change-category", payload, token, "Changing product category failed.");
    }

    public Task<ApiResponse<bool>> SetProductAttributeValueAsync(string productId, SetProductAttributeValueRequest request, string token)
    {
        var payload = new
        {
            Value = string.IsNullOrWhiteSpace(request.Value) ? null : request.Value.Trim(),
            OptionRef = ParseNullableGuid(request.OptionId)
        };

        return PutCommandAsync(
            $"{InventoryApiPrefix}/Product/{productId}/attributes/{request.AttributeId}",
            payload,
            token,
            "Setting product attribute value failed.");
    }

    public Task<ApiResponse<bool>> RemoveProductAttributeValueAsync(string productId, string attributeId, string token) =>
        DeleteCommandAsync(
            $"{InventoryApiPrefix}/Product/{productId}/attributes/{attributeId}",
            token,
            "Removing product attribute value failed.");

    public async Task<ApiResponse<List<ProductAttributeValueModel>>> GetProductAttributeValuesByProductIdAsync(string productId, string token)
    {
        var details = await GetProductDetailsWithAttributesAsync(productId, token);
        if (!details.IsSuccess)
        {
            return new ApiResponse<List<ProductAttributeValueModel>>
            {
                IsSuccess = false,
                ErrorMessage = details.ErrorMessage
            };
        }

        return new ApiResponse<List<ProductAttributeValueModel>>
        {
            IsSuccess = true,
            Data = details.Data?.Attributes ?? new List<ProductAttributeValueModel>()
        };
    }

    public async Task<ApiResponse<List<ProductSummaryModel>>> SearchProductsAsync(
        string token,
        string? searchTerm = null,
        string? categoryId = null,
        int page = 1,
        int pageSize = 2000)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Product/search",
            ("Name", searchTerm),
            ("CategoryRef", categoryId),
            ("Page", page.ToString()),
            ("PageSize", pageSize.ToString()));

        var result = await GetQueryAsync<SearchProductsQueryResultDto>(route, token, "Loading products failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<ProductSummaryModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(item => new ProductSummaryModel
        {
            Id = item.ProductBusinessKey.ToString("D"),
            CategoryId = item.CategoryRef.ToString("D"),
            BaseSku = item.BaseSku,
            Name = item.Name,
            DefaultUomRef = item.DefaultUomRef.ToString("D"),
            TaxCategoryRef = item.TaxCategoryRef?.ToString("D"),
            IsActive = item.IsActive
        }).ToList() ?? new List<ProductSummaryModel>();

        return new ApiResponse<List<ProductSummaryModel>> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<ProductDetailsModel>> GetProductDetailsWithAttributesAsync(string productId, string token)
    {
        var route = $"{InventoryApiPrefix}/Product/{productId}/details/full";
        var result = await GetQueryAsync<ProductFullDetailsQueryResultDto>(route, token, "Loading product details failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<ProductDetailsModel> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        if (result.Data?.Item is null)
        {
            return new ApiResponse<ProductDetailsModel> { IsSuccess = false, ErrorMessage = "Product details were not found." };
        }

        var item = result.Data.Item;
        var mapped = new ProductDetailsModel
        {
            Id = item.Product.ProductBusinessKey.ToString("D"),
            CategoryId = item.Product.CategoryRef.ToString("D"),
            BaseSku = item.Product.BaseSku,
            Name = item.Product.Name,
            DefaultUomRef = item.Product.DefaultUomRef.ToString("D"),
            TaxCategoryRef = item.Product.TaxCategoryRef?.ToString("D"),
            IsActive = item.Product.IsActive,
            CategoryName = item.CategoryName,
            Attributes = item.ProductAttributes.Select(attribute => new ProductAttributeValueModel
            {
                AttributeId = attribute.AttributeRef.ToString("D"),
                AttributeCode = attribute.AttributeCode,
                AttributeName = attribute.AttributeName,
                DataType = attribute.DataType,
                Scope = attribute.Scope,
                Value = attribute.Value,
                OptionId = attribute.OptionRef?.ToString("D"),
                OptionValue = attribute.OptionValue,
                IsRequired = false
            }).ToList()
        };

        return new ApiResponse<ProductDetailsModel> { IsSuccess = true, Data = mapped };
    }

    public Task<ApiResponse<bool>> CreateProductVariantAsync(string productId, UpsertVariantRequest request, string token)
    {
        var payload = new
        {
            ProductRef = ParseGuidOrEmpty(productId),
            VariantSku = request.Sku.Trim(),
            Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim(),
            TrackingPolicy = request.TrackingPolicy.Trim(),
            BaseUomRef = ParseGuidOrEmpty(request.BaseUomRef),
            AttributeValues = MapAttributeValues(request.AttributeValues)
        };

        return PostCommandAsync($"{InventoryApiPrefix}/ProductVariant", payload, token, "Creating variant failed.");
    }

    public Task<ApiResponse<bool>> UpdateProductVariantAsync(string productId, string variantId, UpsertVariantRequest request, string token)
    {
        var payload = new
        {
            VariantSku = request.Sku.Trim(),
            Barcode = string.IsNullOrWhiteSpace(request.Barcode) ? null : request.Barcode.Trim(),
            TrackingPolicy = request.TrackingPolicy.Trim(),
            BaseUomRef = ParseGuidOrEmpty(request.BaseUomRef),
            IsActive = request.IsActive
        };

        return PutCommandAsync($"{InventoryApiPrefix}/ProductVariant/{variantId}", payload, token, "Updating variant failed.");
    }

    public Task<ApiResponse<bool>> ActivateProductVariantAsync(string variantId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/ProductVariant/{variantId}/activate", new { }, token, "Activating variant failed.");

    public Task<ApiResponse<bool>> DeactivateProductVariantAsync(string variantId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/ProductVariant/{variantId}/deactivate", new { }, token, "Deactivating variant failed.");

    public Task<ApiResponse<bool>> DeleteProductVariantAsync(string variantId, string token) =>
        DeleteCommandAsync($"{InventoryApiPrefix}/ProductVariant/{variantId}", token, "Deleting variant failed.");

    public Task<ApiResponse<bool>> ChangeProductVariantTrackingPolicyAsync(string variantId, ChangeVariantTrackingPolicyRequest request, string token)
    {
        var payload = new
        {
            TrackingPolicy = request.TrackingPolicy.Trim()
        };

        return PostCommandAsync(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/tracking-policy",
            payload,
            token,
            "Changing variant tracking policy failed.");
    }

    public Task<ApiResponse<bool>> ChangeProductVariantBaseUomAsync(string variantId, ChangeVariantBaseUomRequest request, string token)
    {
        var payload = new
        {
            BaseUomRef = ParseGuidOrEmpty(request.BaseUomRef)
        };

        return PostCommandAsync(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/base-uom",
            payload,
            token,
            "Changing variant base UOM failed.");
    }

    public Task<ApiResponse<bool>> LockProductVariantInventoryMovementAsync(string variantId, string token) =>
        PostCommandAsync(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/lock-inventory-movement",
            new { },
            token,
            "Locking variant inventory movement failed.");

    public Task<ApiResponse<bool>> SetVariantAttributeValueAsync(string variantId, SetVariantAttributeValueRequest request, string token)
    {
        var payload = new
        {
            Value = string.IsNullOrWhiteSpace(request.Value) ? null : request.Value.Trim(),
            OptionRef = ParseNullableGuid(request.OptionId)
        };

        return PutCommandAsync(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/attributes/{request.AttributeId}",
            payload,
            token,
            "Setting variant attribute value failed.");
    }

    public Task<ApiResponse<bool>> RemoveVariantAttributeValueAsync(string variantId, string attributeId, string token) =>
        DeleteCommandAsync(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/attributes/{attributeId}",
            token,
            "Removing variant attribute value failed.");

    public async Task<ApiResponse<List<VariantAttributeValueModel>>> GetVariantAttributeValuesByVariantIdAsync(string variantId, string token)
    {
        var details = await GetProductVariantFullDetailsAsync(variantId, token);
        if (!details.IsSuccess)
        {
            return new ApiResponse<List<VariantAttributeValueModel>>
            {
                IsSuccess = false,
                ErrorMessage = details.ErrorMessage
            };
        }

        return new ApiResponse<List<VariantAttributeValueModel>>
        {
            IsSuccess = true,
            Data = details.Data?.Attributes ?? new List<VariantAttributeValueModel>()
        };
    }

    public Task<ApiResponse<bool>> UpsertVariantUomConversionAsync(string variantId, UpsertVariantUomConversionRequest request, string token)
    {
        var payload = new
        {
            FromUomRef = ParseGuidOrEmpty(request.FromUomRef),
            ToUomRef = ParseGuidOrEmpty(request.ToUomRef),
            Factor = request.Factor,
            RoundingMode = ParseRoundingMode(request.RoundingMode),
            IsBasePath = request.IsBasePath
        };

        return PutCommandAsync(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/uom-conversions",
            payload,
            token,
            "Saving variant UOM conversion failed.");
    }

    public Task<ApiResponse<bool>> RemoveVariantUomConversionAsync(string variantId, string fromUomRef, string toUomRef, string token)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/ProductVariant/{variantId}/uom-conversions",
            ("fromUomRef", fromUomRef),
            ("toUomRef", toUomRef));

        return DeleteCommandAsync(route, token, "Removing variant UOM conversion failed.");
    }

    public async Task<ApiResponse<List<ProductVariantSummaryModel>>> GetProductVariantsByProductIdAsync(
        string productId,
        string token,
        bool includeInactive = true)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/ProductVariant/by-product/{productId}",
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));

        var result = await GetQueryAsync<VariantsByProductQueryResultDto>(route, token, "Loading variants failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<ProductVariantSummaryModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = MapVariantListItems(result.Data?.Items ?? new List<VariantListItemDto>());

        return new ApiResponse<List<ProductVariantSummaryModel>> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<ProductVariantSearchResultModel>> SearchProductVariantsAsync(
        string token,
        string? searchTerm = null,
        string? productId = null,
        string? categoryId = null,
        string? attributeOptionIds = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 10)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/ProductVariant/search",
            ("SearchTerm", searchTerm),
            ("ProductRef", productId),
            ("CategoryRef", categoryId),
            ("AttributeOptionRefs", attributeOptionIds),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        var result = await GetQueryAsync<SearchVariantsQueryResultDto>(route, token, "Loading variants failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<ProductVariantSearchResultModel> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        return new ApiResponse<ProductVariantSearchResultModel>
        {
            IsSuccess = true,
            Data = new ProductVariantSearchResultModel
            {
                TotalCount = result.Data?.TotalCount ?? 0,
                Page = result.Data?.Page ?? Math.Max(page, 1),
                PageSize = result.Data?.PageSize ?? Math.Clamp(pageSize, 1, 200),
                Items = MapVariantListItems(result.Data?.Items ?? new List<VariantListItemDto>())
            }
        };
    }

    public async Task<ApiResponse<ProductVariantDetailsModel>> GetProductVariantFullDetailsAsync(string variantId, string token)
    {
        var route = $"{InventoryApiPrefix}/ProductVariant/{variantId}/details/full";
        var result = await GetQueryAsync<VariantFullDetailsQueryResultDto>(route, token, "Loading variant details failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<ProductVariantDetailsModel> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        if (result.Data?.Item is null)
        {
            return new ApiResponse<ProductVariantDetailsModel> { IsSuccess = false, ErrorMessage = "Variant details were not found." };
        }

        var item = result.Data.Item;
        var mapped = new ProductVariantDetailsModel
        {
            Id = item.Variant.VariantBusinessKey.ToString("D"),
            ProductId = item.ProductBusinessKey.ToString("D"),
            Sku = item.Variant.VariantSku,
            Barcode = item.Variant.Barcode,
            BaseUomRef = item.Variant.BaseUomRef.ToString("D"),
            BaseUom = item.Variant.BaseUomRef.ToString("D"),
            TrackingPolicy = item.Variant.TrackingPolicy,
            IsActive = item.Variant.IsActive,
            InventoryMovementLocked = item.Variant.InventoryMovementLocked,
            ProductName = item.ProductName,
            ProductBaseSku = item.ProductBaseSku,
            CategoryId = item.CategoryBusinessKey.ToString("D"),
            CategoryName = item.CategoryName,
            Attributes = item.AttributeValues.Select(attribute => new VariantAttributeValueModel
            {
                AttributeId = attribute.AttributeRef.ToString("D"),
                AttributeCode = attribute.AttributeCode,
                AttributeName = attribute.AttributeName,
                DataType = attribute.DataType,
                Scope = attribute.Scope,
                Value = attribute.Value,
                OptionId = attribute.OptionRef?.ToString("D"),
                OptionValue = attribute.OptionValue
            }).ToList()
        };

        return new ApiResponse<ProductVariantDetailsModel> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<List<VariantUomConversionModel>>> GetVariantUomConversionsByVariantIdAsync(string variantId, string token)
    {
        var route = $"{InventoryApiPrefix}/ProductVariant/{variantId}/uom-conversions";
        var result = await GetQueryAsync<VariantUomConversionsQueryResultDto>(route, token, "Loading variant UOM conversions failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<VariantUomConversionModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(item => new VariantUomConversionModel
        {
            ConversionId = item.VariantUomConversionBusinessKey.ToString("D"),
            VariantId = item.VariantRef.ToString("D"),
            FromUomRef = item.FromUomRef.ToString("D"),
            ToUomRef = item.ToUomRef.ToString("D"),
            Factor = item.Factor,
            RoundingMode = item.RoundingMode,
            IsBasePath = item.IsBasePath
        }).ToList() ?? new List<VariantUomConversionModel>();

        return new ApiResponse<List<VariantUomConversionModel>> { IsSuccess = true, Data = mapped };
    }

    public async Task<ApiResponse<List<VariantInventoryTransactionModel>>> GetInventoryTransactionsByVariantAsync(string variantId, string token)
    {
        var result = await GetQueryAsync<ItemsQueryResultDto<VariantInventoryTransactionModel>>(
            $"{InventoryApiPrefix}/InventoryTransaction/by-variant/{variantId}",
            token,
            "Loading variant inventory documents failed.");

        if (!result.IsSuccess)
        {
            return new ApiResponse<List<VariantInventoryTransactionModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        return new ApiResponse<List<VariantInventoryTransactionModel>>
        {
            IsSuccess = true,
            Data = result.Data?.Items ?? new List<VariantInventoryTransactionModel>()
        };
    }

    public Task<ApiResponse<bool>> CreateWarehouseAsync(UpsertWarehouseRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "WH"),
            Name = request.Name.Trim()
        };

        return PostCommandAsync($"{InventoryApiPrefix}/Warehouse", payload, token, "Creating warehouse failed.");
    }

    public Task<ApiResponse<bool>> UpdateWarehouseAsync(string warehouseId, UpsertWarehouseRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "WH"),
            Name = request.Name.Trim(),
            IsActive = request.IsActive
        };

        return PutCommandAsync($"{InventoryApiPrefix}/Warehouse/{warehouseId}", payload, token, "Updating warehouse failed.");
    }

    public Task<ApiResponse<bool>> ActivateWarehouseAsync(string warehouseId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Warehouse/{warehouseId}/activate", new { }, token, "Activating warehouse failed.");

    public Task<ApiResponse<bool>> DeactivateWarehouseAsync(string warehouseId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Warehouse/{warehouseId}/deactivate", new { }, token, "Deactivating warehouse failed.");

    public Task<ApiResponse<bool>> DeleteWarehouseAsync(string warehouseId, string token) =>
        DeleteCommandAsync($"{InventoryApiPrefix}/Warehouse/{warehouseId}", token, "Deleting warehouse failed.");

    public Task<ApiResponse<WarehouseSearchResultModel>> SearchWarehousesAsync(
        string token,
        string? code = null,
        string? name = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Warehouse/search",
            ("Code", code),
            ("Name", name),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<WarehouseSearchResultModel>(route, token, "Loading warehouses failed.");
    }

    public async Task<ApiResponse<List<WarehouseLookupItemModel>>> GetWarehouseLookupAsync(string token, bool includeInactive = true)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Warehouse/lookup",
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));
        var result = await GetQueryAsync<WarehouseLookupQueryResultDto>(route, token, "Loading warehouse lookup failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<WarehouseLookupItemModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        return new ApiResponse<List<WarehouseLookupItemModel>> { IsSuccess = true, Data = result.Data?.Items ?? new List<WarehouseLookupItemModel>() };
    }

    public async Task<ApiResponse<WarehouseWithLocationsModel>> GetWarehouseWithLocationsAsync(string warehouseId, string token, bool includeInactiveLocations = true)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Warehouse/{warehouseId}/with-locations",
            ("includeInactiveLocations", includeInactiveLocations.ToString().ToLowerInvariant()));
        var result = await GetQueryAsync<WarehouseWithLocationsQueryResultDto>(route, token, "Loading warehouse details failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<WarehouseWithLocationsModel> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        return new ApiResponse<WarehouseWithLocationsModel> { IsSuccess = true, Data = result.Data?.Item };
    }

    public Task<ApiResponse<bool>> CreateLocationAsync(UpsertLocationRequest request, string token)
    {
        var payload = new
        {
            WarehouseRef = ParseNullableGuid(request.WarehouseId),
            LocationCode = NormalizeCode(request.LocationCode, request.LocationCode, "LOC"),
            LocationType = request.LocationType.Trim(),
            Aisle = NormalizeOptional(request.Aisle),
            Rack = NormalizeOptional(request.Rack),
            Shelf = NormalizeOptional(request.Shelf),
            Bin = NormalizeOptional(request.Bin)
        };

        return PostCommandAsync($"{InventoryApiPrefix}/Location", payload, token, "Creating location failed.");
    }

    public Task<ApiResponse<bool>> UpdateLocationAsync(string locationId, UpsertLocationRequest request, string token)
    {
        var payload = new
        {
            WarehouseRef = ParseNullableGuid(request.WarehouseId),
            LocationCode = NormalizeCode(request.LocationCode, request.LocationCode, "LOC"),
            LocationType = request.LocationType.Trim(),
            Aisle = NormalizeOptional(request.Aisle),
            Rack = NormalizeOptional(request.Rack),
            Shelf = NormalizeOptional(request.Shelf),
            Bin = NormalizeOptional(request.Bin),
            IsActive = request.IsActive
        };

        return PutCommandAsync($"{InventoryApiPrefix}/Location/{locationId}", payload, token, "Updating location failed.");
    }

    public Task<ApiResponse<bool>> ActivateLocationAsync(string locationId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Location/{locationId}/activate", new { }, token, "Activating location failed.");

    public Task<ApiResponse<bool>> DeactivateLocationAsync(string locationId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Location/{locationId}/deactivate", new { }, token, "Deactivating location failed.");

    public Task<ApiResponse<bool>> DeleteLocationAsync(string locationId, string token) =>
        DeleteCommandAsync($"{InventoryApiPrefix}/Location/{locationId}", token, "Deleting location failed.");

    public Task<ApiResponse<bool>> MoveLocationToWarehouseAsync(string locationId, string targetWarehouseId, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/Location/{locationId}/move-warehouse/{targetWarehouseId}", new { }, token, "Moving location failed.");

    public Task<ApiResponse<LocationSearchResultModel>> SearchLocationsAsync(
        string token,
        string? warehouseId = null,
        string? locationCode = null,
        string? locationType = null,
        string? aisle = null,
        string? rack = null,
        string? shelf = null,
        string? bin = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Location/search",
            ("WarehouseRef", warehouseId),
            ("LocationCode", locationCode),
            ("LocationType", locationType),
            ("Aisle", aisle),
            ("Rack", rack),
            ("Shelf", shelf),
            ("Bin", bin),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<LocationSearchResultModel>(route, token, "Loading locations failed.");
    }

    public async Task<ApiResponse<List<UnitOfMeasureLookupModel>>> GetUnitOfMeasureLookupAsync(string token)
    {
        var route = BuildRouteWithQuery($"{InventoryApiPrefix}/UnitOfMeasure/lookup", ("includeInactive", "false"));
        var result = await GetQueryAsync<UnitOfMeasureLookupQueryResultDto>(route, token, "Loading unit of measures failed.");
        if (!result.IsSuccess)
        {
            return new ApiResponse<List<UnitOfMeasureLookupModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        var mapped = result.Data?.Items.Select(x => new UnitOfMeasureLookupModel
        {
            Id = x.UnitOfMeasureBusinessKey.ToString("D"),
            Code = x.Code,
            Name = x.Name
        }).ToList() ?? new List<UnitOfMeasureLookupModel>();

        return new ApiResponse<List<UnitOfMeasureLookupModel>> { IsSuccess = true, Data = mapped };
    }

    public Task<ApiResponse<QualityStatusLookupResultModel>> GetQualityStatusLookupAsync(string token, bool includeInactive = false)
    {
        var route = BuildRouteWithQuery($"{InventoryApiPrefix}/QualityStatus/lookup", ("includeInactive", includeInactive.ToString().ToLowerInvariant()));
        return GetQueryAsync<QualityStatusLookupResultModel>(route, token, "Loading quality statuses failed.");
    }

    public Task<ApiResponse<InventoryDocumentSearchResultModel>> SearchInventoryDocumentsAsync(
        string token,
        string? documentNo = null,
        string? documentType = null,
        string? status = null,
        string? warehouseId = null,
        int page = 1,
        int pageSize = 20)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/InventoryDocument/search",
            ("DocumentNo", documentNo),
            ("DocumentType", documentType),
            ("Status", status),
            ("WarehouseRef", warehouseId),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<InventoryDocumentSearchResultModel>(route, token, "Loading inventory documents failed.");
    }

    public Task<ApiResponse<Guid>> CreateInventoryDocumentAsync(InventoryDocumentForm form, IReadOnlyList<InventoryDocumentLineForm> lines, string token)
    {
        var payloadLines = lines.Select(line =>
        {
            var qty = Math.Abs(line.Quantity);
            return new
            {
                VariantRef = ParseNullableGuid(line.VariantId),
                Qty = qty,
                UomRef = ParseNullableGuid(line.BaseUomRef),
                BaseQty = qty,
                BaseUomRef = ParseNullableGuid(line.BaseUomRef),
                SourceLocationRef = ParseNullableGuid(line.SourceLocationId),
                DestinationLocationRef = ParseNullableGuid(line.DestinationLocationId),
                QualityStatusRef = ParseNullableGuid(line.QualityStatusId),
                LotBatchNo = NormalizeOptional(line.LotBatchNo),
                ReasonCode = NormalizeOptional(line.ReasonCode ?? form.ReasonCode),
                AdjustmentDirection = NormalizeOptional(line.AdjustmentDirection)
            };
        }).ToList();

        var payload = new
        {
            DocumentNo = NormalizeOptional(form.DocumentNo),
            WarehouseRef = ParseNullableGuid(form.WarehouseId),
            SellerRef = ParseNullableGuid(form.SellerId),
            OccurredAt = form.OccurredAt == default ? DateTime.UtcNow : form.OccurredAt,
            ReasonCode = NormalizeOptional(form.ReasonCode),
            Lines = payloadLines
        };

        var route = (form.DocumentType ?? string.Empty).Trim().ToLowerInvariant() switch
        {
            "issue" => $"{InventoryApiPrefix}/InventoryDocument/issue",
            "transfer" => $"{InventoryApiPrefix}/InventoryDocument/transfer",
            "adjustment" => $"{InventoryApiPrefix}/InventoryDocument/adjustment",
            _ => $"{InventoryApiPrefix}/InventoryDocument/receipt"
        };

        return PostCommandWithDataAsync<Guid>(route, payload, token, "Creating inventory document failed.");
    }

    public Task<ApiResponse<bool>> PostInventoryDocumentAsync(Guid documentBusinessKey, string token) =>
        PostCommandAsync($"{InventoryApiPrefix}/InventoryDocument/{documentBusinessKey:D}/post", new { }, token, "Posting inventory document failed.");

    public async Task<ApiResponse<List<InventoryDocumentLineItemModel>>> GetInventoryDocumentLinesAsync(Guid documentBusinessKey, string token)
    {
        var result = await GetQueryAsync<ItemsQueryResultDto<InventoryDocumentLineItemModel>>(
            $"{InventoryApiPrefix}/InventoryDocument/{documentBusinessKey:D}/lines",
            token,
            "Loading inventory document lines failed.");

        if (!result.IsSuccess)
        {
            return new ApiResponse<List<InventoryDocumentLineItemModel>> { IsSuccess = false, ErrorMessage = result.ErrorMessage };
        }

        return new ApiResponse<List<InventoryDocumentLineItemModel>> { IsSuccess = true, Data = result.Data?.Items ?? new List<InventoryDocumentLineItemModel>() };
    }

    public Task<ApiResponse<PriceTypeSearchResultModel>> SearchPriceTypesAsync(
        string token,
        string? code = null,
        string? name = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 50)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/PriceType/search",
            ("Code", code),
            ("Name", name),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<PriceTypeSearchResultModel>(route, token, "Loading price types failed.");
    }

    public Task<ApiResponse<PriceTypeLookupResultModel>> GetPriceTypeLookupAsync(string token, bool includeInactive = false)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/PriceType/lookup",
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));

        return GetQueryAsync<PriceTypeLookupResultModel>(route, token, "Loading price type lookup failed.");
    }

    public Task<ApiResponse<bool>> CreatePriceTypeAsync(UpsertPriceTypeRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "PRICE_TYPE"),
            Name = request.Name.Trim()
        };

        return PostCommandAsync($"{InventoryApiPrefix}/PriceType", payload, token, "Creating price type failed.");
    }

    public Task<ApiResponse<bool>> UpdatePriceTypeAsync(Guid priceTypeId, UpsertPriceTypeRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "PRICE_TYPE"),
            Name = request.Name.Trim(),
            IsActive = request.IsActive
        };

        return PutCommandAsync($"{InventoryApiPrefix}/PriceType/{priceTypeId:D}", payload, token, "Updating price type failed.");
    }

    public Task<ApiResponse<PriceChannelSearchResultModel>> SearchPriceChannelsAsync(
        string token,
        string? code = null,
        string? name = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 50)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/PriceChannel/search",
            ("Code", code),
            ("Name", name),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<PriceChannelSearchResultModel>(route, token, "Loading price channels failed.");
    }

    public Task<ApiResponse<PriceChannelLookupResultModel>> GetPriceChannelLookupAsync(string token, bool includeInactive = false)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/PriceChannel/lookup",
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));

        return GetQueryAsync<PriceChannelLookupResultModel>(route, token, "Loading price channel lookup failed.");
    }

    public Task<ApiResponse<bool>> CreatePriceChannelAsync(UpsertPriceChannelRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "PRICE_CHANNEL"),
            Name = request.Name.Trim()
        };

        return PostCommandAsync($"{InventoryApiPrefix}/PriceChannel", payload, token, "Creating price channel failed.");
    }

    public Task<ApiResponse<bool>> UpdatePriceChannelAsync(Guid priceChannelId, UpsertPriceChannelRequest request, string token)
    {
        var payload = new
        {
            Code = NormalizeCode(request.Code, request.Name, "PRICE_CHANNEL"),
            Name = request.Name.Trim(),
            IsActive = request.IsActive
        };

        return PutCommandAsync($"{InventoryApiPrefix}/PriceChannel/{priceChannelId:D}", payload, token, "Updating price channel failed.");
    }

    public Task<ApiResponse<SellerVariantPriceSearchResultModel>> SearchSellerVariantPricesAsync(
        string token,
        Guid? sellerRef = null,
        Guid? variantRef = null,
        Guid? priceTypeRef = null,
        Guid? priceChannelRef = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 50)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/SellerVariantPrice/search",
            ("SellerRef", sellerRef?.ToString("D")),
            ("VariantRef", variantRef?.ToString("D")),
            ("PriceTypeRef", priceTypeRef?.ToString("D")),
            ("PriceChannelRef", priceChannelRef?.ToString("D")),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<SellerVariantPriceSearchResultModel>(route, token, "Loading variant prices failed.");
    }

    public Task<ApiResponse<bool>> CreateSellerVariantPriceAsync(UpsertSellerVariantPriceRequest request, string token)
    {
        var payload = BuildSellerVariantPricePayload(request, includeRefs: true);
        return PostCommandAsync($"{InventoryApiPrefix}/SellerVariantPrice", payload, token, "Creating variant price failed.");
    }

    public Task<ApiResponse<bool>> UpdateSellerVariantPriceAsync(Guid sellerVariantPriceId, UpsertSellerVariantPriceRequest request, string token)
    {
        var payload = BuildSellerVariantPricePayload(request, includeRefs: false);
        return PutCommandAsync($"{InventoryApiPrefix}/SellerVariantPrice/{sellerVariantPriceId:D}", payload, token, "Updating variant price failed.");
    }

    public Task<ApiResponse<SellerLookupResultModel>> GetSellerLookupAsync(string token, bool includeInactive = false)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Seller/lookup",
            ("includeInactive", includeInactive.ToString().ToLowerInvariant()));

        return GetQueryAsync<SellerLookupResultModel>(route, token, "Loading sellers failed.");
    }

    public Task<ApiResponse<SellerSearchResultModel>> SearchSellersAsync(
        string token,
        string? code = null,
        string? name = null,
        bool? isSystemOwner = null,
        bool? isActive = null,
        int page = 1,
        int pageSize = 20)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/Seller/search",
            ("Code", code),
            ("Name", name),
            ("IsSystemOwner", isSystemOwner?.ToString().ToLowerInvariant()),
            ("IsActive", isActive?.ToString().ToLowerInvariant()),
            ("Page", Math.Max(page, 1).ToString()),
            ("PageSize", Math.Clamp(pageSize, 1, 200).ToString()));

        return GetQueryAsync<SellerSearchResultModel>(route, token, "Loading sellers failed.");
    }

    public Task<ApiResponse<StockDetailBucketResultModel>> GetAvailableStockBucketsAsync(
        string token,
        Guid? variantRef = null,
        Guid? sellerRef = null,
        decimal minQuantity = 0)
    {
        var route = BuildRouteWithQuery(
            $"{InventoryApiPrefix}/StockDetail/available-buckets",
            ("VariantRef", variantRef?.ToString("D")),
            ("SellerRef", sellerRef?.ToString("D")),
            ("MinQuantity", minQuantity.ToString(System.Globalization.CultureInfo.InvariantCulture)));

        return GetQueryAsync<StockDetailBucketResultModel>(route, token, "Loading available stock buckets failed.");
    }

    private static CategoryNodeModel MapCategoryTreeNode(CategoryTreeItemDto item)
    {
        var children = item.Children
            .Select(MapCategoryTreeNode)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .ToList();

        return new CategoryNodeModel
        {
            Id = item.CategoryBusinessKey.ToString("D"),
            Code = item.Code,
            Name = item.Name,
            ParentCategoryId = item.ParentCategoryRef?.ToString("D"),
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive,
            Children = children
        };
    }

    private static AttributeDefinitionModel MapAttributeDefinition(CategoryAttributeItemDto item)
    {
        return new AttributeDefinitionModel
        {
            Id = item.AttributeRef.ToString("D"),
            Code = item.AttributeCode,
            Name = item.AttributeName,
            DataType = item.DataType,
            Scope = item.Scope,
            IsActive = item.AttributeIsActive,
            IsRequired = item.RuleIsRequired,
            IsVariant = item.RuleIsVariant,
            DisplayOrder = item.RuleDisplayOrder,
            IsInherited = item.IsInherited,
            SourceCategoryId = item.SourceCategoryRef.ToString("D"),
            SourceCategoryName = item.SourceCategoryName,
            Options = item.Options.Select(option => new AttributeOptionModel
            {
                Id = option.OptionBusinessKey.ToString("D"),
                OptionName = string.IsNullOrWhiteSpace(option.Name) ? option.Value : option.Name,
                OptionValue = option.Value,
                DisplayOrder = option.DisplayOrder,
                IsActive = option.IsActive
            }).ToList()
        };
    }

    private static AttributeDefinitionModel MapAttributeDefinitionListItem(AttributeDefinitionListItemDto item)
    {
        return new AttributeDefinitionModel
        {
            Id = item.AttributeDefinitionBusinessKey.ToString("D"),
            Code = item.Code,
            Name = item.Name,
            DataType = item.DataType,
            Scope = item.Scope,
            IsActive = item.IsActive
        };
    }

    private static CategoryAttributeRuleModel MapCategoryAttributeRule(CategoryAttributeRuleItemDto item)
    {
        return new CategoryAttributeRuleModel
        {
            RuleId = item.RuleBusinessKey.ToString("D"),
            CategoryId = item.CategoryBusinessKey.ToString("D"),
            AttributeId = item.AttributeRef.ToString("D"),
            AttributeCode = item.AttributeCode,
            AttributeName = item.AttributeName,
            DataType = item.DataType,
            Scope = item.Scope,
            AttributeIsActive = item.AttributeIsActive,
            RuleIsRequired = item.RuleIsRequired,
            RuleIsVariant = item.RuleIsVariant,
            RuleDisplayOrder = item.RuleDisplayOrder,
            RuleIsOverridden = item.RuleIsOverridden,
            RuleIsActive = item.RuleIsActive,
            IsInherited = item.IsInherited,
            SourceCategoryId = item.SourceCategoryRef.ToString("D"),
            SourceCategoryCode = item.SourceCategoryCode,
            SourceCategoryName = item.SourceCategoryName,
            Options = item.Options.Select(MapAttributeOption).ToList()
        };
    }

    private static AttributeOptionModel MapAttributeOption(AttributeOptionItemDto item)
    {
        return new AttributeOptionModel
        {
            Id = item.OptionBusinessKey.ToString("D"),
            OptionName = string.IsNullOrWhiteSpace(item.Name) ? item.Value : item.Name,
            OptionValue = item.Value,
            DisplayOrder = item.DisplayOrder,
            IsActive = item.IsActive
        };
    }

    private static List<ProductVariantSummaryModel> MapVariantListItems(IEnumerable<VariantListItemDto> items)
        => items.Select(item => new ProductVariantSummaryModel
        {
            Id = item.VariantBusinessKey.ToString("D"),
            ProductId = item.ProductRef.ToString("D"),
            Sku = item.VariantSku,
            Barcode = item.Barcode,
            TrackingPolicy = item.TrackingPolicy,
            BaseUomRef = item.BaseUomRef.ToString("D"),
            BaseUom = item.BaseUomRef.ToString("D"),
            IsActive = item.IsActive,
            InventoryMovementLocked = item.InventoryMovementLocked
        }).ToList();

    private static string NormalizeAttributeDataType(string? dataType)
    {
        if (string.IsNullOrWhiteSpace(dataType))
        {
            return "Text";
        }

        var normalized = dataType.Trim().ToLowerInvariant();
        return normalized switch
        {
            "string" => "Text",
            "text" => "Text",
            "number" => "Number",
            "boolean" => "Boolean",
            "date" => "Date",
            "enum" => "Option",
            "select" => "Option",
            "option" => "Option",
            _ => "Text"
        };
    }

    private static string NormalizeAttributeScope(string? scope)
    {
        if (string.IsNullOrWhiteSpace(scope))
        {
            return "Both";
        }

        var normalized = scope.Trim().ToLowerInvariant();
        return normalized switch
        {
            "product" => "Product",
            "variant" => "Variant",
            _ => "Both"
        };
    }

    private static string NormalizeCode(string? code, string? fallbackSource, string prefix)
    {
        var raw = string.IsNullOrWhiteSpace(code) ? fallbackSource : code;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return $"{prefix}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        var validChars = raw.Trim().ToUpperInvariant()
            .Select(ch => char.IsLetterOrDigit(ch) ? ch : '_')
            .ToArray();

        var collapsed = new string(validChars);
        while (collapsed.Contains("__", StringComparison.Ordinal))
        {
            collapsed = collapsed.Replace("__", "_", StringComparison.Ordinal);
        }

        collapsed = collapsed.Trim('_');
        if (string.IsNullOrWhiteSpace(collapsed))
        {
            return $"{prefix}_{DateTime.UtcNow:yyyyMMddHHmmss}";
        }

        return collapsed.Length <= 64 ? collapsed : collapsed[..64];
    }

    private static IReadOnlyList<object> MapAttributeValues(IEnumerable<CatalogAttributeValueInputModel>? items)
    {
        return (items ?? Array.Empty<CatalogAttributeValueInputModel>())
            .Where(x => !string.IsNullOrWhiteSpace(x.AttributeId))
            .Select(x => new
            {
                AttributeRef = ParseGuidOrEmpty(x.AttributeId),
                Value = string.IsNullOrWhiteSpace(x.Value) ? null : x.Value.Trim(),
                OptionRef = ParseNullableGuid(x.OptionId)
            })
            .Where(x => x.AttributeRef != Guid.Empty)
            .ToList<object>();
    }

    private static object BuildSellerVariantPricePayload(UpsertSellerVariantPriceRequest request, bool includeRefs)
    {
        var common = new
        {
            Amount = request.Amount,
            Currency = string.IsNullOrWhiteSpace(request.Currency) ? "IRR" : request.Currency.Trim().ToUpperInvariant(),
            MinQty = request.MinQty <= 0 ? 1 : request.MinQty,
            Priority = request.Priority,
            EffectiveFrom = request.EffectiveFrom,
            EffectiveTo = request.EffectiveTo,
            IsActive = request.IsActive,
            Offers = request.Offers ?? new List<SellerVariantPriceOfferInputModel>()
        };

        if (!includeRefs)
        {
            return common;
        }

        return new
        {
            request.SellerRef,
            request.VariantRef,
            request.PriceTypeRef,
            request.PriceChannelRef,
            common.Amount,
            common.Currency,
            common.MinQty,
            common.Priority,
            common.EffectiveFrom,
            common.EffectiveTo,
            common.Offers
        };
    }

    private static Guid ParseGuidOrEmpty(string? value)
    {
        return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
    }

    private static int ParseRoundingMode(string? roundingMode)
    {
        if (string.IsNullOrWhiteSpace(roundingMode))
        {
            return 1;
        }

        return roundingMode.Trim().ToLowerInvariant() switch
        {
            "up" => 2,
            "down" => 3,
            "nearest" => 4,
            _ => 1
        };
    }

    private static Guid? ParseNullableGuid(string? value)
    {
        return Guid.TryParse(value, out var guid) ? guid : null;
    }

    private static string? NormalizeOptional(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static string BuildRouteWithQuery(string route, params (string Key, string? Value)[] queryItems)
    {
        var items = queryItems
            .Where(item => !string.IsNullOrWhiteSpace(item.Value))
            .Select(item => $"{Uri.EscapeDataString(item.Key)}={Uri.EscapeDataString(item.Value!)}")
            .ToList();

        if (items.Count == 0)
        {
            return route;
        }

        return $"{route}?{string.Join("&", items)}";
    }

    private async Task<ApiResponse<T>> GetQueryAsync<T>(string route, string token, string fallbackError)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("GET {Route} failed: {StatusCode} / {Body}", route, response.StatusCode, body);
                return new ApiResponse<T> { IsSuccess = false, ErrorMessage = fallbackError };
            }

            var result = JsonSerializer.Deserialize<QueryResultWrapper<T>>(body, JsonOptions);
            if (result is { IsSuccess: true, Data: not null })
            {
                return new ApiResponse<T> { IsSuccess = true, Data = result.Data };
            }

            return new ApiResponse<T> { IsSuccess = false, ErrorMessage = result?.ErrorMessage ?? fallbackError };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in GET {Route}", route);
            return new ApiResponse<T> { IsSuccess = false, ErrorMessage = fallbackError };
        }
    }

    private Task<ApiResponse<bool>> PostCommandAsync(string route, object payload, string token, string fallbackError) =>
        SendBoolCommandAsync(HttpMethod.Post, route, payload, token, fallbackError);

    private Task<ApiResponse<bool>> PutCommandAsync(string route, object payload, string token, string fallbackError) =>
        SendBoolCommandAsync(HttpMethod.Put, route, payload, token, fallbackError);

    private Task<ApiResponse<bool>> DeleteCommandAsync(string route, string token, string fallbackError) =>
        SendBoolCommandAsync(HttpMethod.Delete, route, payload: null, token, fallbackError);

    private async Task<ApiResponse<bool>> SendBoolCommandAsync(
        HttpMethod method,
        string route,
        object? payload,
        string token,
        string fallbackError)
    {
        try
        {
            using var request = new HttpRequestMessage(method, route);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            if (payload is not null)
            {
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            }

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("{Method} {Route} failed: {StatusCode} / {Body}", method, route, response.StatusCode, body);
                return new ApiResponse<bool> { IsSuccess = false, Data = false, ErrorMessage = fallbackError };
            }

            var result = JsonSerializer.Deserialize<CommandResultWrapper<object>>(body, JsonOptions);
            if (result is { IsSuccess: true })
            {
                return new ApiResponse<bool> { IsSuccess = true, Data = true };
            }

            return new ApiResponse<bool>
            {
                IsSuccess = false,
                Data = false,
                ErrorMessage = result?.ErrorMessages?.FirstOrDefault() ?? fallbackError
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in {Method} {Route}", method, route);
            return new ApiResponse<bool> { IsSuccess = false, Data = false, ErrorMessage = fallbackError };
        }
    }

    private async Task<ApiResponse<T>> PostCommandWithDataAsync<T>(
        string route,
        object payload,
        string? token,
        string fallbackError)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("POST {Route} failed: {StatusCode} / {Body}", route, response.StatusCode, body);
                return new ApiResponse<T> { IsSuccess = false, ErrorMessage = fallbackError };
            }

            var result = JsonSerializer.Deserialize<CommandResultWrapper<T>>(body, JsonOptions);
            if (result is { IsSuccess: true, Data: not null })
            {
                return new ApiResponse<T> { IsSuccess = true, Data = result.Data };
            }

            return new ApiResponse<T>
            {
                IsSuccess = false,
                ErrorMessage = result?.ErrorMessages?.FirstOrDefault() ?? fallbackError
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in POST {Route}", route);
            return new ApiResponse<T> { IsSuccess = false, ErrorMessage = fallbackError };
        }
    }

    private sealed class CreateAttributeDefinitionCommandResultDto
    {
        public Guid AttributeDefinitionBusinessKey { get; set; }
    }

    private sealed class CreateProductCommandResultDto
    {
        public Guid ProductBusinessKey { get; set; }
        public string BaseSku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    private sealed class CategoryTreeQueryResultDto
    {
        public List<CategoryTreeItemDto> Items { get; set; } = new();
    }

    private sealed class CategoryTreeItemDto
    {
        public Guid CategoryBusinessKey { get; set; }
        public Guid? ParentCategoryRef { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public List<CategoryTreeItemDto> Children { get; set; } = new();
    }

    private sealed class CategoryAttributesQueryResultDto
    {
        public List<CategoryAttributeItemDto> Items { get; set; } = new();
    }

    private sealed class CategoryAttributeRulesQueryResultDto
    {
        public Guid CategoryBusinessKey { get; set; }
        public List<CategoryAttributeRuleItemDto> Items { get; set; } = new();
    }

    private sealed class AttributeDefinitionsQueryResultDto
    {
        public List<AttributeDefinitionListItemDto> Items { get; set; } = new();
    }

    private sealed class AttributeDefinitionListItemDto
    {
        public Guid AttributeDefinitionBusinessKey { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int OptionCount { get; set; }
    }

    private sealed class CategoryAttributeItemDto
    {
        public Guid AttributeRef { get; set; }
        public string AttributeCode { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool AttributeIsActive { get; set; }
        public bool RuleIsRequired { get; set; }
        public bool RuleIsVariant { get; set; }
        public int RuleDisplayOrder { get; set; }
        public bool IsInherited { get; set; }
        public Guid SourceCategoryRef { get; set; }
        public string SourceCategoryName { get; set; } = string.Empty;
        public List<CategoryAttributeOptionItemDto> Options { get; set; } = new();
    }

    private sealed class CategoryAttributeOptionItemDto
    {
        public Guid OptionBusinessKey { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class CategoryAttributeRuleItemDto
    {
        public Guid RuleBusinessKey { get; set; }
        public Guid CategoryBusinessKey { get; set; }
        public Guid AttributeRef { get; set; }
        public string AttributeCode { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public bool AttributeIsActive { get; set; }
        public bool RuleIsRequired { get; set; }
        public bool RuleIsVariant { get; set; }
        public int RuleDisplayOrder { get; set; }
        public bool RuleIsOverridden { get; set; }
        public bool RuleIsActive { get; set; }
        public bool IsInherited { get; set; }
        public Guid SourceCategoryRef { get; set; }
        public string SourceCategoryCode { get; set; } = string.Empty;
        public string SourceCategoryName { get; set; } = string.Empty;
        public List<AttributeOptionItemDto> Options { get; set; } = new();
    }

    private sealed class AttributeOptionsQueryResultDto
    {
        public List<AttributeOptionItemDto> Items { get; set; } = new();
    }

    private sealed class AttributeOptionItemDto
    {
        public Guid OptionBusinessKey { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class SearchProductsQueryResultDto
    {
        public List<ProductListItemDto> Items { get; set; } = new();
    }

    private sealed class ProductListItemDto
    {
        public Guid ProductBusinessKey { get; set; }
        public Guid CategoryRef { get; set; }
        public string BaseSku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid DefaultUomRef { get; set; }
        public Guid? TaxCategoryRef { get; set; }
        public bool IsActive { get; set; }
    }

    private sealed class ProductFullDetailsQueryResultDto
    {
        public ProductFullDetailsItemDto? Item { get; set; }
    }

    private sealed class ProductFullDetailsItemDto
    {
        public ProductListItemDto Product { get; set; } = new();
        public string? CategoryName { get; set; }
        public List<ProductAttributeValueWithDefinitionDto> ProductAttributes { get; set; } = new();
    }

    private sealed class ProductAttributeValueWithDefinitionDto
    {
        public Guid AttributeRef { get; set; }
        public string AttributeCode { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string? Value { get; set; }
        public Guid? OptionRef { get; set; }
        public string? OptionValue { get; set; }
    }

    private sealed class VariantsByProductQueryResultDto
    {
        public List<VariantListItemDto> Items { get; set; } = new();
    }

    private sealed class SearchVariantsQueryResultDto
    {
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public List<VariantListItemDto> Items { get; set; } = new();
    }

    private sealed class ItemsQueryResultDto<T>
    {
        public List<T> Items { get; set; } = new();
    }

    private sealed class VariantListItemDto
    {
        public Guid VariantBusinessKey { get; set; }
        public Guid ProductRef { get; set; }
        public string VariantSku { get; set; } = string.Empty;
        public string? Barcode { get; set; }
        public string TrackingPolicy { get; set; } = string.Empty;
        public Guid BaseUomRef { get; set; }
        public bool IsActive { get; set; }
        public bool InventoryMovementLocked { get; set; }
    }

    private sealed class VariantFullDetailsQueryResultDto
    {
        public VariantFullDetailsItemDto? Item { get; set; }
    }

    private sealed class VariantUomConversionsQueryResultDto
    {
        public List<VariantUomConversionItemDto> Items { get; set; } = new();
    }

    private sealed class VariantFullDetailsItemDto
    {
        public VariantListItemDto Variant { get; set; } = new();
        public Guid ProductBusinessKey { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductBaseSku { get; set; } = string.Empty;
        public Guid CategoryBusinessKey { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<VariantAttributeValueWithDefinitionDto> AttributeValues { get; set; } = new();
    }

    private sealed class VariantAttributeValueWithDefinitionDto
    {
        public Guid AttributeRef { get; set; }
        public string AttributeCode { get; set; } = string.Empty;
        public string AttributeName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public string Scope { get; set; } = string.Empty;
        public string? Value { get; set; }
        public Guid? OptionRef { get; set; }
        public string? OptionValue { get; set; }
    }

    private sealed class VariantUomConversionItemDto
    {
        public Guid VariantUomConversionBusinessKey { get; set; }
        public Guid VariantRef { get; set; }
        public Guid FromUomRef { get; set; }
        public Guid ToUomRef { get; set; }
        public decimal Factor { get; set; }
        public string RoundingMode { get; set; } = string.Empty;
        public bool IsBasePath { get; set; }
    }

    private sealed class WarehouseLookupQueryResultDto
    {
        public List<WarehouseLookupItemModel> Items { get; set; } = new();
    }

    private sealed class WarehouseWithLocationsQueryResultDto
    {
        public WarehouseWithLocationsModel? Item { get; set; }
    }

    private sealed class UnitOfMeasureLookupQueryResultDto
    {
        public List<UnitOfMeasureLookupItemDto> Items { get; set; } = new();
    }

    private sealed class UnitOfMeasureLookupItemDto
    {
        public Guid UnitOfMeasureBusinessKey { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
