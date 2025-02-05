using CatalogService.Core.Domain.Models;
using CatalogService.Infrastructure.Context;
using Microsoft.Data.SqlClient;
using Polly;
using System.Globalization;
using System.IO.Compression;

namespace CatalogService.Api.Infrastructure.Context;

public class CatalogContextSeed
{
    public async Task SeedAsync(CatalogContext context, IWebHostEnvironment env, ILogger<CatalogContextSeed> logger)
    {
        var policy = Policy.Handle<SqlException>().
            WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception, $"Exception with message detected on attempt of {retry} within time interval {timeSpan} for {ctx}");
                });

        var setupDirPath = Path.Combine(env.ContentRootPath, "Infrastructure", "Setup", "SeedFiles");
        var picturePath = "Pics";

        await policy.ExecuteAsync(() => ProcessSeeding(context, setupDirPath, picturePath));
    }

    private async Task ProcessSeeding(CatalogContext context, string setupDirPath, string picturePath)
    {
        if (!context.CatalogBrands.Any())
        {
            await context.CatalogBrands.AddRangeAsync(GetCatalogBrandsFromFile(setupDirPath));

            await context.SaveChangesAsync();
        }

        if (!context.CatalogTypes.Any())
        {
            await context.CatalogTypes.AddRangeAsync(GetCatalogTypesFromFile(setupDirPath));

            await context.SaveChangesAsync();
        }

        if (!context.CatalogItems.Any())
        {
            await context.CatalogItems.AddRangeAsync(GetCatalogItemsFromFile(setupDirPath, context));

            await context.SaveChangesAsync();

            GetCatalogItemPictures(setupDirPath, picturePath);
        }
    }

    private IEnumerable<CatalogBrand> GetCatalogBrandsFromFile(string contentPath)
    {
        IEnumerable<CatalogBrand> GetPreconfiguredCatalogBrands()
        {
            return new List<CatalogBrand>()
            {
                new CatalogBrand() { Name = "Azure" },
                new CatalogBrand() { Name = ".NET" },
                new CatalogBrand() { Name = "Visual Studio" },
                new CatalogBrand() { Name = "SQL Server" },
                new CatalogBrand() { Name = "Other" }
            };
        }

        string fileName = Path.Combine(contentPath, "BrandsTextFile.txt");

        if (!File.Exists(fileName))
        {
            GetPreconfiguredCatalogBrands();
        }

        var fileContent = File.ReadAllLines(fileName);

        var brandList = fileContent.Select(b => new CatalogBrand()
        {
            Name = b.Trim('"')
        }).Where(b => b != null);

        return brandList ?? GetPreconfiguredCatalogBrands();
    }

    private IEnumerable<CatalogType> GetCatalogTypesFromFile(string contentPath)
    {
        IEnumerable<CatalogType> GetPreconfiguredCatalogTypes()
        {
            return new List<CatalogType>()
            {
                new CatalogType() { Name = "Mug" },
                new CatalogType() { Name = "T-Shirt" },
                new CatalogType() { Name = "Sheet" },
                new CatalogType() { Name = "USB Memory Stick" },
            };
        }

        string fileName = Path.Combine(contentPath, "CatalogTypes.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredCatalogTypes();
        }

        var fileContent = File.ReadAllLines(fileName);

        var typeList = fileContent.Select(ct => new CatalogType()
        {
            Name = ct.Trim('"')
        }).Where(ct => ct != null);

        return typeList ?? GetPreconfiguredCatalogTypes();
    }

    private IEnumerable<CatalogItem> GetCatalogItemsFromFile(string contentPath, CatalogContext context)
    {
        IEnumerable<CatalogItem> GetPreconfiguredItems()
        {
            return new List<CatalogItem>()
            {
                new CatalogItem { CatalogTypeId = 2, CatalogBrandId = 2,  Description = ".NET Bot Black Hoodie, and more", Name = ".NET Bot Black Hoodie", Price = 19.5m, PictureFilename = "1.png"},
                new CatalogItem { CatalogTypeId = 1, CatalogBrandId = 5, Stock = 89, Description = ".NET Black & White Mug", Name = ".NET Black & White Mug", Price = 8.5m, PictureFilename = "2.png"},
                new CatalogItem { CatalogTypeId = 3, CatalogBrandId = 5, Stock = 55, Description = "Roslyn Red Sheet", Name = "Roslyn Red Sheet", Price = 8.5m, PictureFilename = "5.png"}
            };
        }

        string fileName = Path.Combine(contentPath, "CatalogItems.txt");

        if (!File.Exists(fileName))
        {
            return GetPreconfiguredItems();
        }

        var catalogTypeIdLookup = context.CatalogTypes.ToDictionary(ct => ct.Name, ct => ct.Id);
        var catalogBrandIdLookup = context.CatalogBrands.ToDictionary(ct => ct.Name, ct => ct.Id);

        var fileContent = File.ReadAllLines(fileName)
            .Skip(1) // Skip the header row
            .Select(f => f.Split(','))
            .Select(f => new CatalogItem()
            {
                CatalogTypeId = catalogTypeIdLookup[f[0]],
                CatalogBrandId = catalogBrandIdLookup[f[1]],
                Description = f[2].Trim('"').Trim(),
                Name = f[3].ToString().Trim('"').Trim(),
                Price = Decimal.Parse(f[4].Trim('"').Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                PictureFilename = f[5].Trim('"').Trim(),
                Stock = string.IsNullOrWhiteSpace(f[6]) ? 0 : int.Parse(f[6]),
            });

        return fileContent;
    }

    private void GetCatalogItemPictures(string contentPath, string picturePath)
    {
        picturePath ??= "pics";

        if (picturePath != null)
        {
            DirectoryInfo directory = new DirectoryInfo(picturePath);

            // To delete previous pictures
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            string zipFileCatalogItemPictures = Path.Combine(contentPath, "CatalogItems.zip");
            ZipFile.ExtractToDirectory(zipFileCatalogItemPictures, picturePath);
        }
    }
}
