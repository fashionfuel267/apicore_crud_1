using apicore_crud_1.Models;

namespace apicore_crud_1.DTO
{
    public class orderDTO
    {
        public Order Order { get; set; }
        public IFormFile PicFile { get; set; }
    }
}
