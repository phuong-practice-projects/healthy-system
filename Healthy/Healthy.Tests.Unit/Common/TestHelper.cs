using Microsoft.EntityFrameworkCore;
using Healthy.Infrastructure.Persistence;
using Healthy.Domain.Entities;

namespace Healthy.Tests.Unit.Common;

public class TestDbContext : ApplicationDbContext
{
    public TestDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Don't apply seed data in tests
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Override to prevent seeding in tests
    }
}

public static class TestHelper
{
    public static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new TestDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    public static List<Column> GetSampleColumns()
    {
        var columns = new List<Column>
        {
            new Column
            {
                Id = Guid.NewGuid(),
                Title = "Fish Diet Benefits",
                Content = "魚を食べて頭もカラダも元気に！知っておきたい魚を食べるメリ…",
                ImageUrl = "/images/column-01.jpg",
                Category = "diet",
                Tags = "#魚料理,#和食,#DHA",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new Column
            {
                Id = Guid.NewGuid(),
                Title = "Healthy Diet Recipes",
                Content = "簡単！美味しくヘルシーに！オススメのダイエットレシピを紹介します。",
                ImageUrl = "/images/column-02.jpg",
                Category = "recommended",
                Tags = "#ダイエット,#ヘルシー,#レシピ",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new Column
            {
                Id = Guid.NewGuid(),
                Title = "Beauty and Anti-aging",
                Content = "美肌効果抜群！アンチエイジングに効果的な食材とスキンケア方法",
                ImageUrl = "/images/column-03.jpg",
                Category = "beauty",
                Tags = "#美容,#アンチエイジング,#スキンケア",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new Column
            {
                Id = Guid.NewGuid(),
                Title = "Exercise for Beginners",
                Content = "運動習慣を身につけて健康的な体づくりを始めよう！初心者向けガイド",
                ImageUrl = "/images/column-04.jpg",
                Category = "recommended",
                Tags = "#運動,#健康,#初心者",
                IsPublished = false, // Unpublished column for testing
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            },
            new Column
            {
                Id = Guid.NewGuid(),
                Title = "Immune System Boost",
                Content = "バランスの良い食事で免疫力アップ！栄養士おすすめの食材と調理法",
                ImageUrl = "/images/column-05.jpg",
                Category = "diet",
                Tags = "#栄養,#免疫力,#バランス",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            }
        };

        // Mark one column as soft deleted for testing
        var deletedColumn = columns.LastOrDefault(c => c.Content.Contains("バランスの良い食事"));
        if (deletedColumn != null)
        {
            deletedColumn.Delete("test-user");
        }

        return columns;
    }
}
