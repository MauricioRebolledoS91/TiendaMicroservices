using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using TiendaServicios.Api.CarritoCompra.RemoteInterface;
using TiendaServicios.Api.CarritoCompra.RemoteModel;

namespace TiendaServicios.Api.CarritoCompra.RemoteService
{
    public class LibrosService : ILibrosService
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly ILogger<LibrosService> _logger;//ILogger<LibrosService> qquiere decir que vamos a hacer uso de ILogger en la clase LibrosService
        public LibrosService(IHttpClientFactory httpClient, ILogger<LibrosService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        //Etse método devuelve una tuple multiple (resultado,Libro,ErrorMessage)
        public async Task<(bool resultado, LibroRemote Libro, string ErrorMessage)> GetLibro(Guid LibroId)
        {
            try
            {
                var cliente = _httpClient.CreateClient("Libros");//tomando la url base de la microservice de Libros, la cual esta en la clase starup
                var response =  await cliente.GetAsync($"api/LibrosMateriales/{LibroId}");//GetAync invoca al endpoint que yo quiera
                if (response.IsSuccessStatusCode)
                {
                    var contenido = await response.Content.ReadAsStringAsync();
                    //esto lo hacemos, para no tener inconvenientes con las mayusuculas y minusculas
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    //Aquí decimos: quiero que me deserialices lo que hay en la variable contenido y me lo mapees en la LibroRemote
                    //tambieén le pasamos como parámetro la variable options, para que no tengamos problemas con las mayusuculas y munusuclas
                    var resultado = JsonSerializer.Deserialize<LibroRemote>(contenido, options);

                    return (true, resultado, null);
                }

                return (false, null, response.ReasonPhrase);
            }
            catch (Exception e)
            {
                _logger?.LogError(e.ToString());
                return (false, null, e.Message);
            }
        }
    }
}
