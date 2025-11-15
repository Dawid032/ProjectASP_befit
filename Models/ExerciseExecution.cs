using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models;

public class ExerciseExecution
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Sesja treningowa jest wymagana")]
    [Display(Name = "Sesja treningowa")]
    public int TrainingSessionId { get; set; }

    [ForeignKey("TrainingSessionId")]
    public TrainingSession? TrainingSession { get; set; }

    [Required(ErrorMessage = "Typ ćwiczenia jest wymagany")]
    [Display(Name = "Typ ćwiczenia")]
    public int ExerciseTypeId { get; set; }

    [ForeignKey("ExerciseTypeId")]
    public ExerciseType? ExerciseType { get; set; }

    [Required(ErrorMessage = "Obciążenie jest wymagane")]
    [Display(Name = "Obciążenie (kg)")]
    [Range(0.01, 1000, ErrorMessage = "Obciążenie musi być między 0.01 a 1000 kg")]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Weight { get; set; }

    [Required(ErrorMessage = "Liczba serii jest wymagana")]
    [Display(Name = "Liczba serii")]
    [Range(1, 100, ErrorMessage = "Liczba serii musi być między 1 a 100")]
    public int NumberOfSets { get; set; }

    [Required(ErrorMessage = "Liczba powtórzeń jest wymagana")]
    [Display(Name = "Liczba powtórzeń w serii")]
    [Range(1, 1000, ErrorMessage = "Liczba powtórzeń musi być między 1 a 1000")]
    public int RepetitionsPerSet { get; set; }

    [Display(Name = "Uwagi")]
    [StringLength(500, ErrorMessage = "Uwagi nie mogą przekraczać 500 znaków")]
    public string? Notes { get; set; }
}

