using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointments.Dto;

namespace Appointments.ReadModel
{
    public class ReadModelContext : DbContext
    {

        public ReadModelContext() : base("appointments")
        {
        }

        public DbSet<AppointmentReadModel> Appointments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }

}
