using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityPractise.Data.Migrations
{
    public partial class AssignAdminUserToAllRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [Security].[UserRoles] (UserId, RoleId) SELECT 'b6ba0b68-3b78-4740-b4f6-97e3b8080c96', Id FROM [Security].[Roles];");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Security].[UserRoles] WHERE UserID = 'b6ba0b68-3b78-4740-b4f6-97e3b8080c96'");
        }
    }
}
