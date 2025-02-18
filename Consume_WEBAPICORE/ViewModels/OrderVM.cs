using System.ComponentModel.DataAnnotations;

namespace Consume_WEBAPICORE.ViewModels
{
    public class OrderVM
    {
        public int Id { get; set; }

        [Required, StringLength(50)]
        public string CustomerName { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }
        public string Picture { get; set; }
      public IFormFile Image { get; set; }

        public bool IsDelivered { get; set; }

     
        public virtual ICollection<DetailsVM> Details { get; set; } = new List<DetailsVM>();
    }
}
