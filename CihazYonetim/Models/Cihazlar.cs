namespace CihazYonetim.Models;
public enum CihazDurumu
{
    Offline = 0,
    Online = 1,
    Ariza = 2,
    Bakim = 3
}

public enum CihazTuru
{
    Router=0,
    Printer=1,
    Computer=2
}
public class Cihazlar
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CihazName  { get; set; }
    
    public CihazTuru Turu { get; set; }
    
    public CihazDurumu Durum { get; set; }
    public User User { get; set; } // Cihazın sahibi olan kullanıcı nesnesi
    public double PositionX { get; set; }
    public double PositionY { get; set; }
}