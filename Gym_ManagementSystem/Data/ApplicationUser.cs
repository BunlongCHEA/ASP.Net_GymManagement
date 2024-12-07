using Gym_ManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

namespace Gym_ManagementSystem.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }

        //[JsonIgnore]
        public List<MemberViewModel> Member { get; set; }

        //[JsonIgnore]
        public List<TrainerViewModel> Trainers { get; set; }

        //[JsonIgnore]
        public List<ClassViewModel> Classes { get; set; }

        //[JsonIgnore]
        public List<WorkoutViewModel> Workouts { get; set; }

        //[JsonIgnore]
        public List<MembershipViewModel> Memberships { get; set; }
    }
}
