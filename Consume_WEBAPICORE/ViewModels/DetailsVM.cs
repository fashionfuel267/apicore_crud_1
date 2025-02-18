using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Consume_WEBAPICORE.ViewModels
{
    public class DetailsVM
    {
        public int Id { get; set; }
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }
        [Required, DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public virtual ProductVM Product { get; set; }
        public virtual OrderVM Order { get; set; }

    }
}
