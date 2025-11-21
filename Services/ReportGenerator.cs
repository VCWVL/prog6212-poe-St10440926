using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Services
{
    public static class ReportGenerator
    {
        
        //  LECTURER REPORT (MULTIPLE APPROVED CLAIMS)
        
        public static byte[] GenerateLecturerReport(LecturerProfile lecturer, List<ClaimModel> claims)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            double totalHours = claims.Sum(c => c.HoursWorked);
            double totalAmount = claims.Sum(c => c.HoursWorked * c.HourlyRate);

            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    page.Margin(40);

                   
                    // HEADER
                    
                    page.Header().Text("Contract Monthly Claim Report")
                        .FontSize(24).Bold().AlignCenter();

                    
                    // CONTENT
                  
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Generated: {DateTime.Now:yyyy/MM/dd HH:mm:ss}")
                            .FontSize(10).FontColor("#666");

                        col.Item().PaddingTop(10).Text("Lecturer Details")
                            .FontSize(16).Bold();

                        col.Item().Text($"Name: {lecturer.FullName}");
                        col.Item().Text($"Email: {lecturer.Email}");
                        col.Item().Text($"Hourly Rate: R {lecturer.HourlyRate}");

                        col.Item().PaddingTop(20).Text("Approved Claims")
                            .FontSize(16).Bold();

                        // Claim table
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(); // ID
                                columns.RelativeColumn(); // Hours
                                columns.RelativeColumn(); // Total
                                columns.RelativeColumn(); // Date
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                header.Cell().Text("Claim ID").Bold();
                                header.Cell().Text("Hours").Bold();
                                header.Cell().Text("Amount (R)").Bold();
                                header.Cell().Text("Date Submitted").Bold();
                            });

                            // Table rows
                            foreach (var claim in claims)
                            {
                                table.Cell().Text(claim.Id);
                                table.Cell().Text(claim.HoursWorked.ToString());
                                table.Cell().Text((claim.HoursWorked * claim.HourlyRate).ToString("0.00"));
                                table.Cell().Text(claim.DateSubmitted.ToString("yyyy-MM-dd"));
                            }
                        });

                        // Totals
                        col.Item().PaddingTop(20).Text($"Total Hours: {totalHours}")
                            .FontSize(14).Bold();
                        col.Item().Text($"Total Amount: R {totalAmount:0.00}")
                            .FontSize(14).Bold();

                        col.Item().PaddingTop(20)
                            .Text("This report includes all approved claims for the selected lecturer.")
                            .FontSize(10).FontColor("#666");
                    });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated on {DateTime.Now:yyyy-MM-dd HH:mm}");
                });
            }).GeneratePdf();
        }
    }
}
