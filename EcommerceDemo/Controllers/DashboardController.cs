using EcommerceDemo.Enums;
using EcommerceDemo.Models;
using EcommerceDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Controllers
{
    public class DashboardController : Controller
    {
        
        private readonly EcommerceContext _context;
        private readonly IServicioUsuario _servicioUsuario;

        public DashboardController(EcommerceContext context, IServicioUsuario servicioUsuario)
        {
            _context = context;
            _servicioUsuario = servicioUsuario;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.CantidadUsuarios = _context.Usuarios.Count();
            ViewBag.CantidadProductos = _context.Productos.Count();
            ViewBag.CantidadVentas = _context.Ventas.Where(o => o.Estado == Estado.Nuevo).Count();
            ViewBag.CantidadVentasConfirmadas = _context.Ventas.Where(o => o.Estado == Estado.Confirmado).Count();

            return View(await _context.Temporales
                    .Include(u => u.Usuario)
                    .Include(p => p.Producto).ToListAsync());
        }
    }
}
