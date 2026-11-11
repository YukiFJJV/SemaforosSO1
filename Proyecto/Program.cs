using System;
using System.Threading.Tasks;
using Proyecto;

// * ======= Integrantes del grupo ============================================================
// * Jafet Fernando Jimenez Valladares 20241031469
// * Luis Felipe Suazo Martínez 20241002179
// * Jose Mario Antunez Trejo 20211024229
// *Elian Alberto Posadas Barahona 20181032112
// * ==========================================================================================
class Program
{
    static async Task Main()
    {
        var barbershop = new Barbershop();

        _ = Task.Run(() => barbershop.Barber.WorkAsync());

        int totalClientes = 7;
        Task[] tareasClientes = new Task[totalClientes];

        Console.WriteLine($"--- Abriendo la barbería. Hay 5 sillas de espera y llegarán {totalClientes} clientes. ---\n");

        for (int i = 1; i <= totalClientes; i++)
        {
            int customerId = i;
            tareasClientes[i - 1] = Task.Run(async () =>
            {
                await Task.Delay(Random.Shared.Next(200, 1500));
                await barbershop.EnterBarbershopAsync(customerId);
            });
        }

        await Task.WhenAll(tareasClientes);

        Console.WriteLine("\n--- Simulación finalizada. Presiona cualquier tecla para salir. ---");
        if (!Console.IsInputRedirected)
        {
            Console.ReadKey();
        }
    }
}