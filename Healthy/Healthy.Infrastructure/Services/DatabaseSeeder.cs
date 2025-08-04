using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Domain.Entities;
using Healthy.Infrastructure.Persistence;
using BCrypt.Net;

namespace Healthy.Infrastructure.Services;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();
            
            // Check if data already exists
            if (await IsDatabaseSeededAsync())
            {
                _logger.LogInformation("Database already seeded, skipping seed process");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed basic data
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedUserRolesAsync();
            
            // Seed content data
            await SeedCategoriesAsync();
            await SeedColumnsAsync();
            
            // Seed user-specific data for normal users
            await SeedBodyRecordsAsync();
            await SeedDiariesAsync();
            await SeedExercisesAsync();
            await SeedMealsAsync();

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new Role
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin",
                Description = "System Administrator with full access",
                IsActive = true,
            },
            new Role
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "User",
                Description = "Regular user with basic access",
                IsActive = true,
            },
            new Role
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Moderator",
                Description = "Moderator with limited admin access",
                IsActive = true
            }
        };

        await _context.Roles.AddRangeAsync(roles);
        _logger.LogInformation("Seeded {Count} roles", roles.Length);
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new User
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@healthysystem.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                
            },
            new User
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                FirstName = "Test",
                LastName = "User",
                Email = "user@healthysystem.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123", 12),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                
            },
            new User
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                FirstName = "Test",
                LastName = "Moderator",
                Email = "moderator@healthysystem.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Moderator@123", 12),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                
            }
        };

        await _context.Users.AddRangeAsync(users);
        _logger.LogInformation("Seeded {Count} users", users.Length);
    }

    private async Task SeedUserRolesAsync()
    {
        var userRoles = new[]
        {
            // Admin user -> Admin role
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow,
                
            },
            // Test user -> User role
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CreatedAt = DateTime.UtcNow,
                
            },
            // Moderator user -> Moderator role
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CreatedAt = DateTime.UtcNow,
                
            }
        };

        await _context.UserRoles.AddRangeAsync(userRoles);
        _logger.LogInformation("Seeded {Count} user roles", userRoles.Length);
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new[]
        {
            new Category
            {
                Id = Guid.Parse("111111aa-ccaa-ccaa-ccaa-111111111111"),
                ImageUrl = "/images/categories/diet-category.jpg",
                Description = "健康的な食事とダイエットに関する情報とヒント",
                CategoryType = "diet",
                Tags = "#食事,#ダイエット,#栄養",
                IsActive = true,
                PublishedAt = new DateTime(2024, 6, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 6, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("222222aa-ccaa-ccaa-ccaa-222222222222"),
                ImageUrl = "/images/categories/exercise-category.jpg",
                Description = "効果的な運動方法とフィットネスガイド",
                CategoryType = "exercise",
                Tags = "#運動,#フィットネス,#トレーニング",
                IsActive = true,
                PublishedAt = new DateTime(2024, 6, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 6, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("333333aa-ccaa-ccaa-ccaa-333333333333"),
                ImageUrl = "/images/categories/beauty-category.jpg",
                Description = "美容とスキンケアに関する専門的なアドバイス",
                CategoryType = "beauty",
                Tags = "#美容,#スキンケア,#アンチエイジング",
                IsActive = true,
                PublishedAt = new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("444444aa-ccaa-ccaa-ccaa-444444444444"),
                ImageUrl = "/images/categories/wellness-category.jpg",
                Description = "心と体の健康を保つためのウェルネス情報",
                CategoryType = "wellness",
                Tags = "#ウェルネス,#健康,#心理",
                IsActive = true,
                PublishedAt = new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("555555aa-ccaa-ccaa-ccaa-555555555555"),
                ImageUrl = "/images/categories/recommended-category.jpg",
                Description = "専門家が推奨する健康に関する総合的な情報",
                CategoryType = "recommended",
                Tags = "#推奨,#専門家,#総合",
                IsActive = true,
                PublishedAt = new DateTime(2024, 6, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 6, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("666666aa-ccaa-ccaa-ccaa-666666666666"),
                ImageUrl = "/images/categories/lifestyle-category.jpg",
                Description = "健康的なライフスタイルを送るためのヒント",
                CategoryType = "lifestyle",
                Tags = "#ライフスタイル,#習慣,#日常",
                IsActive = true,
                PublishedAt = new DateTime(2024, 6, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 6, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("777777aa-ccaa-ccaa-ccaa-777777777777"),
                ImageUrl = "/images/categories/recipe-category.jpg",
                Description = "健康的で美味しいレシピとクッキングガイド",
                CategoryType = "recipe",
                Tags = "#レシピ,#料理,#ヘルシー",
                IsActive = true,
                PublishedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Category
            {
                Id = Guid.Parse("888888aa-ccaa-ccaa-ccaa-888888888888"),
                ImageUrl = "/images/categories/supplement-category.jpg",
                Description = "栄養補助食品とサプリメントに関する情報",
                CategoryType = "supplement",
                Tags = "#サプリメント,#栄養補助,#健康食品",
                IsActive = true,
                PublishedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            }
        };

        await _context.Categories.AddRangeAsync(categories);
        _logger.LogInformation("Seeded {Count} categories", categories.Length);
    }

    private async Task SeedColumnsAsync()
    {
        var columns = new[]
        {
            new Column
            {
                Id = Guid.Parse("aaaaaaaa-cccc-cccc-cccc-cccccccccccc"),
                Title = "魚を食べて頭もカラダも元気に！",
                Content = "魚を食べて頭もカラダも元気に！知っておきたい魚を食べるメリット",
                ImageUrl = "/images/column-01.jpg",
                Category = "diet",
                Tags = "#魚料理,#和食,#DHA",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Column
            {
                Id = Guid.Parse("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"),
                Title = "ダイエットレシピ特集",
                Content = "簡単！美味しくヘルシーに！オススメのダイエットレシピを紹介します。",
                ImageUrl = "/images/column-02.jpg",
                Category = "recommended",
                Tags = "#ダイエット,#ヘルシー,#レシピ",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Column
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Title = "美肌効果抜群！アンチエイジング",
                Content = "美肌効果抜群！アンチエイジングに効果的な食材とスキンケア方法",
                ImageUrl = "/images/column-03.jpg",
                Category = "beauty",
                Tags = "#美容,#アンチエイジング,#スキンケア",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Column
            {
                Id = Guid.Parse("dddddddd-cccc-cccc-cccc-cccccccccccc"),
                Title = "運動習慣を身につけよう",
                Content = "運動習慣を身につけて健康的な体づくりを始めよう！初心者向けガイド",
                ImageUrl = "/images/column-04.jpg",
                Category = "recommended",
                Tags = "#運動,#健康,#初心者",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            },
            new Column
            {
                Id = Guid.Parse("eeeeeeee-cccc-cccc-cccc-cccccccccccc"),
                Title = "バランスの良い食事で免疫力アップ",
                Content = "バランスの良い食事で免疫力アップ！栄養士おすすめの食材と調理法",
                ImageUrl = "/images/column-05.jpg",
                Category = "diet",
                Tags = "#栄養,#免疫力,#バランス",
                IsPublished = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system"
            }
        };

        await _context.Columns.AddRangeAsync(columns);
        _logger.LogInformation("Seeded {Count} columns", columns.Length);
    }

    private async Task SeedBodyRecordsAsync()
    {
        var bodyRecords = new[]
        {
            new BodyRecord
            {
                Id = Guid.Parse("11111111-bbbb-bbbb-bbbb-111111111111"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 65.5m,
                BodyFatPercentage = 18.5m,
                RecordDate = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                Notes = "健康診断後の記録",
                CreatedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new BodyRecord
            {
                Id = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 65.2m,
                BodyFatPercentage = 18.2m,
                RecordDate = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                Notes = "ダイエット開始",
                CreatedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new BodyRecord
            {
                Id = Guid.Parse("33333333-bbbb-bbbb-bbbb-333333333333"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 64.8m,
                BodyFatPercentage = 17.9m,
                RecordDate = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                Notes = "順調に減量中",
                CreatedAt = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new BodyRecord
            {
                Id = Guid.Parse("44444444-bbbb-bbbb-bbbb-444444444444"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 64.5m,
                BodyFatPercentage = 17.6m,
                RecordDate = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                Notes = "運動効果が見えてきた",
                CreatedAt = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new BodyRecord
            {
                Id = Guid.Parse("55555555-bbbb-bbbb-bbbb-555555555555"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 64.2m,
                BodyFatPercentage = 17.3m,
                RecordDate = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                Notes = "目標体重に近づいている",
                CreatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new BodyRecord
            {
                Id = Guid.Parse("66666666-bbbb-bbbb-bbbb-666666666666"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 64.0m,
                BodyFatPercentage = 17.0m,
                RecordDate = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                Notes = "理想的な体重に到達",
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new BodyRecord
            {
                Id = Guid.Parse("77777777-bbbb-bbbb-bbbb-777777777777"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Weight = 63.8m,
                BodyFatPercentage = 16.8m,
                RecordDate = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Notes = "維持していきたい",
                CreatedAt = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            }
        };

        await _context.BodyRecords.AddRangeAsync(bodyRecords);
        _logger.LogInformation("Seeded {Count} body records", bodyRecords.Length);
    }

    private async Task SeedDiariesAsync()
    {
        var diaries = new[]
        {
            new Diary
            {
                Id = Guid.Parse("11111111-dddd-dddd-dddd-111111111111"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "ダイエット開始の決意",
                Content = "今日からダイエットを始めることにしました。健康的な食事と運動を心がけて、3ヶ月で5kg減量することが目標です。",
                Tags = "#ダイエット,#決意,#目標設定",
                Mood = "やる気満々",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("22222222-dddd-dddd-dddd-222222222222"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "初回ランニング体験",
                Content = "初めて本格的にランニングをしました。思っていたより辛かったですが、最後まで走り切れて達成感がありました。",
                Tags = "#ランニング,#運動,#達成感",
                Mood = "充実感",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("33333333-dddd-dddd-dddd-333333333333"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "食事制限の工夫",
                Content = "カロリー計算アプリを使って食事管理を始めました。野菜を多めに取り入れて、満足感のある食事を心がけています。",
                Tags = "#食事管理,#カロリー,#野菜",
                Mood = "前向き",
                IsPrivate = true,
                DiaryDate = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("44444444-dddd-dddd-dddd-444444444444"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "筋トレデビュー",
                Content = "ジムに入会して筋力トレーニングを始めました。トレーナーさんに正しいフォームを教えてもらい、効果的な運動ができそうです。",
                Tags = "#筋トレ,#ジム,#トレーナー",
                Mood = "ワクワク",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("55555555-dddd-dddd-dddd-555555555555"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "体重減少の実感",
                Content = "ダイエットを始めて2週間が経ち、体重が2kg減りました。体も少し軽くなった気がして、モチベーションが上がっています。",
                Tags = "#体重減少,#モチベーション,#成果",
                Mood = "嬉しい",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("66666666-dddd-dddd-dddd-666666666666"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "新しい運動に挑戦",
                Content = "今日はヨガクラスに参加しました。普段使わない筋肉を使って、体の柔軟性も向上しそうです。心もリラックスできました。",
                Tags = "#ヨガ,#柔軟性,#リラックス",
                Mood = "穏やか",
                IsPrivate = true,
                DiaryDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("77777777-dddd-dddd-dddd-777777777777"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "友人との励まし合い",
                Content = "同じくダイエット中の友人と一緒に運動しました。お互いに励まし合えて、一人では続けられないことも楽しく続けられそうです。",
                Tags = "#友人,#励まし,#チームワーク",
                Mood = "楽しい",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Diary
            {
                Id = Guid.Parse("88888888-dddd-dddd-dddd-888888888888"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "目標達成への道のり",
                Content = "ダイエット開始から1ヶ月が経ちました。体重は順調に減っていて、目標まであと2kgです。継続することの大切さを実感しています。",
                Tags = "#継続,#目標,#成長",
                Mood = "満足",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            }
        };

        await _context.Diaries.AddRangeAsync(diaries);
        _logger.LogInformation("Seeded {Count} diaries", diaries.Length);
    }

    private async Task SeedExercisesAsync()
    {
        var exercises = new[]
        {
            new Exercise
            {
                Id = Guid.Parse("11111111-eeee-eeee-eeee-111111111111"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "朝のランニング",
                Description = "公園で30分のジョギング",
                DurationMinutes = 30,
                CaloriesBurned = 250,
                ExerciseDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                Category = "有酸素運動",
                Notes = "天気が良くて気持ちよかった",
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Exercise
            {
                Id = Guid.Parse("22222222-eeee-eeee-eeee-222222222222"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "筋力トレーニング",
                Description = "ジムでの筋トレセッション",
                DurationMinutes = 45,
                CaloriesBurned = 300,
                ExerciseDate = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                Category = "筋力トレーニング",
                Notes = "腕と胸を重点的に",
                CreatedAt = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Exercise
            {
                Id = Guid.Parse("33333333-eeee-eeee-eeee-333333333333"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "ヨガクラス",
                Description = "ハタヨガ60分クラス",
                DurationMinutes = 60,
                CaloriesBurned = 180,
                ExerciseDate = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                Category = "ヨガ",
                Notes = "リラックスできた",
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Exercise
            {
                Id = Guid.Parse("44444444-eeee-eeee-eeee-444444444444"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "水泳",
                Description = "プールで自由形25m×20本",
                DurationMinutes = 40,
                CaloriesBurned = 320,
                ExerciseDate = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                Category = "水泳",
                Notes = "久しぶりの水泳で疲れた",
                CreatedAt = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Exercise
            {
                Id = Guid.Parse("55555555-eeee-eeee-eeee-555555555555"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "サイクリング",
                Description = "川沿いを1時間サイクリング",
                DurationMinutes = 60,
                CaloriesBurned = 400,
                ExerciseDate = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                Category = "有酸素運動",
                Notes = "景色を楽しみながら",
                CreatedAt = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Exercise
            {
                Id = Guid.Parse("66666666-eeee-eeee-eeee-666666666666"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "ダンスレッスン",
                Description = "ズンバクラス",
                DurationMinutes = 50,
                CaloriesBurned = 350,
                ExerciseDate = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Category = "ダンス",
                Notes = "楽しく汗をかけた",
                CreatedAt = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Exercise
            {
                Id = Guid.Parse("77777777-eeee-eeee-eeee-777777777777"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                Title = "散歩",
                Description = "近所を軽く散歩",
                DurationMinutes = 25,
                CaloriesBurned = 120,
                ExerciseDate = new DateTime(2024, 8, 2, 0, 0, 0, DateTimeKind.Utc),
                Category = "有酸素運動",
                Notes = "リフレッシュできた",
                CreatedAt = new DateTime(2024, 8, 2, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            }
        };

        await _context.Exercises.AddRangeAsync(exercises);
        _logger.LogInformation("Seeded {Count} exercises", exercises.Length);
    }

    private async Task SeedMealsAsync()
    {
        var meals = new[]
        {
            new Meal
            {
                Id = Guid.Parse("11111111-bbbb-bbbb-aaaa-111111111111"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/breakfast-01.jpg",
                Type = "Morning",
                Date = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("22222222-bbbb-bbbb-aaaa-222222222222"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/lunch-01.jpg",
                Type = "Lunch",
                Date = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("33333333-bbbb-bbbb-aaaa-333333333333"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/dinner-01.jpg",
                Type = "Dinner",
                Date = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("44444444-bbbb-bbbb-aaaa-444444444444"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/breakfast-02.jpg",
                Type = "Morning",
                Date = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("55555555-bbbb-bbbb-aaaa-555555555555"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/lunch-02.jpg",
                Type = "Lunch",
                Date = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("66666666-bbbb-bbbb-aaaa-666666666666"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/snack-01.jpg",
                Type = "Snack",
                Date = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("77777777-bbbb-bbbb-aaaa-777777777777"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/dinner-02.jpg",
                Type = "Dinner",
                Date = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("88888888-bbbb-bbbb-aaaa-888888888888"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/breakfast-03.jpg",
                Type = "Morning",
                Date = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            },
            new Meal
            {
                Id = Guid.Parse("99999999-bbbb-bbbb-aaaa-999999999999"),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), // Test User
                ImageUrl = "/images/meals/lunch-03.jpg",
                Type = "Lunch",
                Date = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"
            }
        };

        await _context.Meals.AddRangeAsync(meals);
        _logger.LogInformation("Seeded {Count} meals", meals.Length);
    }

    public async Task<bool> IsDatabaseSeededAsync()
    {
        return await _context.Roles.AnyAsync() && 
               await _context.Users.AnyAsync() && 
               await _context.Categories.AnyAsync() && 
               await _context.Columns.AnyAsync();
    }

    public async Task ClearDataAsync()
    {
        _logger.LogInformation("Clearing all seed data...");
        
        // Delete in order to respect foreign key constraints
        _context.BodyRecords.RemoveRange(_context.BodyRecords);
        _context.Diaries.RemoveRange(_context.Diaries);
        _context.Exercises.RemoveRange(_context.Exercises);
        _context.Meals.RemoveRange(_context.Meals);
        _context.UserRoles.RemoveRange(_context.UserRoles);
        _context.Users.RemoveRange(_context.Users);
        _context.Roles.RemoveRange(_context.Roles);
        _context.Categories.RemoveRange(_context.Categories);
        _context.Columns.RemoveRange(_context.Columns);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("All seed data cleared");
    }
}
