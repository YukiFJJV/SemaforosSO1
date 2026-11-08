namespace Proyecto
{
    public class Barbershop
    {
        private readonly SemaphoreSlim _mutex = new(1, 1);
        private readonly SemaphoreSlim _clientReady = new(0, 10);
        private readonly SemaphoreSlim _freeChair = new(5, 5);
        // FIFO cola de clientes esperando para ser atendidos
        private readonly Queue<CustomerRequest> _waitingCustomers = new();

        public Barber Barber { get; set; }

        public Barbershop()
        {
            Barber = new Barber(this);
        }

        public async Task EnterBarbershopAsync(int customerId)
        {
            var client = new Client(customerId);
            Console.WriteLine($"[Cliente {customerId}] Llegó a la barbería.");

            // Intentar tomar una silla en la sala de espera
            bool isChairFree = await _freeChair.WaitAsync(TimeSpan.FromMilliseconds(50));
            if (!isChairFree)
            {
                await client.LeaveBarbershop();
                return;
            }

            try
            {
                client.WaitBarber();
                await _mutex.WaitAsync();
                CustomerRequest request;
                try
                {
                    if (Barber.Status == BarberStatus.Dormido)
                    {
                        await client.AwakeBarber();
                        await Barber.AwakeAsync();
                    }

                    request = new CustomerRequest(customerId);
                    _waitingCustomers.Enqueue(request);
                    if (_waitingCustomers.Count == 1)
                    {
                        _clientReady.Release();
                    }
                }
                finally
                {
                    _mutex.Release();
                }
                // Esperamos a que el barbero nos atienda
                await request.Assigned.Task;
                // Esperamos a que el barbero termine de cortarnos el cabello
                await request.Completed.Task;
            }
            catch
            {
                _freeChair.Release();
            }
        }

        // Métodos del barbero en su ciclo
        public async Task WaitForCustomerAsync()
        {
            await _clientReady.WaitAsync();
        }

        public async Task<CustomerRequest?> TakeNextCustomerAsync()
        {
            await _mutex.WaitAsync();
            try
            {
                if (_waitingCustomers.Count == 0)
                {
                    return null;
                }

                CustomerRequest request = _waitingCustomers.Dequeue();
                _freeChair.Release();
                request.Assigned.TrySetResult(true);
                return request;
            }
            finally
            {
                _mutex.Release();
            }
        }

        // ? Sealed es: no se puede heredar de esta clase
        public sealed class CustomerRequest(int customerId)
        {
            public int CustomerId { get; } = customerId;

            // TaskCompletionSource permite crear tareas que se completan manualmente, en este caso para sincronizar la asignación y finalización del corte de cabello.
            public TaskCompletionSource<bool> Assigned { get; } =
                new(TaskCreationOptions.RunContinuationsAsynchronously);
            public TaskCompletionSource<bool> Completed { get; } =
                new(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}