using SimpleFunctions;

internal class Test
{
    private static void Main()
    {
        NonlinearSystemsOfEquations systemsOfEquations = new NonlinearSystemsOfEquations.MethodNewthon();
        BigFunction[] functions = new BigFunction[2] { (double[] arg) => arg[0] + arg[1] - 4, (double[] arg) => (arg[0] * arg[0]) - (4 * arg[1]) - 5 };
        double[] startArgs = new double[2] { 3, 1 };
        double[] rezult = systemsOfEquations.Method(functions/*, startArgs*/);
        Console.WriteLine(rezult.ShowArray());
    }
}
