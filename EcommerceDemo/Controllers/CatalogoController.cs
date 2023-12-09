using Azure;
using EcommerceDemo.Models;
using EcommerceDemo.Models.Entidades;
using EcommerceDemo.Services;
using Firebase.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Controllers
{
    public class CatalogoController : Controller
    {
        
        private readonly EcommerceContext _context;
        private readonly IServicioUsuario _servicioUsuario;
        private readonly IServicioVenta _servicioVenta;

        public CatalogoController(EcommerceContext context, IServicioUsuario servicioUsuario, IServicioVenta servicioVenta)
        {
            _context = context;
            _servicioUsuario = servicioUsuario;
            _servicioVenta = servicioVenta;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "NameDesc" : "";
            ViewData["PriceSortParm"] = sortOrder == "Price" ? "PriceDesc" : "Price";
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewData["CurrentFilter"] = searchString;

            IQueryable<Producto> query = _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Inventario > 0);

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query
                    .Where(p => (p.Nombre.ToLower().Contains(searchString.ToLower()) ||
                                p.Categoria.Nombre.ToLower().Contains(searchString.ToLower())));
            }

            switch (sortOrder)
            {
                case "NameDesc":
                    query = query.OrderByDescending(p => p.Nombre);
                    break;
                default:
                    query = query.OrderBy(p => p.Nombre);
                    break;
            }

            int pageSize = 8;

            CatalogoViewModel model = new()
            {
                Productos = await PaginatedList<Producto>.CreateAsync(query, pageNumber ?? 1, pageSize),
                Categorias = await _context.Categorias.ToListAsync(),
            };

            return View(model);
        }

        public async Task<IActionResult> Detalles(int? id)
        {
            if (id == null || _context.Productos == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        public async Task<IActionResult> AgregarAlCarrito(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("IniciarSesion", "Login");
            }

            Producto producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            Usuario usuario = await _servicioUsuario.ObtenerUsuario(User.Identity.Name);
            if (usuario == null)
            {
                return NotFound();
            }

            Temporal temporalSale = new()
            {
                Producto = producto,
                Cantidad = 1,
                Usuario = usuario
            };

           

            _context.Temporales.Add(temporalSale);
            await _context.SaveChangesAsync();
            return View(nameof(VerCarrito));
        }

        [Authorize]
        public IActionResult ConfirmarVenta()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> VerCarrito()
        {
            Usuario usuario = await _servicioUsuario.ObtenerUsuario(User.Identity.Name);
            if (usuario == null)
            {
                return NotFound();
            }

            List<Temporal>? temporalSale = await _context.Temporales
                .Include(ts => ts.Producto)                
                .Where(ts => ts.Usuario.Id == usuario.Id)
                .ToListAsync();

            CarritoViewModel model = new()
            {
                Usuario = usuario,
                Temporales = temporalSale,
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerCarrito(CarritoViewModel model)
        {
            Usuario usuario = await _servicioUsuario.ObtenerUsuario(User.Identity.Name);
            if (usuario == null)
            {
                return NotFound();
            }

            model.Usuario = usuario;
            model.Temporales = await _context.Temporales
                .Include(ts => ts.Producto)                
                .Where(ts => ts.Usuario.Id == usuario.Id)
                .ToListAsync();

            Services.Response response = await _servicioVenta.ProcesarVenta(model);
            if (response.IsSuccess)
            {
                return RedirectToAction(nameof(ConfirmarVenta));
            }

            ModelState.AddModelError(string.Empty, response.Message);
            return View(model);
        }

        public async Task<IActionResult> DisminuirCantidad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Temporal temporal = await _context.Temporales.FindAsync(id);
            if (temporal == null)
            {
                return NotFound();
            }

            if (temporal.Cantidad > 1)
            {
                temporal.Cantidad--;
                _context.Temporales.Update(temporal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(VerCarrito));
        }

        public async Task<IActionResult> IncrementarCantidad(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Temporal temporal = await _context.Temporales.FindAsync(id);
            if (temporal == null)
            {
                return NotFound();
            }

            temporal.Cantidad++;
            _context.Temporales.Update(temporal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(VerCarrito));
        }

        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Temporal temporal = await _context.Temporales.FindAsync(id);
            if (temporal == null)
            {
                return NotFound();
            }

            _context.Temporales.Remove(temporal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(VerCarrito));
        }

        

    }
}
