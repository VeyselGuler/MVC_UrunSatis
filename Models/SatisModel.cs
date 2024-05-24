using System.ComponentModel.DataAnnotations;

namespace urunsatis_HW.Models;

public class UrunModel
{ 
    public string Ad { get; set; }
    public int Stok { get; set; }
    public int Fiyat { get; set; }
    public string Img { get; set; }

}

public class Satis
{
    public string Ad { get; set; }
    public int Para { get; set; }
    public string odemeSekli { get; set; }
    public int Adet { get; set; }

}

public class LogModel
{
    public string Name { get; set; }
    public int Price { get; set; }
    public string Stok { get; set; }
    public string OdemeSekli { get; set; }
}
