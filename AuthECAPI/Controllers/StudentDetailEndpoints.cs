using AuthECAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthECAPI.Controllers
{
    public static class StudentDetailEndpoints
    {
        public static IEndpointRouteBuilder MapStudentDetailEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/StudentDetail").RequireAuthorization(policy =>
                       policy.RequireRole("Admin")); ;

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
    }
}