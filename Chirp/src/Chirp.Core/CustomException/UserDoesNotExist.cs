namespace Chirp.Core.CustomException;

public class UserDoesNotExist(string message) : Exception(message)
{
    public UserDoesNotExist() : this("User does not exist")
    {
    }   
}