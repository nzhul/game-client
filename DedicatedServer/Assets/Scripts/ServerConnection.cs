/// <summary>
/// This class will hold the information for every player connection:
/// His connectionId, Token, UserInformation, Loaded Regions and other information.
/// </summary>
public class ServerConnection
{
    /// <summary>
    /// Database Id
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unity connection Idl
    /// </summary>
    public int ConnectionId { get; set; }

    public int GameId { get; set; }

    public string Username { get; set; }

    public int MMR { get; set; }

    public string Token { get; set; }
}
