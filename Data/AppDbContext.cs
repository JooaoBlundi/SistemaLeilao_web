using Microsoft.EntityFrameworkCore;
using SistemaLeilao_web.Models;

namespace SistemaLeilao_web.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<UsuarioModel> Usuarios { get; set; }
    }
}
