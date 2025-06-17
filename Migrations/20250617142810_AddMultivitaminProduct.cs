using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zdrav_I_SIlen.Migrations
{
    /// <inheritdoc />
    public partial class AddMultivitaminProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert the new Multivitamin product
            migrationBuilder.Sql(@"
                DECLARE @VitaminCategoryId UNIQUEIDENTIFIER;
                SELECT @VitaminCategoryId = Id FROM Categories WHERE Name = N'Витамини';

                IF @VitaminCategoryId IS NOT NULL
                BEGIN
                    INSERT INTO Products (Id, Name, Description, Size, ImagePath, UnitPrice, Quantity, CategoryId)
                    VALUES (
                        NEWID(),
                        N'Мултивитамини Комплекс',
                        N'Пълен комплекс от основни витамини и минерали за ежедневна подкрепа на организма. Съдържа витамини A, B-комплекс, C, D3, E и K, заедно с важни минерали като цинк, желязо и магнезий. Подходящ за активни хора и всички, които искат да поддържат оптимално здраве.',
                        N'90 таблетки',
                        N'/product_images/vitamini.jpg',
                        32.99,
                        40,
                        @VitaminCategoryId
                    );
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the Multivitamin product
            migrationBuilder.Sql(@"
                DELETE FROM Products 
                WHERE Name = N'Мултивитамини Комплекс' 
                AND ImagePath = N'/product_images/vitamini.jpg';
            ");
        }
    }
}
