using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Gym_ManagementSystem.Data;
using System.Text.Json.Serialization;

namespace Gym_ManagementSystem.Models
{
    public class WorkoutViewModel
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey("ClassId")]
        public int ClassId { get; set; }

        //[ForeignKey("TrainerId")]
        public int TrainerId { get; set; }

        //[ForeignKey("MemberId")]
        public int MemberId { get; set; }

        //[ForeignKey("CreatedByUserID")]
        public string CreatedByUserID { get; set; }

        // Navigation property back to ClassViewModel
        [JsonIgnore]
        public ClassViewModel Class { get; set; }

        // Navigation property back to TrainerViewModel
        [JsonIgnore]
        public TrainerViewModel Trainer { get; set; }

        // Navigation property back to MemberViewModel
        [JsonIgnore]
        public MemberViewModel Member { get; set; }

        // Navigation property back to ApplicationUser
        [JsonIgnore]
        public ApplicationUser User { get; set; }
    }
}
