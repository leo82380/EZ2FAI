public class RandomStringGenerator
{
    public static readonly System.Random random = new System.Random();

    public static string GenerateRandomString(int length)
    {
        char[] randomString = new char[length];

        for(int i = 0;i < length;i++)
        {
            randomString[i] = (char) GetNumber(random.Next(62));
        }

        return new string(randomString);
    }

    private static int GetNumber(int i)
    {
        if(i >= 36) return i + 61;
        if(i >= 10) return i + 55;
        return i + 48;
    }
}