using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ProductSearchApp
{
    public class Product //Get products to list for User
    {
        public string Product_Number { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Units_On_Hand { get; set; }
        public decimal Price { get; set; }

        [Key] //Prim key reference
        public int ProductId { get; set; }
    }

    public class ProductDBContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //connection string directly to file location.
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\brent\source\repos\CPT206Lab1BrentBindewald\CPT206Lab1BrentBindewald\ProductDB.mdf;Integrated Security=True");
        }
    }

    class Program
    {
        static void Main()
        {
            using (var dbContext = new ProductDBContext())  // Display all On-Hand products sorted by units in asc. order
            {

                var allProducts = from product in dbContext.Products
                                  orderby product.Units_On_Hand
                                  select product;

                Console.WriteLine("All Products sorted by Units On Hand:");
                foreach (var product in allProducts)
                {
                    Console.WriteLine($"Product Number: {product.Product_Number}, Units On Hand: {product.Units_On_Hand}");
                }

                
                Console.WriteLine("\nEnter product number to search:"); //Product Search Function
                string productNumber = Console.ReadLine()!;
                Console.WriteLine("Enter text to search in descriptions:");
                string searchText = Console.ReadLine()!;
                ProductSearch(productNumber, searchText, dbContext);

                
                Console.WriteLine("\nEnter minimum units on hand:"); // Units on Hand Search Function
                int minUnits = int.Parse(Console.ReadLine()!);
                Console.WriteLine("Enter maximum units on hand:");
                int maxUnits = int.Parse(Console.ReadLine()!);
                ProductUnitsOnHandSearch(minUnits, maxUnits, dbContext);

                
                Console.WriteLine("\nEnter minimum price:"); // Price Search Function
                decimal minPrice = decimal.Parse(Console.ReadLine()!);
                Console.WriteLine("Enter maximum price:");
                decimal maxPrice = decimal.Parse(Console.ReadLine()!);
                ProductPriceSearch(minPrice, maxPrice, dbContext);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        // Part 1: PRODUCT SEARCH
        static void ProductSearch(string productNumber, string searchText, ProductDBContext dbContext)
        {
            var products = from product in dbContext.Products
                           where product.Product_Number == productNumber
                                 && product.Description.Contains(searchText)
                           select product;

            Console.WriteLine("\nProducts matching the search criteria:");
            foreach (var product in products)
            {
                Console.WriteLine($"Product Number: {product.Product_Number}, Description: {product.Description}");
            }
        }

        
        static void ProductUnitsOnHandSearch(int minUnits, int maxUnits, ProductDBContext dbContext) //Units on Hand Search
        {
            var productsMoreThan = from product in dbContext.Products
                                   where product.Units_On_Hand > minUnits
                                   select product;

            var productsLessThan = from product in dbContext.Products
                                   where product.Units_On_Hand < maxUnits
                                   select product;

            Console.WriteLine($"\nProducts with more than {minUnits} units on hand:");
            foreach (var product in productsMoreThan)
            {
                Console.WriteLine($"Product Number: {product.Product_Number}, Units On Hand: {product.Units_On_Hand}");
            }

            Console.WriteLine($"\nProducts with less than {maxUnits} units on hand:");
            foreach (var product in productsLessThan)
            {
                Console.WriteLine($"Product Number: {product.Product_Number}, Units On Hand: {product.Units_On_Hand}");
            }
        }

        
        static void ProductPriceSearch(decimal minPrice, decimal maxPrice, ProductDBContext dbContext) // Product Price Search
        {
            var productsInRange = from product in dbContext.Products
                                  where product.Price >= minPrice && product.Price <= maxPrice
                                  orderby product.Price
                                  select product;

            Console.WriteLine($"\nProducts within the price range {minPrice:C} - {maxPrice:C}:");
            foreach (var product in productsInRange)
            {
                Console.WriteLine($"Product Number: {product.Product_Number}, Price: {product.Price:C}");
            }
        }
    }
}
