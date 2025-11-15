using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models;

public class TrainingSession
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa sesji jest wymagana")]
    [Display(Name = "Nazwa sesji")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa sesji musi mieć od 2 do 100 znaków")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Data i czas rozpoczęcia są wymagane")]
    [Display(Name = "Data i czas rozpoczęcia")]
    [DataType(DataType.DateTime)]
    public DateTime StartDateTime { get; set; }

    [Required(ErrorMessage = "Data i czas zakończenia są wymagane")]
    [Display(Name = "Data i czas zakończenia")]
    [DataType(DataType.DateTime)]
    public DateTime EndDateTime { get; set; }

    [Display(Name = "Uwagi")]
    [StringLength(1000, ErrorMessage = "Uwagi nie mogą przekraczać 1000 znaków")]
    public string? Notes { get; set; }

    // Foreign key to Identity User
    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public Microsoft.AspNetCore.Identity.IdentityUser? User { get; set; }

    // Navigation property
    public ICollection<ExerciseExecution> ExerciseExecutions { get; set; } = new List<ExerciseExecution>();
}

