using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace University_MS.Models
{
    public class Student
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Student Id")]
        public int ID { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        [Column("FirstName")]
        public string LastName { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z""'\s-]*$")]
        public string FirstMidName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        [Display(Name="Enrollmet Date")]
        public DateTime EnrollmentDate { get; set; }
        
        [Display(Name="Semister")]
        
        public string Semi_Id { get; set; }
        
       
        [Display(Name="Accadamic Year")]
        public string Acc_Year { get; set; }
      
        public string FullName
        {
            get
            {
                return LastName + "" + FirstMidName;
            }
        }
        public ICollection<Enrollment> Enrollments { get; set; }


       


    }
}

