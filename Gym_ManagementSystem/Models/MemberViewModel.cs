using Gym_ManagementSystem.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gym_ManagementSystem.Models
{
    public class MemberViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MemberName { get; set;}

        [Required]
        public string Contact {  get; set;}

        [Required]
        [EmailAddress]
        public string Email { get; set;}

        [Required]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly JoinedMembershipDate { get; set; }

        // Foreign key to the ApplicationUser
        //[ForeignKey("CreatedByUserID")]
        //[Required]
        public string CreatedByUserID { get; set; }

        //Navigation property back to ApplicationUser
        //[JsonIgnore]
        public ApplicationUser User { get; set; }

        // Navigation one MemberViewModel to many WorkoutViewModel
        //[JsonIgnore]
        public List<WorkoutViewModel> Workouts { get; set; }

        // Navigation one MemberViewModel to many MembershipViewModel
        //[JsonIgnore]
        public MembershipViewModel Memberships { get; set; }
    }
}
