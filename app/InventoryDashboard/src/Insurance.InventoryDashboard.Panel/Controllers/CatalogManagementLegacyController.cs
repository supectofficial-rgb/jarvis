using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

// Legacy compatibility routes.
// Keep this controller while existing bookmarks/external links still point to /CatalogManagement/*.
// All new links should target CategoryManagement, ProductManagement, and VariantManagement directly.
[Route("CatalogManagement")]
public sealed class CatalogManagementLegacyController : Controller
{
    [HttpGet("")]
    public IActionResult Index() => RedirectToAction("Categories", "CategoryManagement");

    [HttpGet("Categories")]
    public IActionResult Categories() => RedirectToAction("Categories", "CategoryManagement");

    [HttpGet("Products")]
    public IActionResult Products() => RedirectToAction("Products", "ProductManagement");

    [HttpGet("Variants")]
    public IActionResult Variants() => RedirectToAction("Variants", "VariantManagement");
}
