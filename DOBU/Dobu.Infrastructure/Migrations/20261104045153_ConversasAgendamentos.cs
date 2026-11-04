using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dobu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConversasAgendamentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MENSAGEM_AGENDAMENTO",
                columns: table => new
                {
                    ID_MENSAGEM_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TEXTO_MENSAGEM = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    TIPO_MENSAGEM = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    ID_AGENDAMENTO_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ID_AUTOR_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MENSAGEM_AGENDAMENTO", x => x.ID_MENSAGEM_PK);
                    table.ForeignKey(
                        name: "FK_MENSAGEM_AGENDAMENTO_AGENDAMENTO_ID_AGENDAMENTO_FK",
                        column: x => x.ID_AGENDAMENTO_FK,
                        principalTable: "AGENDAMENTO",
                        principalColumn: "ID_AGENDAMENTO_PK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MENSAGEM_AGENDAMENTO_USUARIO_ID_AUTOR_FK",
                        column: x => x.ID_AUTOR_FK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MENSAGEM_AGENDAMENTO_ID_AGENDAMENTO_FK",
                table: "MENSAGEM_AGENDAMENTO",
                column: "ID_AGENDAMENTO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_MENSAGEM_AGENDAMENTO_ID_AUTOR_FK",
                table: "MENSAGEM_AGENDAMENTO",
                column: "ID_AUTOR_FK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MENSAGEM_AGENDAMENTO");
        }
    }
}
