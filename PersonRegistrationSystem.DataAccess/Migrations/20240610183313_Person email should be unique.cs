using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Personemailshouldbeunique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Persons",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "zN3v0bJZnh8a3rH1ebrzqLwUggho/4JZhDe9KVbM7lV7S513nHVXP8NoBiPp1GPCWW3hqZgiNUpwRitTcp2LbA==", "+J+Ldoa7jULrX3dXxw4av0En81UTAc1BRwefamJxjv0dgTGoUGdOJf0YXm+yEYqNL2pt3XTmgC8Q++6/0FaprqnHlPQTG7Vb+4/GBcPDIq8B62Luwc2dQVv26/D2RRgwjZCPFxtViyjfuOVBY8muyVl0O7ye0AUHVeai1Bbxny8=" });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_Email",
                table: "Persons",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_Email",
                table: "Persons");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "d3BG5xj/2PW4E+tUivM8ye0myWLDfru1mUHpVuX9VYo/6lIw4Da2yXXhMQpoA5pfZlRVx9rBDC39Uekb71+pkQ==", "ucFdZBewXiT0k/jX+3V0xPLLr5br3ja97js096Ela86tkz+jTadNYZ8S88FGdgwftdut+CqD8Ww0i+xhh1q4mlIBonGWiiwh4nidrReRIJueUVbX4RvOgDmEtP2RJUZREE8POkZx0uceKht4V8XN2J44YQVvGpnEIpHIGq4N2iA=" });
        }
    }
}
