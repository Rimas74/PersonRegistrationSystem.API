using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class changes6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "7/zauvD4+425b/v3osnjo7MK1bj4epz8aBzOxywyoWcGdIazC6iy6lmDnchwHYalBK9lDR3Lu/yg444R3NZekA==", "atAT88oJb3bj8bWjL5jRzkthFLq/qWBS2WiJlBUkJNj6UFnMzI0p6aG73M/s+zkSvmP32kZfIzxmkGx/vvZ9BthVmrotXj3Kq9VBPsY2rwG7rkpxnVpxoStNf0wu/I2NdgFF7Nk+y9dHpgPrWaWnbGMbZg7/YEsWiVnSX8r6D18=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "9MQDXWayovxrpeV5B/PUwdQHxyMDiPZQukmU6Ak1EoyIiKR+drEOV9TMWdR/vmTeoDz+Vk6zUeisYzqcy5mDtg==", "pCj/sAMQtu91s205YcqL9DnnI6HMsXKqNTw1XtZJ29ORyIDdJ7vKPyu8eqX74t+6tPxCIRuKg/9xhepvmnq2Qd+zSC5SRyrMXIMALRMNvRv6lxPQTu05EEO4HrodPovfPuOD13ShLdmy4QJI/UYt/epPFOc37y27hc5CXCNqBQU=" });
        }
    }
}
