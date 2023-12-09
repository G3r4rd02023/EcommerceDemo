using EcommerceDemo.Models.Entidades;
using EcommerceDemo.Services;

namespace EcommerceDemo.Models
{
    public class CatalogoViewModel
    {
        public int Cantidad { get; set; }

        public PaginatedList<Producto> Productos { get; set; }

        public ICollection<Categoria> Categorias { get; set; }
    }
}
