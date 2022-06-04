using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApartmentRental.Infrastructure.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartment_Tenant_TenantId",
                table: "Apartment");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Apartment",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartment_Tenant_TenantId",
                table: "Apartment",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartment_Tenant_TenantId",
                table: "Apartment");

            migrationBuilder.AlterColumn<int>(
                name: "TenantId",
                table: "Apartment",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Apartment_Tenant_TenantId",
                table: "Apartment",
                column: "TenantId",
                principalTable: "Tenant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
