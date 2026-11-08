using System;
using System.Threading.Tasks;

namespace Proyecto;

public class Barber(Barbershop barbershop)
{
    private readonly Barbershop _barbershop = barbershop;
    public BarberStatus Status { get; set; } = BarberStatus.Dormido;

    public async Task WorkAsync()
    {
        while (true)
        {
            Status = BarberStatus.Dormido;
            await _barbershop.WaitForCustomerAsync();

            await AwakeAsync();
            Status = BarberStatus.Despierto;

            while (true)
            {
                // Esperamos a que llegue un cliente
                Barbershop.CustomerRequest? request =
                    await _barbershop.TakeNextCustomerAsync();

                if (request is null)
                {
                    await SleepAsync();
                    break;
                }

                await CutHairAsync(request.CustomerId);
                // Señalamos que el corte ha terminado para que el cliente pueda irse
                request.Completed.TrySetResult(true);
            }
        }
    }

    public async Task<BarberStatus> CutHairAsync(int id)
    {
        Status = BarberStatus.Cortando;
        Console.WriteLine($"---> [Barbero] Cortando el cabello del cliente {id}...");
        await Task.Delay(100); // Simula el tiempo que toma el corte
        Console.WriteLine($"<--- [Barbero] Terminó el corte del cliente {id}.");
        return Status;
    }

    public async Task<BarberStatus> SleepAsync()
    {
        Console.WriteLine("[Barbero] ZZZ... El barbero está durmiendo (cola vacía).");
        await Task.Delay(500);
        return Status = BarberStatus.Dormido;
    }

    public async Task<BarberStatus> AwakeAsync()
    {
        return Status = BarberStatus.Despierto;
    }
}