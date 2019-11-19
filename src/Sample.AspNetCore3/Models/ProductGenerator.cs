﻿namespace Sample.AspNetCore3.Models
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Sample.AspNetCore3.Data;

    public class ProductGenerator
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new StoreDBContext(
                serviceProvider.GetRequiredService<DbContextOptions<StoreDBContext>>()))
            {
                
                if (context.Products.Any())
                {
                    return;   
                }

                context.Products.AddRange(
                    new Product()
                    {
                        ProductId = 1,
                        Class = "ProductGroup1",
                        Type = "PRODUCT",
                        Name = "Puma Black Sneakers Shoes",
                        Reference = "P1",
                        Price = 230
                    }, new Product
                    {
                        ProductId = 2,
                        Class = "ProductGroup1",
                        Type = "PRODUCT",
                        Name = "Nike Metcon 5",
                        Reference = "P2",
                        Price = 750
                    });

                context.SaveChanges();
            }
        }
    }
}