using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zdrav_I_SIlen.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVitaminImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update the image path to the correct location in wwwroot/images
            migrationBuilder.Sql(@"
                UPDATE Products 
                SET ImagePath = N'/images/vitamini.jpg'
                WHERE Name = N'Мултивитамини Комплекс' 
                AND ImagePath = N'/product_images/vitamini.jpg';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert the image path back to the original location
            migrationBuilder.Sql(@"
                UPDATE Products 
                SET ImagePath = N'/product_images/vitamini.jpg'
                WHERE Name = N'Мултивитамини Комплекс' 
                AND ImagePath = N'/images/vitamini.jpg';
            ");
        }
    }
}
