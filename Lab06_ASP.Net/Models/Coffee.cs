using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
namespace Lab06_ASP.Net.Models
{
    public class Coffee
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Small size price is required.")]
        public decimal S_Price { get; set; }
        [Required(ErrorMessage = "Large size price is required.")]
        public decimal L_Price { get; set; }
        [ValidateNever]
        public string ImagePath { get; set; }
        [Required(ErrorMessage = "Coffee type is required.")]
        [RegularExpression("^(hot|cold)$", ErrorMessage = "Coffee Type must be 'hot' or 'cold'.")]
        public string Type { get; set; }
        [NotMapped]
        [ValidateNever]
        public IFormFile ImageFile { get; set; }
    }
    public class Order
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }

    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {
        }
        public DbSet<Coffee> Coffees { get; set; }
        public DbSet<Order> Orders { get; set; }

    }
    public class CoffeeViewModel
    {
        public List<Coffee> CoffeeHotList { get; set; }
        public List<Coffee> CoffeeColdList { get; set; }
    }

}
