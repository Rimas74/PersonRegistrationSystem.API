using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Role", "Salt", "Username" },
                values: new object[] { 3, "0fWFFXzV9oP76YYujXQVVPd3MnT+2NmU2RBgGhxfdgzFljtOvgAjxQrToJ5JVO3zVaStc4/Uzcmu8mVjUyQ4Vg==", "Admin", "QFXFNvm9//3PIKNH5AxnCk6+cQKbVTEb9KHsfklDudE5fDWAJwm/yjwCky2J+PrBx50399dnqze82VCVrg9lzUVzQPqDXRYlOeg8Jol/Ytq6PzswXAmXKz1NPBFPZrC9o7buuFoAJRnwFbnDtsN70m5kemq3PxWwcqJFxxVpwro=", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "PasswordHash", "Role", "Salt", "Username" },
                values: new object[] { 1, "9WWQLzJv/BK6W/At6RoMuGObuwy9wG1ojJ1GuuK1XKfFUAIaYCdFg+RCEPP2n1EkCASzj9ZN7yf19bECYH1YCw==", "Admin", "6GfAHbJe9oohU9O8D9ArMjvNQDty4czhiAHp+WzlB7SV7vaHCdlAP0d2bSxmWwQaZMR5XaAPsKcW1OeaSgBQ7M8744PzUNaQVI1zzvFwHHwlf02L6o480Omer7fr9AgNAIQpx5zI8o/f+kOe4l4LXueWS2D5on0pbXzhCSRyOtQ=", "admin" });
        }
    }
}
