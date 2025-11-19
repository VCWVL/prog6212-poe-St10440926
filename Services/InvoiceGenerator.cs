using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using st10440926_poeparttwo.Models;

namespace st10440926_poeparttwo.Services
{
    public static class InvoiceGenerator
    {
        public static byte[] Generate(LecturerProfile lecturer, List<ClaimModel> claims)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);

                    // HEADER
                    page.Header()
                        .Text("Contract Monthly Claim System")
                        .SemiBold().FontSize(20)
                        .FontColor(Colors.Blue.Medium);

                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Lecturer: {lecturer.FullName}");
                        col.Item().Text($"Email: {lecturer.Email}");
                        col.Item().Text($"Hourly Rate: R {lecturer.HourlyRate}");
                        col.Item().Text($"Generated On: {DateTime.Now}");
                        col.Item().PaddingVertical(10);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(120); // Date
                                cols.RelativeColumn();    // Hours
                                cols.RelativeColumn();    // Rate
                                cols.RelativeColumn();    // Total
                                cols.RelativeColumn();    // Status
                            });

                            // HEADERS
                            table.Header(header =>
                            {
                                header.Cell().Text("Date");
                                header.Cell().Text("Hours");
                                header.Cell().Text("Rate");
                                header.Cell().Text("Total");
                                header.Cell().Text("Status");
                            });

                            // ROWS
                            foreach (var claim in claims)
                            {
                                table.Cell().Text(claim.DateSubmitted.ToString("yyyy-MM-dd"));
                                table.Cell().Text($"{claim.HoursWorked}");
                                table.Cell().Text($"R {claim.HourlyRate}");
                                table.Cell().Text($"R {claim.HoursWorked * claim.HourlyRate}");
                                table.Cell().Text($"{claim.Status}");
                            }
                        });

                        double grandTotal = claims.Sum(c => c.HoursWorked * c.HourlyRate);

                        col.Item().PaddingTop(15);
                        col.Item().Text($"Total Amount Due: R {grandTotal}")
                            .FontSize(16).SemiBold();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
