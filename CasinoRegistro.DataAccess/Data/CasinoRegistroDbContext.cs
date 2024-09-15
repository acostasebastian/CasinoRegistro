using CasinoRegistro.Data;
using CasinoRegistro.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CasinoRegistro.DataAccess.Data
{
    public class CasinoRegistroDbContext :DbContext
    {
        public CasinoRegistroDbContext(DbContextOptions<CasinoRegistroDbContext> options)
            : base(options)
        {
                
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test;ConnectRetryCount=0");
        //}
        public DbSet<Plataforma> Plataforma { get; set; }
    }
}
