using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Gym_ManagementSystem.Models
{
    public class ClassScheduleViewModel
    {
        [Key]
        public int Id { get; set; }

        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeOnly OpenTime { get; set; }

        //[DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public TimeOnly EndTime { get; set; }

        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly OpenDate { get; set; }

        // Foreign key to the ClassViewModel
        //[ForeignKey("ClassId")]
        public int ClassId { get; set; }

        // Navigation property back to ClassViewModel
        public ClassViewModel Class {  get; set; }
    }
}
