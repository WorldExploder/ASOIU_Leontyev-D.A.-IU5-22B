using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Department
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = "";
    
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}