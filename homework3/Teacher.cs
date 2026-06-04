using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Teacher
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = "";
    
    [Range(0, int.MaxValue)]
    public int Publications { get; set; }
    
    [ForeignKey("Department")]
    public int DepartmentId { get; set; }
    
    public Department? Department { get; set; }
}