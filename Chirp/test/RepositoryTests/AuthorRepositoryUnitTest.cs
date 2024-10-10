using Moq;
using SimpleDB;

namespace RepositoryTests;

public class AuthorRepositoryUnitTest
{
    [Fact]
    public void GetAuthorByName_NameCantBeFound_ReturnErrorMessage()
    {
        //Arrange an arbitrary DB
        
        //Act scenario where you want to find a name that isn't in the DB
        
        //Assert Message that informs the unsuccessful found
    }

    [Fact]
    public void GetAuthorByName_NameIsHelge_ReturnsAuthorDTOOfHelge()
    {
        //Arrange an arbitrary author with name 'Helge' and create arbitrary database to put up
        
        //Act a scenario where the repository can get an author by the name of 'Helge'
        
        //Assert the value of the arbitrary author that was arranged
    }
}