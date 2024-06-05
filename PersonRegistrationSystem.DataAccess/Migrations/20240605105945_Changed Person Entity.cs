using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedPersonEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlaceOfResidenceId",
                table: "Persons");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "A3wkPKToiOmlTGody724a8/9zfTjoxjkTVg/R3iouZ+UkRj5Ei7cxuN5Hhv7r+HoqeuJqigJ64/et5SYL/y3mQ==", "GCg5RG6OII9Z+Mn88LxYaHDcWb00xjb/HabrzSGnNgE0YMgT4a4EHxgsazqcDTwMBELTkG0ubS21cpKAsILmC4aXwe56vl3l3BHzIxLVPouvsVpsi1z5obVLk5ezdv7hxmTINsu6lWnzqan01CldUwebVk3X4h9OJzDzbZj9IAo=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaceOfResidenceId",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "7/zauvD4+425b/v3osnjo7MK1bj4epz8aBzOxywyoWcGdIazC6iy6lmDnchwHYalBK9lDR3Lu/yg444R3NZekA==", "atAT88oJb3bj8bWjL5jRzkthFLq/qWBS2WiJlBUkJNj6UFnMzI0p6aG73M/s+zkSvmP32kZfIzxmkGx/vvZ9BthVmrotXj3Kq9VBPsY2rwG7rkpxnVpxoStNf0wu/I2NdgFF7Nk+y9dHpgPrWaWnbGMbZg7/YEsWiVnSX8r6D18=" });
        }
    }
}
