using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceWebApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public string ImagePath { get; set; }
        public int Stock { get; set; }
       public string SellerId { get; set; }
        [NotMapped]
        public int Quantity { get; set; }

        public string Category { get; set; }   

        public int Rating { get; set; }


    }
}
