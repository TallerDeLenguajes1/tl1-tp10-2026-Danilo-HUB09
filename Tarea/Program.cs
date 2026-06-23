using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http; // (3)
using System.Text.Json; // (4)
using System.Threading.Tasks; // (5)
using Tareas.Modelos;

namespace Tareas
{
    class Program
    {
        // (3)
        private static readonly HttpClient client = new HttpClient();

        // (5)
        static async Task Main(string[] args)
        {
            string url = "https://jsonplaceholder.typicode.com/todos/";

            Console.WriteLine("Obteniendo tareas de la API...\n");

            // (6)
            HttpResponseMessage response = await client.GetAsync(url);
            // (7)
            response.EnsureSuccessStatusCode();

            // (6)
            string json = await response.Content.ReadAsStringAsync();

            // (4) (8)
            List<Tarea>? tareas = JsonSerializer.Deserialize<List<Tarea>>(json);

            // (9)
            if (tareas == null)
            {
                Console.WriteLine("No se pudieron obtener las tareas.");
                return;
            }

            // (10)
            List<Tarea> pendientes = tareas.FindAll(t => !t.Completed);
            List<Tarea> completadas = tareas.FindAll(t => t.Completed);

            // [Impresión en Consola - Pendientes]
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine($"  TAREAS PENDIENTES ({pendientes.Count})");
            Console.WriteLine("═══════════════════════════════════════");
            foreach (var tarea in pendientes)
            {
                // (11)
                Console.WriteLine($"  [{tarea.Id,3}] {tarea.Title}");
                Console.WriteLine($"        Estado: ⏳ PENDIENTE | Usuario: {tarea.UserId}");
            }

            // [Impresión en Consola - Completadas]
            Console.WriteLine("\n═══════════════════════════════════════");
            Console.WriteLine($"  TAREAS COMPLETADAS ({completadas.Count})");
            Console.WriteLine("═══════════════════════════════════════");
            foreach (var tarea in completadas)
            {
                Console.WriteLine($"  [{tarea.Id,3}] {tarea.Title}");
                Console.WriteLine($"        Estado: ✅ COMPLETADA | Usuario: {tarea.UserId}");
            }

            // (4)
            string jsonOutput = JsonSerializer.Serialize(tareas, new JsonSerializerOptions
            {
                WriteIndented = true // (12)
            });
            
            // (6) (13)
            await File.WriteAllTextAsync("tareas.json", jsonOutput);

            Console.WriteLine($"\nTotal de tareas: {tareas.Count}");
            Console.WriteLine("✅ Archivo tareas.json guardado en el directorio de ejecución.");
        }
    }
}

/* ══════════════════════════════════════════════════════════════════════════
   DETALLES Y CONCEPTOS NUEVOS DE PROGRAM.CS
   ══════════════════════════════════════════════════════════════════════════
    (1) y (2) de Tarea.cs

   (3) HttpClient & System.Net.Http: Clase provista por .NET para enviar 
       peticiones HTTP (GET, POST, etc.) y recibir respuestas de una URL. 
       Se define como 'static readonly' para que la aplicación reutilice la 
       misma conexión y no agote los recursos del sistema operativo.

   (4) JsonSerializer & System.Text.Json: La librería nativa de .NET para 
       manejar JSON. Contiene los métodos para transformar texto plano (JSON) 
       en objetos estructurados de C# (Deserialize) y viceversa (Serialize).

   (5) async Task Main: Modifica el punto de entrada del programa para que 
       pueda soportar operaciones asíncronas. En lugar de devolver 'void', 
       devuelve un 'Task' (una promesa de que la tarea finalizará).

   (6) await: Palabra clave fundamental del asincronismo. Le dice al programa: 
       "Esta operación (ir a internet o escribir un archivo) va a demorar; 
       libera el hilo principal para no congelar la aplicación y avísame 
       cuando los datos hayan llegado".

   (7) EnsureSuccessStatusCode(): Método de control. Verifica el código de 
       estado HTTP de la respuesta. Si la API devolvió un error (como un 404 
       o 500), interrumpe el programa lanzando una excepción de inmediato.

   (8) <List<Tarea>> (Tipos Genéricos): Al usar Deserialize, debemos indicarle 
       entre los signos '< >' la estructura exacta que queremos que construya. 
       Aquí espera que el JSON sea una lista de objetos tipo 'Tarea'.

   (9) List<Tarea>?: El signo de pregunta indica que la variable permite un 
       valor nulo. Si la deserialización falla por completo, 'tareas' será 
       null, por lo que el bloque 'if (tareas == null)' previene un crash.

   (10) .FindAll(t => !t.Completed): Método de listas en C# que utiliza una 
        expresión Lambda (el operador '=>'). Funciona como un filtro rápido: 
        recorre la lista completa y extrae en una nueva lista únicamente 
        las tareas donde la condición booleana se cumpla.

   (11) {tarea.Id,3}: Formateo de alineación en cadenas intercaladas ($""). 
        El ',3' le dice a la consola que reserve un ancho fijo de 3 caracteres 
        para el ID. Logra que los números queden alineados a la derecha 
        dejando espacios si el número tiene un solo dígito (ej: [  1], [ 12]).

   (12) WriteIndented = true: Configuración de formato para el JSON de salida. 
        Hace que el texto se guarde con saltos de línea y tabulaciones ("lado 
        humano") en vez de guardarse todo apretado en una sola línea continua.

   (13) File.WriteAllTextAsync: Método asíncrono de la librería System.IO. 
        Crea un archivo de texto, vuelca todo el string dentro de él y lo 
        cierra. Si el archivo ya existía, sobrescribe su contenido.
   ══════════════════════════════════════════════════════════════════════════ */