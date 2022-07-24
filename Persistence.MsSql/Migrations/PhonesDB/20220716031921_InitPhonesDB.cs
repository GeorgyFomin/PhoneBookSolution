using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.MsSql.Migrations.PhonesDB
{
    public partial class InitPhonesDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Phones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumder_HasCountryCode = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_CountryCode = table.Column<int>(type: "int", nullable: true),
                    PhoneNumder_HasNationalNumber = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_NationalNumber = table.Column<decimal>(type: "decimal(20,0)", nullable: true),
                    PhoneNumder_HasExtension = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumder_HasItalianLeadingZero = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_ItalianLeadingZero = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_HasNumberOfLeadingZeros = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_NumberOfLeadingZeros = table.Column<int>(type: "int", nullable: true),
                    PhoneNumder_HasRawInput = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_RawInput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumder_HasCountryCodeSource = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_CountryCodeSource = table.Column<int>(type: "int", nullable: true),
                    PhoneNumder_HasPreferredDomesticCarrierCode = table.Column<bool>(type: "bit", nullable: true),
                    PhoneNumder_PreferredDomesticCarrierCode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phones", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Phones");
        }
    }
}
