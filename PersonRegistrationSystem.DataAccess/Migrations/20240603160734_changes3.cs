using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "3a0fI9iUnutEmPzNnDcQJpBhBvYigbQVLr++S1/9kJ1ShI1efV3kAiSzjQ7QW7vH1dVHKvlwHqlcjLPwk1LxBA==", "y2ltsLvj6+/FV4SWMH/2084vtzDjihUu5eHa69ybedNe9Vl8JwTwFL88puZIZUx19RLklK3Hy7SsLJIz2/f4KZJ2Dr2OO/2kvdOcNGOmZOIGB+ktI2i+INqHgu1adxEmkK/ieSJE6qUjXbuMmRcrtgqXomRlME5Gl4dmempwHhk=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "lo4dWxddp0oyU2Lq/TP3F2zUJV1Rs1VitXss7a9IQBACRYQbe40WUc1hrC1dyWN09gl7D+GamH+0vCmksYyKFA==", "i8TEJ/uf+b2yXIfLzZNpzoTMxLxqB73t30bEPHMRM987xB6tysHN4oJs1W4MFXV4SQWC8k8XoK+3tKAHmp+bR5g+1jiBX6oHkU1k8mT1NtVEKk7kEsXrgupDC6EBzGZllzqbkIKK/pUqTh+W5d1P/R1J3pGPttLnGtvy6bfNyeE=" });
        }
    }
}
