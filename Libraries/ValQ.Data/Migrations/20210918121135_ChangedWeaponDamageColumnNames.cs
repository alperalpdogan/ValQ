using Microsoft.EntityFrameworkCore.Migrations;

namespace ValQ.Data.Migrations
{
    public partial class ChangedWeaponDamageColumnNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DamageFromMinDistance",
                table: "WeaponDamage",
                newName: "MinDamage");

            migrationBuilder.RenameColumn(
                name: "DamageFromMaxDistance",
                table: "WeaponDamage",
                newName: "MaxDamage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinDamage",
                table: "WeaponDamage",
                newName: "DamageFromMinDistance");

            migrationBuilder.RenameColumn(
                name: "MaxDamage",
                table: "WeaponDamage",
                newName: "DamageFromMaxDistance");
        }
    }
}
