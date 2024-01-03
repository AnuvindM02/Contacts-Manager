using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class InsertPerson_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"CREATE PROCEDURE [dbo].[InsertPerson]
                                        (@PersonID uniqueidentifier, @PersonName nvarchar(40), @Email nvarchar(40),
                                        @Address nvarchar(200), @CountryID uniqueidentifier, @Gender nvarchar(10),
                                        @DateOfBirth datetime2(7), @ReceiveNewsLetters bit)
                                        AS BEGIN
                                        INSERT INTO [dbo].[Persons](PersonId,PersonName,Email,Address,CountryID,Gender,DateOfBirth,ReceiveNewsLetters)
                                        VALUES(@PersonID,@PersonName,@Email,@Address,@CountryID,@Gender,@DateOfBirth,@ReceiveNewsLetters)
                                        END
                                        ";
            migrationBuilder.Sql(sp_InsertPerson);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPerson = @"DROP PROCEDURE [dbo].[InsertPerson]";
            migrationBuilder.Sql(sp_InsertPerson);
        }
    }
}
