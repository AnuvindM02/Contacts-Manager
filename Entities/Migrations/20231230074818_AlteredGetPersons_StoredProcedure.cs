using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class AlteredGetPersons_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                ALTER PROCEDURE [dbo].[GetAllPersons]
                AS BEGIN
                SELECT PersonID, PersonName, Email, DateOfBirth, Gender, CountryID,
                Address, ReceiveNewsLetters, TIN FROM [dbo].[Persons]
                END

            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                DROP PROCEDURE [dbo].[GetAllPersons]
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
