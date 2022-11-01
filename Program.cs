using ManejoPresupuesto.Models;
using ManejoPresupuesto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

var politicaUsuarioAutenticado = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                                                                 .Build();
// Add services to the container.
builder.Services.AddControllersWithViews(opc => {
    opc.Filters.Add(new AuthorizeFilter(politicaUsuarioAutenticado));
});

/* Relacionando Servicio con Interfaz */
builder.Services.AddTransient<ITiposCuentasRepository, TiposCuentasRepository>();
builder.Services.AddTransient<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddTransient<ICuentasRepository, CuentasRepository>();
builder.Services.AddTransient<ICategoriasRepository, CategoriasRepository>();
builder.Services.AddTransient<ITransaccionesRepository, TransaccionesRepository>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IReportesRepository, ReportesRepository>();
builder.Services.AddTransient<IUserStore<UsuarioModel>, UsuarioStore>();
builder.Services.AddTransient<SignInManager<UsuarioModel>>();

builder.Services.AddIdentityCore<UsuarioModel>(opc => {
    opc.Password.RequireDigit = false;
    opc.Password.RequireLowercase = false;
    opc.Password.RequireUppercase = false;
    opc.Password.RequireNonAlphanumeric = false;
}).AddErrorDescriber<MensajesErrorIdentity>();

builder.Services.AddAuthentication(opc => {
    opc.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    opc.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    opc.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, opc => {
    opc.LoginPath = "/Usuarios/Login";
});
 

/* Configuracion de AutoMapper */
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Index}/{id?}");

app.Run();
