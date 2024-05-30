using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedAdminseeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "9+tsUbFePL+WJrZuWNuDPFiSoH6Ggs9CtTxFwcStCjrrt8OFQEEVRn9I8/OpPWuPVv0wHF9inri2r6u4qByxIQ==", "SqiBlEzScE5MixejdlqEjXFpHphbVfXbRQN79lcsaTKGg7/WYoRclZVHo+dj/a9zvluwzhUVzBQHBNsMJDUIimiOTEp2OREEisYI+Hhk99qCmT2UwYJtZXoD9E4l3OmaDMDguydvfhhO9sKb/vNO8NCcOvNx2CNpPcR20VRFK1E=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "AdminPasword", "salt" });
        }
    }
}
