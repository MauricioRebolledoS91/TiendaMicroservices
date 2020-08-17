using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TiendaServicios.Api.Libro.Tests
{
    //esta clase, básicamente lo que hace es evaluar el arreglo que te va a devolver
    //el entity framework y recuerda que es un método asincrono, por eso estamos indicando que trabaje
    //con una variable global a nivel de clase, que se llame enumerator
    public class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> enumerator;
        public T Current => enumerator.Current;

        public AsyncEnumerator(IEnumerator<T> enumerator)
            => this.enumerator = enumerator ?? throw new ArgumentNullException();

        //método para eliminar el método cuando se haya completado la tarea
        public async ValueTask DisposeAsync()
        {
            await Task.CompletedTask;
        }

        //método que devuelve los siguientes valores que existan dentro del arreglo, gracias
        //al task.FromResult
        public async ValueTask<bool> MoveNextAsync()
        {
            return await Task.FromResult(enumerator.MoveNext());
        }
    }
}
