using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dobu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialDobu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ESPECIE",
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
                    table.PrimaryKey("PK_ESPECIE", x => x.ID_ESPECIE_PK);
                });

            migrationBuilder.CreateTable(
                name: "USUARIO",
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
                    table.PrimaryKey("PK_USUARIO", x => x.ID_USUARIO_PK);
                });

            migrationBuilder.CreateTable(
                name: "RACA",
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
                    table.PrimaryKey("PK_RACA", x => x.ID_RACA_PK);
                    table.ForeignKey(
                        name: "FK_RACA_ESPECIE_ID_ESPECIE_FK",
                        column: x => x.ID_ESPECIE_FK,
                        principalTable: "ESPECIE",
                        principalColumn: "ID_ESPECIE_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LOG_ERRO",
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
                    table.PrimaryKey("PK_LOG_ERRO", x => x.ID_LOG_ERRO_PK);
                    table.ForeignKey(
                        name: "FK_LOG_ERRO_USUARIO_ID_USUARIO_FK",
                        column: x => x.ID_USUARIO_FK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PET",
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
                    table.PrimaryKey("PK_PET", x => x.ID_PET_PK);
                    table.ForeignKey(
                        name: "FK_PET_RACA_ID_RACA_FK",
                        column: x => x.ID_RACA_FK,
                        principalTable: "RACA",
                        principalColumn: "ID_RACA_PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PET_USUARIO_ID_RESPONSAVEL_FK",
                        column: x => x.ID_RESPONSAVEL_FK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AGENDAMENTO",
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
                    table.PrimaryKey("PK_AGENDAMENTO", x => x.ID_AGENDAMENTO_PK);
                    table.ForeignKey(
                        name: "FK_AGENDAMENTO_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AGENDAMENTO_USUARIO_ID_VETERINARIO_FK",
                        column: x => x.ID_VETERINARIO_FK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONSULTA",
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
                    table.PrimaryKey("PK_CONSULTA", x => x.ID_CONSULTA_PK);
                    table.ForeignKey(
                        name: "FK_CONSULTA_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONSULTA_USUARIO_ID_VETERINARIO_FK",
                        column: x => x.ID_VETERINARIO_FK,
                        principalTable: "USUARIO",
                        principalColumn: "ID_USUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DOBUCAM",
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
                    table.PrimaryKey("PK_DOBUCAM", x => x.ID_DOBUCAM_PK);
                    table.ForeignKey(
                        name: "FK_DOBUCAM_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEMBRETE",
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
                    table.PrimaryKey("PK_LEMBRETE", x => x.ID_LEMBRETE_PK);
                    table.ForeignKey(
                        name: "FK_LEMBRETE_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VACINA",
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
                    table.PrimaryKey("PK_VACINA", x => x.ID_VACINA_PK);
                    table.ForeignKey(
                        name: "FK_VACINA_PET_ID_PET_FK",
                        column: x => x.ID_PET_FK,
                        principalTable: "PET",
                        principalColumn: "ID_PET_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PAGAMENTO",
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
                    table.PrimaryKey("PK_PAGAMENTO", x => x.ID_PAGAMENTO_PK);
                    table.ForeignKey(
                        name: "FK_PAGAMENTO_CONSULTA_ID_CONSULTA_FK",
                        column: x => x.ID_CONSULTA_FK,
                        principalTable: "CONSULTA",
                        principalColumn: "ID_CONSULTA_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PRONTUARIO",
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
                    table.PrimaryKey("PK_PRONTUARIO", x => x.ID_PRONTUARIO_PK);
                    table.ForeignKey(
                        name: "FK_PRONTUARIO_CONSULTA_ID_CONSULTA_FK",
                        column: x => x.ID_CONSULTA_FK,
                        principalTable: "CONSULTA",
                        principalColumn: "ID_CONSULTA_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ANALISE_IA",
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
                    table.PrimaryKey("PK_ANALISE_IA", x => x.ID_ANALISE_IA_PK);
                    table.ForeignKey(
                        name: "FK_ANALISE_IA_PRONTUARIO_ID_PRONTUARIO_FK",
                        column: x => x.ID_PRONTUARIO_FK,
                        principalTable: "PRONTUARIO",
                        principalColumn: "ID_PRONTUARIO_PK",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AGENDAMENTO_ID_PET_FK",
                table: "AGENDAMENTO",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_AGENDAMENTO_ID_VETERINARIO_FK",
                table: "AGENDAMENTO",
                column: "ID_VETERINARIO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_ANALISE_IA_ID_PRONTUARIO_FK",
                table: "ANALISE_IA",
                column: "ID_PRONTUARIO_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_PET_FK",
                table: "CONSULTA",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_CONSULTA_ID_VETERINARIO_FK",
                table: "CONSULTA",
                column: "ID_VETERINARIO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_DOBUCAM_ID_PET_FK",
                table: "DOBUCAM",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_LEMBRETE_ID_PET_FK",
                table: "LEMBRETE",
                column: "ID_PET_FK");

            migrationBuilder.CreateIndex(
                name: "IX_LOG_ERRO_ID_USUARIO_FK",
                table: "LOG_ERRO",
                column: "ID_USUARIO_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PAGAMENTO_ID_CONSULTA_FK",
                table: "PAGAMENTO",
                column: "ID_CONSULTA_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PET_ID_RACA_FK",
                table: "PET",
                column: "ID_RACA_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ID_RESPONSAVEL_FK",
                table: "PET",
                column: "ID_RESPONSAVEL_FK");

            migrationBuilder.CreateIndex(
                name: "IX_PRONTUARIO_ID_CONSULTA_FK",
                table: "PRONTUARIO",
                column: "ID_CONSULTA_FK",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RACA_ID_ESPECIE_FK",
                table: "RACA",
                column: "ID_ESPECIE_FK");

            migrationBuilder.CreateIndex(
                name: "IX_USUARIO_DESC_EMAIL",
                table: "USUARIO",
                column: "DESC_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VACINA_ID_PET_FK",
                table: "VACINA",
                column: "ID_PET_FK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AGENDAMENTO");

            migrationBuilder.DropTable(
                name: "ANALISE_IA");

            migrationBuilder.DropTable(
                name: "DOBUCAM");

            migrationBuilder.DropTable(
                name: "LEMBRETE");

            migrationBuilder.DropTable(
                name: "LOG_ERRO");

            migrationBuilder.DropTable(
                name: "PAGAMENTO");

            migrationBuilder.DropTable(
                name: "VACINA");

            migrationBuilder.DropTable(
                name: "PRONTUARIO");

            migrationBuilder.DropTable(
                name: "CONSULTA");

            migrationBuilder.DropTable(
                name: "PET");

            migrationBuilder.DropTable(
                name: "RACA");

            migrationBuilder.DropTable(
                name: "USUARIO");

            migrationBuilder.DropTable(
                name: "ESPECIE");
        }
    }
}
