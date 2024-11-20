namespace Chirp.Core.CustomException;

public class UserDoesNotExist(string message) : Exception(message)
{
    
}