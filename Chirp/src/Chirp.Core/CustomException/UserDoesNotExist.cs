namespace Chirp.Core.CustomException;
/// <summary>
/// Exception to be thrown if some User does not exist in the database
/// </summary>
/// <param name="message"></param>
public class UserDoesNotExist(string message) : Exception(message)
{
    public UserDoesNotExist() : this("User does not exist")
    {
    }   
}