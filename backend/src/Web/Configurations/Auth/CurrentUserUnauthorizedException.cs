namespace Web.Configurations.Auth;

public class CurrentUserUnauthorizedException(string message) : Exception(message);
