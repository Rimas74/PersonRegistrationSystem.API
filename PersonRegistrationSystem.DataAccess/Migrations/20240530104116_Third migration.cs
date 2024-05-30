using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Thirdmigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "9WWQLzJv/BK6W/At6RoMuGObuwy9wG1ojJ1GuuK1XKfFUAIaYCdFg+RCEPP2n1EkCASzj9ZN7yf19bECYH1YCw==", "6GfAHbJe9oohU9O8D9ArMjvNQDty4czhiAHp+WzlB7SV7vaHCdlAP0d2bSxmWwQaZMR5XaAPsKcW1OeaSgBQ7M8744PzUNaQVI1zzvFwHHwlf02L6o480Omer7fr9AgNAIQpx5zI8o/f+kOe4l4LXueWS2D5on0pbXzhCSRyOtQ=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "9+tsUbFePL+WJrZuWNuDPFiSoH6Ggs9CtTxFwcStCjrrt8OFQEEVRn9I8/OpPWuPVv0wHF9inri2r6u4qByxIQ==", "SqiBlEzScE5MixejdlqEjXFpHphbVfXbRQN79lcsaTKGg7/WYoRclZVHo+dj/a9zvluwzhUVzBQHBNsMJDUIimiOTEp2OREEisYI+Hhk99qCmT2UwYJtZXoD9E4l3OmaDMDguydvfhhO9sKb/vNO8NCcOvNx2CNpPcR20VRFK1E=" });
        }
    }
}
