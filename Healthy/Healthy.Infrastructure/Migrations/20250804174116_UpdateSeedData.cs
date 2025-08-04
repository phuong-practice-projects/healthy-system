using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Healthy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("11111111-bbbb-bbbb-bbbb-111111111111"));

            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"));

            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("33333333-bbbb-bbbb-bbbb-333333333333"));

            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("44444444-bbbb-bbbb-bbbb-444444444444"));

            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("55555555-bbbb-bbbb-bbbb-555555555555"));

            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("66666666-bbbb-bbbb-bbbb-666666666666"));

            migrationBuilder.DeleteData(
                table: "BodyRecords",
                keyColumn: "Id",
                keyValue: new Guid("77777777-bbbb-bbbb-bbbb-777777777777"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("111111aa-ccaa-ccaa-ccaa-111111111111"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("222222aa-ccaa-ccaa-ccaa-222222222222"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("333333aa-ccaa-ccaa-ccaa-333333333333"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("444444aa-ccaa-ccaa-ccaa-444444444444"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("555555aa-ccaa-ccaa-ccaa-555555555555"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("666666aa-ccaa-ccaa-ccaa-666666666666"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("777777aa-ccaa-ccaa-ccaa-777777777777"));

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: new Guid("888888aa-ccaa-ccaa-ccaa-888888888888"));

            migrationBuilder.DeleteData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("dddddddd-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Columns",
                keyColumn: "Id",
                keyValue: new Guid("eeeeeeee-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("11111111-dddd-dddd-dddd-111111111111"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("22222222-dddd-dddd-dddd-222222222222"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("33333333-dddd-dddd-dddd-333333333333"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("44444444-dddd-dddd-dddd-444444444444"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("55555555-dddd-dddd-dddd-555555555555"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("66666666-dddd-dddd-dddd-666666666666"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("77777777-dddd-dddd-dddd-777777777777"));

            migrationBuilder.DeleteData(
                table: "Diaries",
                keyColumn: "Id",
                keyValue: new Guid("88888888-dddd-dddd-dddd-888888888888"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("11111111-eeee-eeee-eeee-111111111111"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("22222222-eeee-eeee-eeee-222222222222"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("33333333-eeee-eeee-eeee-333333333333"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("44444444-eeee-eeee-eeee-444444444444"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("55555555-eeee-eeee-eeee-555555555555"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("66666666-eeee-eeee-eeee-666666666666"));

            migrationBuilder.DeleteData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: new Guid("77777777-eeee-eeee-eeee-777777777777"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("11111111-bbbb-bbbb-aaaa-111111111111"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-aaaa-222222222222"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("33333333-bbbb-bbbb-aaaa-333333333333"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("44444444-bbbb-bbbb-aaaa-444444444444"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("55555555-bbbb-bbbb-aaaa-555555555555"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("66666666-bbbb-bbbb-aaaa-666666666666"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("77777777-bbbb-bbbb-aaaa-777777777777"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("88888888-bbbb-bbbb-aaaa-888888888888"));

            migrationBuilder.DeleteData(
                table: "Meals",
                keyColumn: "Id",
                keyValue: new Guid("99999999-bbbb-bbbb-aaaa-999999999999"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-aaaa-aaaa-aaaa-111111111111"));

            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-bbbb-bbbb-bbbb-222222222222"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CategoryType", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "ImageUrl", "IsActive", "PublishedAt", "Tags", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("111111aa-ccaa-ccaa-ccaa-111111111111"), "diet", new DateTime(2024, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "健康的な食事とダイエットに関する情報とヒント", "/images/categories/diet-category.jpg", true, new DateTime(2024, 6, 4, 0, 0, 0, 0, DateTimeKind.Utc), "#食事,#ダイエット,#栄養", null, null },
                    { new Guid("222222aa-ccaa-ccaa-ccaa-222222222222"), "exercise", new DateTime(2024, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "効果的な運動方法とフィットネスガイド", "/images/categories/exercise-category.jpg", true, new DateTime(2024, 6, 9, 0, 0, 0, 0, DateTimeKind.Utc), "#運動,#フィットネス,#トレーニング", null, null },
                    { new Guid("333333aa-ccaa-ccaa-ccaa-333333333333"), "beauty", new DateTime(2024, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "美容とスキンケアに関する専門的なアドバイス", "/images/categories/beauty-category.jpg", true, new DateTime(2024, 6, 14, 0, 0, 0, 0, DateTimeKind.Utc), "#美容,#スキンケア,#アンチエイジング", null, null },
                    { new Guid("444444aa-ccaa-ccaa-ccaa-444444444444"), "wellness", new DateTime(2024, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "心と体の健康を保つためのウェルネス情報", "/images/categories/wellness-category.jpg", true, new DateTime(2024, 6, 19, 0, 0, 0, 0, DateTimeKind.Utc), "#ウェルネス,#健康,#心理", null, null },
                    { new Guid("555555aa-ccaa-ccaa-ccaa-555555555555"), "recommended", new DateTime(2024, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "専門家が推奨する健康に関する総合的な情報", "/images/categories/recommended-category.jpg", true, new DateTime(2024, 6, 24, 0, 0, 0, 0, DateTimeKind.Utc), "#推奨,#専門家,#総合", null, null },
                    { new Guid("666666aa-ccaa-ccaa-ccaa-666666666666"), "lifestyle", new DateTime(2024, 6, 29, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "健康的なライフスタイルを送るためのヒント", "/images/categories/lifestyle-category.jpg", true, new DateTime(2024, 6, 29, 0, 0, 0, 0, DateTimeKind.Utc), "#ライフスタイル,#習慣,#日常", null, null },
                    { new Guid("777777aa-ccaa-ccaa-ccaa-777777777777"), "recipe", new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "健康的で美味しいレシピとクッキングガイド", "/images/categories/recipe-category.jpg", true, new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "#レシピ,#料理,#ヘルシー", null, null },
                    { new Guid("888888aa-ccaa-ccaa-ccaa-888888888888"), "supplement", new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "栄養補助食品とサプリメントに関する情報", "/images/categories/supplement-category.jpg", true, new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), "#サプリメント,#栄養補助,#健康食品", null, null }
                });

            migrationBuilder.InsertData(
                table: "Columns",
                columns: new[] { "Id", "Category", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "ImageUrl", "IsPublished", "Tags", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-cccc-cccc-cccc-cccccccccccc"), "diet", "魚を食べて頭もカラダも元気に！知っておきたい魚を食べるメリット", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "/images/column-01.jpg", true, "#魚料理,#和食,#DHA", "魚を食べて頭もカラダも元気に！", null, null },
                    { new Guid("bbbbbbbb-cccc-cccc-cccc-cccccccccccc"), "recommended", "簡単！美味しくヘルシーに！オススメのダイエットレシピを紹介します。", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "/images/column-02.jpg", true, "#ダイエット,#ヘルシー,#レシピ", "ダイエットレシピ特集", null, null },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "beauty", "美肌効果抜群！アンチエイジングに効果的な食材とスキンケア方法", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "/images/column-03.jpg", true, "#美容,#アンチエイジング,#スキンケア", "美肌効果抜群！アンチエイジング", null, null },
                    { new Guid("dddddddd-cccc-cccc-cccc-cccccccccccc"), "recommended", "運動習慣を身につけて健康的な体づくりを始めよう！初心者向けガイド", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "/images/column-04.jpg", true, "#運動,#健康,#初心者", "運動習慣を身につけよう", null, null },
                    { new Guid("eeeeeeee-cccc-cccc-cccc-cccccccccccc"), "diet", "バランスの良い食事で免疫力アップ！栄養士おすすめの食材と調理法", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, "/images/column-05.jpg", true, "#栄養,#免疫力,#バランス", "バランスの良い食事で免疫力アップ", null, null }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "System Administrator with full access", true, "Admin" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Regular user with basic access", true, "User" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Moderator with limited admin access", true, "Moderator" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DateOfBirth", "DeletedAt", "DeletedBy", "Email", "FirstName", "Gender", "IsActive", "LastLoginAt", "LastName", "PasswordHash", "PhoneNumber", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, null, "admin@healthysystem.com", "System", null, true, null, "Administrator", "$2a$11$3tEK5ZODo.jJF5nJv.wgbeOaE4j3RgD8xTg2Pl3.wIzBzJo7MKf6W", null, null, null },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, null, "user@healthysystem.com", "Test", null, true, null, "User", "$2a$11$3tEK5ZODo.jJF5nJv.wgbeOaE4j3RgD8xTg2Pl3.wIzBzJo7MKf6W", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "BodyRecords",
                columns: new[] { "Id", "BodyFatPercentage", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Notes", "RecordDate", "UpdatedAt", "UpdatedBy", "UserId", "Weight" },
                values: new object[,]
                {
                    { new Guid("11111111-bbbb-bbbb-bbbb-111111111111"), 18.5m, new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "健康診断後の記録", new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 65.5m },
                    { new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), 18.2m, new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "ダイエット開始", new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 65.2m },
                    { new Guid("33333333-bbbb-bbbb-bbbb-333333333333"), 17.9m, new DateTime(2024, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "順調に減量中", new DateTime(2024, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 64.8m },
                    { new Guid("44444444-bbbb-bbbb-bbbb-444444444444"), 17.6m, new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "運動効果が見えてきた", new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 64.5m },
                    { new Guid("55555555-bbbb-bbbb-bbbb-555555555555"), 17.3m, new DateTime(2024, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "目標体重に近づいている", new DateTime(2024, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 64.2m },
                    { new Guid("66666666-bbbb-bbbb-bbbb-666666666666"), 17.0m, new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "理想的な体重に到達", new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 64.0m },
                    { new Guid("77777777-bbbb-bbbb-bbbb-777777777777"), 16.8m, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "維持していきたい", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 63.8m }
                });

            migrationBuilder.InsertData(
                table: "Diaries",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DiaryDate", "Mood", "Tags", "Title", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-dddd-dddd-dddd-111111111111"), "今日からダイエットを始めることにしました。健康的な食事と運動を心がけて、3ヶ月で5kg減量することが目標です。", new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 4, 0, 0, 0, 0, DateTimeKind.Utc), "やる気満々", "#ダイエット,#決意,#目標設定", "ダイエット開始の決意", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("22222222-dddd-dddd-dddd-222222222222"), "初めて本格的にランニングをしました。思っていたより辛かったですが、最後まで走り切れて達成感がありました。", new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 9, 0, 0, 0, 0, DateTimeKind.Utc), "充実感", "#ランニング,#運動,#達成感", "初回ランニング体験", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "Diaries",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DiaryDate", "IsPrivate", "Mood", "Tags", "Title", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { new Guid("33333333-dddd-dddd-dddd-333333333333"), "カロリー計算アプリを使って食事管理を始めました。野菜を多めに取り入れて、満足感のある食事を心がけています。", new DateTime(2024, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 14, 0, 0, 0, 0, DateTimeKind.Utc), true, "前向き", "#食事管理,#カロリー,#野菜", "食事制限の工夫", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.InsertData(
                table: "Diaries",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DiaryDate", "Mood", "Tags", "Title", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("44444444-dddd-dddd-dddd-444444444444"), "ジムに入会して筋力トレーニングを始めました。トレーナーさんに正しいフォームを教えてもらい、効果的な運動ができそうです。", new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Utc), "ワクワク", "#筋トレ,#ジム,#トレーナー", "筋トレデビュー", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("55555555-dddd-dddd-dddd-555555555555"), "ダイエットを始めて2週間が経ち、体重が2kg減りました。体も少し軽くなった気がして、モチベーションが上がっています。", new DateTime(2024, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 24, 0, 0, 0, 0, DateTimeKind.Utc), "嬉しい", "#体重減少,#モチベーション,#成果", "体重減少の実感", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "Diaries",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DiaryDate", "IsPrivate", "Mood", "Tags", "Title", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[] { new Guid("66666666-dddd-dddd-dddd-666666666666"), "今日はヨガクラスに参加しました。普段使わない筋肉を使って、体の柔軟性も向上しそうです。心もリラックスできました。", new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), true, "穏やか", "#ヨガ,#柔軟性,#リラックス", "新しい運動に挑戦", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.InsertData(
                table: "Diaries",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "DiaryDate", "Mood", "Tags", "Title", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("77777777-dddd-dddd-dddd-777777777777"), "同じくダイエット中の友人と一緒に運動しました。お互いに励まし合えて、一人では続けられないことも楽しく続けられそうです。", new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "楽しい", "#友人,#励まし,#チームワーク", "友人との励まし合い", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("88888888-dddd-dddd-dddd-888888888888"), "ダイエット開始から1ヶ月が経ちました。体重は順調に減っていて、目標まであと2kgです。継続することの大切さを実感しています。", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "満足", "#継続,#目標,#成長", "目標達成への道のり", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "CaloriesBurned", "Category", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "Description", "DurationMinutes", "ExerciseDate", "Notes", "Title", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-eeee-eeee-eeee-111111111111"), 250, "有酸素運動", new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "公園で30分のジョギング", 30, new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "天気が良くて気持ちよかった", "朝のランニング", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("22222222-eeee-eeee-eeee-222222222222"), 300, "筋力トレーニング", new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "ジムでの筋トレセッション", 45, new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "腕と胸を重点的に", "筋力トレーニング", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("33333333-eeee-eeee-eeee-333333333333"), 180, "ヨガ", new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "ハタヨガ60分クラス", 60, new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "リラックスできた", "ヨガクラス", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("44444444-eeee-eeee-eeee-444444444444"), 320, "水泳", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "プールで自由形25m×20本", 40, new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), "久しぶりの水泳で疲れた", "水泳", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("55555555-eeee-eeee-eeee-555555555555"), 400, "有酸素運動", new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "川沿いを1時間サイクリング", 60, new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), "景色を楽しみながら", "サイクリング", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("66666666-eeee-eeee-eeee-666666666666"), 350, "ダンス", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "ズンバクラス", 50, new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Utc), "楽しく汗をかけた", "ダンスレッスン", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("77777777-eeee-eeee-eeee-777777777777"), 120, "有酸素運動", new DateTime(2024, 8, 2, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", null, null, "近所を軽く散歩", 25, new DateTime(2024, 8, 2, 0, 0, 0, 0, DateTimeKind.Utc), "リフレッシュできた", "散歩", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "Meals",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Date", "DeletedAt", "DeletedBy", "ImageUrl", "Type", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-bbbb-bbbb-aaaa-111111111111"), new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/breakfast-01.jpg", "Morning", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("22222222-bbbb-bbbb-aaaa-222222222222"), new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/lunch-01.jpg", "Lunch", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("33333333-bbbb-bbbb-aaaa-333333333333"), new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 27, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/dinner-01.jpg", "Dinner", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("44444444-bbbb-bbbb-aaaa-444444444444"), new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/breakfast-02.jpg", "Morning", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("55555555-bbbb-bbbb-aaaa-555555555555"), new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 28, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/lunch-02.jpg", "Lunch", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("66666666-bbbb-bbbb-aaaa-666666666666"), new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/snack-01.jpg", "Snack", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("77777777-bbbb-bbbb-aaaa-777777777777"), new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 29, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/dinner-02.jpg", "Dinner", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("88888888-bbbb-bbbb-aaaa-888888888888"), new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 30, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/breakfast-03.jpg", "Morning", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") },
                    { new Guid("99999999-bbbb-bbbb-aaaa-999999999999"), new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", new DateTime(2024, 7, 31, 0, 0, 0, 0, DateTimeKind.Utc), null, null, "/images/meals/lunch-03.jpg", "Lunch", null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "RoleId", "UpdatedAt", "UpdatedBy", "UserId" },
                values: new object[,]
                {
                    { new Guid("11111111-aaaa-aaaa-aaaa-111111111111"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, new Guid("11111111-1111-1111-1111-111111111111"), null, null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                    { new Guid("22222222-bbbb-bbbb-bbbb-222222222222"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system", null, null, new Guid("22222222-2222-2222-2222-222222222222"), null, null, new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb") }
                });
        }
    }
}
