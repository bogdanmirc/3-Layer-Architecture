using Microsoft.EntityFrameworkCore.Migrations;
using System;


#nullable disable

namespace Data.Migrations
{
    public partial class CustomerRelation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
			throw new NotSupportedException("This migration cannot be reversed.");
		}
    }
}
