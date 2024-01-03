using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class UpdatePerson_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_UpdatePerson = @"CREATE PROCEDURE [dbo].[UpdatePerson]
                                        (@PersonID uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40),
                                        @Address nvarchar(200), @CountryID uniqueidentifier, @Gender nvarchar(10),
                                        @DateOfBirth datetime2(7), @ReceiveNewsLetters bit)
                                        AS BEGIN
                                        UPDATE [dbo].[Persons]
                                        SET PersonName=@PersonName,Email=@Email,Address=@Address,CountryID=@CountryID,
                                        Gender=@Gender,DateOfBirth=@DateOfBirth,ReceiveNewsLetters=@ReceiveNewsLetters
                                        WHERE PersonID=@PersonID
                                        END";

            migrationBuilder.Sql(sp_UpdatePerson);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_UpdatePerson = @"DROP PROCEDURE [dbo].[UpdatePerson]";
            migrationBuilder.Sql(sp_UpdatePerson);
        }
    }
}
