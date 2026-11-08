using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto
{
    public class Client(int customerId)
    {
        private int leaveBarberTime = 5000;
        public void WaitBarber()
        {
            Console.WriteLine($"[Cliente {customerId}] Esperando para ser atendido...");
        }

        public async Task AwakeBarber()
        {
            Console.WriteLine($"[Cliente {customerId}] Despertando al barbero...");
            await Task.Delay(500); // Simula el tiempo de despertar al barbero
        }

        public async Task LeaveBarbershop()
        {
            Console.WriteLine($"[Cliente {customerId}] Se va de la barbería.");
            await Task.Delay(leaveBarberTime); // Simula el tiempo de irse
            return;
        }
    }
}