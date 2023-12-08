using System.ComponentModel.DataAnnotations;

namespace EcommerceDemo.Models.Entidades
{
    public class Temporal
    {
        public int Id { get; set; }

        public Usuario Usuario { get; set; }

        public Producto Producto { get; set; }

        public int Cantidad { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Comentario { get; set; } = null;
    }
}
