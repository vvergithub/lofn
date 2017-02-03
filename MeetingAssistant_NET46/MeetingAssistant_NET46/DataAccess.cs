using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MeetingAssistant_NET46
{
    public class AzureDb : DbContext
    {
        public AzureDb(string connectionString) : base(connectionString)
        {   
        }

        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<Category> Categories { get; set; }
    }

    [Table("Categories")]
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Category()
        {
            Id = Guid.NewGuid();
        }
    }

    [Table("Content")]
    public class Content
    {
        public Guid Id { get; set; }

        // [Key, Column(Order = 0)]
        public Guid MeetingId { get; set; }
        // [Key, Column(Order = 1)]
        public int Sequence { get; set; }

        public Guid EmployeeId { get; set; }

        public string Line { get; set; }
        public Guid CategoryId { get; set; }

        public Content()
        {
            Id = Guid.NewGuid();
        }
    }

    [Table("Employees")]
    public class Employee
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Employee()
        {
            Id = Guid.NewGuid();
        }
    }

    [Table("Meetings")]
    public class Meeting
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Location { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public Meeting()
        {
            Id = Guid.NewGuid();
        }
    }

    [Table("Participants")]
    public class Participant
    {
        [Key, Column(Order = 0)]
        public Guid MeetingId { get; set; }
        [Key, Column(Order = 1)]
        public Guid EmployeeId { get; set; }

        public bool Organizer { get; set; }
    }

}