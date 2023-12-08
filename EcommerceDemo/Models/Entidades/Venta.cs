using EcommerceDemo.Enums;
using System.ComponentModel.DataAnnotations;

namespace EcommerceDemo.Models.Entidades
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        public Usuario Usuario { get; set; }

        [DataType(DataType.MultilineText)]
        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Comentario { get; set; } = null;
        public Estado Estado { get; set; }

    }
}
