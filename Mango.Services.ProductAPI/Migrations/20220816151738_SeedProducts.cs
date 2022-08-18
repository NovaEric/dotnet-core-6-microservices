using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mango.Services.ProductAPI.Migrations
{
    public partial class SeedProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductId", "ImageUrl", "ProductCategoryName", "ProductDescription", "ProductName", "ProductPrice" },
                values: new object[,]
                {
                    { 1, "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843779/heroes/marvel-wolverine.jpg", "Entree", "Fry chicken Dominican style", "PolloPica", 12.0 },
                    { 2, "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843779/heroes/marvel-thor.jpg", "Appetizer", "Mango Dominican style", "MangoChimi", 6.0 },
                    { 3, "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843778/heroes/marvel-spider.jpg", "Appetizer", "Mango shake Dominican style", "Mango Shake", 5.0 },
                    { 4, "https://res.cloudinary.com/dyej4hpgt/image/upload/v1646843777/heroes/marvel-iron.jpg", "Appetizer", "Mixed Fruits", "Mango & friends", 12.0 }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "ProductId",
                keyValue: 4);
        }
    }
}
