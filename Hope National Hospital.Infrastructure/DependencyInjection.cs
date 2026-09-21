using Hope_National_Hospital.Application.Interfaces;
using Hope_National_Hospital.Infrastructure.Services;
using Hope_National_Hospital.Infrastructure.Data;
using Hope_National_Hospital.Infrastructure.Identity;
using Hope_National_Hospital.Infrastructure.JWT;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hope_National_Hospoital.Infrastructure.Services;

namespace Hope_National_Hospoital.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("constr")));

            // Identity
            services
                .AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // JWT Setting
            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            // Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<ITreatmentService, TreatmentService>();
            services.AddScoped<IReceptionistService, ReceptionistService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IBedService, BedService>();
            services.AddScoped<IAdmissionService, AdmissionService>();

            return services;
        }
    }
}