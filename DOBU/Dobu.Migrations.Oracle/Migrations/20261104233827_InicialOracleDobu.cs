using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dobu.Migrations.Oracle.Migrations
{
    /// <inheritdoc />
    public partial class InicialOracleDobu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DOBU_ESPECIE",
                columns: table => new
                {
                    ID_ESPECIE_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME_ESPECIE = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    DESC_ESPECIE = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_ESPECIE", x => x.ID_ESPECIE_PK);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_USUARIO",
                columns: table => new
                {
                    ID_USUARIO_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME_USUARIO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESC_EMAIL = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    DESC_SENHA = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    TIPO_USUARIO = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_USUARIO", x => x.ID_USUARIO_PK);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_RACA",
                columns: table => new
                {
                    ID_RACA_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME_RACA = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    TIPO_PORTE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    NUMERO_EXPECTATIVA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DESC_RACA = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    DESC_CUIDADOS = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ID_ESPECIE_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_RACA", x => x.ID_RACA_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_RACA_DOBU_ESPECIE_ID_ESPECIE_FK",
                        column: x => x.ID_ESPECIE_FK,
                        principalTable: "DOBU_ESPECIE",
                        principalColumn: "ID_ESPECIE_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_LOG_ERRO",
                columns: table => new
                {
                    ID_LOG_ERRO_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME_PROCEDURE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DESC_ERRO = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: false),
                    DATA_ERRO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_USUARIO_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_LOG_ERRO", x => x.ID_LOG_ERRO_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_LOG_ERRO_DOBU_USUARIO_ID_USUARIO_FK",
                        column: x => x.ID_USUARIO_FK,
                        principalTable: "DOBU_USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_PET",
                columns: table => new
                {
                    ID_PET_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME_PET = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false),
                    NUMERO_IDADE = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_RACA_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ID_RESPONSAVEL_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_PET", x => x.ID_PET_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_PET_DOBU_RACA_ID_RACA_FK",
                        column: x => x.ID_RACA_FK,
                        principalTable: "DOBU_RACA",
                        principalColumn: "ID_RACA_PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOBU_PET_DOBU_USUARIO_ID_RESPONSAVEL_FK",
                        column: x => x.ID_RESPONSAVEL_FK,
                        principalTable: "DOBU_USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_AGENDAMENTO",
                columns: table => new
                {
                    ID_AGENDAMENTO_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DATA_AGENDAMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS_AGENDAMENTO = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    ID_PET_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ID_VETERINARIO_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_AGENDAMENTO", x => x.ID_AGENDAMENTO_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_AGENDAMENTO_DOBU_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "DOBU_PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOBU_AGENDAMENTO_DOBU_USUARIO_ID_VETERINARIO_FK",
                        column: x => x.ID_VETERINARIO_FK,
                        principalTable: "DOBU_USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DATA_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DESC_CONSULTA = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    VALOR_CONSULTA = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    ID_PET_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ID_VETERINARIO_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_CONSULTA", x => x.ID_CONSULTA_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_CONSULTA_DOBU_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "DOBU_PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DOBU_CONSULTA_DOBU_USUARIO_ID_VETERINARIO_FK",
                        column: x => x.ID_VETERINARIO_FK,
                        principalTable: "DOBU_USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_DOBUCAM",
                columns: table => new
                {
                    ID_DOBUCAM_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DESC_LOCALIZACAO = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    STATUS_CAMERA = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    DATA_ULTIMA_MOVIMENTACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ID_PET_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_DOBUCAM", x => x.ID_DOBUCAM_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_DOBUCAM_DOBU_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "DOBU_PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_INFORMACAO_CUIDADO",
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
                    table.PrimaryKey("PK_DOBU_INFORMACAO_CUIDADO", x => x.ID_INFORMACAO_CUIDADO_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_INFORMACAO_CUIDADO_DOBU_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "DOBU_PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_LEMBRETE",
                columns: table => new
                {
                    ID_LEMBRETE_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DESC_LEMBRETE = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    DATA_LEMBRETE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    STATUS_LEMBRETE = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    ID_PET_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_LEMBRETE", x => x.ID_LEMBRETE_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_LEMBRETE_DOBU_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "DOBU_PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_VACINA",
                columns: table => new
                {
                    ID_VACINA_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    NOME_VACINA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DATA_APLICACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DATA_PROXIMA_DOSE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ID_PET_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_VACINA", x => x.ID_VACINA_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_VACINA_DOBU_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "DOBU_PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_MENSAGEM_AGENDAMENTO",
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
                    table.PrimaryKey("PK_DOBU_MENSAGEM_AGENDAMENTO", x => x.ID_MENSAGEM_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_MENSAGEM_AGENDAMENTO_DOBU_AGENDAMENTO_ID_AGENDAMENTO_FK",
                        column: x => x.ID_AGENDAMENTO_FK,
                        principalTable: "DOBU_AGENDAMENTO",
                        principalColumn: "ID_AGENDAMENTO_PK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DOBU_MENSAGEM_AGENDAMENTO_DOBU_USUARIO_ID_AUTOR_FK",
                        column: x => x.ID_AUTOR_FK,
                        principalTable: "DOBU_USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_PAGAMENTO",
                columns: table => new
                {
                    ID_PAGAMENTO_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    VALOR_PAGAMENTO = table.Column<decimal>(type: "NUMBER(10,2)", nullable: false),
                    TIPO_FORMA_PAGAMENTO = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    DATA_PAGAMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_CONSULTA_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_PAGAMENTO", x => x.ID_PAGAMENTO_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_PAGAMENTO_DOBU_CONSULTA_ID_CONSULTA_FK",
                        column: x => x.ID_CONSULTA_FK,
                        principalTable: "DOBU_CONSULTA",
                        principalColumn: "ID_CONSULTA_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_PRONTUARIO",
                columns: table => new
                {
                    ID_PRONTUARIO_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DESC_DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    DESC_OBSERVACOES = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ID_CONSULTA_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_PRONTUARIO", x => x.ID_PRONTUARIO_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_PRONTUARIO_DOBU_CONSULTA_ID_CONSULTA_FK",
                        column: x => x.ID_CONSULTA_FK,
                        principalTable: "DOBU_CONSULTA",
                        principalColumn: "ID_CONSULTA_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBU_ANALISE_IA",
                columns: table => new
                {
                    ID_ANALISE_IA_PK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DESC_ANALISE = table.Column<string>(type: "NVARCHAR2(800)", maxLength: 800, nullable: false),
                    NUMERO_RISCO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    DATA_ANALISE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ID_PRONTUARIO_FK = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    CREATED_AT = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    ACTIVE = table.Column<int>(type: "NUMBER(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOBU_ANALISE_IA", x => x.ID_ANALISE_IA_PK);
                    table.ForeignKey(
                        name: "FK_DOBU_ANALISE_IA_DOBU_PRONTUARIO_ID_PRONTUARIO_FK",
                        column: x => x.ID_PRONTUARIO_FK,
                        principalTable: "DOBU_PRONTUARIO",
                        principalColumn: "ID_PRONTUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_AGENDAMENTO_ID_PET_FK",
                table: "DOBU_AGENDAMENTO",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_AGENDAMENTO_ID_VETERINARIO_FK",
                table: "DOBU_AGENDAMENTO",
                column: "ID_VETERINARIO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_ANALISE_IA_ID_PRONTUARIO_FK",
                table: "DOBU_ANALISE_IA",
                column: "ID_PRONTUARIO_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_CONSULTA_ID_PET_FK",
                table: "DOBU_CONSULTA",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_CONSULTA_ID_VETERINARIO_FK",
                table: "DOBU_CONSULTA",
                column: "ID_VETERINARIO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_DOBUCAM_ID_PET_FK",
                table: "DOBU_DOBUCAM",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_INFORMACAO_CUIDADO_ID_PET_FK",
                table: "DOBU_INFORMACAO_CUIDADO",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_LEMBRETE_ID_PET_FK",
                table: "DOBU_LEMBRETE",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_LOG_ERRO_ID_USUARIO_FK",
                table: "DOBU_LOG_ERRO",
                column: "ID_USUARIO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_MENSAGEM_AGENDAMENTO_ID_AGENDAMENTO_FK",
                table: "DOBU_MENSAGEM_AGENDAMENTO",
                column: "ID_AGENDAMENTO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_MENSAGEM_AGENDAMENTO_ID_AUTOR_FK",
                table: "DOBU_MENSAGEM_AGENDAMENTO",
                column: "ID_AUTOR_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_PAGAMENTO_ID_CONSULTA_FK",
                table: "DOBU_PAGAMENTO",
                column: "ID_CONSULTA_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_PET_ID_RACA_FK",
                table: "DOBU_PET",
                column: "ID_RACA_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_PET_ID_RESPONSAVEL_FK",
                table: "DOBU_PET",
                column: "ID_RESPONSAVEL_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_PRONTUARIO_ID_CONSULTA_FK",
                table: "DOBU_PRONTUARIO",
                column: "ID_CONSULTA_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_RACA_ID_ESPECIE_FK",
                table: "DOBU_RACA",
                column: "ID_ESPECIE_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_USUARIO_DESC_EMAIL",
                table: "DOBU_USUARIO",
                column: "DESC_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOBU_VACINA_ID_PET_FK",
                table: "DOBU_VACINA",
                column: "ID_PET_FK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DOBU_ANALISE_IA");

            migrationBuilder.DropTable(
                name: "DOBU_DOBUCAM");

            migrationBuilder.DropTable(
                name: "DOBU_INFORMACAO_CUIDADO");

            migrationBuilder.DropTable(
                name: "DOBU_LEMBRETE");

            migrationBuilder.DropTable(
                name: "DOBU_LOG_ERRO");

            migrationBuilder.DropTable(
                name: "DOBU_MENSAGEM_AGENDAMENTO");

            migrationBuilder.DropTable(
                name: "DOBU_PAGAMENTO");

            migrationBuilder.DropTable(
                name: "DOBU_VACINA");

            migrationBuilder.DropTable(
                name: "DOBU_PRONTUARIO");

            migrationBuilder.DropTable(
                name: "DOBU_AGENDAMENTO");

            migrationBuilder.DropTable(
                name: "DOBU_CONSULTA");

            migrationBuilder.DropTable(
                name: "DOBU_PET");

            migrationBuilder.DropTable(
                name: "DOBU_RACA");

            migrationBuilder.DropTable(
                name: "DOBU_USUARIO");

            migrationBuilder.DropTable(
                name: "DOBU_ESPECIE");
        }
    }
}
