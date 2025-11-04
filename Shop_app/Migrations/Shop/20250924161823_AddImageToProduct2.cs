using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop_app.Migrations.Shop
{
    /// <inheritdoc />
    public partial class AddImageToProduct2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageData",
                table: "Products",
                newName: "ImageFile");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFile",
                table: "Products",
                newName: "ImageData");
        }
    }
}
