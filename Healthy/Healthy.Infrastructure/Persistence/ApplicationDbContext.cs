using Microsoft.EntityFrameworkCore;
using Healthy.Application.Common.Interfaces;
using Healthy.Domain.Entities;
using Healthy.Domain.Common;
using Healthy.Infrastructure.Persistence.Configurations;

namespace Healthy.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, 
        ICurrentUserService currentUserService) : base(options)
    {
        _currentUserService = currentUserService;
    }

    // DbSet properties
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Column> Columns => Set<Column>();
    public DbSet<BodyRecord> BodyRecords => Set<BodyRecord>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<Diary> Diaries => Set<Diary>();
    public DbSet<Meal> Meals => Set<Meal>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        // Apply specific configurations explicitly for better control
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new ColumnConfiguration());
        modelBuilder.ApplyConfiguration(new BodyRecordConfiguration());
        modelBuilder.ApplyConfiguration(new ExerciseConfiguration());
        modelBuilder.ApplyConfiguration(new DiaryConfiguration());
        modelBuilder.ApplyConfiguration(new MealConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        
        // Seed initial data
        SeedData(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Roles với GUIDs cố định
        var adminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var userRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var moderatorRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = adminRoleId,
                Name = "Admin",
                Description = "System Administrator with full access",
                IsActive = true
            },
            new Role
            {
                Id = userRoleId,
                Name = "User",
                Description = "Regular user with basic access",
                IsActive = true
            },
            new Role
            {
                Id = moderatorRoleId,
                Name = "Moderator",
                Description = "Moderator with limited admin access",
                IsActive = true
            }
        );

        // Seed Users với GUIDs cố định
        var adminUserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var testUserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");

        // Passwords: "Admin@123" and "User@123" hashed with BCrypt
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = adminUserId,
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@healthysystem.com",
                PasswordHash = "$2a$11$3tEK5ZODo.jJF5nJv.wgbeOaE4j3RgD8xTg2Pl3.wIzBzJo7MKf6W", // Admin@123
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new User
            {
                Id = testUserId,
                FirstName = "Test",
                LastName = "User",
                Email = "user@healthysystem.com",
                PasswordHash = "$2a$11$3tEK5ZODo.jJF5nJv.wgbeOaE4j3RgD8xTg2Pl3.wIzBzJo7MKf6W", // User@123
                IsActive = true,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            }
        );

        // Seed UserRoles
        modelBuilder.Entity<UserRole>().HasData(
            new UserRole
            {
                Id = Guid.Parse("11111111-aaaa-aaaa-aaaa-111111111111"),
                UserId = adminUserId,
                RoleId = adminRoleId,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new UserRole
            {
                Id = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222"),
                UserId = testUserId,
                RoleId = userRoleId,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            }
        );

        // Seed sample Columns
        modelBuilder.Entity<Column>().HasData(
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
                CreatedBy = "system",
                IsDeleted = false
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
                CreatedBy = "system",
                IsDeleted = false
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
                CreatedBy = "system",
                IsDeleted = false
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
                CreatedBy = "system",
                IsDeleted = false
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
                CreatedBy = "system",
                IsDeleted = false
            }
        );

        // Seed sample BodyRecords for Test User
        modelBuilder.Entity<BodyRecord>().HasData(
            new BodyRecord
            {
                Id = Guid.Parse("11111111-bbbb-bbbb-bbbb-111111111111"),
                UserId = testUserId,
                Weight = 65.5m,
                BodyFatPercentage = 18.5m,
                RecordDate = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                Notes = "健康診断後の記録",
                CreatedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new BodyRecord
            {
                Id = Guid.Parse("22222222-bbbb-bbbb-bbbb-222222222222"),
                UserId = testUserId,
                Weight = 65.2m,
                BodyFatPercentage = 18.2m,
                RecordDate = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                Notes = "ダイエット開始",
                CreatedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new BodyRecord
            {
                Id = Guid.Parse("33333333-bbbb-bbbb-bbbb-333333333333"),
                UserId = testUserId,
                Weight = 64.8m,
                BodyFatPercentage = 17.9m,
                RecordDate = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                Notes = "順調に減量中",
                CreatedAt = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new BodyRecord
            {
                Id = Guid.Parse("44444444-bbbb-bbbb-bbbb-444444444444"),
                UserId = testUserId,
                Weight = 64.5m,
                BodyFatPercentage = 17.6m,
                RecordDate = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                Notes = "運動効果が見えてきた",
                CreatedAt = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new BodyRecord
            {
                Id = Guid.Parse("55555555-bbbb-bbbb-bbbb-555555555555"),
                UserId = testUserId,
                Weight = 64.2m,
                BodyFatPercentage = 17.3m,
                RecordDate = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                Notes = "目標体重に近づいている",
                CreatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new BodyRecord
            {
                Id = Guid.Parse("66666666-bbbb-bbbb-bbbb-666666666666"),
                UserId = testUserId,
                Weight = 64.0m,
                BodyFatPercentage = 17.0m,
                RecordDate = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                Notes = "理想的な体重に到達",
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new BodyRecord
            {
                Id = Guid.Parse("77777777-bbbb-bbbb-bbbb-777777777777"),
                UserId = testUserId,
                Weight = 63.8m,
                BodyFatPercentage = 16.8m,
                RecordDate = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Notes = "維持していきたい",
                CreatedAt = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            }
        );

        // Seed sample Exercises for Test User
        modelBuilder.Entity<Exercise>().HasData(
            new Exercise
            {
                Id = Guid.Parse("11111111-eeee-eeee-eeee-111111111111"),
                UserId = testUserId,
                Title = "朝のランニング",
                Description = "公園で30分のジョギング",
                DurationMinutes = 30,
                CaloriesBurned = 250,
                ExerciseDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                Category = "有酸素運動",
                Notes = "天気が良くて気持ちよかった",
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Exercise
            {
                Id = Guid.Parse("22222222-eeee-eeee-eeee-222222222222"),
                UserId = testUserId,
                Title = "筋力トレーニング",
                Description = "ジムでの筋トレセッション",
                DurationMinutes = 45,
                CaloriesBurned = 300,
                ExerciseDate = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                Category = "筋力トレーニング",
                Notes = "腕と胸を重点的に",
                CreatedAt = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Exercise
            {
                Id = Guid.Parse("33333333-eeee-eeee-eeee-333333333333"),
                UserId = testUserId,
                Title = "ヨガクラス",
                Description = "ハタヨガ60分クラス",
                DurationMinutes = 60,
                CaloriesBurned = 180,
                ExerciseDate = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                Category = "ヨガ",
                Notes = "リラックスできた",
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Exercise
            {
                Id = Guid.Parse("44444444-eeee-eeee-eeee-444444444444"),
                UserId = testUserId,
                Title = "水泳",
                Description = "プールで自由形25m×20本",
                DurationMinutes = 40,
                CaloriesBurned = 320,
                ExerciseDate = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                Category = "水泳",
                Notes = "久しぶりの水泳で疲れた",
                CreatedAt = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Exercise
            {
                Id = Guid.Parse("55555555-eeee-eeee-eeee-555555555555"),
                UserId = testUserId,
                Title = "サイクリング",
                Description = "川沿いを1時間サイクリング",
                DurationMinutes = 60,
                CaloriesBurned = 400,
                ExerciseDate = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                Category = "有酸素運動",
                Notes = "景色を楽しみながら",
                CreatedAt = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Exercise
            {
                Id = Guid.Parse("66666666-eeee-eeee-eeee-666666666666"),
                UserId = testUserId,
                Title = "ダンスレッスン",
                Description = "ズンバクラス",
                DurationMinutes = 50,
                CaloriesBurned = 350,
                ExerciseDate = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                Category = "ダンス",
                Notes = "楽しく汗をかけた",
                CreatedAt = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Exercise
            {
                Id = Guid.Parse("77777777-eeee-eeee-eeee-777777777777"),
                UserId = testUserId,
                Title = "散歩",
                Description = "近所を軽く散歩",
                DurationMinutes = 25,
                CaloriesBurned = 120,
                ExerciseDate = new DateTime(2024, 8, 2, 0, 0, 0, DateTimeKind.Utc),
                Category = "有酸素運動",
                Notes = "リフレッシュできた",
                CreatedAt = new DateTime(2024, 8, 2, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            }
        );

        // Seed sample Meals for Test User
        modelBuilder.Entity<Meal>().HasData(
            new Meal
            {
                Id = Guid.Parse("11111111-bbbb-bbbb-aaaa-111111111111"),
                UserId = testUserId,
                ImageUrl = "/images/meals/breakfast-01.jpg",
                Type = "Morning",
                Date = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("22222222-bbbb-bbbb-aaaa-222222222222"),
                UserId = testUserId,
                ImageUrl = "/images/meals/lunch-01.jpg",
                Type = "Lunch",
                Date = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("33333333-bbbb-bbbb-aaaa-333333333333"),
                UserId = testUserId,
                ImageUrl = "/images/meals/dinner-01.jpg",
                Type = "Dinner",
                Date = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("44444444-bbbb-bbbb-aaaa-444444444444"),
                UserId = testUserId,
                ImageUrl = "/images/meals/breakfast-02.jpg",
                Type = "Morning",
                Date = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("55555555-bbbb-bbbb-aaaa-555555555555"),
                UserId = testUserId,
                ImageUrl = "/images/meals/lunch-02.jpg",
                Type = "Lunch",
                Date = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 28, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("66666666-bbbb-bbbb-aaaa-666666666666"),
                UserId = testUserId,
                ImageUrl = "/images/meals/snack-01.jpg",
                Type = "Snack",
                Date = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("77777777-bbbb-bbbb-aaaa-777777777777"),
                UserId = testUserId,
                ImageUrl = "/images/meals/dinner-02.jpg",
                Type = "Dinner",
                Date = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("88888888-bbbb-bbbb-aaaa-888888888888"),
                UserId = testUserId,
                ImageUrl = "/images/meals/breakfast-03.jpg",
                Type = "Morning",
                Date = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 30, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Meal
            {
                Id = Guid.Parse("99999999-bbbb-bbbb-aaaa-999999999999"),
                UserId = testUserId,
                ImageUrl = "/images/meals/lunch-03.jpg",
                Type = "Lunch",
                Date = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 31, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            }
        );

        // Seed sample Diaries for Test User
        modelBuilder.Entity<Diary>().HasData(
            new Diary
            {
                Id = Guid.Parse("11111111-dddd-dddd-dddd-111111111111"),
                UserId = testUserId,
                Title = "ダイエット開始の決意",
                Content = "今日からダイエットを始めることにしました。健康的な食事と運動を心がけて、3ヶ月で5kg減量することが目標です。",
                Tags = "#ダイエット,#決意,#目標設定",
                Mood = "やる気満々",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("22222222-dddd-dddd-dddd-222222222222"),
                UserId = testUserId,
                Title = "初回ランニング体験",
                Content = "初めて本格的にランニングをしました。思っていたより辛かったですが、最後まで走り切れて達成感がありました。",
                Tags = "#ランニング,#運動,#達成感",
                Mood = "充実感",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("33333333-dddd-dddd-dddd-333333333333"),
                UserId = testUserId,
                Title = "食事制限の工夫",
                Content = "カロリー計算アプリを使って食事管理を始めました。野菜を多めに取り入れて、満足感のある食事を心がけています。",
                Tags = "#食事管理,#カロリー,#野菜",
                Mood = "前向き",
                IsPrivate = true,
                DiaryDate = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("44444444-dddd-dddd-dddd-444444444444"),
                UserId = testUserId,
                Title = "筋トレデビュー",
                Content = "ジムに入会して筋力トレーニングを始めました。トレーナーさんに正しいフォームを教えてもらい、効果的な運動ができそうです。",
                Tags = "#筋トレ,#ジム,#トレーナー",
                Mood = "ワクワク",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("55555555-dddd-dddd-dddd-555555555555"),
                UserId = testUserId,
                Title = "体重減少の実感",
                Content = "ダイエットを始めて2週間が経ち、体重が2kg減りました。体も少し軽くなった気がして、モチベーションが上がっています。",
                Tags = "#体重減少,#モチベーション,#成果",
                Mood = "嬉しい",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("66666666-dddd-dddd-dddd-666666666666"),
                UserId = testUserId,
                Title = "新しい運動に挑戦",
                Content = "今日はヨガクラスに参加しました。普段使わない筋肉を使って、体の柔軟性も向上しそうです。心もリラックスできました。",
                Tags = "#ヨガ,#柔軟性,#リラックス",
                Mood = "穏やか",
                IsPrivate = true,
                DiaryDate = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 27, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("77777777-dddd-dddd-dddd-777777777777"),
                UserId = testUserId,
                Title = "友人との励まし合い",
                Content = "同じくダイエット中の友人と一緒に運動しました。お互いに励まし合えて、一人では続けられないことも楽しく続けられそうです。",
                Tags = "#友人,#励まし,#チームワーク",
                Mood = "楽しい",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 7, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            },
            new Diary
            {
                Id = Guid.Parse("88888888-dddd-dddd-dddd-888888888888"),
                UserId = testUserId,
                Title = "目標達成への道のり",
                Content = "ダイエット開始から1ヶ月が経ちました。体重は順調に減っていて、目標まであと2kgです。継続することの大切さを実感しています。",
                Tags = "#継続,#目標,#成長",
                Mood = "満足",
                IsPrivate = false,
                DiaryDate = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedAt = new DateTime(2024, 8, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = testUserId.ToString(),
                IsDeleted = false
            }
        );

        // Seed sample Categories
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = Guid.Parse("111111aa-ccaa-ccaa-ccaa-111111111111"),
                ImageUrl = "/images/categories/diet-category.jpg",
                Description = "健康的な食事とダイエットに関する情報とヒント",
                CategoryType = "diet",
                PublishedAt = new DateTime(2024, 6, 4, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#食事,#ダイエット,#栄養",
                CreatedAt = new DateTime(2024, 6, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("222222aa-ccaa-ccaa-ccaa-222222222222"),
                ImageUrl = "/images/categories/exercise-category.jpg",
                Description = "効果的な運動方法とフィットネスガイド",
                CategoryType = "exercise",
                PublishedAt = new DateTime(2024, 6, 9, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#運動,#フィットネス,#トレーニング",
                CreatedAt = new DateTime(2024, 6, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("333333aa-ccaa-ccaa-ccaa-333333333333"),
                ImageUrl = "/images/categories/beauty-category.jpg",
                Description = "美容とスキンケアに関する専門的なアドバイス",
                CategoryType = "beauty",
                PublishedAt = new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#美容,#スキンケア,#アンチエイジング",
                CreatedAt = new DateTime(2024, 6, 14, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("444444aa-ccaa-ccaa-ccaa-444444444444"),
                ImageUrl = "/images/categories/wellness-category.jpg",
                Description = "心と体の健康を保つためのウェルネス情報",
                CategoryType = "wellness",
                PublishedAt = new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#ウェルネス,#健康,#心理",
                CreatedAt = new DateTime(2024, 6, 19, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("555555aa-ccaa-ccaa-ccaa-555555555555"),
                ImageUrl = "/images/categories/recommended-category.jpg",
                Description = "専門家が推奨する健康に関する総合的な情報",
                CategoryType = "recommended",
                PublishedAt = new DateTime(2024, 6, 24, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#推奨,#専門家,#総合",
                CreatedAt = new DateTime(2024, 6, 24, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("666666aa-ccaa-ccaa-ccaa-666666666666"),
                ImageUrl = "/images/categories/lifestyle-category.jpg",
                Description = "健康的なライフスタイルを送るためのヒント",
                CategoryType = "lifestyle",
                PublishedAt = new DateTime(2024, 6, 29, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#ライフスタイル,#習慣,#日常",
                CreatedAt = new DateTime(2024, 6, 29, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("777777aa-ccaa-ccaa-ccaa-777777777777"),
                ImageUrl = "/images/categories/recipe-category.jpg",
                Description = "健康的で美味しいレシピとクッキングガイド",
                CategoryType = "recipe",
                PublishedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#レシピ,#料理,#ヘルシー",
                CreatedAt = new DateTime(2024, 7, 4, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            },
            new Category
            {
                Id = Guid.Parse("888888aa-ccaa-ccaa-ccaa-888888888888"),
                ImageUrl = "/images/categories/supplement-category.jpg",
                Description = "栄養補助食品とサプリメントに関する情報",
                CategoryType = "supplement",
                PublishedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                IsActive = true,
                Tags = "#サプリメント,#栄養補助,#健康食品",
                CreatedAt = new DateTime(2024, 7, 9, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "system",
                IsDeleted = false
            }
        );
    }

    // Remove OnConfiguring - let DependencyInjection handle this
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     // Configuration is handled in DependencyInjection.cs
    // }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessAuditableEntities();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        ProcessAuditableEntities();
        return base.SaveChanges();
    }

    private void ProcessAuditableEntities()
    {
        var currentUserId = _currentUserService?.UserId;
        var currentTime = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IEntityAuditableBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = currentTime;
                    entry.Entity.CreatedBy = currentUserId;
                    break;
                    
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = currentTime;
                    entry.Entity.UpdatedBy = currentUserId;
                    // Prevent modification of CreatedAt and CreatedBy
                    entry.Property(nameof(IEntityAuditableBase.CreatedAt)).IsModified = false;
                    entry.Property(nameof(IEntityAuditableBase.CreatedBy)).IsModified = false;
                    break;
            }
        }

        // Handle BaseEntity instances that don't implement IEntityAuditableBase (like Role)
        foreach (var entry in ChangeTracker.Entries<EntityBase>())
        {
            // Skip if this entity already implements IEntityAuditableBase (handled above)
            if (entry.Entity is IEntityAuditableBase)
                continue;

            switch (entry.State)
            {
                case EntityState.Added:
                case EntityState.Modified:
                    // For simple entities, we don't track audit info
                    // They only inherit Id from EntityBase
                    break;
            }
        }
    }
    }
