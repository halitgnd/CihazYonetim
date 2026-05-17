using System;
using System.ComponentModel.DataAnnotations;

namespace CihazYonetim.Models
{
    public class CihazLog
    {
        [Key]
        public int Id { get; set; }

        // Hangi cihazda işlem yapıldığını tutar. 
        // Yanına '?' koydum (nullable) ki yarın öbür gün cihazı komple silersen, 
        // ona ait geçmiş loglar veritabanında patlamadan kalabilsin.
        public int? CihazId { get; set; } 

        [Required]
        [MaxLength(100)]
        public string Islem { get; set; }

        [Required]
        [MaxLength(500)]
        public string Detay { get; set; } // Örn: "Computer-1 durumu Arıza olarak değiştirildi."

        // Veri eklendiği an o anın saatini otomatik basar, senin ekstra uğraşmana gerek kalmaz
        public DateTime LogTarihi { get; set; } = DateTime.Now; 
    }
}