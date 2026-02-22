using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ArchieHealthTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_symptoms_Symptom",
                table: "symptoms");

            migrationBuilder.DropIndex(
                name: "IX_medical_events_Type",
                table: "medical_events");

            migrationBuilder.CreateIndex(
                name: "IX_symptoms_Symptom_CreatedAt",
                table: "symptoms",
                columns: new[] { "Symptom", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_medical_events_Type_Date",
                table: "medical_events",
                columns: new[] { "Type", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_hygiene_Date_Event",
                table: "hygiene",
                columns: new[] { "Date", "Event" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_symptoms_Symptom_CreatedAt",
                table: "symptoms");

            migrationBuilder.DropIndex(
                name: "IX_medical_events_Type_Date",
                table: "medical_events");

            migrationBuilder.DropIndex(
                name: "IX_hygiene_Date_Event",
                table: "hygiene");

            migrationBuilder.CreateIndex(
                name: "IX_symptoms_Symptom",
                table: "symptoms",
                column: "Symptom");

            migrationBuilder.CreateIndex(
                name: "IX_medical_events_Type",
                table: "medical_events",
                column: "Type");
        }
    }
}
