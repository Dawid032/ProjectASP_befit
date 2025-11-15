using System.ComponentModel.DataAnnotations;

namespace BeFit.Models;

public class ExerciseType
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Nazwa ćwiczenia jest wymagana")]
    [Display(Name = "Nazwa ćwiczenia")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Nazwa ćwiczenia musi mieć od 2 do 100 znaków")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Opis")]
    [StringLength(500, ErrorMessage = "Opis nie może przekraczać 500 znaków")]
    public string? Description { get; set; }

    // Navigation property
    public ICollection<ExerciseExecution> ExerciseExecutions { get; set; } = new List<ExerciseExecution>();
}

