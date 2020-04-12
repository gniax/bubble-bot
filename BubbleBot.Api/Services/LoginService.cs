using BubbleBot.Api.Extensions;
using BubbleBot.Website.Models;

namespace BubbleBot.Website.Services
{
    public class LoginService
    {

        // Fields
        private readonly PanelDb _panelDb;


        // Constructors
        public LoginService() { }

        public LoginService(PanelDb panelDb)
        {
            _panelDb = panelDb;
        }


        public User LoginUser(string username, string password, out int error)
        {
            error = -1;
            var user = _panelDb.GetUser(username);

            // User not found
            if (user == null)
            {
                error = 1;
            }
            // User found
            else
            {
                string givenPassword = $"{user.Salt.GetMD5()}{password.GetMD5()}".GetMD5();

                // Wrong password
                if (givenPassword != user.Password)
                {
                    error = 4;
                }
                // Good password
                else
                {
                    // Awaiting activation
                    if (user.UserGroup == 5)
                    {
                        error = 3;
                    }
                    // Banned
                    else if (user.UserGroup == 6)
                    {
                        error = 2;
                    }
                    // Good user group
                    else
                    {
                        // Update (or add) the user's informations in the panel's db
                        return user;
                    }
                }
            }

            return null;
        }

        //private User UpdateOrAdd(PanelDb forumUser)
        //{
        //    if (forumUser == null)
        //        return null;

        //    var user = _panelDb.Users.FirstOrDefault(u => u.Username == forumUser.Username);

        //    // If the user doesn't exist in the panel's database, add it
        //    if (user == null)
        //    {
        //        user = new User(forumUser.Username, forumUser.Email, forumUser.AvatarUrl);
        //        _panelDb.Users.Add(user);
        //    }
        //    // Otherwise just update the informations
        //    else
        //    {
        //        user.Avatar = forumUser.AvatarUrl;
        //    }

        //    // Finally, save the panel's db
        //    _panelDbContext.SaveChanges();

        //    return _panelDbContext.GetUser(forumUser.Username);
        //}

    }
}
