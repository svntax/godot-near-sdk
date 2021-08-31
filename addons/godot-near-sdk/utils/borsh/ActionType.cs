namespace NearClient
{
    public enum ActionType : byte
    {
        CreateAccount = 0,
        DeployContract = 1,
        FunctionCall = 2,
        Transfer = 3,
        Stake = 4,
        AddKey = 5,
        DeleteKey = 6,
        DeleteAccount = 7
    }
}