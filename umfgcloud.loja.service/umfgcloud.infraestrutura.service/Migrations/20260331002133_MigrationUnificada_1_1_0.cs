using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace umfgcloud.infraestrutura.service.Migrations
{
    /// <inheritdoc />
    public partial class MigrationUnificada_1_1_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PRODUTO",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "char(36)", nullable: false),
                    DS_PRODUTO = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false),
                    CD_BARRA_EAN = table.Column<string>(type: "varchar(13)", maxLength: 13, nullable: false),
                    VL_COMPRA = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    VL_VENDA = table.Column<decimal>(type: "decimal(14,2)", precision: 14, scale: 2, nullable: false),
                    ID_USER_CREEATE = table.Column<string>(type: "longtext", nullable: false),
                    DS_USER_EMAIL_CREATE = table.Column<string>(type: "longtext", nullable: false),
                    DT_CREATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ID_USER_UPDATE = table.Column<string>(type: "longtext", nullable: false),
                    DS_USER_EMAIL_UPDATE = table.Column<string>(type: "longtext", nullable: false),
                    DT_UPDATE = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IN_ACTIVE = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PRODUTO", x => x.ID);
                })
                .Annotation("MySQL:Charset", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PRODUTO");
        }
    }
}
