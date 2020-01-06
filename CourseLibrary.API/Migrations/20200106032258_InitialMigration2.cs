using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CourseLibrary.API.Migrations
{
    public partial class InitialMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "DateOfBirth", "FirstName", "Genre", "LastName" },
                values: new object[,]
                {
                    { new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"), new DateTimeOffset(new DateTime(1947, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -6, 0, 0, 0)), "Stephen", "Horror", "King" },
                    { new Guid("76053df4-6687-4353-8937-b45556748abe"), new DateTimeOffset(new DateTime(1948, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -6, 0, 0, 0)), "George", "Fantasy", "RR Martin" },
                    { new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"), new DateTimeOffset(new DateTime(1960, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -6, 0, 0, 0)), "Neil", "Fantasy", "Gaiman" },
                    { new Guid("578359b7-1967-41d6-8b87-64ab7605587e"), new DateTimeOffset(new DateTime(1958, 8, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -6, 0, 0, 0)), "Tom", "Various", "Lanoye" },
                    { new Guid("f74d6899-9ed2-4137-9876-66b070553f8f"), new DateTimeOffset(new DateTime(1952, 3, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -6, 0, 0, 0)), "Douglas", "Science fiction", "Adams" },
                    { new Guid("a1da1d8e-1988-4634-b538-a01709477b77"), new DateTimeOffset(new DateTime(1974, 5, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, -6, 0, 0, 0)), "Jens", "Thriller", "Lapidus" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("25320c5e-f58a-4b1f-b63a-8ee07a840bdf"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("412c3012-d891-4f5e-9613-ff7aa63e6bb3"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("578359b7-1967-41d6-8b87-64ab7605587e"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("76053df4-6687-4353-8937-b45556748abe"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("a1da1d8e-1988-4634-b538-a01709477b77"));

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("f74d6899-9ed2-4137-9876-66b070553f8f"));
        }
    }
}
