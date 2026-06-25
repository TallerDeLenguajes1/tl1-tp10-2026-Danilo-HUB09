using System.Text.Json;  // <--- Requerimiento del enunciado. Al usar un .NET moderno, basta con el using sin instalar paquetes extra.
using Modelos.Usuarios;  // <--- Importa las clases mapeadas (Usuario, Address, etc.) desde su propio archivo y namespace.

HttpClient client = new HttpClient();
string url = "https://jsonplaceholder.typicode.com/users/";

try
{
    // 1. PETICIÓN HTTP: Envía la solicitud GET de forma asíncrona. 
    // 'await' libera el hilo principal mientras espera la respuesta de la API
    HttpResponseMessage response = await client.GetAsync(url);
    
    // 2. CONTROL DE ÉXITO: Si la API responde con un error (ej: 404 o 500), arroja una excepción HTTP
    response.EnsureSuccessStatusCode();
    
    // 3. LECTURA DEL TEXTO: Descarga el cuerpo de la respuesta en un string largo con formato JSON
    string responseBody = await response.Content.ReadAsStringAsync();

    // 4. DESERIALIZACIÓN: Transforma el string JSON en una lista de objetos C# de tipo 'Usuario'
    // Usamos CamelCase para "hermanar" las propiedades del JSON con los nombres definidos en la clase
    List<Usuario>? usuarios = JsonSerializer.Deserialize<List<Usuario>>(responseBody, new JsonSerializerOptions 
    { 
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
    });

    // 5. CONTROL DE BUCLE: Recorre los primeros 5 elementos. 
    // Math.Min evita errores si la API devolviera menos usuarios de los esperados.
    for (int i = 0; i < Math.Min(5, usuarios.Count); i++)
    {
        Usuario u = usuarios[i];
        
        // 6. IMPRESIÓN DE DATOS: Acceso a las propiedades en minúscula (coincidentes con el JSON).
        Console.WriteLine($"Nombre: {u.name} | Email: {u.email}");
        
        // El operador '?.street' y '?.city' previene caídas fatales si la propiedad 'address' viniera nula
        Console.WriteLine($"Domicilio: {u.address?.street}, {u.address?.city}"); 
        Console.WriteLine(new string('-', 40));
    }

    // 7. PERSISTENCIA LOCAL: Guarda el JSON crudo original en un archivo de texto en el sistema de archivos local
    await File.WriteAllTextAsync("usuarios.json", responseBody);
    Console.WriteLine("Archivo guardado con éxito.");
}
catch (Exception ex)
{
    // 8. CONTROL DE EXCEPCIONES: Captura cualquier error en el flujo (red caída, problemas de disco, etc.)
    // Muestra un mensaje amigable y técnico en lugar de colapsar la aplicación de consola
    Console.WriteLine($"Ocurrió un error: {ex.Message}");
}

/* ====================================================================================
   VERIFICACIÓN DE REQUERIMIENTO: PERSISTENCIA EN EL SISTEMA DE ARCHIVOS LOCAL
   ====================================================================================
   
   El requerimiento de "guardar los datos en el sistema de archivos local" se cumple
   estrictamente mediante la siguiente instrucción:
   
        await File.WriteAllTextAsync("usuarios.json", responseBody);

   MECANISMO DE TRABAJO TRAS BAMBALINAS:
   -------------------------------------
   1. Clase 'System.IO.File': 
      Es la encargada de interactuar directamente con el sistema operativo para gestionar
      archivos. Al usar el método asíncrono 'WriteAllTextAsync', la escritura en el disco
      no congela la aplicación de consola.
      
   2. Ubicación Relativa:
      Al pasar únicamente el nombre del archivo ("usuarios.json") sin una ruta absoluta 
      (ej: C:\Users\...), .NET infiere que el archivo debe crearse en el directorio de 
      ejecución actual. En este caso, quedará guardado exactamente dentro de tu carpeta 
      local del proyecto llamada 'Usuarios/'.

   3. Comportamiento del Archivo:
      * Si el archivo NO existe: El framework lo crea de forma automática en el disco.
      * Si el archivo YA existe: Sobrescribe por completo el contenido anterior con los 
        nuevos datos limpios devueltos por la API en la última ejecución.

   4. Datos Almacenados:
      Se almacena la variable 'responseBody', resguardando la estructura JSON original 
      completa que envió el webservice. Esto permite que el archivo local funcione como 
      un respaldo fidedigno (backup) de la información de la API.
==================================================================================== */
