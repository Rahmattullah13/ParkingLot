using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        const int totalLotMobil = 10; // jumlah lot untuk mobil
        const int totalLotMotor = 10; // jumlah lot untuk motor

        var parkir = new Dictionary<string, Kendaraan>();

        while (true)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Check-In");
            Console.WriteLine("2. Check-Out");
            Console.WriteLine("3. Report");
            Console.WriteLine("4. Exit");

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("Masukkan nomor polisi kendaraan:");
                    var nomorPolisi = Console.ReadLine().Trim().ToUpper();
                    Console.WriteLine("Masukkan jenis kendaraan (Mobil/Motor):");
                    var jenisKendaraan = Console.ReadLine().Trim().ToLower();

                    Kendaraan kendaraan;

                    if (jenisKendaraan == "mobil")
                    {
                        kendaraan = new Mobil(nomorPolisi);
                    }
                    else if (jenisKendaraan == "motor")
                    {
                        kendaraan = new Motor(nomorPolisi);
                    }
                    else
                    {
                        Console.WriteLine("Jenis kendaraan tidak valid.");
                        break;
                    }

                    if (parkir.ContainsKey(nomorPolisi))
                    {
                        Console.WriteLine("Kendaraan sudah terdaftar.");
                        break;
                    }

                    int lot;

                    if (kendaraan is Mobil)
                    {
                        lot = parkir.Count(k => k.Value is Mobil) + 1;

                        if (lot > totalLotMobil)
                        {
                            Console.WriteLine("Lot untuk mobil sudah penuh.");
                            break;
                        }
                    }
                    else
                    {
                        lot = parkir.Count(k => k.Value is Motor) + 1;

                        if (lot > totalLotMotor)
                        {
                            Console.WriteLine("Lot untuk motor sudah penuh.");
                            break;
                        }
                    }

                    kendaraan.Lot = lot;
                    parkir[nomorPolisi] = kendaraan;

                    Console.WriteLine($"Kendaraan berhasil parkir di lot {lot}.");
                    break;

                case "2":
                    Console.WriteLine("Masukkan nomor polisi kendaraan:");
                    nomorPolisi = Console.ReadLine().Trim().ToUpper();

                    if (!parkir.ContainsKey(nomorPolisi))
                    {
                        Console.WriteLine("Kendaraan tidak terdaftar.");
                        break;
                    }

                    kendaraan = parkir[nomorPolisi];

                    // periksa apakah kendaraan sudah parkir selama minimal 1 jam
                    TimeSpan durasiParkir = DateTime.Now - kendaraan.CheckInTime;
                    if (durasiParkir.TotalHours < 1)
                    {
                        Console.WriteLine("Kendaraan belum mencapai 1 jam.");
                        break;
                    }

                    if (kendaraan.CheckOut())
                    {
                        Console.WriteLine($"Biaya parkir: {kendaraan.BiayaParkir.ToString("C")}.");
                        parkir.Remove(nomorPolisi);
                        Console.WriteLine("Kendaraan berhasil check-out.");
                    }
                    else
                    {
                        Console.WriteLine("Kendaraan belum mencapai 1 jam.");
                    }

                    break;


                case "3":
                    Console.WriteLine($"Jumlah lot untuk mobil terisi: {parkir.Count(k => k.Value is Mobil)}.");
                    Console.WriteLine($"Jumlah lot untuk mobil tersedia: {totalLotMobil - parkir.Count(k => k.Value is Mobil)}.");
                    Console.WriteLine($"Jumlah lot untuk motor terisi: {parkir.Count(k => k.Value is Motor)}.");
                    Console.WriteLine($"Jumlah lot untuk motor tersedia: {totalLotMotor - parkir.Count(k => k.Value is Motor)}.");

                    var kendaraanGanjil = parkir.Values.Where(k => k.NomorPolisi[k.NomorPolisi.Length - 1] % 2 == 1);
                    var kendaraanGenap = parkir.Values.Where(k => k.NomorPolisi[k.NomorPolisi.Length - 1] % 2 == 0);

                    Console.WriteLine($"Jumlah kendaraan dengan nomor polisi ganjil: {kendaraanGanjil.Count()}.");
                    Console.WriteLine($"Jumlah kendaraan dengan nomor polisi genap: {kendaraanGenap.Count()}.");

                    var kendaraanMobil = parkir.Values.Where(k => k is Mobil);
                    var kendaraanMotor = parkir.Values.Where(k => k is Motor);

                    Console.WriteLine($"Jumlah kendaraan mobil: {kendaraanMobil.Count()}.");
                    Console.WriteLine($"Jumlah kendaraan motor: {kendaraanMotor.Count()}.");

                    var warnaKendaraan = parkir.Values.GroupBy(k => k.Warna).Select(g => new { Warna = g.Key, Jumlah = g.Count() });

                    foreach (var data in warnaKendaraan)
                    {
                        Console.WriteLine($"Jumlah kendaraan dengan warna {data.Warna}: {data.Jumlah}.");
                    }

                    break;

                case "4":
                    return;

                default:
                    Console.WriteLine("Input tidak valid.");
                    break;
            }
        }
    }
}

abstract class Kendaraan
{
    public string NomorPolisi { get; }
    public int Lot { get; set; }
    public DateTime CheckInTime { get; }

    public Kendaraan(string nomorPolisi)
    {
        NomorPolisi = nomorPolisi;
        CheckInTime = DateTime.Now;
    }

    public decimal BiayaParkir => (decimal)(DateTime.Now - CheckInTime).TotalHours * BiayaPerJam;

    public abstract decimal BiayaPerJam { get; }
    public abstract string Warna { get; }

    public bool CheckOut()
    {
        return (DateTime.Now - CheckInTime).TotalHours >= 1;
    }
}

class Mobil : Kendaraan
{
    public override decimal BiayaPerJam => 5000;
    public override string Warna => "Merah";

    public Mobil(string nomorPolisi) : base(nomorPolisi) { }
}

class Motor : Kendaraan

{
    public override decimal BiayaPerJam => 2000;
    public override string Warna => "Biru";

    public Motor(string nomorPolisi) : base(nomorPolisi) { }
}
