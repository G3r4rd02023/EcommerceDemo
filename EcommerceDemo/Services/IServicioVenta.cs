using EcommerceDemo.Models;

namespace EcommerceDemo.Services
{
    public interface IServicioVenta
    {
        Task<Response> ProcesarVenta(CarritoViewModel model);
        Task<Response> CancelarVenta(int id);


    }
}
