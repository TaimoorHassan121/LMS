using DaewooLMS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DaewooLMS.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
          : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<Terminal> Terminals { get; set; }
        public DbSet<TerminalCCP> TerminalCCPs { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Training> Trainings { get; set; }
        public DbSet<QuizQuestionAnswer> QuizQuestionAnswer { get; set; }
        public DbSet<QuizOptions> QuizOptions { get; set; }
        public DbSet<QuizLog> QuizLogs { get; set; }
        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<NewHrPolicies> NewHrPolicies { get; set; }
        public DbSet<AddPolicies> AddPolicies { get; set; }
        public DbSet<Emp_Chat_M> Emp_Chat_M { get; set; }
        public DbSet<Emp_Chat_Reply> Emp_Chat_Replies { get; set; }
        public DbSet<LibraryData> LibraryDatas { get; set; }
        public DbSet<Videos> Videos { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Event_Logs> EventLogs { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<myNotes> MyNotes { get; set; }
        public DbSet<Support> Support { get; set; }
    }
}
