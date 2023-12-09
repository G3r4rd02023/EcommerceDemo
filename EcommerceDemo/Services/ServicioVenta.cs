using EcommerceDemo.Enums;
using EcommerceDemo.Models;
using EcommerceDemo.Models.Entidades;
using Microsoft.EntityFrameworkCore;

namespace EcommerceDemo.Services
{
    public class ServicioVenta : IServicioVenta
    {
        private readonly EcommerceContext _context;

        public ServicioVenta(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<Response> CancelarVenta(int id)
        {
            Venta venta = await _context.Ventas
                .Include(s => s.DetallesVenta)
                .ThenInclude(sd => sd.Producto)
                .FirstOrDefaultAsync(s => s.Id == id);

            foreach (DetalleVenta detalleVenta in venta.DetallesVenta)
            {
                Producto producto = await _context.Productos.FindAsync(detalleVenta.Producto.Id);
                if (producto != null)
                {
                    producto.Inventario += detalleVenta.Cantidad;
                }
            }

            venta.Estado = Estado.Cancelado;
            await _context.SaveChangesAsync();
            return new Response { IsSuccess = true };
        }

        public async Task<Response> ProcesarVenta(CarritoViewModel model)
        {
            Response response = await ConfirmarInventario(model);
            if (!response.IsSuccess)
            {
                return response;
            }

            Venta venta = new()
            {
                Fecha = DateTime.UtcNow,
                Usuario = model.Usuario,
                Comentario = model.Comentario,
                DetallesVenta = new List<DetalleVenta>(),
                Estado = Estado.Nuevo
            };

            foreach (Temporal? item in model.Temporales)
            {
                venta.DetallesVenta.Add(new DetalleVenta
                {
                    Producto = item.Producto,
                    Cantidad = item.Cantidad,
                    Comentario = item.Comentario,
                });

                Producto producto = await _context.Productos.FindAsync(item.Producto.Id);
                if (producto != null)
                {
                    producto.Inventario -= item.Cantidad;
                    _context.Productos.Update(producto);
                }

                _context.Temporales.Remove(item);
            }

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();
            return response;
        }

        private async Task<Response> ConfirmarInventario(CarritoViewModel model)
        {
            Response response = new() { IsSuccess = true };
            foreach (Temporal? item in model.Temporales)
            {
                Producto producto = await _context.Productos.FindAsync(item.Producto.Id);
                if (producto == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"El producto {item.Producto.Nombre}, ya no está disponible";
                    return response;
                }
                if (producto.Inventario < item.Cantidad)
                {
                    response.IsSuccess = false;
                    response.Message = $"Lo sentimos no tenemos existencias suficientes del producto {item.Producto.Nombre}, para tomar su pedido. Por favor disminuir la cantidad o sustituirlo por otro.";
                    return response;
                }
            }
            return response;
        }
    }
}
