using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dobu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3Oracle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "INFORMACAO_CUIDADO",
                columns: table => new
                {
                    ID_INFORMACAO_CUIDADO_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TITULO = table.Column<string>(type: "NVARCHAR2(120)", maxLength: 120, nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    ID_PET_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INFORMACAO_CUIDADO", x => x.ID_INFORMACAO_CUIDADO_PK);
                    table.ForeignKey(
                        name: "FK_INFORMACAO_CUIDADO_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_INFORMACAO_CUIDADO_ID_PET_FK",
                table: "INFORMACAO_CUIDADO",
                column: "ID_PET_FK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "INFORMACAO_CUIDADO");
        }
    }
}
