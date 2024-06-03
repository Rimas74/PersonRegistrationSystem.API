using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt", "Username" },
                values: new object[] { "lo4dWxddp0oyU2Lq/TP3F2zUJV1Rs1VitXss7a9IQBACRYQbe40WUc1hrC1dyWN09gl7D+GamH+0vCmksYyKFA==", "i8TEJ/uf+b2yXIfLzZNpzoTMxLxqB73t30bEPHMRM987xB6tysHN4oJs1W4MFXV4SQWC8k8XoK+3tKAHmp+bR5g+1jiBX6oHkU1k8mT1NtVEKk7kEsXrgupDC6EBzGZllzqbkIKK/pUqTh+W5d1P/R1J3pGPttLnGtvy6bfNyeE=", "admin2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt", "Username" },
                values: new object[] { "0fWFFXzV9oP76YYujXQVVPd3MnT+2NmU2RBgGhxfdgzFljtOvgAjxQrToJ5JVO3zVaStc4/Uzcmu8mVjUyQ4Vg==", "QFXFNvm9//3PIKNH5AxnCk6+cQKbVTEb9KHsfklDudE5fDWAJwm/yjwCky2J+PrBx50399dnqze82VCVrg9lzUVzQPqDXRYlOeg8Jol/Ytq6PzswXAmXKz1NPBFPZrC9o7buuFoAJRnwFbnDtsN70m5kemq3PxWwcqJFxxVpwro=", "admin" });
        }
    }
}
