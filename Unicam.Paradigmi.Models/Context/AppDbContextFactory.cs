﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Unicam.Paradigmi.Models.Context
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost;Database=Paradigmi2;Trusted_Connection=True;TrustServerCertificate=True");
            
            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
