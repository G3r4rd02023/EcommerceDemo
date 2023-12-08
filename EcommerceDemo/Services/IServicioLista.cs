using Microsoft.AspNetCore.Mvc.Rendering;

namespace EcommerceDemo.Services
{
    public interface IServicioLista
    {
        Task<IEnumerable<SelectListItem>> GetListaCategorias();

    }
}
