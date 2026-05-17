using System.Collections.Generic; // ICollection kullanmak için bu şart
using System.ComponentModel.DataAnnotations; 

namespace CihazYonetim.Models;

public class User
{
    public int Id { get; set; }

    [MaxLength(50)] 
    public string Username { get; set; } = string.Empty; 

    [MaxLength(256)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Role { get; set; } = string.Empty;

    // YENİ BALYOZ: Bir kullanıcının birden fazla cihazı olabilir (Liste mantığı)
    public virtual ICollection<Cihazlar> Cihazlar { get; set; } = new List<Cihazlar>();
}