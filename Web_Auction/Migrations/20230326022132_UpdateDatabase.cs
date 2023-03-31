using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Auction.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LotProducts_AuctionInformationS_AuctionInformationAuctionId",
                table: "LotProducts");

            migrationBuilder.DropTable(
                name: "AppUserLotProduct");

            migrationBuilder.DropTable(
                name: "AuctionInformationS");

            migrationBuilder.DropIndex(
                name: "IX_LotProducts_AuctionInformationAuctionId",
                table: "LotProducts");

            migrationBuilder.DropColumn(
                name: "AuctionInformationAuctionId",
                table: "LotProducts");

            migrationBuilder.DropColumn(
                name: "PriceLotOffer",
                table: "LotProducts");

            migrationBuilder.AlterColumn<double>(
                name: "PriceOpen",
                table: "Products",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Products",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateUpdated",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<double>(
                name: "PriceLotOpen",
                table: "LotProducts",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeEnded",
                table: "LotProducts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStarted",
                table: "LotProducts",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSent",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateTable(
                name: "BidInformation",
                columns: table => new
                {
                    BidId = table.Column<Guid>(type: "uuid", nullable: false),
                    PriceLotOffer = table.Column<double>(type: "double precision", nullable: true),
                    LotProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    AppUserId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidInformation", x => x.BidId);
                    table.ForeignKey(
                        name: "FK_BidInformation_LotProducts_LotProductId",
                        column: x => x.LotProductId,
                        principalTable: "LotProducts",
                        principalColumn: "LotProductId");
                    table.ForeignKey(
                        name: "FK_BidInformation_Users_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_BidInformation_AppUserId",
                table: "BidInformation",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BidInformation_LotProductId",
                table: "BidInformation",
                column: "LotProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BidInformation");

            migrationBuilder.DropColumn(
                name: "TimeEnded",
                table: "LotProducts");

            migrationBuilder.DropColumn(
                name: "TimeStarted",
                table: "LotProducts");

            migrationBuilder.AlterColumn<double>(
                name: "PriceOpen",
                table: "Products",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateUpdated",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateCreated",
                table: "Posts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PriceLotOpen",
                table: "LotProducts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AuctionInformationAuctionId",
                table: "LotProducts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PriceLotOffer",
                table: "LotProducts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateSent",
                table: "Contacts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AppUserLotProduct",
                columns: table => new
                {
                    LotProductsLotProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserLotProduct", x => new { x.LotProductsLotProductId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AppUserLotProduct_LotProducts_LotProductsLotProductId",
                        column: x => x.LotProductsLotProductId,
                        principalTable: "LotProducts",
                        principalColumn: "LotProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppUserLotProduct_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuctionInformationS",
                columns: table => new
                {
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuctionName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    TimeEnded = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeRemaining = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeStarted = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionInformationS", x => x.AuctionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LotProducts_AuctionInformationAuctionId",
                table: "LotProducts",
                column: "AuctionInformationAuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUserLotProduct_UsersId",
                table: "AppUserLotProduct",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_LotProducts_AuctionInformationS_AuctionInformationAuctionId",
                table: "LotProducts",
                column: "AuctionInformationAuctionId",
                principalTable: "AuctionInformationS",
                principalColumn: "AuctionId");
        }
    }
}
