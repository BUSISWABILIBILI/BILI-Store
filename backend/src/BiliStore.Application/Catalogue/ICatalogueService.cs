using BiliStore.Application.DTOs.Catalogue;

namespace BiliStore.Application.Catalogue;

public interface ICatalogueService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ColourDto>> GetColoursAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<MeasurementTypeDto>> GetMeasurementTypesAsync(CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductSummaryDto>> GetProductsAsync(ProductCatalogueQuery query, CancellationToken cancellationToken);

    Task<ProductDetailsDto?> GetProductBySlugAsync(string slug, CancellationToken cancellationToken);
}
