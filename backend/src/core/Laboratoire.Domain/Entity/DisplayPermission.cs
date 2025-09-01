using System.ComponentModel.DataAnnotations;

namespace Laboratoire.Domain.Entity;

public class DisplayPermission
{
    [Required]
    public int? PermissionId { get; set; }
    [Required]
    public int? RoleId { get; set; }
    [Required]
    public string? RoleName { get; set; }
    public bool? Protocol { get; set; }
    public bool? Client { get; set; }
    public bool? Property { get; set; }
    public bool? CashFlow { get; set; }
    public bool? Partner { get; set; }
    public bool? Users { get; set; }
    public bool? Chemical { get; set; }
}
