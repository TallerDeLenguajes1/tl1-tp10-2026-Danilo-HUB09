using System;
using System.Text.Json.Serialization;

/* Lo ubtuve entrando al link del enunciado, esta es la forma del JSON */
/*{
    "userId": 1,
    "id": 1,
    "title": "delectus aut autem",
    "completed": false
  }*/

namespace Tareas.Modelos // <-- Cambiado aquí para evitar conflicto
{
    public class Tarea // <-- Ahora 'Tarea' se reconoce perfectamente como clase
    {
        // (1)
        [JsonPropertyName("userId")]  // <-- (3)
        public int UserId { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty; // (2) <-- Para evitar posible valor nulo

        [JsonPropertyName("completed")]
        public bool Completed { get; set; }
    }
}

/* ══════════════════════════════════════════════════════════════════════════
   DETALLES Y CONCEPTOS NUEVOS DE TAREA.CS
   ══════════════════════════════════════════════════════════════════════════
   (1) [JsonPropertyName("...")]: Atributo de serialización. Mapea o "conecta" 
       la clave exacta que viene en el texto JSON de la API (que suele usar 
       camelCase, ej: "userId") con la propiedad en C# escrita en PascalCase 
       (UserId). Evita que los datos se pierdan por diferencias de mayúsculas.

   (2) = string.Empty;: Inicialización para evitar el Warning de nulabilidad. 
       En las versiones modernas de C#, el compilador advierte si un 'string' 
       puede quedar como 'null' al crearse el objeto. Al asignarle un texto 
       vacío por defecto, le aseguramos a C# que la propiedad nunca será nula.

   (3) = En C#, los corchetes [...] se utilizan para declarar Atributos (Attributes).
        Un atributo es una forma de añadir metadatos (información adicional) a tu código. 
        No cambia la lógica de la variable en sí, sino que le da "instrucciones especiales" 
        al compilador o a librerías externas (en este caso, a System.Text.Json) 
        sobre cómo debe comportarse con esa propiedad.
   ══════════════════════════════════════════════════════════════════════════ */