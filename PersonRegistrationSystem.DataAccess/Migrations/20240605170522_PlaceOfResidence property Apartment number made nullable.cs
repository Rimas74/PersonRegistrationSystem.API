using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonRegistrationSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PlaceOfResidencepropertyApartmentnumbermadenullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApartmentNumber",
                table: "PlacesOfResidence",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "d3BG5xj/2PW4E+tUivM8ye0myWLDfru1mUHpVuX9VYo/6lIw4Da2yXXhMQpoA5pfZlRVx9rBDC39Uekb71+pkQ==", "ucFdZBewXiT0k/jX+3V0xPLLr5br3ja97js096Ela86tkz+jTadNYZ8S88FGdgwftdut+CqD8Ww0i+xhh1q4mlIBonGWiiwh4nidrReRIJueUVbX4RvOgDmEtP2RJUZREE8POkZx0uceKht4V8XN2J44YQVvGpnEIpHIGq4N2iA=" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ApartmentNumber",
                table: "PlacesOfResidence",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PasswordHash", "Salt" },
                values: new object[] { "A3wkPKToiOmlTGody724a8/9zfTjoxjkTVg/R3iouZ+UkRj5Ei7cxuN5Hhv7r+HoqeuJqigJ64/et5SYL/y3mQ==", "GCg5RG6OII9Z+Mn88LxYaHDcWb00xjb/HabrzSGnNgE0YMgT4a4EHxgsazqcDTwMBELTkG0ubS21cpKAsILmC4aXwe56vl3l3BHzIxLVPouvsVpsi1z5obVLk5ezdv7hxmTINsu6lWnzqan01CldUwebVk3X4h9OJzDzbZj9IAo=" });
        }
    }
}
