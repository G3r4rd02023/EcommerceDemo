using EcommerceDemo.Models.Entidades;

namespace EcommerceDemo.Services
{
    public interface IServicioUsuario
    {
        Task<Usuario> AutenticarUsuario(string correo, string clave);
        Task<Usuario> CrearUsuario(Usuario usuario);
        Task<Usuario> ObtenerUsuario(string nombreUsuario);

    }
}
