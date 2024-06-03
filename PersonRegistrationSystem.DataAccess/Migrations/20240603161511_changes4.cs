using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changes4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "9MQDXWayovxrpeV5B/PUwdQHxyMDiPZQukmU6Ak1EoyIiKR+drEOV9TMWdR/vmTeoDz+Vk6zUeisYzqcy5mDtg==", "pCj/sAMQtu91s205YcqL9DnnI6HMsXKqNTw1XtZJ29ORyIDdJ7vKPyu8eqX74t+6tPxCIRuKg/9xhepvmnq2Qd+zSC5SRyrMXIMALRMNvRv6lxPQTu05EEO4HrodPovfPuOD13ShLdmy4QJI/UYt/epPFOc37y27hc5CXCNqBQU=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "3a0fI9iUnutEmPzNnDcQJpBhBvYigbQVLr++S1/9kJ1ShI1efV3kAiSzjQ7QW7vH1dVHKvlwHqlcjLPwk1LxBA==", "y2ltsLvj6+/FV4SWMH/2084vtzDjihUu5eHa69ybedNe9Vl8JwTwFL88puZIZUx19RLklK3Hy7SsLJIz2/f4KZJ2Dr2OO/2kvdOcNGOmZOIGB+ktI2i+INqHgu1adxEmkK/ieSJE6qUjXbuMmRcrtgqXomRlME5Gl4dmempwHhk=" });
        }
    }
}
