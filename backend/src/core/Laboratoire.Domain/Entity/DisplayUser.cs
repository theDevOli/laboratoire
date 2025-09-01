using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class DisplayUser
{
    [Required]
    public Guid? UserId { get; set; }
    [Required]
    public string? RoleName { get; set; }
    [Required]
    public int? RoleId { get; set; }
    public Guid? PartnerId { get; set; }
    [Required]
    public string? Username { get; set; }
    [Required]
    public string? Name { get; set; }
    [Required]
    public bool? IsActive { get; set; } = true;
}
