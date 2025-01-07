﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
namespace Lab06_ASP.Net.Models
{
    public class Coffee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CoffeeID { get; set; }
        [Required]
        public string CoffeeName { get; set; }
        [Required]
        public decimal S_Price { get; set; }
        [Required]
        public decimal L_Price { get; set; }
        [Required]
        public int? CategoryID { get; set; }
        [ForeignKey("CategoryID")]
        [ValidateNever]
        public virtual Category Category { get; set; }
        public string Description { get; set; }
        [ValidateNever]
        public string ImagePath { get; set; }
        [NotMapped]
        [ValidateNever]
        public IFormFile ImageFile { get; set; }
    }
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int CategoryID { get; set; }
        [Required]
        public string CategoryName { get; set; }
        public virtual ICollection<Coffee> Coffee { get; set; }
    }
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MessageID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerEmail { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CustomerPhoneNumber { get; set; }
        public string Type { get; set; }
    }
    public class CartItem
    {
        public int CoffeeID { get; set; }

        [ForeignKey("CoffeeID")]
        public virtual Coffee Coffee { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
    }

    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Total amount must be a positive value.")]
        public decimal TotalAmount { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; }

        [ValidateNever]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; } = new List<OrderDetail>();

        [Required]
        [StringLength(100, ErrorMessage = "Customer name cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string CustomerEmail { get; set; }

        [Required]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string CustomerPhone { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Delivery address cannot exceed 500 characters.")]
        public string DeliveryAddress { get; set; }
    }


    public enum OrderStatus
    {
        Pending,        // Đang chờ xử lý
        Processing,     // Đang xử lý
        Shipped,        // Đã giao hàng
        Delivered,      // Đã nhận hàng
        Cancelled       // Đã hủy
    }

    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [ForeignKey("OrderID")]
        public virtual Order Order { get; set; }

        [Required]
        public int CoffeeID { get; set; }

        [ForeignKey("CoffeeID")]
        public virtual Coffee Coffee { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "Size cannot exceed 10 characters.")]
        public string Size { get; set; }
    }

    public class IndexViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Coffee> Coffees { get; set; }
    }
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options) : base(options)
        {
        }
        public DbSet<Coffee> Coffees { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrdersDetail { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => od.OrderDetailID);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetail)
                .HasForeignKey(od => od.OrderID);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Coffee)
                .WithMany() // Không cần WithMany nếu Coffee không có Navigation Property đến OrderDetail
                .HasForeignKey(od => od.CoffeeID);

            modelBuilder.Entity<Coffee>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Coffee) // Giả sử Category có thuộc tính ICollection<Coffee> Coffees
                .HasForeignKey(c => c.CategoryID);
        }
    }

}
