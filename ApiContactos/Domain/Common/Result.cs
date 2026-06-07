namespace Domain.Common;

// Implementamos un patrón de diseño llamado "Result". 
// Esto nos permite comunicar si una acción fue exitosa o fallida de forma muy limpia. 
// Hace que la aplicación sea más rápida, predecible y fácil de mantener a futuro.
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Error { get; }

    protected Result(bool isSuccess, string error)
    {
        // Protecciones de seguridad: Aseguramos que el sistema no pueda mentir 
        if (isSuccess && !string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException("Un resultado exitoso no puede contener un error.");
        }
        if (!isSuccess && string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException("Un resultado fallido debe contener un error.");
        }

        IsSuccess = isSuccess;
        Error = error ?? string.Empty;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string error) => new(false, error);
    
    public static Result<T> Success<T>(T value) => new(value, true, string.Empty);
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

// Extensión para cuando necesitamos devolver un resultado exitoso que además incluya datos (como un Contacto)
public class Result<T> : Result
{
    private readonly T? _value;

    public T Value => IsSuccess 
        ? _value!
        // Evitamos que la aplicación se caiga si alguien intenta leer un dato de una operación que falló.
        : throw new InvalidOperationException("El valor de un resultado fallido no puede ser accedido.");

    protected internal Result(T? value, bool isSuccess, string error) 
        : base(isSuccess, error)
    {
        _value = value;
    }
}
