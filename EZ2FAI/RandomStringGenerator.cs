public class RandomStringGenerator
{
    public static readonly System.Random random = new System.Random();
    public const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public static string GenerateRandomString(int length)
    {
        char[] randomString = new char[length];

        for(int i = 0;i < length;i++)
        {
            randomString[i] = chars[random.Next(chars.Length)];
        }

        return new string(randomString);
    }
}