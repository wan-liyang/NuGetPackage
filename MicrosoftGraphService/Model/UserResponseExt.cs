namespace MicrosoftGraphService.Model
{
    public static class UserResponseExt
    {
        public static UserModel ToUserModule(this UserResponse user)
        {
            UserModel module = new UserModel();
            if (user != null)
            {
                module.Id = user.id;
                module.EmailAddress = user.mail;
                module.Name = user.displayName;
            }
            return module;
        }
    }
}
