using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpiderNet.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
// Xóa unique constraints mặc định
            migrationBuilder.DropIndex(name: "IX_Users_Email", table: "Users");
            migrationBuilder.DropIndex(name: "IX_Users_Username", table: "Users");
    
            // Tạo partial unique indexes
            migrationBuilder.Sql(@"
        CREATE UNIQUE INDEX ""IX_Users_Email_NotNull"" 
        ON ""Users"" (""Email"") 
        WHERE ""Email"" IS NOT NULL AND ""Email"" != '';
    ");
    
            migrationBuilder.Sql(@"
        CREATE UNIQUE INDEX ""IX_Users_Username_NotNull"" 
        ON ""Users"" (""Username"") 
        WHERE ""Username"" IS NOT NULL AND ""Username"" != '';
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
