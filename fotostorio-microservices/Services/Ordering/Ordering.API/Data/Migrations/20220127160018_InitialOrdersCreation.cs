using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Data.Migrations
{
    public partial class InitialOrdersCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BuyerEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SendToAddress_FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAddress_LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAddress_SecondLine = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAddress_County = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SendToAddress_PostCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemOrdered_ProductItemId = table.Column<int>(type: "int", nullable: true),
                    ItemOrdered_ProductSku = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemOrdered_ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemOrdered_ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
