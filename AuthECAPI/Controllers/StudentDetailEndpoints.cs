using AuthECAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace AuthECAPI.Controllers
{
    public static class StudentDetailEndpoints
    {
        public static IEndpointRouteBuilder MapStudentDetailEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/StudentDetail").RequireAuthorization(policy =>
                       policy.RequireRole("Admin")); ;
            group.MapGet("/export", ExportStudentDetails);
            group.MapGet("/", GetStudentDetails);
            group.MapGet("/{id:int}", GetStudentDetail);
            group.MapPost("/", PostStudentDetail);
            group.MapPut("/{id:int}", PutStudentDetail);
            group.MapDelete("/{id:int}", DeleteStudentDetail);

            return app;
        }

        private static async Task<IResult> GetStudentDetails(AppDbContext context)
        {
            var students = await context.StudentDetails.ToListAsync();
            return Results.Ok(students);
        }

        private static async Task<IResult> GetStudentDetail(int id, AppDbContext context)
        {
            var student = await context.StudentDetails.FindAsync(id);

            return student is null
                ? Results.NotFound()
                : Results.Ok(student);
        }

        private static async Task<IResult> PostStudentDetail(
            StudentDetail studentDetail,
            AppDbContext context)
        {
            context.StudentDetails.Add(studentDetail);
            await context.SaveChangesAsync();

            return Results.Ok(
                await context.StudentDetails.ToListAsync());
        }

        private static async Task<IResult> PutStudentDetail(
            int id,
            StudentDetail studentDetail,
            AppDbContext context)
        {
            if (id != studentDetail.StudentDetailId)
                return Results.BadRequest();

            context.Entry(studentDetail).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await context.StudentDetails
                    .AnyAsync(e => e.StudentDetailId == id);

                if (!exists)
                    return Results.NotFound();

                throw;
            }

            return Results.Ok(
                await context.StudentDetails.ToListAsync());
        }

        private static async Task<IResult> DeleteStudentDetail(
            int id,
            AppDbContext context)
        {
            var student = await context.StudentDetails.FindAsync(id);

            if (student is null)
                return Results.NotFound();

            context.StudentDetails.Remove(student);
            await context.SaveChangesAsync();

            return Results.Ok(
                await context.StudentDetails.ToListAsync());
        }

        private static async Task<IResult> ExportStudentDetails(AppDbContext context)
        {
            var students = await context.StudentDetails.ToListAsync();

            // Set license if not already set in Program.cs
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("Student Detail Register").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                            col.Item().Text($"{DateTime.Now:MMMM dd, yyyy}").FontSize(10);
                        });
                    });

                    page.Content().PaddingVertical(10).Column(col =>
                    {
                        col.Spacing(10);
                        foreach (var student in students)
                        {
                            col.Item().Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Row(row =>
                            {
                                // Robust Image Handling
                                if (!string.IsNullOrWhiteSpace(student.ImgPath))
                                {
                                    // Ensure we use the correct physical path
                                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), student.ImgPath);

                                    if (File.Exists(fullPath))
                                    {
                                        try
                                        {
                                            row.ConstantItem(50).Image(fullPath);
                                        }
                                        catch
                                        {
                                            // If file is corrupted or not a valid image, skip it
                                            row.ConstantItem(50).Placeholder();
                                        }
                                    }
                                    else
                                    {
                                        row.ConstantItem(50).Placeholder(); // File missing on disk
                                    }
                                }
                                else
                                {
                                    row.ConstantItem(50).Placeholder(); // No path in DB
                                }

                                row.RelativeItem().PaddingLeft(15).Column(c => {
                                    c.Item().Text(student.StudentFullName).FontSize(12).Bold();
                                    c.Item().Text($"Age: {student.Age}");
                                    c.Item().Text($"Card No: {student.InscriptionCardNumber}");
                                    c.Item().Text($"Valid until: {student.ExpirationCardDate}");
                                });
                            });
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
                });
            });

            try
            {
                byte[] pdfBytes = document.GeneratePdf();
                return Results.File(pdfBytes, "application/pdf", "StudentList.pdf");
            }
            catch (Exception ex)
            {               
                Console.WriteLine($"PDF Error: {ex.Message}");
                return Results.Problem("PDF Generation Failed: " + ex.Message);
            }
        }
    }
}