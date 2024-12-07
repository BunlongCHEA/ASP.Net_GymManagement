using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Gym_ManagementSystem.Data;
using System.Text.Json.Serialization;

namespace Gym_ManagementSystem.Models
{
    public class ClassViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ClassName { get; set; }

        // Foreign key to the ApplicationUser
        //[ForeignKey("CreatedByUserID")]
        public string CreatedByUserID { get; set; }

        // Navigation property back to ApplicationUser
        public ApplicationUser User { get; set; }

        // Navigation one ClassViewModel to many ClassScheduleViewModel
        public List<ClassScheduleViewModel> ClassSchedules { get; set; }

        // Navigation one ClassViewModel to many WorkoutViewModel
        public List<WorkoutViewModel> Workouts { get; set; }
    }
}
