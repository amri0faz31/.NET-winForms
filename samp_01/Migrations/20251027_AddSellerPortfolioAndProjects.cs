using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure; // for DbContextAttribute
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace samp_01.Migrations
{
    [DbContext(typeof(samp_01.Data.AppDbContext))]
    [Migration("20251027_AddSellerPortfolioAndProjects")]
    public partial class AddSellerPortfolioAndProjects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `seller_portfolios` (
 `id` INT NOT NULL AUTO_INCREMENT,
 `sellerid` INT NOT NULL,
 `description` TEXT NULL,
 `pricerangemin` DECIMAL(10,2) NULL,
 `pricerangemax` DECIMAL(10,2) NULL,
 `skills` VARCHAR(1000) NULL,
 `profilepicpath` VARCHAR(512) NULL,
 `updatedat` DATETIME(6) NOT NULL,
 PRIMARY KEY (`id`),
 UNIQUE KEY `IX_Portfolio_SellerId` (`sellerid`),
 CONSTRAINT `FK_Portfolio_Seller` FOREIGN KEY (`sellerid`) REFERENCES `sellers` (`id`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");

            migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS `seller_projects` (
 `id` INT NOT NULL AUTO_INCREMENT,
 `sellerid` INT NOT NULL,
 `title` VARCHAR(200) NOT NULL,
 `description` TEXT NULL,
 `imagepath` VARCHAR(512) NULL,
 `createdat` DATETIME(6) NOT NULL,
 PRIMARY KEY (`id`),
 KEY `IX_Projects_SellerId` (`sellerid`),
 CONSTRAINT `FK_Project_Seller` FOREIGN KEY (`sellerid`) REFERENCES `sellers` (`id`) ON DELETE CASCADE
 ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS `seller_projects`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `seller_portfolios`;");
        }
    }
}
