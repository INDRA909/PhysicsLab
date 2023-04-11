using AIPS;
using System.Numerics;

class Program
{
    public static void Main()
    {

        var signal = new StreamReader(@"C:\Users\bid57\OneDrive\Desktop\PR\PhysicsLab\Сигналы\2.txt")
                                                                  .ReadToEnd()
                                                                  .Trim()
                                                                  .Split("\n")
                                                                  .Select(sig => float.Parse(sig));

        if (signal == null) throw new Exception("Проверьте путь к файлу");

        var signalComplex = signal.Select(sig => new Complex(sig, 0))
                          .ToArray();

        var result = FourierTransforsform
            .GetDFTTransform(signalComplex)
            .Select(compl => Complex.Abs(compl))
            .Take(7)
            .Select(modulo => Math.Round(modulo)); 


        var symbols = result.Select(modulo => (char)modulo);

        var str = string.Join("", symbols);
        Console.WriteLine("Result: " + str);
    }


}