using EcommerceDemo.Models.Entidades;
using System.ComponentModel.DataAnnotations;

namespace EcommerceDemo.Models
{
    public class CarritoViewModel
    {
        public Usuario Usuario { get; set; }

        [DataType(DataType.MultilineText)]        
        public string Comentario { get; set; } = null;

        public ICollection<Temporal> Temporales { get; set; }
        
        public int Cantidad => Temporales == null ? 0 : Temporales.Sum(ts => ts.Cantidad);

        [DisplayFormat(DataFormatString = "{0:C2}")]
       
        public decimal Total => Temporales == null ? 0 : Temporales.Sum(ts => ts.Total);
    }
}
