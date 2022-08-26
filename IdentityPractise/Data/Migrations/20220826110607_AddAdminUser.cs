using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityPractise.Data.Migrations
{
    public partial class AddAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO [Security].[Users] ([Id], [FirstName], [LastName], [ProfilePicture], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'b6ba0b68-3b78-4740-b4f6-97e3b8080c96', N'adminahmed', N'alitgfbd', NULL, N'admin', N'ADMIN', N'admin@test.com', N'ADMIN@TEST.COM', 0, N'AQAAAAEAACcQAAAAECfJyPyoCsG/Dbx70hNDjXuVFvmI30RpT4ayxtZfr+WG7zheFs1vQumRXBQEIHQUZg==', N'YHQYUQXJLROTVC6G2I7ZRVY6SMH7KULL', N'89cba767-934b-49d9-a528-265400ed17d6', NULL, 0, 0, NULL, 1, 0)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [Security].[Users] WHERE Id = 'b6ba0b68-3b78-4740-b4f6-97e3b8080c96'");
        }
    }
}
