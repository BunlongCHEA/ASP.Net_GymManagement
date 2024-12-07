using Gym_ManagementSystem.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_ManagementSystem.Models
{
    public class MembershipViewModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SubscribedMonth { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal DiscountPercentage { get; set; }

        [Required]
        [Column(TypeName = "decimal(20, 2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly RenewalDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateOnly ExpiredDate {  get; set; }

        // Foreign key to the MemberViewModel
        //[ForeignKey("MemberId")]
        //[Required]
        public int MemberId { get; set; }

        //Navigation property back to MemberViewModel
        //[JsonIgnore]
        public MemberViewModel Member { get; set; }

        // Foreign key to the ApplicationUser
        //[ForeignKey("CreatedByUserID")]
        //[Required]
        public string CreatedByUserID { get; set; }

        //Navigation property back to ApplicationUser
        //[JsonIgnore]
        public ApplicationUser User { get; set; }
    }
}
