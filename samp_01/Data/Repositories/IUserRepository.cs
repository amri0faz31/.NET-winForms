using System;
using samp_01.Domain.DTO;
using samp_01.Domain.Entities;

namespace samp_01.Data.Repositories
{
    // Repository interface abstracts data access for User-related operations
    //service layer depends on this interface, not on concrete implementations
    // the interface is implemented by AdoUserRepository which uses ADO.NET for data access
    public interface IUserRepository
    {
        // Checks if a user with the given name exists
        bool ExistsByName(string name);

        // Adds a new user and returns the newly created user's ID
        int AddUser(Domain.Entities.User user);

        // Retrieves user credentials by their name
        (int Id, string? Hash, string? Salt)? GetCredentialsByName(string name);

        // Retrieves user profile by their ID
        UserProfileDTO? GetProfileById(int id);

        // Retrieves user profile by their name
        UserProfileDTO? GetProfileByName(string name);
    }
}
