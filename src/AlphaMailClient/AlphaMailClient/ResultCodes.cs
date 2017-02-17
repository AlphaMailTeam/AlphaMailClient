using System;

namespace AlphaMailClient.AlphaMailClient
{
    public enum AuthResultCode
    {
        LoginBadPassword,
        LoginBadUser,
        LoginSuccess,
        NotAuthenticated,
        RegisterBadUser,
        RegisterSuccess
    }
    public enum MessageResultCode
    {
        MessageSuccess,
        NoUser
    }
    public enum PKeyResultCode
    {
        Success,
        NoUser
    }
    public enum UpdateResultCode
    {
        BadUser,
        Success
    }
}

