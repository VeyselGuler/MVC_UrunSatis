using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using urunsatis_HW.Models;

namespace urunsatis_HW.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var urunler = getAllItems();
        return View(urunler);
    }
    
    [HttpGet]
    public IActionResult Admin()
    {
        
        return View(getAllLogs());
    }

    public List<UrunModel> getAllItems()
    {
        var urunler = new List<UrunModel>();
        using StreamReader reader = new("App_Data/index.txt");
        var urunlerTxt = reader.ReadToEnd();

        var urunlerLines = urunlerTxt.Split('\n');
        foreach (var urunlerLine in urunlerLines)
        {
            var urunParts = urunlerLine.Split('|');
            urunler.Add(
                new UrunModel()
                {
                    Ad = urunParts[0],
                    Stok = int.Parse(urunParts[1]),
                    Fiyat = int.Parse(urunParts[2]),
                    Img = urunParts[3]
                }
            );
        }
        
        return urunler;
    }

    public List<LogModel> getAllLogs()
    {
        var loglar = new List<LogModel>();
        using StreamReader reader = new("App_Data/log.txt");
        var loglarTxt = reader.ReadToEnd();

        var loglarLines = loglarTxt.Split('\n');
        foreach (var loglarLine in loglarLines)
        {
            var logParts = loglarLine.Split('|');
            loglar.Add(
                new LogModel()
                {
                    Name = logParts[0],
                    Price = int.Parse(logParts[1]),
                    Stok = logParts[2],
                    OdemeSekli = logParts[3],
                }
            );
        }

        int toplamKazanc = 0;
        foreach (var log in loglar)
        {
            toplamKazanc += log.Price;
        }

        ViewData["ciro"] = $"<div class=\"report\"><h3>Kazanç Raporu</h3><p>Toplam Ciro: {toplamKazanc}TL</p></div>\n";
        return loglar;
    }
    
    [HttpPost]
    public IActionResult Index(Satis model)
    {
        var urunler = getAllItems();
        
        var alinacakUrun = new UrunModel();
        foreach (var urun in urunler)
        {
            if (urun.Ad == model.Ad)
            {
                alinacakUrun = urun;
                break;
            }
        }

        if (alinacakUrun.Stok <= 0)
        {
            ViewData["report"] = $"<div class=\"report\"> <h3>Ödeme Raporu</h3> <p>Bu ürünün stoğu kalmadı.</p> </div>\n";
            return View(urunler);
        }

        var paraUstu = model.Para - (alinacakUrun.Fiyat * model.Adet);
        if (paraUstu >= 0)
        {
            alinacakUrun.Stok -= model.Adet;
            SaveUpdate(urunler);
            
            ViewData["report"] = $"<div class=\"report\"> <h3>Ödeme Raporu</h3> <p>Teşekkür ederiz. {(paraUstu > 0 ? $"Para üstünüz {paraUstu} TL" : "")}</p> </div>\n";
            
            using StreamReader reader = new("App_Data/log.txt");
            var logTxt = reader.ReadToEnd();
            reader.Close();
        
            if (!string.IsNullOrEmpty(logTxt))
            {
                logTxt += "\n";
            }
        
            using StreamWriter writer = new("App_Data/log.txt");
            writer.Write($"{logTxt}{alinacakUrun.Ad}|{(alinacakUrun.Fiyat*model.Adet)}|{model.Adet}|{model.odemeSekli}");
            writer.Close();
            
        }
        else
        {
            ViewData["report"] = $"<div class=\"report\"> <h3>Ödeme Raporu</h3> <p>Paranız yetersiz. {(alinacakUrun.Fiyat * model.Adet) - model.Para}TL ekleme yapınız</p> </div>\n";
        }
        
        
        
        
        return View(urunler);
    }

    public void SaveUpdate(List<UrunModel> urunler)
    {
        var satirlarTxt = "";
        foreach (var urun in urunler)
        {
            satirlarTxt += $"{urun.Ad}|{urun.Stok}|{urun.Fiyat}|{urun.Img}{(urun != urunler.Last() ? "\n" : "")}";
        }

        using StreamWriter writer = new("App_Data/index.txt");
        writer.Write(satirlarTxt);
    }
    
}