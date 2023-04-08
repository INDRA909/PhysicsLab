using AIPS;

class Program
{
    public static void Main()
    {
        var signal = new StreamReader(@"C:\Users\Иван\Desktop\АиПС\Сигналы\2.txt")
                                                                  .ReadToEnd()
                                                                  .Trim()
                                                                  .Split("\n")
                                                                  .Select(sig => float.Parse(sig));

        var signalComplex = signal.Select(sig => new Complex(sig, 0))
                          .ToArray();

        var result = Butterfly.GetTransform(signalComplex)
            .Select(compl => Complex.Abs(compl))
            .Take(7)
            .Select(modulo => Math.Round(modulo)); 


        var symbols = result.Select(modulo => (char)modulo);

        var str = string.Join("", symbols);
        Console.WriteLine("Result: " + str);
    }


}