using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class Tin_Updated_CHK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TIN",
                table: "Persons",
                newName: "TaxIdentificationNumber");

            migrationBuilder.AlterColumn<string>(
                name: "TaxIdentificationNumber",
                table: "Persons",
                type: "varchar(8)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_TaxIdentificationNumber",
                table: "Persons",
                column: "TaxIdentificationNumber",
                unique: true,
                filter: "[TaxIdentificationNumber] IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "CHK_TIN",
                table: "Persons",
                sql: "len([TaxIdentificationNumber]) = 8");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Persons_TaxIdentificationNumber",
                table: "Persons");

            migrationBuilder.DropCheckConstraint(
                name: "CHK_TIN",
                table: "Persons");

            migrationBuilder.RenameColumn(
                name: "TaxIdentificationNumber",
                table: "Persons",
                newName: "TIN");

            migrationBuilder.AlterColumn<string>(
                name: "TIN",
                table: "Persons",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(8)",
                oldNullable: true);
        }
    }
}
