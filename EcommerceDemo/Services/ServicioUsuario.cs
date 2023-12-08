using EcommerceDemo.Models;
using EcommerceDemo.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Services
{
    public class ServicioUsuario : IServicioUsuario
    {
        private readonly EcommerceContext _context;

        public ServicioUsuario(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<Usuario> AutenticarUsuario(string correo, string clave)
        {
            Usuario usuario = await _context.Usuarios.Where(u => u.Email == correo && u.Clave == clave)
               .Include(u => u.Rol)
               .FirstOrDefaultAsync();

            return usuario;
        }

        public async Task<Usuario> CrearUsuario(Usuario usuario)
        {                  
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        public async Task<Usuario> ObtenerUsuario(string nombreUsuario)
        {
            return await _context.Usuarios
               .FirstOrDefaultAsync(u => u.Nombre == nombreUsuario);
        }
    }
}
