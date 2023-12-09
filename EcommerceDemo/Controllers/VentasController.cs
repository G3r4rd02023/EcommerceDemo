using EcommerceDemo.Models;
using EcommerceDemo.Models.Entidades;
using EcommerceDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Controllers
{
    public class VentasController : Controller
    {
        private readonly EcommerceContext _context;
        private readonly IServicioVenta _servicioVenta;

        public VentasController(EcommerceContext context, IServicioVenta servicioVenta)
        {
            _context = context;
            _servicioVenta = servicioVenta;
        }

        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ventas
                .Include(s => s.Usuario)
                .Include(s => s.DetallesVenta)
                .ThenInclude(sd => sd.Producto)
                .ToListAsync());
        }

        //[Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Venta venta = await _context.Ventas
                .Include(s => s.Usuario)
                .Include(s => s.DetallesVenta)
                .ThenInclude(sd => sd.Producto)                
                .FirstOrDefaultAsync(s => s.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MisCompras()
        {
            return View(await _context.Ventas
                .Include(s => s.Usuario)
                .Include(s => s.DetallesVenta)
                .ThenInclude(sd => sd.Producto)
                .Where(s => s.Usuario.Email == User.Identity.Name)
                .ToListAsync());
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> MisDetalles(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Venta venta = await _context.Ventas
                .Include(s => s.Usuario)
                .Include(s => s.DetallesVenta)
                .ThenInclude(sd => sd.Producto)              
                .FirstOrDefaultAsync(s => s.Id == id);
            if (venta == null)
            {
                return NotFound();
            }

            return View(venta);
        }

    }
}
