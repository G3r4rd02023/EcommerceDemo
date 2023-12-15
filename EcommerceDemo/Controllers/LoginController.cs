using EcommerceDemo.Models;
using EcommerceDemo.Models.Entidades;
using EcommerceDemo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Controllers
{
    public class LoginController : Controller
    {

        private readonly IServicioUsuario _servicioUsuario;
        private readonly IServicioImagen _servicioImagen;
        private readonly EcommerceContext _context;

        public LoginController(IServicioUsuario servicioUsuario, IServicioImagen servicioImagen, EcommerceContext context)
        {
            _servicioUsuario = servicioUsuario;
            _servicioImagen = servicioImagen;
            _context = context;
        }

        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario, IFormFile Imagen)
        {
            Stream image = Imagen.OpenReadStream();
            string urlImagen = await _servicioImagen.SubirImagen(image, Imagen.FileName);

            usuario.Clave = ServicioClave.EncriptarClave(usuario.Clave);
            usuario.URLFoto = urlImagen;
            bool administradorExiste = _servicioUsuario.ExisteRol("Cliente");
            if (!administradorExiste)
            {
                Rol rol = new() { Nombre = "Cliente" };
                usuario.Rol = rol;
            }
            else
            {
                usuario.Rol.Nombre = "Cliente";
            }
            

            Usuario usuarioCreado = await _servicioUsuario.CrearUsuario(usuario);

            if (usuarioCreado.Id > 0)
            {
                return RedirectToAction("IniciarSesion", "Login");
            }

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }

        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(string correo, string clave)
        {
            Usuario usuarioEncontrado = await _servicioUsuario.AutenticarUsuario(correo, ServicioClave.EncriptarClave(clave));

            if (usuarioEncontrado == null)
            {
                ViewData["Mensaje"] = "Usuario no encontrado";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuarioEncontrado.Nombre),
                new Claim("FotoPerfil", usuarioEncontrado.URLFoto),
                new Claim(ClaimTypes.Role, usuarioEncontrado.Rol.Nombre)
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                properties
                );

            return RedirectToAction("Index", "Home");

        }

        public async Task<IActionResult> ListaUsuarios()
        {
            return View(await _context.Usuarios
            .Include(u => u.Rol)
            .ToListAsync());
        }

        public IActionResult CrearAdministrador()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearAdministrador(Usuario usuario, IFormFile Imagen)
        {
            Stream image = Imagen.OpenReadStream();
            string urlImagen = await _servicioImagen.SubirImagen(image, Imagen.FileName);

            usuario.Clave = ServicioClave.EncriptarClave(usuario.Clave);
            usuario.URLFoto = urlImagen;
            bool administradorExiste = _servicioUsuario.ExisteRol("Administrador");
            if (!administradorExiste)
            {
                Rol rol = new() { Nombre = "Administrador" };
                usuario.Rol = rol;
            }
            else
            {
                Rol rolExistente = _servicioUsuario.ObtenerRol("Administrador");
                usuario.Rol = rolExistente;
            }


            Usuario usuarioCreado = await _servicioUsuario.CrearUsuario(usuario);

            if (usuarioCreado.Id > 0)
            {
                return RedirectToAction("IniciarSesion", "Login");
            }

            ViewData["Mensaje"] = "No se pudo crear el usuario";
            return View();
        }


    }
}
