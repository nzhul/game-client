﻿using Assets.Scripts.Configuration;

namespace Assets.Scripts.Network.Services.HTTP.Interfaces
{
    public interface IUsersService
    {
        User GetUser(int id);
    }
}